using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using TKernel;
using TKMath;
using TriangleNet.Topology.DCEL;
using static OpenTK.Graphics.OpenGL.GL;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OCCPort
{
    //! The BRepLib package provides general utilities for
    //! BRep.
    //!
    //! * FindSurface : Class to compute a surface through
    //! a set of edges.
    //!
    //! * Compute missing 3d curve on an edge.
    public class BRepLib
    {  //! Returns the default precision.
        public static double Precision()
        {
            return thePrecision;

        }
        public static void UpdateDeflection(TopoDS_Shape theShape)
        {
            TopExp_Explorer anExpFace = new TopExp_Explorer(theShape, TopAbs_ShapeEnum.TopAbs_FACE);
            for (; anExpFace.More(); anExpFace.Next())
            {
                TopoDS_Face aFace = TopoDS.Face(anExpFace.Current());
                Geom_Surface aSurf = BRep_Tool.Surface(aFace);
                if (aSurf == null)
                {
                    continue;
                }

                TopLoc_Location aLoc = new TopLoc_Location();
                Poly_Triangulation aPT = BRep_Tool.Triangulation(aFace, ref aLoc);
                if (aPT == null || !aPT.HasUVNodes())
                {
                    continue;
                }

                // Collect all nodes of degenerative edges and skip elements
                // build upon them due to huge distortions introduced by passage
                // from UV space to 3D.
                NCollection_Map<int> aDegNodes = new NCollection_Map<int>();
                TopExp_Explorer anExpEdge = new TopExp_Explorer(aFace, TopAbs_ShapeEnum.TopAbs_EDGE);
                for (; anExpEdge.More(); anExpEdge.Next())
                {
                    TopoDS_Edge aEdge = TopoDS.Edge(anExpEdge.Current());
                    if (BRep_Tool.Degenerated(aEdge))
                    {
                        Poly_PolygonOnTriangulation aPolygon = BRep_Tool.PolygonOnTriangulation(aEdge, aPT, aLoc);
                        if (aPolygon == null)
                        {
                            continue;
                        }

                        for (int aNodeIt = aPolygon.Nodes().Lower(); aNodeIt <= aPolygon.Nodes().Upper(); ++aNodeIt)
                        {
                            aDegNodes.Add(aPolygon.Node(aNodeIt));
                        }
                    }
                }

                EvalDeflection aTool = new EvalDeflection(aFace);
                NCollection_Map<Link> aLinks = new NCollection_Map<Link>();
                double aSqDeflection = 0.0;
                gp_Trsf aTrsf = aLoc.Transformation();
                for (int aTriIt = 1; aTriIt <= aPT.NbTriangles(); ++aTriIt)
                {
                    Poly_Triangle aTriangle = aPT.Triangle(aTriIt);

                    int[] aNode = new int[3];
                    aTriangle.Get(ref aNode[0], ref aNode[1], ref aNode[2]);
                    if (aDegNodes.Contains(aNode[0]) ||
                        aDegNodes.Contains(aNode[1]) ||
                        aDegNodes.Contains(aNode[2]))
                    {
                        continue;
                    }

                    gp_Pnt[] aP3d = {
        aPT.Node (aNode[0]).Transformed (aTrsf),
        aPT.Node (aNode[1]).Transformed (aTrsf),
        aPT.Node (aNode[2]).Transformed (aTrsf)
      };

                    gp_Pnt2d[] aP2d = {
        aPT.UVNode (aNode[0]),
        aPT.UVNode (aNode[1]),
        aPT.UVNode (aNode[2])
      };

                    // Check midpoint of triangle.
                    gp_Pnt aMid3d_t = (aP3d[0].XYZ() + aP3d[1].XYZ() + aP3d[2].XYZ()) / 3.0;
                    gp_Pnt2d aMid2d_t = new gp_Pnt2d((aP2d[0].XY() + aP2d[1].XY() + aP2d[2].XY()) / 3.0);

                    aSqDeflection = Math.Max(aSqDeflection, aTool.Eval(aMid2d_t, aMid3d_t));

                    for (int i = 0; i < 3; ++i)
                    {
                        int j = (i + 1) % 3;
                        Link aLink = new Link(aNode[i], aNode[j]);
                        if (!aLinks.Add(aLink))
                        {
                            // Do not estimate boundary links due to high distortions at the edge.
                            gp_Pnt aP3d1 = aP3d[i];
                            gp_Pnt aP3d2 = aP3d[j];

                            gp_Pnt2d aP2d1 = aP2d[i];
                            gp_Pnt2d aP2d2 = aP2d[j];

                            gp_Pnt aMid3d_l = (aP3d1.XYZ() + aP3d2.XYZ()) / 2.0;
                            gp_Pnt2d aMid2d_l = new gp_Pnt2d((aP2d1.XY() + aP2d2.XY()) / 2.0);

                            aSqDeflection = Math.Max(aSqDeflection, aTool.Eval(aMid2d_l, aMid3d_l));
                        }
                    }
                }

                aPT.Deflection(Math.Sqrt(aSqDeflection));
            }
        }
        //! Represents link of triangulation.
        struct Link
        {
            int[] Node = new int[2];

            //! Constructor
            public Link(int theNode1, int theNode2)
            {
                Node[0] = theNode1;
                Node[1] = theNode2;
            }
        }

        static void InternalUpdateTolerances(TopoDS_Shape theOldShape,
bool IsVerifyTolerance, bool IsMutableInput, BRepTools_ReShape theReshaper)
        {
            TopTools_DataMapOfShapeReal aShToTol;
            // Harmonize tolerances
            // with rule Tolerance(VERTEX)>=Tolerance(EDGE)>=Tolerance(FACE)
            double tol = 0;
            if (IsVerifyTolerance)
            {
                // Set tolerance to its minimum value
                Geom_Surface S;
                TopLoc_Location l;
                TopExp_Explorer ex = new TopExp_Explorer();
                Bnd_Box aB = new Bnd_Box();
                double aXmin, aYmin, aZmin, aXmax, aYmax, aZmax, dMax;
                for (ex.Init(theOldShape, TopAbs_ShapeEnum.TopAbs_FACE); ex.More(); ex.Next())
                {
                    TopoDS_Face curf = TopoDS.Face(ex.Current());
                    S = BRep_Tool.Surface(curf, out l);
                    if (S != null)
                    {
                        aB.SetVoid();
                        BRepBndLib.Add(curf, aB);
                        if (S is Geom_RectangularTrimmedSurface)
                        {
                            S = (Geom_RectangularTrimmedSurface)(S).BasisSurface();
                        }
                        GeomAdaptor_Surface AS = new GeomAdaptor_Surface(S);
                        switch (AS._GetType())
                        {
                            case GeomAbs_SurfaceType.GeomAbs_Plane:
                            case GeomAbs_SurfaceType.GeomAbs_Cylinder:
                            case GeomAbs_SurfaceType.GeomAbs_Cone:
                                {
                                    tol = OCCPort.Precision.Confusion();
                                    break;
                                }
                            case GeomAbs_SurfaceType.GeomAbs_Sphere:
                            case GeomAbs_SurfaceType.GeomAbs_Torus:
                                {
                                    tol = OCCPort.Precision.Confusion() * 2;
                                    break;
                                }
                            default:
                                tol = OCCPort.Precision.Confusion() * 4;
                                break;
                        }
                        if (!aB.IsWhole())
                        {
                            aB.Get(out aXmin, out aYmin, out aZmin, out aXmax, out aYmax, out aZmax);
                            dMax = 1.0;
                            if (!aB.IsOpenXmin() && !aB.IsOpenXmax()) dMax = aXmax - aXmin;
                            if (!aB.IsOpenYmin() && !aB.IsOpenYmax()) aYmin = aYmax - aYmin;
                            if (!aB.IsOpenZmin() && !aB.IsOpenZmax()) aZmin = aZmax - aZmin;
                            if (aYmin > dMax) dMax = aYmin;
                            if (aZmin > dMax) dMax = aZmin;
                            tol = tol * dMax;
                            // Do not process tolerances > 1.
                            if (tol > 1.0) tol = 0.99;
                        }
                        //  aShToTol.Bind(curf, tol);
                    }
                }
            }

            //Process edges
            TopTools_IndexedDataMapOfShapeListOfShape parents = new TopTools_IndexedDataMapOfShapeListOfShape();
            TopExp.MapShapesAndAncestors(theOldShape, TopAbs_ShapeEnum.TopAbs_EDGE, TopAbs_ShapeEnum.TopAbs_FACE, parents);
            TopTools_ListIteratorOfListOfShape lConx = null;
            int iCur;
            //for (iCur = 1; iCur <= parents.Extent(); iCur++)
            //{
            //    tol = 0;
            //    for (lConx.Initialize(parents[iCur]); lConx.More(); lConx.Next())
            //    {
            //        TopoDS_Face FF = TopoDS.Face(lConx.Value());
            //        double Ftol;
            //        if (IsVerifyTolerance && aShToTol.IsBound(FF)) //first condition for speed-up
            //            Ftol = aShToTol(FF);
            //        else
            //            Ftol = BRep_Tool.Tolerance(FF); //tolerance have not been updated
            //        tol = Math.Max(tol, Ftol);
            //    }
            //    // Update can only increase tolerance, so if the edge has a greater
            //    //  tolerance than its faces it is not concerned
            //    TopoDS_Edge EK = TopoDS.Edge(parents.FindKey(iCur));
            //    if (tol > BRep_Tool.Tolerance(EK))
            //        aShToTol.Bind(EK, tol);
            //}

            ////Vertices are processed
            //double BigTol = 1e10;
            //parents.Clear();

            //TopExp.MapShapesAndUniqueAncestors(theOldShape, TopAbs_VERTEX, TopAbs_EDGE, parents);
            //TColStd_MapOfTransient Initialized;
            //int nbV = parents.Extent();
            //for (iCur = 1; iCur <= nbV; iCur++)
            //{
            //    tol = 0;
            //    TopoDS_Vertex V = TopoDS.Vertex(parents.FindKey(iCur));
            //    Bnd_Box box = new Bnd_Box();
            //    box.Add(BRep_Tool.Pnt(V));
            //    gp_Pnt p3d;
            //    for (lConx.Initialize(parents(iCur)); lConx.More(); lConx.Next())
            //    {
            //        TopoDS_Edge E = TopoDS.Edge(lConx.Value());
            //        var aNtol = aShToTol.Seek(E);
            //        tol = Math.Max(tol, aNtol ? *aNtol : BRep_Tool.Tolerance(E));
            //        if (tol > BigTol)
            //            continue;

            //        if (!BRep_Tool.SameRange(E))
            //            continue;

            //        double par = BRep_Tool.Parameter(V, E);
            //        BRep_TEdge TE = (BRep_TEdge)(E.TShape());
            //        //BRep_ListIteratorOfListOfCurveRepresentation itcr(TE->Curves());
            //        TopLoc_Location Eloc = E.Location();
            //        foreach (var cr in TE.Curves())
            //        {
            //            // For each CurveRepresentation, check the provided parameter

            //            TopLoc_Location loc = cr.Location();
            //            TopLoc_Location L = (Eloc * loc);
            //            if (cr.IsCurve3D())
            //            {
            //                Geom_Curve C = cr.Curve3D();
            //                if (C != null)
            //                { // edge non degenerated
            //                    p3d = C.Value(par);
            //                    p3d.Transform(L.Transformation());
            //                    box.Add(p3d);
            //                }
            //            }
            //            else if (cr.IsCurveOnSurface())
            //            {
            //                Geom_Surface Su = cr.Surface();
            //                Geom2d_Curve PC = cr.PCurve();
            //                Geom2d_Curve PC2 = null;
            //                if (cr.IsCurveOnClosedSurface())
            //                {
            //                    PC2 = cr.PCurve2();
            //                }
            //                gp_Pnt2d p2d = PC.Value(par);
            //                p3d = Su.Value(p2d.X(), p2d.Y());
            //                p3d.Transform(L.Transformation());
            //                box.Add(p3d);
            //                if (PC2 != null)
            //                {
            //                    p2d = PC2.Value(par);
            //                    p3d = Su.Value(p2d.X(), p2d.Y());
            //                    p3d.Transform(L.Transformation());
            //                    box.Add(p3d);
            //                }
            //            }

            //        }
            //    }
            //    double aXmin, aYmin, aZmin, aXmax, aYmax, aZmax;
            //    box.Get(out aXmin, out aYmin, out aZmin, out aXmax, out aYmax, out aZmax);
            //    aXmax -= aXmin; aYmax -= aYmin; aZmax -= aZmin;
            //    tol = Math.Max(tol, Math.Sqrt(aXmax * aXmax + aYmax * aYmax + aZmax * aZmax));
            //    tol += 2.0 * Epsilon(tol);
            //    //
            //    double aVTol = BRep_Tool.Tolerance(V);
            //    bool anUpdTol = tol > aVTol;
            //    BRep_TVertex aTV = (BRep_TVertex)(V.TShape());
            //    bool toAdd = false;
            //    if (IsVerifyTolerance)
            //    {
            //        // ASet minimum value of the tolerance 
            //        // Attention to sharing of the vertex by other shapes      
            //        toAdd = Initialized.Add(aTV) && aVTol != tol; //if Vtol == tol => no need to update toler
            //    }
            //    //'Initialized' map is not used anywhere outside this block
            //    if (anUpdTol || toAdd)
            //        aShToTol.Bind(V, tol);
            //}

            //  UpdShTol(aShToTol, IsMutableInput, theReshaper, true);
        }

        //=======================================================================
        //function : UpdShTol
        //purpose  : Update vertices/edges/faces according to ShToTol map (create copies of necessary)
        //=======================================================================
        static void UpdShTol(TopTools_DataMapOfShapeReal theShToTol,
  bool IsMutableInput, BRepTools_ReShape theReshaper,
  bool theVForceUpdate)
        {
            BRep_Builder aB;
            //TopTools_DataMapIteratorOfDataMapOfShapeReal SHToTolit(theShToTol);
            //for (; SHToTolit.More(); SHToTolit.Next())
            //{
            //    TopoDS_Shape aSh = SHToTolit.Key();
            //    double aTol = SHToTolit.Value();
            //    //
            //    TopoDS_Shape aNsh;
            //    TopoDS_Shape aVsh = theReshaper.Value(aSh);
            //    bool UseOldSh = IsMutableInput || theReshaper.IsNewShape(aSh) || !aVsh.IsSame(aSh);
            //    if (UseOldSh)
            //        aNsh = aVsh;
            //    else
            //    {
            //        aNsh = aSh.EmptyCopied();
            //        //add subshapes from the original shape
            //        TopoDS_Iterator sit(aSh);
            //        for (; sit.More(); sit.Next())
            //            aB.Add(aNsh, sit.Value());
            //        //
            //        aNsh.Free(aSh.Free());
            //        aNsh.Checked(aSh.Checked());
            //        aNsh.Orientable(aSh.Orientable());
            //        aNsh.Closed(aSh.Closed());
            //        aNsh.Infinite(aSh.Infinite());
            //        aNsh.Convex(aSh.Convex());
            //        //
            //    }
            //    //
            //    switch (aSh.ShapeType())
            //    {
            //        case TopAbs_FACE:
            //            {
            //                aB.UpdateFace(TopoDS::Face(aNsh), aTol);
            //                break;
            //            }
            //        case TopAbs_EDGE:
            //            {
            //                aB.UpdateEdge(TopoDS::Edge(aNsh), aTol);
            //                break;
            //            }
            //        case TopAbs_VERTEX:
            //            {
            //                const Handle(BRep_TVertex)&aTV = *((Handle(BRep_TVertex) *) & aNsh.TShape());
            //                //
            //                if (aTV->Locked())
            //                    throw TopoDS_LockedShape("BRep_Builder::UpdateVertex");
            //                //
            //                if (theVForceUpdate)
            //                    aTV->Tolerance(aTol);
            //                else
            //                    aTV->UpdateTolerance(aTol);
            //                aTV->Modified(Standard_True);
            //                break;
            //            }
            //        default:
            //            break;
            //    }
            //    //
            //    if (!UseOldSh)
            //        theReshaper.Replace(aSh, aNsh);
            //}
        }
        public static void UpdateTolerances(TopoDS_Shape S, bool verifyFaceTolerance = false)
        {
            BRepTools_ReShape aReshaper = new BRepTools_ReShape();
            InternalUpdateTolerances(S, verifyFaceTolerance, true, aReshaper);

        }

        public static void SameParameter(TopoDS_Shape S,
    double Tolerance,
    bool forced)
        {
            BRepTools_ReShape reshaper = new BRepTools_ReShape();
            InternalSameParameter(S, reshaper, Tolerance, forced, true);
        }
        public static void InternalSameParameter(TopoDS_Shape theSh, BRepTools_ReShape theReshaper,
  double theTol, bool IsForced, bool IsMutableInput)
        {
            TopExp_Explorer ex = new TopExp_Explorer(theSh, TopAbs_ShapeEnum.TopAbs_EDGE);
            TopTools_MapOfShape Done = new TopTools_MapOfShape();
            BRep_Builder aB = new BRep_Builder();
            TopTools_DataMapOfShapeReal aShToTol;

            //while (ex.More())
            //{
            //    TopoDS_Edge aCE = TopoDS.Edge(ex.Current());
            //    if (Done.Add(aCE))
            //    {
            //        TopoDS_Edge aNE = TopoDS.Edge(theReshaper.Value(aCE));
            //        bool UseOldEdge = IsMutableInput || theReshaper.IsNewShape(aCE) || !aNE.IsSame(aCE);
            //        if (IsForced && (BRep_Tool.SameRange(aCE) || BRep_Tool.SameParameter(aCE)))
            //        {
            //            if (!UseOldEdge)
            //            {
            //                aNE = TopoDS.Edge(aCE.EmptyCopied());
            //                TopoDS_Iterator sit = new TopoDS_Iterator(aCE);
            //                for (; sit.More(); sit.Next())
            //                    aB.Add(aNE, sit.Value());
            //                theReshaper.Replace(aCE, aNE);
            //                UseOldEdge = true;
            //            }
            //            aB.SameRange(aNE, false);
            //            aB.SameParameter(aNE, false);
            //        }
            //        double aNewTol = -1;
            //        TopoDS_Edge aResEdge = BRepLib.SameParameter(aNE, theTol, aNewTol, UseOldEdge);
            //        if (!UseOldEdge && !aResEdge.IsNull())
            //            //NE have been empty-copied
            //            theReshaper.Replace(aNE, aResEdge);
            //        if (aNewTol > 0)
            //        {
            //            TopoDS_Vertex aV1 = new TopoDS_Vertex(), aV2 = new TopoDS_Vertex();
            //            TopExp.Vertices(aCE, ref aV1, ref aV2);
            //            if (!aV1.IsNull())
            //                UpdTolMap(aV1, aNewTol, aShToTol);
            //            if (!aV2.IsNull())
            //                UpdTolMap(aV2, aNewTol, aShToTol);
            //        }
            //    }
            //    ex.Next();
            //}

            Done.Clear();
            BRepAdaptor_Surface BS = new BRepAdaptor_Surface();
            for (ex.Init(theSh, TopAbs_ShapeEnum.TopAbs_FACE); ex.More(); ex.Next())
            {
                TopoDS_Face curface = TopoDS.Face(ex.Current());
                if (!Done.Add(curface))
                    continue;

                BS.Initialize(curface);
                if (BS._GetType() != GeomAbs_SurfaceType.GeomAbs_Plane)
                    continue;

                TopExp_Explorer ex2 = new TopExp_Explorer();
                for (ex2.Init(curface, TopAbs_ShapeEnum.TopAbs_EDGE); ex2.More(); ex2.Next())
                {
                    TopoDS_Edge E = TopoDS.Edge(ex2.Current());
                    //TopoDS_Shape aNe = theReshaper.Value(E);
                    double aNewEtol = -1;
                    //  GetEdgeTol(TopoDS.Edge(aNe), curface, aNewEtol);
                    //  if (aNewEtol >= 0) //not equal to -1
                    //    UpdTolMap(E, aNewEtol, aShToTol);
                }
            }

            //
            //UpdShTol(aShToTol, IsMutableInput, theReshaper, false);

            InternalUpdateTolerances(theSh, false, IsMutableInput, theReshaper);
        }

        private static void GetEdgeTol(TopoDS_Edge topoDS_Edge, TopoDS_Face curface, double aNewEtol)
        {
            throw new NotImplementedException();
        }

        private static void UpdTolMap(TopoDS_Edge e, double aNewEtol, TopTools_DataMapOfShapeReal aShToTol)
        {
            throw new NotImplementedException();
        }

        static double thePrecision = OCCPort.Precision.Confusion();

    }
}