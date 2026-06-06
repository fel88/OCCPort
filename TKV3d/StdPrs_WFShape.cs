using OCCPort;
using TKBRep;
using TKernel;
using TKG3d;
using TKGeomBase;
using TKMath;
using TKMesh;
using TKService;

namespace TKV3d
{
    //! Tool for computing wireframe presentation of a TopoDS_Shape.
    class StdPrs_WFShape : Prs3d_Root
    {


        public static void Add(Prs3d_Presentation thePresentation,
                              TopoDS_Shape theShape,
                              Prs3d_Drawer theDrawer,
                              bool theIsParallel = false)
        {
            if (theShape.IsNull())
            {
                return;
            }

            if (theDrawer.IsAutoTriangulation())
            {
                StdPrs_ToolTriangulatedShape.Tessellate(theShape, theDrawer);
            }

            // draw triangulation-only edges
            Graphic3d_ArrayOfPrimitives aTriFreeEdges = AddEdgesOnTriangulation(theShape, true);
            if (aTriFreeEdges != null)
            {
                Graphic3d_Group aGroup = thePresentation.NewGroup();
                //aGroup.SetPrimitivesAspect(theDrawer.FreeBoundaryAspect().Aspect());
                aGroup.AddPrimitiveArray(aTriFreeEdges);
            }

            Prs3d_NListOfSequenceOfPnt aCommonPolylines = new Prs3d_NListOfSequenceOfPnt();
            Prs3d_LineAspect aWireAspect = theDrawer.WireAspect();
            double aShapeDeflection = StdPrs_ToolTriangulatedShape.GetDeflection(theShape, theDrawer);


            // Draw isolines
            {
                Prs3d_NListOfSequenceOfPnt aUPolylines = new Prs3d_NListOfSequenceOfPnt(), aVPolylines = new Prs3d_NListOfSequenceOfPnt();
                Prs3d_NListOfSequenceOfPnt aUPolylinesPtr = aUPolylines;
                Prs3d_NListOfSequenceOfPnt aVPolylinesPtr = aVPolylines;

                Prs3d_LineAspect anIsoAspectU = theDrawer.UIsoAspect();
                Prs3d_LineAspect anIsoAspectV = theDrawer.VIsoAspect();

                if (anIsoAspectV.Aspect().IsEqual(anIsoAspectU.Aspect()))
                {
                    aVPolylinesPtr = aUPolylinesPtr;  // put both U and V isolines into single group
                }
                if (anIsoAspectU.Aspect().IsEqual(aWireAspect.Aspect()))
                {
                    aUPolylinesPtr = aCommonPolylines; // put U isolines into single group with common edges
                }
                if (anIsoAspectV.Aspect().IsEqual(aWireAspect.Aspect()))
                {
                    aVPolylinesPtr = aCommonPolylines; // put V isolines into single group with common edges
                }

                bool isParallelIso = false;
                if (theIsParallel)
                {
                    int aNbFaces = 0;
                    for (TopExp_Explorer aFaceExplorer = new TopExp_Explorer(theShape, TopAbs_ShapeEnum.TopAbs_FACE); aFaceExplorer.More(); aFaceExplorer.Next())
                    {
                        ++aNbFaces;
                    }
                    if (aNbFaces > 1)
                    {
                        isParallelIso = true;
                        List<TopoDS_Face> aFaces = new List<TopoDS_Face>(aNbFaces);
                        aNbFaces = 0;
                        for (TopExp_Explorer aFaceExplorer = new TopExp_Explorer(theShape, TopAbs_ShapeEnum.TopAbs_FACE); aFaceExplorer.More(); aFaceExplorer.Next())
                        {
                            TopoDS_Face aFace = TopoDS.Face(aFaceExplorer.Current());
                            if (theDrawer.IsoOnPlane() || !StdPrs_ShapeTool.IsPlanarFace(aFace))
                            {
                                aFaces[aNbFaces++] = aFace;
                            }
                        }

                        //StdPrs_WFShape_IsoFunctor anIsoFunctor(*aUPolylinesPtr, *aVPolylinesPtr, aFaces, theDrawer, aShapeDeflection);
                        //  OSD_Parallel::For(0, aNbFaces, anIsoFunctor, aNbFaces < 2);
                    }
                }

                if (!isParallelIso)
                {
                    for (TopExp_Explorer aFaceExplorer = new TopExp_Explorer(theShape, TopAbs_ShapeEnum.TopAbs_FACE); aFaceExplorer.More(); aFaceExplorer.Next())
                    {
                        TopoDS_Face aFace = TopoDS.Face(aFaceExplorer.Current());
                        if (theDrawer.IsoOnPlane() || !StdPrs_ShapeTool.IsPlanarFace(aFace))
                        {
                            StdPrs_Isolines.Add(aFace, theDrawer, aShapeDeflection, aUPolylinesPtr, aVPolylinesPtr);
                        }
                    }
                }

                Prs3d.AddPrimitivesGroup(thePresentation, anIsoAspectU, aUPolylines);
                Prs3d.AddPrimitivesGroup(thePresentation, anIsoAspectV, aVPolylines);
            }

            {
                Prs3d_NListOfSequenceOfPnt anUnfree = new Prs3d_NListOfSequenceOfPnt(), aFree = new Prs3d_NListOfSequenceOfPnt();
                Prs3d_NListOfSequenceOfPnt anUnfreePtr = anUnfree;
                Prs3d_NListOfSequenceOfPnt aFreePtr = aFree;
                /*
                if (!theDrawer.UnFreeBoundaryDraw())
                {
                    anUnfreePtr = null;
                }
                else if (theDrawer.UnFreeBoundaryAspect()->Aspect()->IsEqual(aWireAspect.Aspect()))
                {
                    anUnfreePtr = &aCommonPolylines; // put unfree edges into single group with common edges
                }

                if (!theDrawer.FreeBoundaryDraw())
                {
                    aFreePtr = null;
                }
                else if (theDrawer.FreeBoundaryAspect().Aspect().IsEqual(aWireAspect.Aspect()))
                {
                    aFreePtr = &aCommonPolylines; // put free edges into single group with common edges
                }*/

                addEdges(theShape,
                          theDrawer,
                          aShapeDeflection,
                          theDrawer.WireDraw() ? aCommonPolylines : null,
                          aFreePtr,
                          anUnfreePtr);

                Prs3d.AddPrimitivesGroup(thePresentation, theDrawer.UnFreeBoundaryAspect(), anUnfree);
                Prs3d.AddPrimitivesGroup(thePresentation, theDrawer.FreeBoundaryAspect(), aFree);
            }


            Prs3d.AddPrimitivesGroup(thePresentation, theDrawer.WireAspect(), aCommonPolylines);

            Graphic3d_ArrayOfPoints aVertexArray = AddVertexes(theShape, theDrawer.VertexDrawMode());
            if (aVertexArray != null)
            {
                Graphic3d_Group aGroup = thePresentation.NewGroup();
                //aGroup.SetPrimitivesAspect(theDrawer.PointAspect()->Aspect());
                aGroup.AddPrimitiveArray(aVertexArray);
            }
        }

