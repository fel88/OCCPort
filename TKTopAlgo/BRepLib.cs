using OCCPort;
using OCCPort.Common;
using TKBRep;
using TKernel;
using TKG2d;
using TKG3d;
using TKGeomAlgo;
using TKGeomBase;
using TKMath;

namespace TKTopAlgo
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
        public static void ReverseSortFaces(TopoDS_Shape Sh, TopTools_ListOfShape LF)
        {
            LF.Clear();
            // Use the allocator of the result LF for intermediate results
            TopTools_ListOfShape LTri = new TopTools_ListOfShape(LF.Allocator()),
                LPlan = new TopTools_ListOfShape(LF.Allocator()),
    LCyl = new TopTools_ListOfShape(LF.Allocator()), LCon = new TopTools_ListOfShape(LF.Allocator()), LSphere = new TopTools_ListOfShape(LF.Allocator()),
    LTor = new TopTools_ListOfShape(LF.Allocator()), LOther = new TopTools_ListOfShape(LF.Allocator());
            TopExp_Explorer exp = new TopExp_Explorer(Sh, TopAbs_ShapeEnum.TopAbs_FACE);
            TopLoc_Location l = new TopLoc_Location();

            for (; exp.More(); exp.Next())
            {
                TopoDS_Face F = TopoDS.Face(exp.Current());
                Geom_Surface S = BRep_Tool.Surface(F, out l);
                if (S != null)
                {
                    GeomAdaptor_Surface AS = new GeomAdaptor_Surface(S);
                    switch (AS._GetType())
                    {
                        case GeomAbs_SurfaceType.GeomAbs_Plane:
                            {
                                LPlan.Append(F);
                                break;
                            }
                            //case GeomAbs_Cylinder:
                            //    {
                            //        LCyl.Append(F);
                            //        break;
                            //    }
                            //case GeomAbs_Cone:
                            //    {
                            //        LCon.Append(F);
                            //        break;
                            //    }
                            //case GeomAbs_Sphere:
                            //    {
                            //        LSphere.Append(F);
                            //        break;
                            //    }
                            //case GeomAbs_Torus:
                            //    {
                            //        LTor.Append(F);
                            //        break;
                            //    }
                            //default:
                            //    LOther.Append(F);
                    }
                }
                else LTri.Append(F);
            }
            LF.Append(LTri); LF.Append(LOther); LF.Append(LTor); LF.Append(LSphere);
            LF.Append(LCon); LF.Append(LCyl); LF.Append(LPlan);

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
                                    tol = TKMath.Precision.Confusion();
                                    break;
                                }
                            case GeomAbs_SurfaceType.GeomAbs_Sphere:
                            case GeomAbs_SurfaceType.GeomAbs_Torus:
                                {
                                    tol = TKMath.Precision.Confusion() * 2;
                                    break;
                                }
                            default:
                                tol = TKMath.Precision.Confusion() * 4;
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

        static double thePrecision = TKMath.Precision.Confusion();




        //! Default size for memory block allocated by IncAllocator. 
        /**
        * The idea here is that blocks of the given size are returned to the system
        * rather than retained in the malloc heap, at least on WIN32 and WIN64 platforms.
        */
        //# ifdef _WIN64
        public const int MEMORY_BLOCK_SIZE_HUGE = 1024 * 1024;
        //#else
        //      public const size_t MEMORY_BLOCK_SIZE_HUGE = 512 * 1024;
        //#endif

    }
    
    //! Provides Constructors with a Face.
    public class BRepClass_FaceClassifier : BRepClass_FClassifier
    {
        //! Classify  the Point  P  with  Tolerance <T> on the
        //! face described by <F>.
        //! Recommended to use Bnd_Box if the number of edges > 10
        //! and the geometry is mostly spline
        public void Perform(TopoDS_Face theF, gp_Pnt2d theP, double theTol,
                   bool theUseBndBox = false, double theGapCheckTol = 0.1)
        {
            BRepClass_FaceExplorer aFex = new BRepClass_FaceExplorer(theF);
            aFex.SetMaxTolerance(theGapCheckTol);
            aFex.SetUseBndBox(theUseBndBox);
            throw new Standard_NotImplemented();
            //base.Perform(aFex, theP, theTol);
        }

    }

    public class BRepClass_FClassifier
    {
        //! Returns the result of the classification.
        public TopAbs_State State()
        {
            throw new Standard_NotImplemented();
        }

    }

    //! Provides an  algorithm to find  a Surface  through a
    //! set of edges.
    //!
    //! The edges  of  the  shape  given  as  argument are
    //! explored if they are not coplanar at  the required
    //! tolerance  the method Found returns false.
    //!
    //! If a null tolerance is given the max of the  edges
    //! tolerances is used.
    //!
    //! The method Tolerance returns the true distance  of
    //! the edges to the Surface.
    //!
    //! The method Surface returns the Surface if found.
    //!
    //! The method Existed  returns returns  True  if  the
    //! Surface was already attached to some of the edges.
    //!
    //! When Existed  returns True  the  Surface  may have a
    //! location given by the Location method.
    public class BRepLib_FindSurface
    {

        public Geom_Surface Surface()
        {
            return mySurface;
        }
        public TopLoc_Location Location()
        {
            return myLocation;
        }

        public double Tolerance()
        {
            return myTolerance;
        }
        //! Computes the Surface from the edges of  <S> with the
        //! given tolerance.
        //! if <OnlyPlane> is true, the computed surface will be
        //! a plane. If it is not possible to find a plane, the
        //! flag NotDone will be set.
        //! If <OnlyClosed> is true,  then  S  should be a wire
        //! and the existing surface,  on  which wire S is not
        //! closed in 2D, will be ignored.
        public BRepLib_FindSurface(TopoDS_Shape S, double Tol = -1, bool OnlyPlane = false,
            bool OnlyClosed = false)
        {
            Init(S, Tol, OnlyPlane, OnlyClosed);
        }
        Geom_Surface mySurface;
        double myTolerance;
        double myTolReached;
        bool isExisted;
        TopLoc_Location myLocation = new TopLoc_Location();



        public void Init(TopoDS_Shape S,
                                     double Tol,

                                     bool OnlyPlane,
                               bool OnlyClosed)
        {
            myTolerance = Tol;
            myTolReached = 0.0;
            isExisted = false;
            myLocation.Identity();
            mySurface = null;

            // compute the tolerance
            TopExp_Explorer ex = new TopExp_Explorer();

            for (ex.Init(S, TopAbs_ShapeEnum.TopAbs_EDGE); ex.More(); ex.Next())
            {
                double t = BRep_Tool.Tolerance(TopoDS.Edge(ex.Current()));
                if (t > myTolerance)
                    myTolerance = t;

            }

            // search an existing surface
            ex.Init(S, TopAbs_ShapeEnum.TopAbs_EDGE);
            if (!ex.More()) return;    // no edges ....

            TopoDS_Edge E = TopoDS.Edge(ex.Current());
            double f = 0, l = 0, ff = 0, ll = 0;
            Geom2d_Curve PC = null, aPPC = null;
            Geom_Surface SS = null;
            TopLoc_Location L = new TopLoc_Location();
            int i = 0, j;

            // iterate on the surfaces of the first edge
            for (; ; )
            {
                i++;
                BRep_Tool.CurveOnSurface(E, ref PC, ref mySurface, ref myLocation, ref f, ref l, i);
                if (mySurface == null)
                {
                    break;
                }
                // check the other edges
                for (ex.Init(S, TopAbs_ShapeEnum.TopAbs_EDGE); ex.More(); ex.Next())
                {
                    if (!E.IsSame(ex.Current()))
                    {
                        j = 0;
                        for (; ; )
                        {
                            j++;
                            BRep_Tool.CurveOnSurface(TopoDS.Edge(ex.Current()), ref aPPC, ref SS, ref L, ref ff, ref ll, j);
                            if (SS == null)
                            {
                                break;
                            }
                            if ((SS == mySurface) && (L.IsEqual(myLocation)))
                            {
                                break;
                            }
                            SS = null;
                        }

                        if (SS == null)
                        {
                            mySurface = null;
                            break;
                        }
                    }
                }

                // if OnlyPlane, eval if mySurface is a plane.
                if (OnlyPlane && mySurface != null)
                {
                    if (mySurface is Geom_RectangularTrimmedSurface)
                        mySurface = ((Geom_RectangularTrimmedSurface)mySurface).BasisSurface();
                    mySurface = (Geom_Plane)mySurface;
                }

                //if (!mySurface.IsNull())
                //    // if S is e.g. the bottom face of a cylinder, mySurface can be the
                //    // lateral (cylindrical) face of the cylinder; reject an improper mySurface
                //    if (!OnlyClosed || Is2DClosed(S, mySurface, myLocation))
                //        break;
            }

            if (mySurface != null)
            {
                isExisted = true;
                return;
            }
            //
            // no existing surface, search a plane
            // 07/02/02 akm vvv : (OCC157) changed algorithm
            //                    1. Collect the points along all edges of the shape
            //                       For each point calculate the WEIGHT = sum of
            //                       distances from neighboring points (_only_ same edge)
            //                    2. Minimizing the weighed sum of squared deviations
            //                       compute coefficients of the sought plane.

            TColgp_SequenceOfPnt aPoints = new TColgp_SequenceOfPnt();
            TColStd_SequenceOfReal aWeight = new TColStd_SequenceOfReal();

            // ======================= Step #1
            for (ex.Init(S, TopAbs_ShapeEnum.TopAbs_EDGE); ex.More(); ex.Next())
            {
                BRepAdaptor_Curve c = new BRepAdaptor_Curve(TopoDS.Edge(ex.Current()));

                double dfUf = c.FirstParameter();
                double dfUl = c.LastParameter();
                if (Standard_Real.IsEqual(dfUf, dfUl))
                {
                    // Degenerate
                    continue;
                }
                int iNbPoints = 0;

                // Fill the parameters of the sampling points
                NCollection_Vector<double> aParams = new NCollection_Vector<double>();
                switch (c._GetType())
                {
                    //case GeomAbs_BezierCurve:
                    //    {
                    //        Handle(Geom_BezierCurve) GC = c.Bezier();
                    //        TColStd_Array1OfReal aKnots(1, 2);
                    //        aKnots.SetValue(1, GC->FirstParameter());
                    //        aKnots.SetValue(2, GC->LastParameter());

                    //        fillParams(aKnots, GC->Degree(), dfUf, dfUl, aParams);
                    //        break;
                    //    }
                    //case GeomAbs_BSplineCurve:
                    //    {
                    //        Handle(Geom_BSplineCurve) GC = c.BSpline();
                    //        fillParams(GC->Knots(), GC->Degree(), dfUf, dfUl, aParams);
                    //        break;
                    //    }
                    case GeomAbs_CurveType.GeomAbs_Line:
                        {
                            // Two points on a straight segment
                            aParams.Append(dfUf);
                            aParams.Append(dfUl);
                            break;
                        }
                    //case GeomAbs_Circle:
                    //case GeomAbs_Ellipse:
                    //case GeomAbs_Hyperbola:
                    //case GeomAbs_Parabola:
                    //    // Four points on other analytical curves
                    //    iNbPoints = 4;
                    //    Standard_FALLTHROUGH
                    default:
                        {
                            // Put some points on other curves
                            if (iNbPoints == 0)
                                iNbPoints = 15 + c.NbIntervals(GeomAbs_Shape.GeomAbs_C3);

                            //  TColStd_Array1OfReal aBounds = new TColStd_Array1OfReal(1, 2);
                            // aBounds.SetValue(1, dfUf);
                            //  aBounds.SetValue(2, dfUl);

                            // fillParams(aBounds, iNbPoints - 1, dfUf, dfUl, aParams);
                            break;
                        }

                }

                // Add the points with weights to the sequences
                fillPoints(c, aParams, aPoints, aWeight);
            }

            if (aPoints.Length() < 3)
            {
                return;
            }

            // ======================= Step #2
            myLocation.Identity();
            int iPoint;
            math_Matrix aMat = new math_Matrix(1, 3, 1, 3, 0.0);
            math_Vector aVec = new math_Vector(1, 3, 0.0);
            // Find the barycenter and normalize weights 
            double dfMaxWeight = 0.0;
            gp_XYZ aBaryCenter = new gp_XYZ(0.0, 0.0, 0.0);
            double dfSumWeight = 0.0;
            for (iPoint = 1; iPoint <= aPoints.Length(); iPoint++)
            {
                double dfW = aWeight[iPoint];
                aBaryCenter += aPoints[iPoint].XYZ() * dfW;
                dfSumWeight += dfW;
                if (dfW > dfMaxWeight)
                {
                    dfMaxWeight = dfW;
                }
            }
            aBaryCenter /= dfSumWeight;

            // Fill the matrix and the right vector
            for (iPoint = 1; iPoint <= aPoints.Length(); iPoint++)
            {
                gp_XYZ p = aPoints[iPoint].XYZ() - aBaryCenter;
                double w = aWeight[iPoint] / dfMaxWeight;
                aMat[1, 1] += w * p.X() * p.X();
                aMat[1, 2] += w * p.X() * p.Y();
                aMat[1, 3] += w * p.X() * p.Z();
                //  
                aMat[2, 2] += w * p.Y() * p.Y();
                aMat[2, 3] += w * p.Y() * p.Z();
                //  
                aMat[3, 3] += w * p.Z() * p.Z();
            }
            aMat[2, 1] = aMat[1, 2];
            aMat[3, 1] = aMat[1, 3];
            aMat[3, 2] = aMat[2, 3];
            //
            math_Jacobi anEignval = new math_Jacobi(aMat);
            math_Vector anEVals = new math_Vector(1, 3);
            bool isSolved = anEignval.IsDone();
            int isol = 0;
            if (isSolved)
            {
                anEVals = anEignval.Values();
                //We need vector with eigenvalue ~ 0.
                double anEMin = Standard_Real.RealLast();
                double anEMax = -anEMin;
                for (i = 1; i <= 3; ++i)
                {
                    double anE = Math.Abs(anEVals[i]);
                    if (anEMin > anE)
                    {
                        anEMin = anE;
                        isol = i;
                    }
                    if (anEMax < anE)
                    {
                        anEMax = anE;
                    }
                }

                if (isol == 0)
                {
                    isSolved = false;
                }
                else
                {
                    double eps1 = Standard_Real.Epsilon(anEMax);

                    if (anEMin <= eps1)
                    {
                        anEignval.Vector(isol, ref aVec);
                    }
                    else
                    {
                        //try using vector product of other axes
                        int[] ind = { 0, 0 };
                        for (i = 1; i <= 3; ++i)
                        {
                            if (i == isol)
                            {
                                continue;
                            }
                            if (ind[0] == 0)
                            {
                                ind[0] = i;
                                continue;
                            }
                            if (ind[1] == 0)
                            {
                                ind[1] = i;
                                continue;
                            }
                        }
                        math_Vector aVec1 = new math_Vector(1, 3, 0.0), aVec2 = new math_Vector(1, 3, 0.0);
                        //      anEignval.Vector(ind[0], aVec1);
                        //    anEignval.Vector(ind[1], aVec2);
                        gp_Vec aV1 = new gp_Vec(aVec1[1], aVec1[2], aVec1[3]);
                        gp_Vec aV2 = new gp_Vec(aVec2[1], aVec2[2], aVec2[3]);
                        gp_Vec _aN = aV1 ^ aV2;
                        aVec[1] = _aN.X();
                        aVec[2] = _aN.Y();
                        aVec[3] = _aN.Z();
                    }
                    if (aVec.Norm2() < gp.Resolution())
                    {
                        isSolved = false;
                    }
                }
            }

            if (!isSolved)
                return;

            //Removing very small values
            double aMaxV = Math.Max(Math.Abs(aVec[1]), Math.Max(Math.Abs(aVec[2]), Math.Abs(aVec[3])));
            double eps = Standard_Real.Epsilon(aMaxV);
            for (i = 1; i <= 3; ++i)
            {
                if (Math.Abs(aVec[i]) <= eps)
                    aVec[i] = 0.0;
            }
            gp_Vec aN = new gp_Vec(aVec[1], aVec[2], aVec[3]);
            Geom_Plane aPlane = new Geom_Plane(aBaryCenter, aN);
            myTolReached = Controle(aPoints, aPlane);
            double aWeakness = 5.0;
            if (myTolReached <= myTolerance || (Tol < 0 && myTolReached < myTolerance * aWeakness))
            {
                mySurface = aPlane;
                //If S is wire, try to orient surface according to orientation of wire.
                if (S.ShapeType() == TopAbs_ShapeEnum.TopAbs_WIRE && S.Closed())
                {
                    TopoDS_Wire aW = TopoDS.Wire(S);
                    TopoDS_Face aTmpFace = new BRepLib_MakeFace(mySurface, Precision.Confusion());
                    BRep_Builder BB = new BRep_Builder();
                    BB.Add(aTmpFace, aW);
                    BRepTopAdaptor_FClass2d FClass = new BRepTopAdaptor_FClass2d(aTmpFace, 0.0);
                    if (FClass.PerformInfinitePoint() == TopAbs_State.TopAbs_IN)
                    {
                        gp_Dir aNorm = aPlane.Position().Direction();
                        aNorm.Reverse();
                        mySurface = new Geom_Plane(aPlane.Position().Location(), aNorm);
                    }
                }
            }
        }

        public static double Controle(TColgp_SequenceOfPnt thePoints,
     Geom_Plane thePlane)
        {
            double dfMaxDist = 0.0;
            double a = 0, b = 0, c = 0, d = 0, dist;
            int ii;
            thePlane.Coefficients(ref a, ref b, ref c, ref d);
            for (ii = 1; ii <= thePoints.Length(); ii++)
            {
                gp_XYZ xyz = thePoints.Value(ii).XYZ();
                dist = Math.Abs(a * xyz.X() + b * xyz.Y() + c * xyz.Z() + d);
                if (dist > dfMaxDist)
                    dfMaxDist = dist;
            }

            return dfMaxDist;
        }
        private void fillPoints(BRepAdaptor_Curve theCurve, NCollection_Vector<double> theParams,
                    TColgp_SequenceOfPnt thePoints, TColStd_SequenceOfReal theWeights)
        {
            double aDistPrev = 0.0, aDistNext;
            gp_Pnt aPPrev = (theCurve.Value(theParams[0]));
            gp_Pnt aPNext = new gp_Pnt();

            for (int iP = 1; iP <= theParams.Length(); ++iP)
            {
                if (iP < theParams.Length())
                {
                    double aParam = theParams[iP];
                    aPNext = theCurve.Value(aParam);
                    aDistNext = aPPrev.Distance(aPNext);
                }
                else
                    aDistNext = 0.0;

                thePoints.Append(aPPrev);
                theWeights.Append(aDistPrev + aDistNext);
                aDistPrev = aDistNext;
                aPPrev = aPNext;
            }

        }

        public bool Found()
        {
            return mySurface != null;
        }
        public double ToleranceReached()
        {
            return myTolReached;
        }



    }

    public class BRepTopAdaptor_FClass2d
    {

        BRepTopAdaptor_SeqOfPtr TabClass = new BRepTopAdaptor_SeqOfPtr();
        TColStd_SequenceOfInteger TabOrien = new TColStd_SequenceOfInteger();
        double Toluv;
        TopoDS_Face Face;
        double U1;
        double V1;
        double U2;
        double V2;
        double Umin;
        double Umax;
        double Vmin;
        double Vmax;


        public BRepTopAdaptor_FClass2d(TopoDS_Face aFace, double TolUV)

        {
            Toluv = TolUV;
            Face = (aFace);
            U1 = (0.0);
            V1 = (0.0);
            U2 = (0.0);
            V2 = 0.0;
#if LBRCOMPT
  STAT.NbConstrShape++;
#endif

            //-- dead end on surfaces defined on more than one period

            Face.Orientation(TopAbs_Orientation.TopAbs_FORWARD);
            BRepAdaptor_Surface surf = new BRepAdaptor_Surface();
            surf.Initialize(aFace, false);

            TopoDS_Edge edge;
            TopAbs_Orientation Or;
            double u, du, Tole = 0.0, Tol = 0.0;
            BRepTools_WireExplorer WireExplorer = new BRepTools_WireExplorer();
            TopExp_Explorer FaceExplorer = new TopExp_Explorer();

            Umin = Vmin = 0.0; //RealLast();
            Umax = Vmax = -Umin;

            int aNbE = 0;
            double eps = 1e-10;
            int BadWire = 0;
            for (FaceExplorer.Init(Face, TopAbs_ShapeEnum.TopAbs_WIRE); (FaceExplorer.More() && BadWire == 0); FaceExplorer.Next())
            {
                int nbpnts = 0;
                TColgp_SequenceOfPnt2d SeqPnt2d = new TColgp_SequenceOfPnt2d();
                int firstpoint = 1;
                double FlecheU = 0.0;
                double FlecheV = 0.0;
                bool WireIsNotEmpty = false;
                int NbEdges = 0;

                TopExp_Explorer Explorer = new TopExp_Explorer();
                for (Explorer.Init(FaceExplorer.Current(), TopAbs_ShapeEnum.TopAbs_EDGE); Explorer.More(); Explorer.Next()) NbEdges++;
                aNbE = NbEdges;

                gp_Pnt Ancienpnt3d = new(0, 0, 0);
                bool Ancienpnt3dinitialise = false;

                for (WireExplorer.Init(TopoDS.Wire(FaceExplorer.Current()), Face); WireExplorer.More(); WireExplorer.Next())
                {

                    NbEdges--;
                    edge = (TopoDS_Edge)WireExplorer.Current();
                    Or = edge.Orientation();
                    if (Or == TopAbs_Orientation.TopAbs_FORWARD || Or == TopAbs_Orientation.TopAbs_REVERSED)
                    {
                        double pfbid = 0, plbid = 0;
                        if (BRep_Tool.CurveOnSurface(edge, Face, ref pfbid, ref plbid) == null)
                            return;

                        BRepAdaptor_Curve2d C = new BRepAdaptor_Curve2d(edge, Face);

                        //                        //-- ----------------------------------------
                        bool degenerated = false;
                        if (BRep_Tool.Degenerated(edge)) degenerated = true;
                        if (BRep_Tool.IsClosed(edge, Face)) degenerated = true;
                        TopoDS_Vertex Va = new TopoDS_Vertex(), Vb = new TopoDS_Vertex();
                        TopExp.Vertices(edge, ref Va, ref Vb);
                        double TolVertex1 = 0.0, TolVertex = 0.0;
                        if (Va.IsNull()) degenerated = true;
                        else TolVertex1 = BRep_Tool.Tolerance(Va);
                        if (Vb.IsNull()) degenerated = true;
                        else TolVertex = BRep_Tool.Tolerance(Vb);
                        if (TolVertex < TolVertex1) TolVertex = TolVertex1;
                        BRepAdaptor_Curve C3d = new BRepAdaptor_Curve();

                        if (Math.Abs(plbid - pfbid) < 1e-9)
                            continue;

                        //if(degenerated==false)
                        //  C3d.Initialize(edge,Face);

                        //-- Check cases when it was forgotten to code degenerated :  PRO17410 (janv 99)
                        if (degenerated == false)
                        {
                            C3d.Initialize(edge, Face);
                            du = (plbid - pfbid) * 0.1;
                            u = pfbid + du;
                            gp_Pnt P3da = C3d.Value(u);
                            degenerated = true;
                            u += du;
                            do
                            {

                                gp_Pnt P3db = C3d.Value(u);
                                // 		      if(P3da.SquareDistance(P3db)) { degenerated=false; break; }
                                if (P3da.SquareDistance(P3db) > Precision.Confusion()) { degenerated = false; break; }
                                u += du;
                            }
                            while (u < plbid);
                        }

                        //-- ----------------------------------------

                        Tole = BRep_Tool.Tolerance(edge);
                        if (Tole > Tol) Tol = Tole;

                        //int nbs = 1 + Geom2dInt_Geom2dCurveTool.NbSamples(C);
                        int nbs = Geom2dInt_Geom2dCurveTool.NbSamples(C);
                        //-- Attention to rational bsplines of degree 3. (ends of circles among others)
                        if (nbs > 2) nbs *= 4;
                        du = (plbid - pfbid) / (double)(nbs - 1);

                        if (Or == TopAbs_Orientation.TopAbs_FORWARD) u = pfbid;
                        else { u = plbid; du = -du; }

                        //                        //-- ------------------------------------------------------------
                        //                        //-- Check distance uv between the start point of the edge
                        //                        //-- and the last point registered in SeqPnt2d
                        //                        //-- Try to remote the first point of the current edge 
                        //                        //-- from the last saved point
                        //                        //# ifdef OCCT_DEBUG
                        //                        //                        gp_Pnt2d Pnt2dDebutEdgeCourant = C.Value(u); (void)Pnt2dDebutEdgeCourant;
                        //                        //#endif

                        //                        //double Baillement2dU=0;
                        //                        //double Baillement2dV=0;
                        //#if AFFICHAGE
                        //	      if(nbpnts>1) printf("\nTolVertex %g ",TolVertex);
                        //#endif

                        if (firstpoint == 2) u += du;
                        int Avant = nbpnts;
                        for (int e = firstpoint; e <= nbs; e++)
                        {
                            gp_Pnt2d P2d = C.Value(u);
                            if (P2d.X() < Umin) Umin = P2d.X();
                            if (P2d.X() > Umax) Umax = P2d.X();
                            if (P2d.Y() < Vmin) Vmin = P2d.Y();
                            if (P2d.Y() > Vmax) Vmax = P2d.Y();

                            double dist3dptcourant_ancienpnt = 1e+20;//RealLast();
                            gp_Pnt P3d = new gp_Pnt();
                            if (degenerated == false)
                            {
                                P3d = C3d.Value(u);
                                if (nbpnts > 1 && Ancienpnt3dinitialise) dist3dptcourant_ancienpnt = P3d.Distance(Ancienpnt3d);
                            }
                            bool IsRealCurve3d = true; //patch
                            if (dist3dptcourant_ancienpnt < Precision.Confusion())
                            {
                                gp_Pnt MidP3d = C3d.Value(u - du / 2.0);
                                if (P3d.Distance(MidP3d) < Precision.Confusion()) IsRealCurve3d = false;
                            }
                            if (IsRealCurve3d)
                            {
                                if (degenerated == false) { Ancienpnt3d = P3d; Ancienpnt3dinitialise = true; }
                                nbpnts++;
                                SeqPnt2d.Append(P2d);
                            }
                            //#if AFFICHAGE
                            //                  else { static int mm=0; printf("\npoint p%d  %g %g %g",++mm,P3d.X(),P3d.Y(),P3d.Z());	}
                            //#endif
                            u += du;
                            int ii = nbpnts;
                            //-- printf("\n nbpnts:%4d  u=%7.5g   FlecheU=%7.5g  FlecheV=%7.5g  ii=%3d  Avant=%3d ",nbpnts,u,FlecheU,FlecheV,ii,Avant);
                            // 		  if(ii>(Avant+4))
                            //  Modified by Sergey KHROMOV - Fri Apr 19 09:46:12 2002 Begin
                            if (ii > (Avant + 4) && SeqPnt2d[ii - 2].SquareDistance(SeqPnt2d[ii]) != 0)
                            //  Modified by Sergey KHROMOV - Fri Apr 19 09:46:13 2002 End
                            {
                                gp_Lin2d Lin = new gp_Lin2d(SeqPnt2d[ii - 2], new gp_Dir2d(new gp_Vec2d(SeqPnt2d[ii - 2], SeqPnt2d[ii])));
                                double ul = ElCLib.Parameter(Lin, SeqPnt2d[ii - 1]);
                                gp_Pnt2d Pp = ElCLib.Value(ul, Lin);
                                double dU = Math.Abs(Pp.X() - SeqPnt2d[ii - 1].X());
                                double dV = Math.Abs(Pp.Y() - SeqPnt2d[ii - 1].Y());
                                //-- printf(" (du=%7.5g   dv=%7.5g)",dU,dV);
                                if (dU > FlecheU) FlecheU = dU;
                                if (dV > FlecheV) FlecheV = dV;
                            }
                        }//for(e=firstpoint
                        if (firstpoint == 1) firstpoint = 2;
                        WireIsNotEmpty = true;
                    }//if(Or==FORWARD,REVERSED
                } //-- Edges -> for(Ware.Explorer

                if (NbEdges != 0)
                { //-- on compte ++ with a normal explorer and with the Wire Explorer
                    ///*
                    //#ifdef OCCT_DEBUG

                    //      std.cout << std.endl;
                    //      std.cout << "*** BRepTopAdaptor_Fclass2d  ** Wire Probablement FAUX **" << std.endl;
                    //      std.cout << "*** WireExplorer does not find all edges " << std.endl;
                    //      std.cout << "*** Connect old classifier" << std.endl;
                    //#endif
                    //*/
                    TColgp_Array1OfPnt2d PClass = new TColgp_Array1OfPnt2d(1, 2);
                    //// modified by jgv, 28.04.2009 ////
                    PClass.Init(new gp_Pnt2d(0.0, 0.0));
                    /////////////////////////////////////
                    TabClass.Append(new CSLib_Class2d(PClass, FlecheU, FlecheV, Umin, Vmin, Umax, Vmax));
                    BadWire = 1;
                    TabOrien.Append(-1);
                }
                else if (WireIsNotEmpty)
                {
                    //double anglep=0,anglem=0;
                    TColgp_Array1OfPnt2d PClass = new TColgp_Array1OfPnt2d(1, nbpnts);
                    double square = 0.0;

                    //-------------------------------------------------------------------
                    //-- ** The mode of calculation was somewhat changed 
                    //-- Before Oct 31 97 , the total angle of  
                    //-- rotation of the wire was evaluated on all angles except for the last 
                    //-- ** Now, exactly the angle of rotation is evaluated
                    //-- If a value remote from 2PI or -2PI is found, it means that there is 
                    //-- an uneven number of loops

                    if (nbpnts > 3)
                    {
                        //	      int im2=nbpnts-2;
                        int im1 = nbpnts - 1;
                        int im0 = 1;
                        //	      PClass(im2)=SeqPnt2d.Value(im2);
                        PClass[im1] = SeqPnt2d.Value(im1);
                        PClass[nbpnts] = SeqPnt2d.Value(nbpnts);

                        double aPer = 0.0;
                        //	      for(int ii=1; ii<nbpnts; ii++,im0++,im1++,im2++)
                        for (int ii = 1; ii < nbpnts; ii++, im0++, im1++)
                        {
                            //		  if(im2>=nbpnts) im2=1;
                            if (im1 >= nbpnts) im1 = 1;
                            PClass[ii] = SeqPnt2d.Value(ii);
                            //		  gp_Vec2d A(PClass(im2),PClass(im1));
                            //		  gp_Vec2d B(PClass(im1),PClass(im0));
                            //		  double N = A.Magnitude() * B.Magnitude();

                            square += (PClass[im0].X() - PClass[im1].X()) * (PClass[im0].Y() + PClass[im1].Y()) * .5;
                            aPer += (PClass[im0].XY() - PClass[im1].XY()).Modulus();

                            //		  if(N>1e-16){ double a=A.Angle(B); angle+=a; }
                        }

                        double anExpThick = Math.Max(2.0 * Math.Abs(square) / aPer, 1e-7);
                        double aDefl = Math.Max(FlecheU, FlecheV);
                        double aDiscrDefl = Math.Min(aDefl * 0.1, anExpThick * 10.0);
                        while (aDefl > anExpThick && aDiscrDefl > 1e-7)
                        {
                            //                            // Deflection of the polygon is too much for this ratio of area and perimeter,
                            //                            // and this might lead to self-intersections.
                            //                            // Discretize the wire more tightly to eliminate the error.
                            //                            firstpoint = 1;
                            //                            SeqPnt2d.Clear();
                            //                            FlecheU = 0.0;
                            //                            FlecheV = 0.0;
                            for (WireExplorer.Init(TopoDS.Wire(FaceExplorer.Current()), Face);
                              WireExplorer.More(); WireExplorer.Next())
                            {
                                edge = (TopoDS_Edge)WireExplorer.Current();
                                Or = edge.Orientation();
                                if (Or == TopAbs_Orientation.TopAbs_FORWARD || Or == TopAbs_Orientation.TopAbs_REVERSED)
                                {
                                    //double pfbid, plbid;
                                    //BRep_Tool.Range(edge, Face, pfbid, plbid);
                                    //if (Abs(plbid - pfbid) < 1.e - 9) continue;
                                    //BRepAdaptor_Curve2d C(edge, Face);
                                    //GCPnts_QuasiUniformDeflection aDiscr(C, aDiscrDefl);
                                    //if (!aDiscr.IsDone())
                                    //    break;
                                    //int nbp = aDiscr.NbPoints();
                                    //int iStep = 1, i = 1, iEnd = nbp + 1;
                                    //if (Or == TopAbs_REVERSED)
                                    //{
                                    //    iStep = -1;
                                    //    i = nbp;
                                    //    iEnd = 0;
                                    //}
                                    //if (firstpoint == 2)
                                    //    i += iStep;
                                    //for (; i != iEnd; i += iStep)
                                    //{
                                    //    gp_Pnt2d aP2d = C.Value(aDiscr.Parameter(i));
                                    //    SeqPnt2d.Append(aP2d);
                                    //}
                                    //if (nbp > 2)
                                    //{
                                    //    int ii = SeqPnt2d.Length();
                                    //    gp_Lin2d Lin = new gp_Lin2d(SeqPnt2d(ii - 2), gp_Dir2d(gp_Vec2d(SeqPnt2d(ii - 2), SeqPnt2d(ii))));
                                    //    double ul = ElCLib.Parameter(Lin, SeqPnt2d(ii - 1));
                                    //    gp_Pnt2d Pp = ElCLib.Value(ul, Lin);
                                    //    double dU = Abs(Pp.X() - SeqPnt2d(ii - 1).X());
                                    //    double dV = Abs(Pp.Y() - SeqPnt2d(ii - 1).Y());
                                    //    if (dU > FlecheU) FlecheU = dU;
                                    //    if (dV > FlecheV) FlecheV = dV;
                                    //}
                                    firstpoint = 2;
                                }
                            }
                            nbpnts = SeqPnt2d.Length();
                            PClass.Resize(1, nbpnts, false);
                            im1 = nbpnts - 1;
                            im0 = 1;
                            PClass[im1] = SeqPnt2d.Value(im1);
                            PClass[nbpnts] = SeqPnt2d.Value(nbpnts);
                            square = 0.0;
                            aPer = 0.0;
                            for (int ii = 1; ii < nbpnts; ii++, im0++, im1++)
                            {
                                if (im1 >= nbpnts) im1 = 1;
                                PClass[ii] = SeqPnt2d.Value(ii);
                                square += (PClass[im0].X() - PClass[im1].X()) * (PClass[im0].Y() + PClass[im1].Y()) * .5;
                                aPer += (PClass[im0].XY() - PClass[im1].XY()).Modulus();
                            }

                            anExpThick = Math.Max(2.0 * Math.Abs(square) / aPer, 1e-7);
                            aDefl = Math.Max(FlecheU, FlecheV);
                            aDiscrDefl = Math.Min(aDiscrDefl * 0.1, anExpThick * 10.0);
                        }

                        //-- FlecheU*=10.0;
                        //-- FlecheV*=10.0;
                        if (aNbE == 1 && FlecheU < eps && FlecheV < eps && Math.Abs(square) < eps)
                        {
                            TabOrien.Append(1);
                        }
                        else
                        {
                            TabOrien.Append(((square < 0.0) ? 1 : 0));
                        }

                        if (FlecheU < Toluv) FlecheU = Toluv;
                        if (FlecheV < Toluv) FlecheV = Toluv;
                        //-- std.cout<<" U:"<<FlecheU<<" V:"<<FlecheV<<std.endl;
                        TabClass.Append(new CSLib_Class2d(PClass, FlecheU, FlecheV, Umin, Vmin, Umax, Vmax));

                        //	      if((angle<2 && angle>-2)||(angle>10)||(angle<-10))
                        //		{
                        //		  BadWire=1;
                        //		  TabOrien.Append(-1);
                        //#ifdef OCCT_DEBUG
                        //		  std.cout << std.endl;
                        //		  std.cout << "*** BRepTopAdaptor_Fclass2d  ** Wire Probably FALSE **" << std.endl;
                        //		  std.cout << "*** Total rotation angle of the wire : " << angle << std.endl;
                        //		  std.cout << "*** Connect the old classifier" << std.endl;
                        //#endif
                        //		} 
                        //	      else TabOrien.Append(((angle>0.0)? 1 : 0));
                    }//if(nbpoints>3


                    else
                    {
                        //# ifdef OCCT_DEBUG
                        //                        std.cout << std.endl;
                        //                        std.cout << "*** BRepTopAdaptor_Fclass2d  ** Wire Probably FALSE **" << std.endl;
                        //                        std.cout << "*** The sample wire contains less than 3 points" << std.endl;
                        //                        std.cout << "*** Connect the old classifier" << std.endl;
                        //#endif
                        BadWire = 1;
                        TabOrien.Append(-1);
                        TColgp_Array1OfPnt2d xPClass = new TColgp_Array1OfPnt2d(1, 2);
                        xPClass[1] = SeqPnt2d[1];
                        xPClass[2] = SeqPnt2d[2];
                        TabClass.Append(new CSLib_Class2d(xPClass, FlecheU, FlecheV, Umin, Vmin, Umax, Vmax));
                    }
                }//else if(WareIsNotEmpty
            }//for(FaceExplorer

            int nbtabclass = TabClass.Length();

            if (nbtabclass > 0)
            {
                //    //-- If an error was detected on a wire: set all TabOrien to -1
                if (BadWire != 0) TabOrien[1] = -1;

                if (surf._GetType() == GeomAbs_SurfaceType.GeomAbs_Cone
               || surf._GetType() == GeomAbs_SurfaceType.GeomAbs_Cylinder
               || surf._GetType() == GeomAbs_SurfaceType.GeomAbs_Torus
               || surf._GetType() == GeomAbs_SurfaceType.GeomAbs_Sphere
               || surf._GetType() == GeomAbs_SurfaceType.GeomAbs_SurfaceOfRevolution)

                {
                    double uuu = Math.PI + Math.PI - (Umax - Umin);
                    if (uuu < 0) uuu = 0;
                    U1 = 0.0;  // modified by NIZHNY-OFV  Thu May 31 14:24:10 2001 ---> //Umin-uuu*0.5;
                    U2 = 2 * Math.PI; // modified by NIZHNY-OFV  Thu May 31 14:24:35 2001 ---> //U1+M_PI+M_PI;
                }
                else { U1 = U2 = 0.0; }

                if (surf._GetType() == GeomAbs_SurfaceType.GeomAbs_Torus)
                {
                    double uuu = Math.PI + Math.PI - (Vmax - Vmin);
                    if (uuu < 0) uuu = 0;
                    V1 = 0.0;  // modified by NIZHNY-OFV  Thu May 31 14:24:55 2001 ---> //Vmin-uuu*0.5;
                    V2 = 2 * Math.PI; // modified by NIZHNY-OFV  Thu May 31 14:24:59 2001 ---> //V1+M_PI+M_PI;
                }
                else { V1 = V2 = 0.0; }
            }
        }
        //   BRepTopAdaptor_SeqOfPtr TabClass;
        //TColStd_SequenceOfInteger TabOrien;

        public TopAbs_State PerformInfinitePoint()
        {

            if (Umax == -Standard_Real.RealLast()
                || Vmax == -Standard_Real.RealLast()
                || Umin == Standard_Real.RealLast()
                || Vmin == Standard_Real.RealLast())
            {
                return (TopAbs_State.TopAbs_IN);
            }
            gp_Pnt2d P = new gp_Pnt2d(Umin - (Umax - Umin), Vmin - (Vmax - Vmin));
            return (Perform(P, false));
        }

        public TopAbs_State Perform(gp_Pnt2d _Puv,
                          bool RecadreOnPeriodic)
        {
            int dedans;
            int nbtabclass = TabClass.Length();

            if (nbtabclass == 0)
            {
                return TopAbs_State.TopAbs_IN;
            }

            //-- U1 is the First Param and U2 in this case is U1+Period
            double u = _Puv.X();
            double v = _Puv.Y();
            double uu = u, vv = v;

            BRepAdaptor_Surface surf = new BRepAdaptor_Surface();
            surf.Initialize(Face, false);
            bool IsUPer = surf.IsUPeriodic();
            bool IsVPer = surf.IsVPeriodic();
            double uperiod = IsUPer ? surf.UPeriod() : 0.0;
            double vperiod = IsVPer ? surf.VPeriod() : 0.0;
            TopAbs_State aStatus = TopAbs_State.TopAbs_UNKNOWN;
            bool urecadre = false, vrecadre = false;

            if (RecadreOnPeriodic)
            {
                if (IsUPer)
                {
                    if (uu < Umin)
                        while (uu < Umin)
                            uu += uperiod;
                    else
                    {
                        while (uu >= Umin)
                            uu -= uperiod;
                        uu += uperiod;
                    }
                }
                if (IsVPer)
                {
                    if (vv < Vmin)
                        while (vv < Vmin)
                            vv += vperiod;
                    else
                    {
                        while (vv >= Vmin)
                            vv -= vperiod;
                        vv += vperiod;
                    }
                }
            }

            for (; ; )
            {
                dedans = 1;
                gp_Pnt2d Puv = new gp_Pnt2d(u, v);

                if (TabOrien[1] != -1)
                {
                    for (int n = 1; n <= nbtabclass; n++)
                    {
                        int cur = ((CSLib_Class2d)TabClass[n]).SiDans(Puv);
                        if (cur == 1)
                        {
                            if (TabOrien[n] == 0)
                            {
                                dedans = -1;
                                break;
                            }
                        }
                        else if (cur == -1)
                        {
                            if (TabOrien[n] == 1)
                            {
                                dedans = -1;
                                break;
                            }
                        }
                        else
                        {
                            dedans = 0;
                            break;
                        }
                    }
                    if (dedans == 0)
                    {
                        BRepClass_FaceClassifier aClassifier = new BRepClass_FaceClassifier();
                        double m_Toluv = (Toluv > 4.0) ? 4.0 : Toluv;
                        //aClassifier.Perform(Face,Puv,Toluv);
                        aClassifier.Perform(Face, Puv, m_Toluv);
                        aStatus = aClassifier.State();
                    }
                    if (dedans == 1)
                    {
                        aStatus = TopAbs_State.TopAbs_IN;
                    }
                    if (dedans == -1)
                    {
                        aStatus = TopAbs_State.TopAbs_OUT;
                    }
                }
                else
                {  //-- TabOrien(1)=-1    False Wire
                    BRepClass_FaceClassifier aClassifier = new BRepClass_FaceClassifier();
                    aClassifier.Perform(Face, Puv, Toluv);
                    aStatus = aClassifier.State();
                }

                if (!RecadreOnPeriodic || (!IsUPer && !IsVPer))
                    return aStatus;
                if (aStatus == TopAbs_State.TopAbs_IN || aStatus == TopAbs_State.TopAbs_ON)
                    return aStatus;

                if (!urecadre)
                {
                    u = uu;
                    urecadre = true;
                }
                else
              if (IsUPer)
                    u += uperiod;
                if (u > Umax || !IsUPer)
                {
                    if (!vrecadre)
                    {
                        v = vv;
                        vrecadre = true;
                    }
                    else
                      if (IsVPer)
                        v += vperiod;

                    u = uu;

                    if (v > Vmax || !IsVPer)
                        return aStatus;
                }
            } //for (;;)
        }

    }

    public class TColgp_SequenceOfPnt2d : List<gp_Pnt2d>
    {
        public void Append(gp_Pnt2d item)
        {
            Add(item);
        }
    }
    public class TColStd_SequenceOfInteger : NCollection_Sequence<int>
    {


    }
    internal class BRepTopAdaptor_SeqOfPtr : NCollection_Sequence<object>
    {
        public int Length()
        {
            return base.Count;
        }
    }
}