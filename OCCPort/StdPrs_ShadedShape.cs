using System;
using System.Collections.Generic;
using System.Diagnostics;

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
            //wireframeFromShape(thePrs, theShape, theDrawer);

            // add special wireframe presentation for faces without triangulation
            //   wireframeNoTriangFacesFromShape(thePrs, theShape, theDrawer);

            // The shape types listed below need advanced analysis as potentially containing
            // both closed and open parts. Solids are also included, because they might
            // contain non-manifold parts inside (internal open shells)
            //if ((theShape.ShapeType() == TopAbs_COMPOUND
            //  || theShape.ShapeType() == TopAbs_COMPSOLID
            //  || theShape.ShapeType() == TopAbs_SOLID)
            // && theVolume == StdPrs_Volume_Autodetection)
            //{
            //    // collect two compounds: for opened and closed (solid) sub-shapes
            //    TopoDS_Compound anOpened, aClosed;
            //    BRep_Builder aBuilder;
            //    aBuilder.MakeCompound(aClosed);
            //    aBuilder.MakeCompound(anOpened);
            //    ExploreSolids(theShape, aBuilder, aClosed, anOpened, Standard_True);

            //    if (aClosed.NbChildren() > 0)
            //    {
            //        shadeFromShape(aClosed, thePrs, theDrawer,
            //                        theHasTexels, theUVOrigin, theUVRepeat, theUVScale, true);
            //    }

            //    if (anOpened.NbChildren() > 0)
            //    {
            //        shadeFromShape(anOpened, thePrs, theDrawer,
            //                        theHasTexels, theUVOrigin, theUVRepeat, theUVScale, false);
            //    }
            //}
            //else
            //{
            //    // if the shape type is not compound, composolid or solid, use autodetection back-facing filled
            //    shadeFromShape(theShape, thePrs, theDrawer,
            //                    theHasTexels, theUVOrigin, theUVRepeat, theUVScale,
            //                    theVolume == StdPrs_Volume_Closed);
            //}

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
        //! Compute boundary presentation for faces of the shape.
        static Graphic3d_ArrayOfSegments fillFaceBoundaries(TopoDS_Shape theShape,
                                                               GeomAbs_Shape theUpperContinuity)
        {
            // collection of all triangulation nodes on edges
            // for computing boundaries presentation
            int aNodeNumber = 0;
            int aNbPolylines = 0;

            TopLoc_Location aTrsf;

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

            return null;
        }
    }
}