        // =========================================================================
        // function : addEdges
        // purpose  :
        // =========================================================================
        public static void addEdges(TopoDS_Shape theShape,
                                Prs3d_Drawer theDrawer,
                                double theShapeDeflection,
                                Prs3d_NListOfSequenceOfPnt theWire,
                                Prs3d_NListOfSequenceOfPnt theFree,
                                Prs3d_NListOfSequenceOfPnt theUnFree)
        {
            if (theShape.IsNull())
            {
                return;
            }

            TopTools_ListOfShape aLWire = new TopTools_ListOfShape();
            TopTools_ListOfShape aLFree = new TopTools_ListOfShape();
            TopTools_ListOfShape aLUnFree = new TopTools_ListOfShape();

            TopTools_IndexedDataMapOfShapeListOfShape anEdgeMap = new TopTools_IndexedDataMapOfShapeListOfShape();
            TopExp.MapShapesAndAncestors(theShape, TopAbs_ShapeEnum.TopAbs_EDGE, TopAbs_ShapeEnum.TopAbs_FACE, anEdgeMap);

            for (TopTools_IndexedDataMapOfShapeListOfShape.Iterator anEdgeIter = new TopTools_IndexedDataMapOfShapeListOfShape.Iterator(anEdgeMap); anEdgeIter.More(); anEdgeIter.Next())
            {
                TopoDS_Edge anEdge = TopoDS.Edge(anEdgeIter.Key());
                int aNbNeighbours = anEdgeIter.Value().Extent();
                switch (aNbNeighbours)
                {
                    case 0:
                        {
                            if (theWire != null)
                            {
                                aLWire.Append(anEdge);
                            }
                            break;
                        }
                    case 1:
                        {
                            if (theFree != null)
                            {
                                aLFree.Append(anEdge);
                            }
                            break;
                        }
                    default:
                        {
                            if (theUnFree != null)
                            {
                                aLUnFree.Append(anEdge);
                            }
                            break;
                        }
                }
            }

            if (!aLWire.IsEmpty())
            {
                addEdges(aLWire, theDrawer, theShapeDeflection, theWire);
            }
            if (!aLFree.IsEmpty())
            {
                addEdges(aLFree, theDrawer, theShapeDeflection, theFree);
            }
            if (!aLUnFree.IsEmpty())
            {
                addEdges(aLUnFree, theDrawer, theShapeDeflection, theUnFree);
            }
        }

