using System;
using System.Runtime.InteropServices.ComTypes;

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
        public enum FocusType
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
        public Graphic3d_Camera()
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

            // (degrees -> radians) * 0.5
            const double DTR_HALF = 0.5 * 0.0174532925;

            // default property values
            const double DEFAULT_ZNEAR = 0.001;
            const double DEFAULT_ZFAR = 3000.0;

            myFOVyTan = (Math.Tan(DTR_HALF * 45.0));

            myZNear = (DEFAULT_ZNEAR);
            myZFar = (DEFAULT_ZFAR);
            myAspect = (1.0);/*
            myIsZeroToOneDepth(false),*/
            myScale = (1000.0);/*
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

        double myFOVyTan;  //!< Field Of View as Tan(DTR_HALF * myFOVy)
        double myZNear;    //!< Distance to near clipping plane.
        double myZFar;     //!< Distance to far clipping plane.
        double myAspect;   //!< Width to height display ratio.
        bool myIsZeroToOneDepth; //!< use [0, 1] depth range or [-1, 1]

        double myScale;      //!< Specifies parallel scale for orthographic projection.
        double myZFocus;     //!< Stereographic focus value.
        FocusType myZFocusType; //!< Stereographic focus definition type.

        private gp_XYZ myAxialScale; //!< World axial scale.

        //! Get camera Up direction vector.
        //! @return Camera's Up direction vector.
        //! Get distance of Eye from camera Center.
        //! @return the distance.
        double Distance() { return myDistance; }

        //! Check that the camera projection is orthographic.
        //! @return boolean flag that indicates whether the camera's projection is
        //! orthographic or not.
        bool IsOrthographic()
        {
            return (myProjType == Projection.Projection_Orthographic);
        }

        internal gp_XYZ ViewDimensions()
        {
            return ViewDimensions(Distance());
        }

        //! Calculate view plane size at center point with specified Z offset
        //! and distance between ZFar and ZNear planes.
        //! @param theZValue [in] the distance from the eye in eye-to-center direction
        //! @return values in form of gp_Pnt (Width, Height, Depth).
        public gp_XYZ ViewDimensions(double theZValue)
        {
            // view plane dimensions
            double aSize = IsOrthographic() ? myScale : (2.0 * theZValue * myFOVyTan);
            double aSizeX, aSizeY;
            if (myAspect > 1.0)
            {
                aSizeX = aSize * myAspect;
                aSizeY = aSize;
            }
            else
            {
                aSizeX = aSize;
                aSizeY = aSize / myAspect;
            }

            // and frustum depth
            return new gp_XYZ(aSizeX, aSizeY, myZFar - myZNear);
        }

        internal gp_Pnt Project(gp_Pnt aBndPnt)
        {
            throw new NotImplementedException();
        }

        internal void Transform(gp_Trsf theTrsf)
        {
            if (theTrsf.Form() == gp_TrsfForm.gp_Identity)
            {
                return;
            }

            myUp.Transform(theTrsf);
            myDirection.Transform(theTrsf);
            myEye.Transform(theTrsf);
            InvalidateOrientation();
        }

        internal gp_Dir Direction()
        {
            return myDirection;
        }

        internal gp_Pnt Eye()
        {
            return myEye;
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

        TransformMatrices<double> myMatricesD = new TransformMatrices<double>();
        TransformMatrices<float> myMatricesF = new TransformMatrices<float>();
        private void InvalidateOrientation()
        {
            myMatricesD.ResetOrientation();
            myMatricesF.ResetOrientation();
            //myWorldViewProjState.WorldViewState() = (Standard_Size)Standard_Atomic_Increment(&THE_STATE_COUNTER);
        }

        internal void SetDirectionFromEye(gp_Dir theDir)
        {
            if (myDirection.IsEqual(theDir, 0.0))
            {
                return;
            }

            myDirection = theDir;
            InvalidateOrientation();
        }

        public void SetEyeAndCenter(gp_Pnt theEye,
                                         gp_Pnt theCenter)
        {
            if (Eye().IsEqual(theEye, 0.0)
             && Center().IsEqual(theCenter, 0.0))
            {
                return;
            }

            myEye = theEye;
            myDistance = theEye.Distance(theCenter);
            if (myDistance > gp.Resolution())
            {
                myDirection = new gp_Dir(theCenter.XYZ() - theEye.XYZ());
            }
            InvalidateOrientation();
        }

        internal void SetEye(gp_Pnt myCamStartOpEye)
        {
            throw new NotImplementedException();
        }

        internal void SetCenter(gp_Pnt myCamStartOpCenter)
        {
            throw new NotImplementedException();
        }

        internal void SetScale(double theScale)
        {
            if (Scale() == theScale)
            {
                return;
            }

            myScale = theScale;

            switch (myProjType)
            {
                case Projection.Projection_Perspective:
                case Projection.Projection_Stereo:
                case Projection.Projection_MonoLeftEye:
                case Projection.Projection_MonoRightEye:
                    {
                        var aDistance = theScale * 0.5 / myFOVyTan;
                        SetDistance(aDistance);
                    }
                    break;
                default:
                    break;
            }

            InvalidateProjection();

        }

        private void InvalidateProjection()
        {
            myMatricesD.ResetProjection();
            myMatricesF.ResetProjection();
            //myWorldViewProjState.ProjectionState() = (Standard_Size)Standard_Atomic_Increment(&THE_STATE_COUNTER);

        }

        private void SetDistance(double theDistance)
        {
            if (myDistance == theDistance)
            {
                return;
            }

            gp_Pnt aCenter = Center();
            myDistance = theDistance;
            myEye = new gp_Pnt(aCenter.XYZ() - myDirection.XYZ() * myDistance);
            InvalidateOrientation();

        }

        public double Scale()
        {
            switch (myProjType)
            {
                case Projection.Projection_Orthographic:
                    return myScale;

                // case Projection_Perspective  :
                // case Projection_Stereo       :
                // case Projection_MonoLeftEye  :
                // case Projection_MonoRightEye :
                default:
                    return Distance() * 2.0 * myFOVyTan;
            }

        }

        internal double Aspect()
        {
            return myAspect;

        }
    }
    //! Identifies the type of a geometric transformation.
    public enum gp_TrsfForm
    {
        gp_Identity,     //!< No transformation (matrix is identity)
        gp_Rotation,     //!< Rotation
        gp_Translation,  //!< Translation
        gp_PntMirror,    //!< Central symmetry
        gp_Ax1Mirror,    //!< Rotational symmetry
        gp_Ax2Mirror,    //!< Bilateral symmetry
        gp_Scale,        //!< Scale
        gp_CompoundTrsf, //!< Combination of the above transformations
        gp_Other         //!< Transformation with not-orthogonal matrix
    };

}
