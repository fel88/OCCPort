﻿using OCCPort.Tester;
using OCCPort;
using System;

namespace OCCPort.Tester
{
    public class AIS_Shape : AIS_InteractiveObject
    {

        public override void Compute(PrsMgr_PresentationManager thePrsMgr,
                                   Prs3d_Presentation thePrs,
                                   int theMode)
        {

            if (myshape.IsNull()
             || (myshape.ShapeType() == TopAbs_ShapeEnum.TopAbs_COMPOUND && myshape.NbChildren() == 0))
            {
                return;
            }


            // wire,edge,vertex -> pas de HLR + priorite display superieure
            if (myshape.ShapeType() >= TopAbs_ShapeEnum.TopAbs_WIRE
             && myshape.ShapeType() <= TopAbs_ShapeEnum.TopAbs_VERTEX)
            {
                // TopAbs_WIRE -> 7, TopAbs_EDGE -> 8, TopAbs_VERTEX -> 9 (Graphic3d_DisplayPriority_Highlight)
                /*int aPrior = (int)Graphic3d_DisplayPriority_Above1
                                             + (int)myshape.ShapeType() - TopAbs_ShapeEnum. TopAbs_WIRE;
               thePrs.SetVisual(Graphic3d_TOS_ALL);
               thePrs.SetDisplayPriority((Graphic3d_DisplayPriority)aPrior);*/
            }

            if (IsInfinite())
            {
                thePrs.SetInfiniteState(true); //not taken in account during FITALL
            }

            theMode = (int)AIS_DisplayMode.AIS_Shaded;
            switch (theMode)
            {

                case (int)AIS_DisplayMode.AIS_Shaded:
                    {
                        StdPrs_ToolTriangulatedShape.ClearOnOwnDeflectionChange(myshape, myDrawer, true);
                        if ((int)myshape.ShapeType() > 4)
                        {
                            StdPrs_WFShape.Add(thePrs, myshape, myDrawer);
                        }
                        else
                        {
                            if (IsInfinite())
                            {
                                StdPrs_WFShape.Add(thePrs, myshape, myDrawer);
                            }
                            else
                            {
                                /* try
                                 {*/
                                //OCC_CATCH_SIGNALS
                                StdPrs_ShadedShape.Add(thePrs, myshape, myDrawer,
                                                         myDrawer.ShadingAspect().Aspect().ToMapTexture()
                                                     && myDrawer.ShadingAspect().Aspect().TextureMap() != null,
                                                         myUVOrigin, myUVRepeat, myUVScale);

                                /*
                                }
                                catch (Standard_Failure const&anException)
          {
                                    Message::SendFail(TCollection_AsciiString("Error: AIS_Shape::Compute() shaded presentation builder has failed (")
                                                     + anException.GetMessageString() + ")");
                                    StdPrs_WFShape::Add(thePrs, myshape, myDrawer);
                                }*/
                            }
                        }
                        break;
                    }

            }
        }



        public AIS_Shape()
        {

        }
        public AIS_Shape(TopoDS_Shape theShape)
            : base(PrsMgr_TypeOfPresentation3d.PrsMgr_TOP_ProjectorDependent)
        {
            myshape = theShape;
            myUVOrigin = new gp_Pnt2d(0.0, 0.0);
            myUVRepeat = new gp_Pnt2d(1.0, 1.0);
            myUVScale = new gp_Pnt2d(1.0, 1.0);
            myInitAng = 0.0;
            myCompBB = true;
        }

        //! Returns this shape object.
        public TopoDS_Shape Shape() { return myshape; }


        protected TopoDS_Shape myshape;    //!< shape to display

        protected Bnd_Box myBB;       //!< cached bounding box of the shape
        protected gp_Pnt2d myUVOrigin; //!< UV origin vector for generating texture coordinates
        protected gp_Pnt2d myUVRepeat; //!< UV repeat vector for generating texture coordinates
        protected gp_Pnt2d myUVScale;  //!< UV scale  vector for generating texture coordinates
        double myInitAng;
        bool myCompBB;   //!< if TRUE, then bounding box should be recomputed



    }
}