        static void addEdges(TopTools_ListOfShape theEdges,
                                   Prs3d_Drawer theDrawer,
                                   double theShapeDeflection,
                                  Prs3d_NListOfSequenceOfPnt thePolylines)
        {
            TopTools_ListIteratorOfListOfShape anEdgesIter = new TopTools_ListIteratorOfListOfShape();
            for (anEdgesIter.Initialize(theEdges); anEdgesIter.More(); anEdgesIter.Next())
            {
                TopoDS_Edge anEdge = TopoDS.Edge(anEdgesIter.Value());
                if (BRep_Tool.Degenerated(anEdge))
                {
                    continue;
                }

                TColgp_SequenceOfPnt aPoints = new TColgp_SequenceOfPnt();

                TopLoc_Location aLocation = new TopLoc_Location();
                Poly_Triangulation aTriangulation = null;
                Poly_PolygonOnTriangulation anEdgeIndicies = null;
                BRep_Tool.PolygonOnTriangulation(anEdge, ref anEdgeIndicies, ref aTriangulation, aLocation);
                Poly_Polygon3D aPolygon;

                if (anEdgeIndicies != null)
                {
                    // Presentation based on triangulation of a face.
                    TColStd_Array1OfInteger anIndices = anEdgeIndicies.Nodes();

                    int anIndex = anIndices.Lower();
                    if (aLocation.IsIdentity())
                    {
                        for (; anIndex <= anIndices.Upper(); ++anIndex)
                        {
                            aPoints.Append(aTriangulation.Node(anIndices[anIndex]));
                        }
                    }
                    else
                    {
                        for (; anIndex <= anIndices.Upper(); ++anIndex)
                        {
                            aPoints.Append(aTriangulation.Node(anIndices[anIndex]).Transformed(aLocation));
                        }
                    }
                }
                else if ((aPolygon = BRep_Tool.Polygon3D(anEdge, ref aLocation)) != null)
                {
                    // Presentation based on triangulation of the free edge on a surface.
                    TColgp_Array1OfPnt aNodes = aPolygon.Nodes();
                    int anIndex = aNodes.Lower();
                    if (aLocation.IsIdentity())
                    {
                        for (; anIndex <= aNodes.Upper(); ++anIndex)
                        {
                            aPoints.Append(aNodes.Value(anIndex));
                        }
                    }
                    else
                    {
                        for (; anIndex <= aNodes.Upper(); ++anIndex)
                        {
                            aPoints.Append(aNodes.Value(anIndex).Transformed(aLocation));
                        }
                    }
                }
                else if (BRep_Tool.IsGeometric(anEdge))
                {
                    // Default presentation for edges without triangulation.
                    BRepAdaptor_Curve aCurve = new BRepAdaptor_Curve(anEdge);
                    StdPrs_DeflectionCurve.Add(null,
                                                 aCurve,
                                                 theShapeDeflection,
                                                 theDrawer,
                                                 aPoints.ChangeSequence(),
                                                 false);
                }

                if (!aPoints.IsEmpty())
                {
                    thePolylines.Append(aPoints);
                }
            }
        }


