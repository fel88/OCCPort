using System;

namespace OCCPort
{
    internal class Graphic3d_Camera
    {


        //! Enumerates supported monographic projections.
        //! - Projection_Orthographic : orthographic projection.
        //! - Projection_Perspective  : perspective projection.
        //! - Projection_Stereo       : stereographic projection.
        //! - Projection_MonoLeftEye  : mono projection for stereo left eye.
        //! - Projection_MonoRightEye : mono projection for stereo right eye.
        public enum Projection
        {
            Projection_Orthographic,
            Projection_Perspective,
            Projection_Stereo,
            Projection_MonoLeftEye,
            Projection_MonoRightEye
        };

        //! Enumerates approaches to define stereographic focus.
        //! - FocusType_Absolute : focus is specified as absolute value.
        //! - FocusType_Relative : focus is specified relative to
        //! (as coefficient of) camera focal length.
        public  enum FocusType
        {
            FocusType_Absolute,
            FocusType_Relative
        };

        //! Enumerates approaches to define Intraocular distance.
        //! - IODType_Absolute : Intraocular distance is defined as absolute value.
        //! - IODType_Relative : Intraocular distance is defined relative to
        //! (as coefficient of) camera focal length.
        public enum IODType
        {
            IODType_Absolute,
            IODType_Relative
        };



        //! Default constructor.
        //! Initializes camera with the following properties:
        //! Eye (0, 0, -2); Center (0, 0, 0); Up (0, 1, 0);
        //! Type (Orthographic); FOVy (45); Scale (1000); IsStereo(false);
        //! ZNear (0.001); ZFar (3000.0); Aspect(1);
        //! ZFocus(1.0); ZFocusType(Relative); IOD(0.05); IODType(Relative)
        Graphic3d_Camera()
        {
            myUp = new gp_Dir(0.0, 1.0, 0.0);
            myDirection = new gp_Dir(0.0, 0.0, 1.0);
            myEye = new gp_Pnt(0.0, 0.0, -1500.0);
            myDistance = (1500.0);
            myAxialScale = new gp_XYZ(1.0, 1.0, 1.0);
            myProjType = Projection.Projection_Orthographic;
            myFOVy = (45.0);
            myFOVx = (45.0);
            myFOV2d = (180.0);
            /*myFOVyTan(Tan(DTR_HALF * 45.0)),
            myZNear(DEFAULT_ZNEAR),
            myZFar(DEFAULT_ZFAR),
            myAspect(1.0),
            myIsZeroToOneDepth(false),
            myScale(1000.0),
            myZFocus(1.0),
            myZFocusType(FocusType_Relative),
            myIOD(0.05),
            myIODType(IODType_Relative),
            myIsCustomProjMatM(false),
            myIsCustomProjMatLR(false),
            myIsCustomFrustomLR(false)*/
        }
        public gp_Dir Up()
        {
            return myUp;
        }


        Projection myProjType; //!< Projection type used for rendering.
        double myFOVy;     //!< Field Of View in y axis.
        double myFOVx;     //!< Field Of View in x axis.
        double myFOV2d;    //!< Field Of View limit for 2d 

        private gp_Dir myUp;       //!< Camera up direction vector
        private gp_Dir myDirection;//!< Camera view direction (from eye)
        private gp_Pnt myEye;      //!< Camera eye position
        private double myDistance; //!< distance from Eye to Center

        private gp_XYZ myAxialScale; //!< World axial scale.

        //! Get camera Up direction vector.
        //! @return Camera's Up direction vector.

        internal gp_Pnt ViewDimensions()
        {
            return new gp_Pnt();
        }

        internal gp_Pnt Project(gp_Pnt aBndPnt)
        {
            throw new NotImplementedException();
        }

        internal void Transform(gp_Trsf aTrsf)
        {

        }

        internal gp_Dir Direction()
        {
            throw new NotImplementedException();
        }

        internal gp_Pnt Eye()
        {
            throw new NotImplementedException();
        }

        //! Get Center of the camera, e.g. the point where camera looks at.
        //! This point is computed as Eye() translated along Direction() at Distance().
        //! @return the point where the camera looks at.
        public gp_Pnt Center()
        {
            var ret = myEye.XYZ() + myDirection.XYZ() * myDistance;
            return new gp_Pnt(ret);
        }

        //! Sets camera Up direction vector, orthogonal to camera direction.
        //! WARNING! This method does NOT verify that the new Up vector is orthogonal to the current Direction().
        //! @param theUp [in] the Up direction vector.
        //! @sa OrthogonalizeUp().
        public void SetUp(gp_Dir theUp)
        {
            if (Up().IsEqual(theUp, 0.0))
            {
                return;
            }

            myUp = theUp;
            InvalidateOrientation();
        }

        TransformMatrices<double> myMatricesD;
        TransformMatrices<float> myMatricesF;
        private void InvalidateOrientation()
        {
            myMatricesD.ResetOrientation();
            myMatricesF.ResetOrientation();
            //myWorldViewProjState.WorldViewState() = (Standard_Size)Standard_Atomic_Increment(&THE_STATE_COUNTER);
        }

        internal void SetDirectionFromEye(gp_Dir myCamStartOpDir)
        {
            throw new NotImplementedException();
        }

        internal void SetEyeAndCenter(gp_Pnt myCamStartOpEye, gp_Pnt myCamStartOpCenter)
        {
            throw new NotImplementedException();
        }
    }
}
