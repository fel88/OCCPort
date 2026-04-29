using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OCCPort.Tester
{
    //! Auxiliary procedures to prepare Shaded presentation of specified shape.
    public class StdPrs_ShadedShape : Prs3d_Root
    {
        public static void Add(Prs3d_Presentation thePrs,
                               TopoDS_Shape theShape,
                               Prs3d_Drawer theDrawer,
                              bool theHasTexels,
                              gp_Pnt2d theUVOrigin,
                              gp_Pnt2d theUVRepeat,
                              gp_Pnt2d theUVScale,
                              StdPrs_Volume theVolume = StdPrs_Volume.StdPrs_Volume_Autodetection)
        {
            if (theShape.IsNull())
            {
                return;
            }

            // Use automatic re-triangulation with deflection-check logic only if this feature is enable
            if (theDrawer.IsAutoTriangulation())
            {
                // Triangulation completeness is important for "open-closed" analysis - perform tessellation beforehand
                StdPrs_ToolTriangulatedShape.Tessellate(theShape, theDrawer);
            }

            // add wireframe presentation for isolated edges and vertices
            wireframeFromShape(thePrs, theShape, theDrawer);

            // add special wireframe presentation for faces without triangulation
            //   wireframeNoTriangFacesFromShape(thePrs, theShape, theDrawer);

            // The shape types listed below need advanced analysis as potentially containing
            // both closed and open parts. Solids are also included, because they might
            // contain non-manifold parts inside (internal open shells)
            if ((theShape.ShapeType() == TopAbs_ShapeEnum.TopAbs_COMPOUND
              || theShape.ShapeType() == TopAbs_ShapeEnum.TopAbs_COMPSOLID
              || theShape.ShapeType() == TopAbs_ShapeEnum.TopAbs_SOLID)
             && theVolume == StdPrs_Volume.StdPrs_Volume_Autodetection)
            {
                // collect two compounds: for opened and closed (solid) sub-shapes
                TopoDS_Compound anOpened = new TopoDS_Compound(), aClosed = new TopoDS_Compound();
                BRep_Builder aBuilder = new BRep_Builder();
                aBuilder.MakeCompound(aClosed);
                aBuilder.MakeCompound(anOpened);
                ExploreSolids(theShape, aBuilder, aClosed, anOpened, true);

                if (aClosed.NbChildren() > 0)
                {
                    shadeFromShape(aClosed, thePrs, theDrawer,
                                    theHasTexels, theUVOrigin, theUVRepeat, theUVScale, true);
                }

                if (anOpened.NbChildren() > 0)
                {
                    shadeFromShape(anOpened, thePrs, theDrawer,
                                    theHasTexels, theUVOrigin, theUVRepeat, theUVScale, false);
                }
            }
            else
            {
                // if the shape type is not compound, composolid or solid, use autodetection back-facing filled
                shadeFromShape(theShape, thePrs, theDrawer,
                                theHasTexels, theUVOrigin, theUVRepeat, theUVScale,
                                theVolume == StdPrs_Volume.StdPrs_Volume_Closed);
            }

            // if (theDrawer.FaceBoundaryDraw())
            {
                Graphic3d_ArrayOfSegments aBndSegments = fillFaceBoundaries(theShape, theDrawer.FaceBoundaryUpperContinuity());
                if (aBndSegments != null)
                {
                    Graphic3d_Group aPrsGrp = thePrs.NewGroup();
                    //aPrsGrp.SetGroupPrimitivesAspect(theDrawer.FaceBoundaryAspect().Aspect());
                    aPrsGrp.AddPrimitiveArray(aBndSegments);
                }
            }
        }

        //! Searches closed and unclosed subshapes in shape structure and puts them
        //! into two compounds for separate processing of closed and unclosed sub-shapes
        public static void ExploreSolids(TopoDS_Shape theShape, BRep_Builder theBuilder, TopoDS_Compound theClosed, TopoDS_Compound theOpened, bool theIgnore1DSubShape)
        {
            if (theShape.IsNull())
            {
                return;
            }

            switch (theShape.ShapeType())
            {
                case TopAbs_ShapeEnum.TopAbs_COMPOUND:
                case TopAbs_ShapeEnum.TopAbs_COMPSOLID:
                    {
                        for (TopoDS_Iterator anIter = new TopoDS_Iterator(theShape); anIter.More(); anIter.Next())
                        {
                            ExploreSolids(anIter.Value(), theBuilder, theClosed, theOpened, theIgnore1DSubShape);
                        }
                        return;
                    }
                case TopAbs_ShapeEnum.TopAbs_SOLID:
                    {
                        for (TopoDS_Iterator anIter = new TopoDS_Iterator(theShape); anIter.More(); anIter.Next())
                        {
                            TopoDS_Shape aSubShape = anIter.Value();
                            bool isClosed = aSubShape.ShapeType() == TopAbs_ShapeEnum.TopAbs_SHELL &&
                                                              BRep_Tool.IsClosed(aSubShape) &&
                                                              StdPrs_ToolTriangulatedShape.IsTriangulated(aSubShape);
                            theBuilder.Add(isClosed ? theClosed : theOpened, aSubShape);
                        }
                        return;
                    }

                case TopAbs_ShapeEnum.TopAbs_SHELL:
                case TopAbs_ShapeEnum.TopAbs_FACE:
                    {
                        theBuilder.Add(theOpened, theShape);
                        return;
                    }
                case TopAbs_ShapeEnum.TopAbs_WIRE:
                case TopAbs_ShapeEnum.TopAbs_EDGE:
                case TopAbs_ShapeEnum.TopAbs_VERTEX:
                    {
                        if (!theIgnore1DSubShape)
                        {
                            theBuilder.Add(theOpened, theShape);
                        }
                        return;
                    }
                case TopAbs_ShapeEnum.TopAbs_SHAPE:
                default:
                    return;
            }
            /*
             * not finished here!
             */
        }


        //! Gets triangulation of every face of shape and fills output array of triangles
        static Graphic3d_ArrayOfTriangles fillTriangles(TopoDS_Shape theShape,
                                                        bool theHasTexels,
                                                        gp_Pnt2d theUVOrigin,
                                                        gp_Pnt2d theUVRepeat,
                                                        gp_Pnt2d theUVScale)
        {
            Poly_Triangulation aT;
            TopLoc_Location aLoc = new TopLoc_Location();
            gp_Pnt aPoint;
            int aNbTriangles = 0;
            int aNbVertices = 0;

            // Precision for compare square distances
            //double aPreci = Precision.SquareConfusion();

            TopExp_Explorer aFaceIt = new TopExp_Explorer(theShape, TopAbs_ShapeEnum.TopAbs_FACE);
            for (; aFaceIt.More(); aFaceIt.Next())
            {
                TopoDS_Face aFace = TopoDS.Face(aFaceIt.Current());
                aT = BRep_Tool.Triangulation(aFace, ref aLoc);
                if (aT != null)
                {
                    aNbTriangles += aT.NbTriangles();
                    aNbVertices += aT.NbNodes();
                }
            }
            if (aNbVertices < 3 || aNbTriangles <= 0)
            {
                return null;
            }

            Graphic3d_ArrayOfTriangles anArray = new Graphic3d_ArrayOfTriangles(aNbVertices, 3 * aNbTriangles,
                                                                                         true, false, theHasTexels);
            double aUmin = (0.0), aUmax = (0.0), aVmin = (0.0),
                aVmax = (0.0), dUmax = (0.0), dVmax = (0.0);
            for (aFaceIt.Init(theShape, TopAbs_ShapeEnum.TopAbs_FACE); aFaceIt.More(); aFaceIt.Next())
            {
                TopoDS_Face aFace = TopoDS.Face(aFaceIt.Current());
                aT = BRep_Tool.Triangulation(aFace, ref aLoc);
                if (aT == null || !aT.HasGeometry())
                {
                    continue;
                }
                gp_Trsf aTrsf = aLoc.Transformation();

                // Determinant of transform matrix less then 0 means that mirror transform applied.
                //  bool isMirrored = aTrsf.VectorialPart().Determinant() < 0;

                // Extracts vertices & normals from nodes
                StdPrs_ToolTriangulatedShape.ComputeNormals(aFace, aT);

                if (theHasTexels)
                {
                    BRepTools.UVBounds(aFace, ref aUmin, ref aUmax, ref aVmin, ref aVmax);
                    dUmax = (aUmax - aUmin);
                    dVmax = (aVmax - aVmin);
                }

                int aDecal = anArray.VertexNumber();
                for (int aNodeIter = 1; aNodeIter <= aT.NbNodes(); ++aNodeIter)
                {
                    //aPoint = aT->Node(aNodeIter);
                    //gp_Dir aNorm = aT->Normal(aNodeIter);
                    //if ((aFace.Orientation() == TopAbs_REVERSED) ^ isMirrored)
                    //{
                    //    aNorm.Reverse();
                    //}
                    //if (!aLoc.IsIdentity())
                    //{
                    //    aPoint.Transform(aTrsf);
                    //    aNorm.Transform(aTrsf);
                    //}

                    //if (theHasTexels && aT->HasUVNodes())
                    //{
                    //    const gp_Pnt2d aNode2d = aT->UVNode(aNodeIter);
                    //    const gp_Pnt2d aTexel = (dUmax == 0.0 || dVmax == 0.0)
                    //                          ? aNode2d
                    //                          : gp_Pnt2d((-theUVOrigin.X() + (theUVRepeat.X() * (aNode2d.X() - aUmin)) / dUmax) / theUVScale.X(),
                    //                                      (-theUVOrigin.Y() + (theUVRepeat.Y() * (aNode2d.Y() - aVmin)) / dVmax) / theUVScale.Y());
                    //    anArray->AddVertex(aPoint, aNorm, aTexel);
                    //}
                    //else
                    //{
                    //    anArray->AddVertex(aPoint, aNorm);
                    //}
                }

                // Fill array with vertex and edge visibility info
                int[] anIndex = new int[3];
                for (int aTriIter = 1; aTriIter <= aT.NbTriangles(); ++aTriIter)
                {
                    //if ((aFace.Orientation() == TopAbs_REVERSED))
                    //{
                    //    aT->Triangle(aTriIter).Get(anIndex[0], anIndex[2], anIndex[1]);
                    //}
                    //else
                    //{
                    //    aT->Triangle(aTriIter).Get(anIndex[0], anIndex[1], anIndex[2]);
                    //}

                    //const gp_Pnt aP1 = aT->Node(anIndex[0]);
                    //const gp_Pnt aP2 = aT->Node(anIndex[1]);
                    //const gp_Pnt aP3 = aT->Node(anIndex[2]);

                    //gp_Vec aV1(aP1, aP2);
                    //if (aV1.SquareMagnitude() <= aPreci)
                    //{
                    //    continue;
                    //}
                    //gp_Vec aV2(aP2, aP3);
                    //if (aV2.SquareMagnitude() <= aPreci)
                    //{
                    //    continue;
                    //}
                    //gp_Vec aV3(aP3, aP1);
                    //if (aV3.SquareMagnitude() <= aPreci)
                    //{
                    //    continue;
                    //}
                    //aV1.Normalize();
                    //aV2.Normalize();
                    //aV1.Cross(aV2);
                    //if (aV1.SquareMagnitude() > aPreci)
                    //{
                    //    anArray->AddEdges(anIndex[0] + aDecal,
                    //                       anIndex[1] + aDecal,
                    //                       anIndex[2] + aDecal);
                    //}
                }
            }
            return anArray;

        }

        //! Prepare shaded presentation for specified shape
        public static bool shadeFromShape(TopoDS_Shape theShape,
                                          Prs3d_Presentation thePrs,
                                          Prs3d_Drawer theDrawer,
                                          bool theHasTexels,
                                          gp_Pnt2d theUVOrigin,
                                          gp_Pnt2d theUVRepeat,
                                          gp_Pnt2d theUVScale,
                                          bool theIsClosed)
        {
            Graphic3d_ArrayOfTriangles aPArray = fillTriangles(theShape, theHasTexels, theUVOrigin, theUVRepeat, theUVScale);
            if (aPArray == null)
            {
                return false;
            }

            Graphic3d_Group aGroup = thePrs.NewGroup();
            aGroup.SetClosed(theIsClosed);
            aGroup.SetGroupPrimitivesAspect(theDrawer.ShadingAspect().Aspect());
            aGroup.AddPrimitiveArray(aPArray);
            return true;
        }
        // =======================================================================
        // function : FillTriangles
        // purpose  :
        // =======================================================================
        Graphic3d_ArrayOfTriangles FillTriangles(TopoDS_Shape theShape,
                                                                      bool theHasTexels,
                                                                      gp_Pnt2d theUVOrigin,
                                                                      gp_Pnt2d theUVRepeat,
                                                                      gp_Pnt2d theUVScale)
        {
            return fillTriangles(theShape, theHasTexels, theUVOrigin, theUVRepeat, theUVScale);
        }


        //! Computes wireframe presentation for free wires and vertices
        public static void wireframeFromShape(Prs3d_Presentation thePrs,
                                TopoDS_Shape theShape,
                                Prs3d_Drawer theDrawer)
        {
            TopExp_Explorer aShapeIter = new TopExp_Explorer(theShape, TopAbs_ShapeEnum.TopAbs_FACE);
            if (!aShapeIter.More())
            {
                StdPrs_WFShape.Add(thePrs, theShape, theDrawer);
                return;
            }
            bool aDrawAllVerticesFlag = (theDrawer.VertexDrawMode() == Prs3d_VertexDrawMode.Prs3d_VDM_All);
            if (!aDrawAllVerticesFlag && theShape.ShapeType() != TopAbs_ShapeEnum.TopAbs_COMPOUND)
            {
                return;
            }

            // We have to create a compound and collect all subshapes not drawn by the shading algo.
            // This includes:
            // - isolated edges
            // - isolated vertices, if aDrawAllVerticesFlag == Standard_False
            // - all shape's vertices, if aDrawAllVerticesFlag == Standard_True
            TopoDS_Compound aCompoundWF = new TopoDS_Compound();
            BRep_Builder aBuilder = new BRep_Builder();
            aBuilder.MakeCompound(aCompoundWF);
            bool hasElement = false;

            // isolated edges
            for (aShapeIter.Init(theShape, TopAbs_ShapeEnum.TopAbs_EDGE, TopAbs_ShapeEnum.TopAbs_FACE); aShapeIter.More(); aShapeIter.Next())
            {
                hasElement = true;
                aBuilder.Add(aCompoundWF, aShapeIter.Current());
            }
            // isolated or all vertices
            aShapeIter.Init(theShape, TopAbs_ShapeEnum.TopAbs_VERTEX, aDrawAllVerticesFlag ? TopAbs_ShapeEnum.TopAbs_SHAPE : TopAbs_ShapeEnum.TopAbs_EDGE);
            for (; aShapeIter.More(); aShapeIter.Next())
            {
                hasElement = true;
                aBuilder.Add(aCompoundWF, aShapeIter.Current());
            }
            if (hasElement)
            {
                StdPrs_WFShape.Add(thePrs, aCompoundWF, theDrawer);
            }
        }

        //! Compute boundary presentation for faces of the shape.
        static Graphic3d_ArrayOfSegments fillFaceBoundaries(TopoDS_Shape theShape,
                                                               GeomAbs_Shape theUpperContinuity)
        {
            // collection of all triangulation nodes on edges
            // for computing boundaries presentation
            int aNodeNumber = 0;
            int aNbPolylines = 0;

            TopLoc_Location aTrsf = new TopLoc_Location();

            TColgp_SequenceOfPnt aSeqPntsExtra = null;
            for (TopExp_Explorer aFaceIter = new TopExp_Explorer(theShape, TopAbs_ShapeEnum.TopAbs_FACE); aFaceIter.More(); aFaceIter.Next())
            {
                TopoDS_Face aFace = TopoDS.Face(aFaceIter.Current());
                if (aFace.NbChildren() == 0)
                {
                    // handle specifically faces without boundary definition (triangulation-only)
                    if (aSeqPntsExtra == null)
                    {
                        //Handle(NCollection_IncAllocator) anIncAlloc = new NCollection_IncAllocator();
                        aSeqPntsExtra = new TColgp_SequenceOfPnt();
                    }
                    StdPrs_WFShape.AddEdgesOnTriangulation(aSeqPntsExtra, aFace, false);
                }
            }

            // explore all boundary edges
            TopTools_IndexedDataMapOfShapeListOfShape anEdgesMap = new TopTools_IndexedDataMapOfShapeListOfShape();
            TopExp.MapShapesAndAncestors(theShape, TopAbs_ShapeEnum.TopAbs_EDGE, TopAbs_ShapeEnum.TopAbs_FACE, anEdgesMap);
            foreach (var item in anEdgesMap.items)

            //  for (TopTools_IndexedDataMapOfShapeListOfShape::Iterator anEdgeIter (anEdgesMap); anEdgeIter.More(); anEdgeIter.Next())
            {
                var anEdgeIter = item;
                // reject free edges
                if (anEdgeIter.list.Extent() == 0)
                {
                    continue;
                }

                // take one of the shared edges and get edge triangulation
                TopoDS_Face aFace = TopoDS.Face(anEdgeIter.list.First());
                Poly_Triangulation aTriangulation = BRep_Tool.Triangulation(aFace, ref aTrsf);
                if (aTriangulation == null)
                {
                    continue;
                }

                TopoDS_Edge anEdge = TopoDS.Edge(anEdgeIter.shape);
                if (theUpperContinuity < GeomAbs_Shape.GeomAbs_CN
                 && anEdgeIter.list.Extent() >= 2
                 && BRep_Tool.MaxContinuity(anEdge) > theUpperContinuity)
                {
                    continue;
                }

                Poly_PolygonOnTriangulation anEdgePoly = BRep_Tool.PolygonOnTriangulation(anEdge, aTriangulation, aTrsf);
                if (anEdgePoly != null
                  && anEdgePoly.Nodes().Length() >= 2)
                {
                    aNodeNumber += anEdgePoly.Nodes().Length();
                    ++aNbPolylines;
                }
            }
            Graphic3d_ArrayOfSegments aSegments = null;

            int aNbExtra = aSeqPntsExtra != null ? aSeqPntsExtra.Size() : 0;
            if (aNodeNumber == 0)
            {
                if (aNbExtra < 2)
                {
                    return null;
                }

                aSegments = new Graphic3d_ArrayOfSegments(aNbExtra);
                foreach (var aPntIter in aSeqPntsExtra.list)

                //for (TColgp_SequenceOfPnt::Iterator aPntIter (*aSeqPntsExtra); aPntIter.More(); aPntIter.Next())
                {
                    aSegments.AddVertex(aPntIter);
                }
                return aSegments;
            }

            // create indexed segments array to pack polylines from different edges into single array
            int aSegmentEdgeNb = (aNodeNumber - aNbPolylines) * 2;

            aSegments = new Graphic3d_ArrayOfSegments(aNodeNumber + aNbExtra, aSegmentEdgeNb + aNbExtra);
            foreach (var anEdgeIter in anEdgesMap.items)
            {


                //for (TopTools_IndexedDataMapOfShapeListOfShape::Iterator anEdgeIter (anEdgesMap); anEdgeIter.More(); anEdgeIter.Next())

                if (anEdgeIter.list.Extent() == 0)
                {
                    continue;
                }

                TopoDS_Face aFace = TopoDS.Face(anEdgeIter.list.First());
                Poly_Triangulation aTriangulation = BRep_Tool.Triangulation(aFace, ref aTrsf);
                if (aTriangulation == null)
                {
                    continue;
                }

                TopoDS_Edge anEdge = TopoDS.Edge(anEdgeIter.shape);
                if (theUpperContinuity < GeomAbs_Shape.GeomAbs_CN
                 && anEdgeIter.list.Extent() >= 2
                 && BRep_Tool.MaxContinuity(anEdge) > theUpperContinuity)
                {
                    continue;
                }

                Poly_PolygonOnTriangulation anEdgePoly = BRep_Tool.PolygonOnTriangulation(anEdge, aTriangulation, aTrsf);
                if (anEdgePoly == null
                 || anEdgePoly.Nodes().Length() < 2)
                {
                    continue;
                }

                // get edge nodes indexes from face triangulation
                TColStd_Array1OfInteger anEdgeNodes = anEdgePoly.Nodes();

                // collect the edge nodes
                int aSegmentEdge = aSegments.VertexNumber() + 1;
                for (int aNodeIdx = anEdgeNodes.Lower(); aNodeIdx <= anEdgeNodes.Upper(); ++aNodeIdx)
                {
                    // node index in face triangulation
                    // get node and apply location transformation to the node
                    int aTriIndex = anEdgeNodes.Value(aNodeIdx);
                    gp_Pnt aTriNode = aTriangulation.Node(aTriIndex);
                    if (!aTrsf.IsIdentity())
                    {
                        aTriNode.Transform(aTrsf);
                    }

                    aSegments.AddVertex(aTriNode);
                    if (aNodeIdx != anEdgeNodes.Lower())
                    {
                        aSegments.AddEdge(aSegmentEdge);
                        aSegments.AddEdge(++aSegmentEdge);
                    }
                }
            }

            if (aSeqPntsExtra != null)
            {
                int aSegmentEdge = aSegments.VertexNumber();
                foreach (var aPntIter in aSeqPntsExtra.list)
                {
                    //for (TColgp_SequenceOfPnt::Iterator aPntIter (*aSeqPntsExtra); aPntIter.More(); aPntIter.Next())
                    //{
                    aSegments.AddVertex(aPntIter);
                    aSegments.AddEdge(++aSegmentEdge);
                }
            }

            return aSegments;
        }
    }
}