        // =========================================================================
        // function : AddVertexes
        // purpose  :
        // =========================================================================
        public static Graphic3d_ArrayOfPoints AddVertexes(TopoDS_Shape theShape,
                                                             Prs3d_VertexDrawMode theVertexMode)
        {
            TColgp_SequenceOfPnt aShapeVertices = new TColgp_SequenceOfPnt();
            if (theVertexMode == Prs3d_VertexDrawMode.Prs3d_VDM_All)
            {
                for (TopExp_Explorer aVertIter = new TopExp_Explorer(theShape, TopAbs_ShapeEnum.TopAbs_VERTEX); aVertIter.More(); aVertIter.Next())
                {
                    TopoDS_Vertex aVert = TopoDS.Vertex(aVertIter.Current());
                    aShapeVertices.Append(BRep_Tool.Pnt(aVert));
                }
            }
            else
            {
                // isolated vertices
                for (TopExp_Explorer aVertIter = new TopExp_Explorer(theShape, TopAbs_ShapeEnum.TopAbs_VERTEX, TopAbs_ShapeEnum.TopAbs_EDGE); aVertIter.More(); aVertIter.Next())
                {
                    TopoDS_Vertex aVert = TopoDS.Vertex(aVertIter.Current());
                    aShapeVertices.Append(BRep_Tool.Pnt(aVert));
                }

                // internal vertices
                for (TopExp_Explorer anEdgeIter = new TopExp_Explorer(theShape, TopAbs_ShapeEnum.TopAbs_EDGE); anEdgeIter.More(); anEdgeIter.Next())
                {
                    for (TopoDS_Iterator aVertIter = new TopoDS_Iterator(anEdgeIter.Current(), false, true); aVertIter.More(); aVertIter.Next())
                    {
                        TopoDS_Shape aVertSh = aVertIter.Value();
                        if (aVertSh.Orientation() == TopAbs_Orientation.TopAbs_INTERNAL
                            && aVertSh.ShapeType() == TopAbs_ShapeEnum.TopAbs_VERTEX)
                        {
                            TopoDS_Vertex aVert = TopoDS.Vertex(aVertSh);
                            aShapeVertices.Append(BRep_Tool.Pnt(aVert));
                        }
                    }
                }
            }

            if (aShapeVertices.IsEmpty())
            {
                return null;
            }

            int aNbVertices = aShapeVertices.Length();
            Graphic3d_ArrayOfPoints aVertexArray = new Graphic3d_ArrayOfPoints(aNbVertices);
            for (int aVertIter = 1; aVertIter <= aNbVertices; ++aVertIter)
            {
                aVertexArray.AddVertex(aShapeVertices.Value(aVertIter));
            }
            return aVertexArray;
        }

        public static void AddEdgesOnTriangulation(TColgp_SequenceOfPnt theSegments,

                                              TopoDS_Shape theShape,
                                              bool theToExcludeGeometric = true)
        {
            TopLoc_Location aLocation = new TopLoc_Location(), aDummyLoc = new TopLoc_Location();
            for (TopExp_Explorer aFaceIter = new TopExp_Explorer(theShape, TopAbs_ShapeEnum.TopAbs_FACE); aFaceIter.More(); aFaceIter.Next())
            {
                TopoDS_Face aFace = TopoDS.Face(aFaceIter.Current());
                if (theToExcludeGeometric)
                {
                    Geom_Surface aSurf = BRep_Tool.Surface(aFace, out aDummyLoc);
                    if (aSurf != null)
                    {
                        continue;
                    }
                }
                Poly_Triangulation aPolyTri = BRep_Tool.Triangulation(aFace, ref aLocation);
                if (aPolyTri != null)
                {
                    Prs3d.AddFreeEdges(theSegments, aPolyTri, aLocation);
                }
            }
        }

        public static Graphic3d_ArrayOfPrimitives AddEdgesOnTriangulation(TopoDS_Shape theShape,
                                                                             bool theToExcludeGeometric)
        {
            TColgp_SequenceOfPnt aSeqPnts = new TColgp_SequenceOfPnt();
            AddEdgesOnTriangulation(aSeqPnts, theShape, theToExcludeGeometric);
            if (aSeqPnts.Size() < 2)
            {
                return null;
            }

            int aNbVertices = aSeqPnts.Size();
            Graphic3d_ArrayOfSegments aSurfArray = new Graphic3d_ArrayOfSegments(aNbVertices);
            for (int anI = 1; anI <= aNbVertices; anI += 2)
            {
                aSurfArray.AddVertex(aSeqPnts.Value(anI));
                aSurfArray.AddVertex(aSeqPnts.Value(anI + 1));
            }
            return aSurfArray;
        }

    }

    //! A framework to provide display of any curve with
    //! respect to the maximal chordal deviation defined in
    //! the Prs3d_Drawer attributes manager.
    public class StdPrs_DeflectionCurve : Prs3d_Root
    {

