using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCCPort
{
    public class V3d_View
    {

        double myOldMouseX;
        double myOldMouseY;
        gp_Dir myCamStartOpUp;
        gp_Dir myCamStartOpDir;
        gp_Pnt myCamStartOpEye;
        gp_Pnt myCamStartOpCenter;
        Graphic3d_Camera myDefaultCamera;
        Graphic3d_CView myView;
        bool myImmediateUpdate;
        //mutable Standard_Boolean myIsInvalidatedImmediate;

        //! Returns camera object of the view.
        //! @return: handle to camera object, or NULL if 3D view does not use
        //! the camera approach.
        Graphic3d_Camera Camera;

        gp_Vec myXscreenAxis;
        gp_Vec myYscreenAxis;
        gp_Vec myZscreenAxis;
        gp_Dir myViewAxis;
        Graphic3d_Vertex myGravityReferencePoint;
        bool myAutoZFitIsOn;
        double myAutoZFitScaleFactor;

        //  V3d_ListOfLight myActiveLights;
        //  gp_Dir myDefaultViewAxis;
        //gp_Pnt myDefaultViewPoint;
        AspectWindow MyWindow;
        int sx;
        int sy;
        double rx;
        double ry;
        gp_Pnt myRotateGravity;
        bool myComputedMode;
        bool SwitchSetFront;
        bool myZRotation;
        bool MyZoomAtPointX;
        bool MyZoomAtPointY;

        //! Converts the PIXEL value
        //! to a value in the projection plane.
        double Convert(int Vp)
        {
            int aDxw, aDyw;

            //V3d_UnMapped_Raise_if(!myView->IsDefined(), "view has no window");

            MyWindow.Size(out aDxw, out aDyw);
            double aValue;

            var aViewDims = Camera.ViewDimensions();
            aValue = aViewDims.X() * (float)Vp / (float)aDxw;

            return aValue;
        }
        double DEUXPI = (2.0 * Math.PI);
        //=============================================================================
        //function : Rotate
        //purpose  :
        //=============================================================================
        public void Rotate(double ax, double ay, double az,
                       double X, double Y, double Z, bool Start)
        {

            double Ax = ax;
            double Ay = ay;
            double Az = az;

            if (Ax > 0.0) while (Ax > DEUXPI) Ax -= DEUXPI;
            else if (Ax < 0.0) while (Ax < -DEUXPI) Ax += DEUXPI;
            if (Ay > 0.0) while (Ay > DEUXPI) Ay -= DEUXPI;
            else if (Ay < 0.0) while (Ay < -DEUXPI) Ay += DEUXPI;
            if (Az > 0.0) while (Az > DEUXPI) Az -= DEUXPI;
            else if (Az < 0.0) while (Az < -DEUXPI) Az += DEUXPI;

            var aCamera = Camera;

            if (Start)
            {
                myGravityReferencePoint.SetCoord(X, Y, Z);
                myCamStartOpUp = aCamera.Up();
                myCamStartOpDir = aCamera.Direction();
                myCamStartOpEye = aCamera.Eye();
                myCamStartOpCenter = aCamera.Center();
            }

            var aVref = myGravityReferencePoint;

            aCamera.SetUp(myCamStartOpUp);
            aCamera.SetEyeAndCenter(myCamStartOpEye, myCamStartOpCenter);
            aCamera.SetDirectionFromEye(myCamStartOpDir);

            // rotate camera around 3 initial axes
            gp_Pnt aRCenter = new gp_Pnt(aVref.X(), aVref.Y(), aVref.Z());

            gp_Dir aZAxis = new gp_Dir(aCamera.Direction().Reversed());
            gp_Dir aYAxis = new gp_Dir(aCamera.Up());
            gp_Dir aXAxis = new gp_Dir(aYAxis.Crossed(aZAxis));

            gp_Trsf[] aRot = new gp_Trsf[3];
            gp_Trsf aTrsf = new gp_Trsf();
            aRot[0].SetRotation(new gp_Ax1(aRCenter, aYAxis), -Ax);
            aRot[1].SetRotation(new gp_Ax1(aRCenter, aXAxis), Ay);
            aRot[2].SetRotation(new gp_Ax1(aRCenter, aZAxis), Az);
            aTrsf.Multiply(aRot[0]);
            aTrsf.Multiply(aRot[1]);
            aTrsf.Multiply(aRot[2]);

            aCamera.Transform(aTrsf);

            ImmediateUpdate();
        }

        /*private object gp_Ax1(gp_Pnt aRCenter, Func<aCamera.Up, (object, object), gpDir> aYAxis)
        {
            throw new NotImplementedException();
        }*/

        const int THE_NB_BOUND_POINTS = 8;
        //=======================================================================
        //function : GravityPoint
        //purpose  :
        //=======================================================================
        gp_Pnt GravityPoint()
        {
            /*Graphic3d_MapOfStructure aSetOfStructures;
            myView.DisplayedStructures(aSetOfStructures);

            bool hasSelection = false;
            for (Graphic3d_MapIteratorOfMapOfStructure aStructIter (aSetOfStructures);
                 aStructIter.More(); aStructIter.Next())
            {
                if (aStructIter.Key()->IsHighlighted()
                 && aStructIter.Key()->IsVisible())
                {
                    hasSelection = true;
                    break;
                }
            }
            */
            double Xmin, Ymin, Zmin, Xmax, Ymax, Zmax;
            int aNbPoints = 0;
            gp_XYZ aResult = new gp_XYZ(0.0, 0.0, 0.0);
            //        for (Graphic3d_MapIteratorOfMapOfStructure aStructIter (aSetOfStructures);
            //             aStructIter.More(); aStructIter.Next())
            //        {
            //            Graphic3dStructure aStruct = aStructIter.Key();
            //            if (!aStruct->IsVisible()
            //              || aStruct->IsInfinite()
            //              || (hasSelection && !aStruct->IsHighlighted()))
            //            {
            //                continue;
            //            }

            //            Graphic3dBndBox3d aBox = aStruct->CStructure()->BoundingBox();
            //            if (!aBox.IsValid())
            //            {
            //                continue;
            //            }

            //            // skip transformation-persistent objects
            //            if (!aStruct.TransformPersistence().IsNull())
            //            {
            //                continue;
            //            }

            //            // use camera projection to find gravity point
            //            Xmin = aBox.CornerMin().x();
            //            Ymin = aBox.CornerMin().y();
            //            Zmin = aBox.CornerMin().z();
            //            Xmax = aBox.CornerMax().x();
            //            Ymax = aBox.CornerMax().y();
            //            Zmax = aBox.CornerMax().z();
            //            gp_Pnt[] aPnts = new gp_Pnt[THE_NB_BOUND_POINTS]
            //            {
            // new gp_Pnt (Xmin, Ymin, Zmin),new  gp_Pnt (Xmin, Ymin, Zmax),
            //new  gp_Pnt (Xmin, Ymax, Zmin), new gp_Pnt (Xmin, Ymax, Zmax),
            // new gp_Pnt (Xmax, Ymin, Zmin), new gp_Pnt (Xmax, Ymin, Zmax),
            // new gp_Pnt (Xmax, Ymax, Zmin), new gp_Pnt (Xmax, Ymax, Zmax)
            //};

            //            for (int aPntIt = 0; aPntIt < THE_NB_BOUND_POINTS; ++aPntIt)
            //            {
            //                gp_Pnt aBndPnt = aPnts[aPntIt];
            //                gp_Pnt aProjected = Camera.Project(aBndPnt);
            //                if (Math.Abs(aProjected.X()) <= 1.0
            //                 && Math.Abs(aProjected.Y()) <= 1.0)
            //                {
            //                    aResult += aBndPnt.XYZ();
            //                    ++aNbPoints;
            //                }
            //            }
            //        }

            if (aNbPoints == 0)
            {
                // fallback - just use bounding box of entire scene
                Bnd_Box aBox = myView.MinMaxValues();
                if (!aBox.IsVoid())
                {
                    aBox.Get(out Xmin, out Ymin, out Zmin,
                             out Xmax, out Ymax, out Zmax);
                    gp_Pnt[] aPnts = new gp_Pnt[THE_NB_BOUND_POINTS]
                    {
       new  gp_Pnt (Xmin, Ymin, Zmin), new gp_Pnt(Xmin, Ymin, Zmax),
       new  gp_Pnt (Xmin, Ymax, Zmin),new gp_Pnt (Xmin, Ymax, Zmax),
       new gp_Pnt (Xmax, Ymin, Zmin), new gp_Pnt(Xmax, Ymin, Zmax),
       new gp_Pnt (Xmax, Ymax, Zmin),new  gp_Pnt (Xmax, Ymax, Zmax)
      };

                    for (int aPntIt = 0; aPntIt < THE_NB_BOUND_POINTS; ++aPntIt)
                    {
                        gp_Pnt aBndPnt = aPnts[aPntIt];
                        aResult.Add(aBndPnt.XYZ());
                        ++aNbPoints;
                    }
                }
            }

            if (aNbPoints > 0)
            {
                aResult.Divide(aNbPoints);
            }

            return new gp_Pnt(aResult);
        }

        private void ImmediateUpdate()
        {
            throw new NotImplementedException();
        }

        //=============================================================================
        //function : StartRotation
        //purpose  :
        //=============================================================================
        public void StartRotation(int X,
                             int Y,
                             double zRotationThreshold = 0)
        {
            sx = X; sy = Y;
            int x, y;
            Size(out x, out y);
            rx = Convert(x);
            ry = Convert(y);
            myRotateGravity = GravityPoint();
            Rotate(0.0, 0.0, 0.0,
                    myRotateGravity.X(), myRotateGravity.Y(), myRotateGravity.Z(),
                    true);
            myZRotation = false;
            if (zRotationThreshold > 0.0)
            {
                var dx = Math.Abs(sx - rx / 2.0);
                var dy = Math.Abs(sy - ry / 2.0);
                //  if( dx > rx/3. || dy > ry/3. ) myZRotation = Standard_True;
                var dd = zRotationThreshold * (rx + ry) / 2.0;
                if (dx > dd || dy > dd) myZRotation = true;
            }

        }

        private void Size(out int x, out int y)
        {
            throw new NotImplementedException();
        }
    }
}
