using OCCPort.Tester;
using System;
using System.Security.Cryptography;

namespace OCCPort.Tester
{
    //! Presentation of the shape with customizable sub-shapes properties.
    public class AIS_ColoredShape : AIS_Shape

    {
        protected AIS_DataMapOfShapeDrawer myShapeColors;
        //=======================================================================
        //function : Compute
        //purpose  :
        //=======================================================================
        public override void Compute(PrsMgr_PresentationManager thePrsMgr,
                                   Prs3d_Presentation thePrs,
                                   int theMode)
        {
            if (myshape == null)
            {
                return;
            }

            if (IsInfinite())
            {
                thePrs.SetInfiniteState(true);
            }

            switch (theMode)
            {
                case (int)AIS_DisplayMode.AIS_WireFrame:
                    {
                        StdPrs_ToolTriangulatedShape.ClearOnOwnDeflectionChange(myshape, myDrawer, true);

                        // After this call if type of deflection is relative
                        // computed deflection coefficient is stored as absolute.
                        StdPrs_ToolTriangulatedShape.GetDeflection(myshape, myDrawer);
                        break;
                    }
                case (int)AIS_DisplayMode.AIS_Shaded:
                    {
                        if (myDrawer.IsAutoTriangulation())
                        {
                            // compute mesh for entire shape beforehand to ensure consistency and optimizations (parallelization)
                            StdPrs_ToolTriangulatedShape.ClearOnOwnDeflectionChange(myshape, myDrawer, true);

                            // After this call if type of deflection is relative
                            // computed deflection coefficient is stored as absolute.
                            bool wasRecomputed = StdPrs_ToolTriangulatedShape.Tessellate(myshape, myDrawer);

                            // Set to update wireframe presentation on triangulation.
                            if (myDrawer.IsoOnTriangulation() && wasRecomputed)
                            {
                                SetToUpdate((int)AIS_DisplayMode.AIS_WireFrame);
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        Compute(thePrsMgr, thePrs, theMode);
                        return;
                    }
                default:
                    {
                        return;
                    }
            }

            // Extract myShapeColors map (KeyshapeColored -> Color) to subshapes map (Subshape -> Color).
            // This needed when colored shape is not part of BaseShape (but subshapes are) and actually container for subshapes.
            AIS_DataMapOfShapeDrawer aSubshapeDrawerMap = new AIS_DataMapOfShapeDrawer();
            fillSubshapeDrawerMap(aSubshapeDrawerMap);

            AIS_ColoredDrawer aBaseDrawer = new AIS_ColoredDrawer();
            myShapeColors.Find(myshape, aBaseDrawer);

            // myShapeColors + anOpened --> array[TopAbs_ShapeEnum] of map of color-to-compound
            DataMapOfDrawerCompd[] aDispatchedOpened = new DataMapOfDrawerCompd[(int)TopAbs_ShapeEnum.TopAbs_SHAPE];
            DataMapOfDrawerCompd aDispatchedClosed = new DataMapOfDrawerCompd();
            dispatchColors(aBaseDrawer, myshape,
                            aSubshapeDrawerMap, TopAbs_ShapeEnum.TopAbs_COMPOUND, false,
                            aDispatchedOpened, theMode == (int)AIS_DisplayMode.AIS_Shaded ? aDispatchedClosed : aDispatchedOpened[(int)TopAbs_ShapeEnum.TopAbs_FACE]);
            addShapesWithCustomProps(thePrs, aDispatchedOpened, aDispatchedClosed, theMode);
        }

        private void SetToUpdate(int aIS_WireFrame)
        {
            throw new NotImplementedException();
        }

        private void dispatchColors(AIS_ColoredDrawer aBaseDrawer, TopoDS_Shape myshape, AIS_DataMapOfShapeDrawer aSubshapeDrawerMap, TopAbs_ShapeEnum topAbs_COMPOUND, bool v, DataMapOfDrawerCompd[] aDispatchedOpened, DataMapOfDrawerCompd dataMapOfDrawerCompd)
        {
            throw new NotImplementedException();
        }

        private void addShapesWithCustomProps(Prs3d_Presentation thePrs,
            DataMapOfDrawerCompd[] aDispatchedOpened, DataMapOfDrawerCompd aDispatchedClosed, int theMode)
        {
            throw new NotImplementedException();
        }

        private void fillSubshapeDrawerMap(AIS_DataMapOfShapeDrawer aSubshapeDrawerMap)
        {
            throw new NotImplementedException();
        }

        private bool IsInfinite()
        {
            throw new NotImplementedException();
        }
    }
}