        static bool FindLimits(Adaptor3d_Curve aCurve,
                                   double aLimit,
                                   out double First,
                                   out double Last)
        {
            First = aCurve.FirstParameter();
            Last = aCurve.LastParameter();
            bool firstInf = Precision.IsNegativeInfinite(First);
            bool lastInf = Precision.IsPositiveInfinite(Last);

            if (firstInf || lastInf)
            {
                gp_Pnt P1 = new gp_Pnt(), P2 = new gp_Pnt();
                double delta = 1;
                int count = 0;
                if (firstInf && lastInf)
                {
                    do
                    {
                        if (count++ == 100000)
                            return false;

                        delta *= 2;
                        First = -delta;
                        Last = delta;
                        aCurve.D0(First, ref P1);
                        aCurve.D0(Last, ref P2);
                    } while (P1.Distance(P2) < aLimit);
                }
                else if (firstInf)
                {
                    aCurve.D0(Last, ref P2);
                    do
                    {
                        if (count++ == 100000) return false;
                        delta *= 2;
                        First = Last - delta;
                        aCurve.D0(First, ref P1);
                    } while (P1.Distance(P2) < aLimit);
                }
                else if (lastInf)
                {
                    aCurve.D0(First, ref P1);
                    do
                    {
                        if (count++ == 100000) return false;
                        delta *= 2;
                        Last = First + delta;
                        aCurve.D0(Last, ref P2);
                    } while (P1.Distance(P2) < aLimit);
                }
            }
            return true;
        }

        //! adds to the presentation aPresentation the drawing of the curve
        //! aCurve with respect to the maximal chordial deviation aDeflection.
        //! The aspect is the current aspect
        //! Points give a sequence of curve points.
        //! If drawCurve equals Standard_False the curve will not be displayed,
        //! it is used if the curve is a part of some shape and PrimitiveArray
        //! visualization approach is activated (it is activated by default).
        public static void Add(Prs3d_Presentation aPresentation,
            Adaptor3d_Curve aCurve,
            double aDeflection,
            Prs3d_Drawer aDrawer,
            TColgp_SequenceOfPnt Points,
             bool theToDrawCurve = true)
        {
            double V1, V2;
            if (!FindLimits(aCurve, aDrawer.MaximalParameterValue(), out V1, out V2))
            {
                return;
            }

            Graphic3d_Group aGroup = null;
            if (theToDrawCurve)
            {
                aGroup = aPresentation.CurrentGroup();
            }
            drawCurve(aCurve, aGroup, aDeflection, aDrawer.DeviationAngle(), V1, V2, Points);
        }

        static void drawCurve(Adaptor3d_Curve aCurve,
                       Graphic3d_Group aGroup,
                       double TheDeflection,
                       double anAngle,
                       double U1,
                       double U2,
                       TColgp_SequenceOfPnt Points)
        {
            switch (aCurve._GetType())
            {
                case GeomAbs_CurveType.GeomAbs_Line:
                    {
                        gp_Pnt p1 = aCurve.Value(U1);
                        gp_Pnt p2 = aCurve.Value(U2);
                        Points.Append(p1);
                        Points.Append(p2);
                        if (aGroup != null)
                        {
                            Graphic3d_ArrayOfSegments aPrims = new Graphic3d_ArrayOfSegments(2);
                            aPrims.AddVertex(p1);
                            aPrims.AddVertex(p2);
                            aGroup.AddPrimitiveArray(aPrims);
                        }
                        break;
                    }
                default:
                    {
                        int nbinter = aCurve.NbIntervals(GeomAbs_Shape.GeomAbs_C1);
                        TColStd_Array1OfReal T = new TColStd_Array1OfReal(1, nbinter + 1);
                        aCurve.Intervals(T, GeomAbs_Shape.GeomAbs_C1);

                        double theU1, theU2;
                        int NumberOfPoints, i, j;
                        TColgp_SequenceOfPnt SeqP = new TColgp_SequenceOfPnt();

                        for (j = 1; j <= nbinter; j++)
                        {
                            theU1 = T[j]; theU2 = T[j + 1];
                            if (theU2 > U1 && theU1 < U2)
                            {
                                theU1 = Math.Max(theU1, U1);
                                theU2 = Math.Min(theU2, U2);

                                GCPnts_TangentialDeflection Algo = new GCPnts_TangentialDeflection(aCurve, theU1, theU2, anAngle, TheDeflection);
                                NumberOfPoints = Algo.NbPoints();

                                if (NumberOfPoints > 0)
                                {
                                    for (i = 1; i <= NumberOfPoints; i++)
                                        SeqP.Append(Algo.Value(i));
                                }
                            }
                        }

                        Graphic3d_ArrayOfPolylines aPrims = null;
                        if (aGroup != null)
                            aPrims = new Graphic3d_ArrayOfPolylines(SeqP.Length());

                        for (i = 1; i <= SeqP.Length(); i++)
                        {
                            gp_Pnt p = SeqP.Value(i);
                            Points.Append(p);
                            if (aGroup != null)
                            {
                                aPrims.AddVertex(p);
                            }
                        }
                        if (aGroup != null)
                        {
                            aGroup.AddPrimitiveArray(aPrims);
                        }
                    }
                    break;
            }
        }

    }
}

