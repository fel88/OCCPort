using OCCPort;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices.ComTypes;
using static System.Net.Mime.MediaTypeNames;

namespace OCCPort
{
    public class Graphic3d_Camera
    {
        Graphic3d_Mat4d myCustomProjMatM;
        Graphic3d_Mat4d myCustomProjMatL;
        Graphic3d_Mat4d myCustomProjMatR;
        Graphic3d_Mat4d myCustomHeadToEyeMatL;
        Graphic3d_Mat4d myCustomHeadToEyeMatR;
        Aspect_FrustumLRBT myCustomFrustumL;
        Aspect_FrustumLRBT myCustomFrustumR;
        //! Get current tile.
        public Graphic3d_CameraTile Tile() { return myTile; }
        //! Get stereographic focus value.
        //! @return absolute or relative stereographic focus value
        //! depending on its definition type.
        public double ZFocus()
        {
            return myZFocus;
        }

        //! Check whether the camera projection is stereo.
        //! Please note that stereo rendering is now implemented with support of
        //! Quad buffering.
        //! @return boolean flag indicating whether the stereographic L/R projection
        //! is chosen.
        public bool IsStereo()
        {
            return (myProjType == Projection.Projection_Stereo);
        }

        public void SetZFocus(FocusType theType, double theZFocus)
        {
            if (ZFocusType() == theType
             && ZFocus() == theZFocus)
            {
                return;
            }

            myZFocusType = theType;
            myZFocus = theZFocus;

            InvalidateProjection();
        }
        public bool FitMinMax(Bnd_Box theBox,
                                  double theResolution,
                                  bool theToEnlargeIfLine)
        {
            // Check bounding box for validness
            if (theBox.IsVoid())
            {
                return false; // bounding box is out of bounds...
            }

            // Apply "axial scaling" to the bounding points.
            // It is not the best approach to make this scaling as a part of fit all operation,
            // but the axial scale is integrated into camera orientation matrix and the other
            // option is to perform frustum plane adjustment algorithm in view camera space,
            // which will lead to a number of additional world-view space conversions and
            // loosing precision as well.
            gp_Pnt aBndMin = theBox.CornerMin().XYZ().Multiplied(myAxialScale).To_gp_Pnt();
            gp_Pnt aBndMax = theBox.CornerMax().XYZ().Multiplied(myAxialScale).To_gp_Pnt();
            if (aBndMax.IsEqual(aBndMin, Standard_Real.RealEpsilon()))
            {
                return false; // nothing to fit all
            }

            // Prepare camera frustum planes.
            gp_Pln[] aFrustumPlaneArray = new gp_Pln[6];
            NCollection_Array1<gp_Pln> aFrustumPlane = new NCollection_Array1<gp_Pln>(aFrustumPlaneArray, 1, 6);
            Frustum(aFrustumPlane[1], aFrustumPlane[2], aFrustumPlane[3],
                     aFrustumPlane[4], aFrustumPlane[5], aFrustumPlane[6]);

            // Prepare camera up, side, direction vectors.
            gp_Dir aCamUp = OrthogonalizedUp();
            gp_Dir aCamDir = Direction();
            gp_Dir aCamSide = aCamDir ^ aCamUp;

            // Prepare scene bounding box parameters.
            gp_Pnt aBndCenter = ((aBndMin.XYZ() + aBndMax.XYZ()) / 2.0).To_gp_Pnt();

            gp_Pnt[] aBndCornerArray = new gp_Pnt[8];
            NCollection_Array1<gp_Pnt> aBndCorner = new NCollection_Array1<gp_Pnt>(aBndCornerArray, 1, 8);
            aBndCorner[1].SetCoord(aBndMin.X(), aBndMin.Y(), aBndMin.Z());
            aBndCorner[2].SetCoord(aBndMin.X(), aBndMin.Y(), aBndMax.Z());
            aBndCorner[3].SetCoord(aBndMin.X(), aBndMax.Y(), aBndMin.Z());
            aBndCorner[4].SetCoord(aBndMin.X(), aBndMax.Y(), aBndMax.Z());
            aBndCorner[5].SetCoord(aBndMax.X(), aBndMin.Y(), aBndMin.Z());
            aBndCorner[6].SetCoord(aBndMax.X(), aBndMin.Y(), aBndMax.Z());
            aBndCorner[7].SetCoord(aBndMax.X(), aBndMax.Y(), aBndMin.Z());
            aBndCorner[8].SetCoord(aBndMax.X(), aBndMax.Y(), aBndMax.Z());

            // Perspective-correct camera projection vector, matching the bounding box is determined geometrically.
            // Knowing the initial shape of a frustum it is possible to match it to a bounding box.
            // Then, knowing the relation of camera projection vector to the frustum shape it is possible to
            // set up perspective-correct camera projection matching the bounding box.
            // These steps support non-asymmetric transformations of view-projection space provided by camera.
            // The zooming can be done by calculating view plane size matching the bounding box at center of
            // the bounding box. The only limitation here is that the scale of camera should define size of
            // its view plane passing through the camera center, and the center of camera should be on the
            // same line with the center of bounding box.

            // The following method is applied:
            // 1) Determine normalized asymmetry of camera projection vector by frustum planes.
            // 2) Determine new location of frustum planes, "matching" the bounding box.
            // 3) Determine new camera projection vector using the normalized asymmetry.
            // 4) Determine new zooming in view space.

            // 1. Determine normalized projection asymmetry (if any).
            double anAssymX = Math.Tan((aCamSide).Angle(aFrustumPlane[1].Axis().Direction()))
                                   - Math.Tan((-aCamSide).Angle(aFrustumPlane[2].Axis().Direction()));
            double anAssymY = Math.Tan((aCamUp).Angle(aFrustumPlane[3].Axis().Direction()))
                                   - Math.Tan((-aCamUp).Angle(aFrustumPlane[4].Axis().Direction()));

            // 2. Determine how far should be the frustum planes placed from center
            //    of bounding box, in order to match the bounding box closely.
            double[] aFitDistanceArray = new double[6];
            NCollection_Array1<double> aFitDistance = new NCollection_Array1<double>(aFitDistanceArray, 1, 6);
            aFitDistance.Init(0.0);
            for (int anI = aFrustumPlane.Lower(); anI <= aFrustumPlane.Upper(); ++anI)
            {
                // Measure distances from center of bounding box to its corners towards the frustum plane.
                gp_Dir aPlaneN = aFrustumPlane[anI].Axis().Direction();

                double aFitDist = aFitDistance[anI];
                for (int aJ = aBndCorner.Lower(); aJ <= aBndCorner.Upper(); ++aJ)
                {
                    aFitDist = Math.Max(aFitDist, new gp_Vec(aBndCenter, aBndCorner[aJ]).Dot(aPlaneN.to_gp_Vec()));
                    //write back aFitDist to aFitDistance[anI];??
                }
            }
            // The center of camera is placed on the same line with center of bounding box.
            // The view plane section crosses the bounding box at its center.
            // To compute view plane size, evaluate coefficients converting "point -> plane distance"
            // into view section size between the point and the frustum plane.
            //       proj
            //       /|\   right half of frame     //
            //        |                           //
            //  point o<--  distance * coeff  -->//---- (view plane section)
            //         \                        //
            //      (distance)                 //
            //                ~               //
            //                 (distance)    //
            //                           \/\//
            //                            \//
            //                            //
            //                      (frustum plane)
            aFitDistance[1] *= Math.Sqrt(1 + Math.Pow(Math.Tan(aCamSide.Angle(aFrustumPlane[1].Axis().Direction())), 2.0));
            aFitDistance[2] *= Math.Sqrt(1 + Math.Pow(Math.Tan((-aCamSide).Angle(aFrustumPlane[2].Axis().Direction())), 2.0));
            aFitDistance[3] *= Math.Sqrt(1 + Math.Pow(Math.Tan(aCamUp.Angle(aFrustumPlane[3].Axis().Direction())), 2.0));
            aFitDistance[4] *= Math.Sqrt(1 + Math.Pow(Math.Tan((-aCamUp).Angle(aFrustumPlane[4].Axis().Direction())), 2.0));
            aFitDistance[5] *= Math.Sqrt(1 + Math.Pow(Math.Tan(aCamDir.Angle(aFrustumPlane[5].Axis().Direction())), 2.0));
            aFitDistance[6] *= Math.Sqrt(1 + Math.Pow(Math.Tan((-aCamDir).Angle(aFrustumPlane[6].Axis().Direction())), 2.0));

            double aViewSizeXv = aFitDistance[1] + aFitDistance[2];
            double aViewSizeYv = aFitDistance[3] + aFitDistance[4];
            double aViewSizeZv = aFitDistance[5] + aFitDistance[6];

            // 3. Place center of camera on the same line with center of bounding
            //    box applying corresponding projection asymmetry (if any).
            double anAssymXv = anAssymX * aViewSizeXv * 0.5;
            double anAssymYv = anAssymY * aViewSizeYv * 0.5;
            double anOffsetXv = (aFitDistance[2] - aFitDistance[1]) * 0.5 + anAssymXv;
            double anOffsetYv = (aFitDistance[4] - aFitDistance[3]) * 0.5 + anAssymYv;
            gp_Vec aTranslateSide = new gp_Vec(aCamSide) * anOffsetXv;
            gp_Vec aTranslateUp = new gp_Vec(aCamUp) * anOffsetYv;
            gp_Pnt aCamNewCenter = aBndCenter.Translated(aTranslateSide).Translated(aTranslateUp);

            gp_Trsf aCenterTrsf = new gp_Trsf();
            aCenterTrsf.SetTranslation(Center(), aCamNewCenter);
            Transform(aCenterTrsf);
            SetDistance(aFitDistance[6] + aFitDistance[5]);

            if (aViewSizeXv < theResolution
             && aViewSizeYv < theResolution)
            {
                // Bounding box collapses to a point or thin line going in depth of the screen
                if (aViewSizeXv < theResolution || !theToEnlargeIfLine)
                {
                    return false; // This is just one point or line and zooming has no effect.
                }

                // Looking along line and "theToEnlargeIfLine" is requested.
                // Fit view to see whole scene on rotation.
                aViewSizeXv = aViewSizeZv;
                aViewSizeYv = aViewSizeZv;
            }

            double anAspect = Aspect();
            if (anAspect > 1.0)
            {
                SetScale(Math.Max(aViewSizeXv / anAspect, aViewSizeYv));
            }
            else
            {
                SetScale(Math.Max(aViewSizeXv, aViewSizeYv * anAspect));
            }
            return true;
        }


        public void Frustum(gp_Pln theLeft,
                                   gp_Pln theRight,
                                   gp_Pln theBottom,
                                   gp_Pln theTop,
                                   gp_Pln theNear,
                                   gp_Pln theFar)
        {
            gp_Vec aProjection = new gp_Vec(Direction());
            var anUp = new gp_Vec(OrthogonalizedUp());
            gp_Vec aSide = aProjection ^ anUp;

            new Standard_ASSERT_RAISE(
         !aProjection.IsParallel(anUp, Precision.Angular()),
          "Can not derive SIDE = PROJ x UP - directions are parallel");

            theNear = new gp_Pln(Eye().Translated(aProjection * ZNear()), aProjection.To_gp_Dir());
            theFar = new gp_Pln(Eye().Translated(aProjection * ZFar()), -aProjection.To_gp_Dir());

            double aHScaleHor = 0.0, aHScaleVer = 0.0;
            if (Aspect() >= 1.0)
            {
                aHScaleHor = Scale() * 0.5 * Aspect();
                aHScaleVer = Scale() * 0.5;
            }
            else
            {
                aHScaleHor = Scale() * 0.5;
                aHScaleVer = Scale() * 0.5 / Aspect();
            }

            gp_Pnt aPntLeft = Center().Translated(aHScaleHor * -aSide);
            gp_Pnt aPntRight = Center().Translated(aHScaleHor * aSide);
            gp_Pnt aPntBottom = Center().Translated(aHScaleVer * -anUp);
            gp_Pnt aPntTop = Center().Translated(aHScaleVer * anUp);

            gp_Vec aDirLeft = aSide;
            gp_Vec aDirRight = -aSide;
            gp_Vec aDirBottom = anUp;
            gp_Vec aDirTop = -anUp;
            if (!IsOrthographic())
            {
                double aHFOVHor = Math.Atan(Math.Tan(DTR_HALF * FOVy()) * Aspect());
                double aHFOVVer = DTR_HALF * FOVy();
                aDirLeft.Rotate(new gp_Ax1(gp.Origin(), anUp.To_gp_Dir()), aHFOVHor);
                aDirRight.Rotate(new gp_Ax1(gp.Origin(), anUp.To_gp_Dir()), -aHFOVHor);
                aDirBottom.Rotate(new gp_Ax1(gp.Origin(), aSide.To_gp_Dir()), -aHFOVVer);
                aDirTop.Rotate(new gp_Ax1(gp.Origin(), aSide.To_gp_Dir()), aHFOVVer);
            }

            theLeft = new gp_Pln(aPntLeft, aDirLeft.To_gp_Dir());
            theRight = new gp_Pln(aPntRight, aDirRight.To_gp_Dir());
            theBottom = new gp_Pln(aPntBottom, aDirBottom.To_gp_Dir());
            theTop = new gp_Pln(aPntTop, aDirTop.To_gp_Dir());
        }
        //! Get Field Of View (FOV) in y axis.
        //! @return the FOV value in degrees.
        public double FOVy() { return myFOVy; }

        //! Get stereographic focus definition type.
        //! @return definition type used for stereographic focus.
        public FocusType ZFocusType()
        {
            return myZFocusType;
        }
        public void stereoEyeProj(NCollection_Mat4 theOutMx,

                                                   Aspect_FrustumLRBT theLRBT,
                                                   double theNear,

                                                   double theFar,

                                                   double theIOD,

                                                   double theZFocus,

                                                   Aspect_Eye theEyeIndex)
        {
            var aDx = theEyeIndex == Aspect_Eye.Aspect_Eye_Left ? (0.5) * theIOD : (-0.5) * theIOD;
            var aDXStereoShift = aDx * theNear / theZFocus;

            // construct eye projection matrix
            Aspect_FrustumLRBT aLRBT = theLRBT;
            aLRBT.Left = theLRBT.Left + aDXStereoShift;
            aLRBT.Right = theLRBT.Right + aDXStereoShift;
            perspectiveProj(theOutMx, aLRBT, theNear, theFar);
        }


        public void orthoProj(NCollection_Mat4 theOutMx,

                                             Aspect_FrustumLRBT theLRBT,
                                             double theNear,

                                             double theFar)
        {
            // row 0
            theOutMx.ChangeValue(0, 0).Assign((2.0) / (theLRBT.Right - theLRBT.Left));
            theOutMx.ChangeValue(0, 1).Assign(0.0);
            theOutMx.ChangeValue(0, 2).Assign(0.0);
            theOutMx.ChangeValue(0, 3).Assign(-(theLRBT.Right + theLRBT.Left) / (theLRBT.Right - theLRBT.Left));

            // row 1
            theOutMx.ChangeValue(1, 0).Assign(0.0);
            theOutMx.ChangeValue(1, 1).Assign((2.0) / (theLRBT.Top - theLRBT.Bottom));
            theOutMx.ChangeValue(1, 2).Assign(0.0);
            theOutMx.ChangeValue(1, 3).Assign(-(theLRBT.Top + theLRBT.Bottom) / (theLRBT.Top - theLRBT.Bottom));

            // row 2
            theOutMx.ChangeValue(2, 0).Assign(0.0);
            theOutMx.ChangeValue(2, 1).Assign(0.0);
            if (myIsZeroToOneDepth)
            {
                theOutMx.ChangeValue(2, 2).Assign((-1.0) / (theFar - theNear));
                theOutMx.ChangeValue(2, 3).Assign(-theNear / (theFar - theNear));
            }
            else
            {
                theOutMx.ChangeValue(2, 2).Assign((-2.0) / (theFar - theNear));
                theOutMx.ChangeValue(2, 3).Assign(-(theFar + theNear) / (theFar - theNear));
            }

            // row 3
            theOutMx.ChangeValue(3, 0).Assign(0.0);
            theOutMx.ChangeValue(3, 1).Assign(0.0);
            theOutMx.ChangeValue(3, 2).Assign(0.0);
            theOutMx.ChangeValue(3, 3).Assign(1.0);


        }

        public void perspectiveProj(NCollection_Mat4 theOutMx,

                                         Aspect_FrustumLRBT theLRBT,
                                         double theNear,

                                         double theFar)
        {
            // column 0
            theOutMx.ChangeValue(0, 0, ((2.0) * theNear) / (theLRBT.Right - theLRBT.Left));
            theOutMx.ChangeValue(1, 0, 0.0);
            theOutMx.ChangeValue(2, 0, 0.0);
            theOutMx.ChangeValue(3, 0, 0.0);

            // column 1
            theOutMx.ChangeValue(0, 1, 0.0);
            theOutMx.ChangeValue(1, 1, ((2.0) * theNear) / (theLRBT.Top - theLRBT.Bottom));
            theOutMx.ChangeValue(2, 1, 0.0);
            theOutMx.ChangeValue(3, 1, 0.0);

            // column 2
            theOutMx.ChangeValue(0, 2).Assign((theLRBT.Right + theLRBT.Left) / (theLRBT.Right - theLRBT.Left));
            theOutMx.ChangeValue(1, 2).Assign((theLRBT.Top + theLRBT.Bottom) / (theLRBT.Top - theLRBT.Bottom));
            if (myIsZeroToOneDepth)
            {
                theOutMx.ChangeValue(2, 2).Assign(theFar / (theNear - theFar));
            }
            else
            {
                theOutMx.ChangeValue(2, 2).Assign(-(theFar + theNear) / (theFar - theNear));
            }
            theOutMx.ChangeValue(3, 2).Assign(-1.0);

            // column 3
            theOutMx.ChangeValue(0, 3).Assign(0.0);
            theOutMx.ChangeValue(1, 3).Assign(0.0);
            if (myIsZeroToOneDepth)
            {
                theOutMx.ChangeValue(2, 3).Assign(-(theFar * theNear) / (theFar - theNear));
            }
            else
            {
                theOutMx.ChangeValue(2, 3).Assign(-((2.0) * theFar * theNear) / (theFar - theNear));
            }
            theOutMx.ChangeValue(3, 3).Assign(0.0);
        }



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
            myZFocusType(FocusType_Relative),*/
            myIOD = (0.05);
            myIODType = IODType.IODType_Relative;
            myIsCustomProjMatM = (false);
            myIsCustomProjMatLR = (false);
            myIsCustomFrustomLR = (false);
        }

        public Graphic3d_Camera(Graphic3d_Camera graphic3d_Camera)
        {
        }

        Graphic3d_CameraTile myTile;
        bool myIsCustomProjMatM;  //!< flag indicating usage of custom projection matrix
        bool myIsCustomProjMatLR; //!< flag indicating usage of custom stereo projection matrices
        bool myIsCustomFrustomLR; //!< flag indicating usage of custom stereo frustums

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

        double myIOD;     //!< Intraocular distance value.
        IODType myIODType; //!< Intraocular distance definition type.

        private gp_XYZ myAxialScale; //!< World axial scale.

        //! Get camera Up direction vector.
        //! @return Camera's Up direction vector.
        //! Get distance of Eye from camera Center.
        //! @return the distance.
        public double Distance() { return myDistance; }

        //! Check that the camera projection is orthographic.
        //! @return boolean flag that indicates whether the camera's projection is
        //! orthographic or not.
        public bool IsOrthographic()
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

        internal gp_Pnt Project(gp_Pnt thePnt)
        {
            Graphic3d_Mat4d aViewMx = OrientationMatrix();
            var aProjMx = ProjectionMatrix();

            // use compatible type of point
            var aPnt = safePointCast(thePnt);

            aPnt = aViewMx * aPnt; // convert to view coordinate space
            aPnt = aProjMx * aPnt; // convert to projection coordinate space

            double aInvW = 1.0 / (aPnt.w());

            return new gp_Pnt(aPnt.x() * aInvW, aPnt.y() * aInvW, aPnt.z() * aInvW);

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

        internal void SetEye(gp_Pnt theEye)
        {
            if (Eye().IsEqual(theEye, 0.0))
            {
                return;
            }

            gp_Pnt aCenter = Center();
            myEye = theEye;
            myDistance = myEye.Distance(aCenter);
            if (myDistance > gp.Resolution())
            {
                myDirection = new gp_Dir(aCenter.XYZ() - myEye.XYZ());
            }
            InvalidateOrientation();
        }

        internal void SetCenter(gp_Pnt theCenter)
        {
            double aDistance = myEye.Distance(theCenter);
            if (myDistance == aDistance)
            {
                return;
            }

            myDistance = aDistance;
            if (myDistance > gp.Resolution())
            {
                myDirection = new gp_Dir(theCenter.XYZ() - myEye.XYZ());
            }
            InvalidateOrientation();
        }

        public void SetScale(double theScale)
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
        private double ZFar()
        {
            return myZFar;
        }

        private double ZNear()
        {

            return myZNear;

        }
        public Graphic3d_Mat4d OrientationMatrix()
        {
            return UpdateOrientation(myMatricesD).Orientation;
        }
        public NCollection_Mat4 ProjectionMatrix()
        {
            return UpdateProjection(myMatricesD).MProjection;
        }
        private TransformMatrices<double> UpdateOrientation(TransformMatrices<double> theMatrices)
        {

            if (theMatrices.IsOrientationValid())
            {
                return theMatrices; // for inline accessors
            }

            theMatrices.InitOrientation();

            NCollection_Vec3 anEye = new NCollection_Vec3((myEye.X()),
                                           (myEye.Y()),
                                           (myEye.Z()));

            NCollection_Vec3 aViewDir = new NCollection_Vec3((myDirection.X()),
                                              (myDirection.Y()),
                                              (myDirection.Z()));

            NCollection_Vec3 anUp = new NCollection_Vec3((myUp.X()),
                                          (myUp.Y()),
                                          (myUp.Z()));

            NCollection_Vec3 anAxialScale = new NCollection_Vec3((myAxialScale.X()),
                                                  (myAxialScale.Y()),
                                                  (myAxialScale.Z()));

            LookOrientation(anEye, aViewDir, anUp, anAxialScale, theMatrices.Orientation);

            return theMatrices; // for inline accessors

        }

        private void LookOrientation(NCollection_Vec3 theEye, NCollection_Vec3 theFwdDir,

            NCollection_Vec3 theUpDir, NCollection_Vec3 theAxialScale,

            Graphic3d_Mat4d theOutMx)
        {

            NCollection_Vec3 aForward = theFwdDir;
            aForward.Normalize();

            // side = forward x up
            NCollection_Vec3 aSide = NCollection_Vec3.Cross(aForward, theUpDir);
            aSide.Normalize();

            // recompute up as: up = side x forward
            NCollection_Vec3 anUp = NCollection_Vec3.Cross(aSide, aForward);

            NCollection_Mat4 aLookMx = new NCollection_Mat4();
            aLookMx.SetRow(0, aSide);
            aLookMx.SetRow(1, anUp);
            aLookMx.SetRow(2, -aForward);

            theOutMx.InitIdentity();
            theOutMx.Multiply(aLookMx);
            theOutMx.Translate(-theEye);

            NCollection_Mat4 anAxialScaleMx = new NCollection_Mat4();
            anAxialScaleMx.ChangeValue(0, 0, theAxialScale.x());
            anAxialScaleMx.ChangeValue(1, 1, theAxialScale.y());
            anAxialScaleMx.ChangeValue(2, 2, theAxialScale.z());

            theOutMx.Multiply(anAxialScaleMx);

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
        internal gp_Pnt ConvertWorld2View(gp_Pnt thePnt)
        {
            Graphic3d_Mat4d aViewMx = OrientationMatrix();

            // use compatible type of point
            NCollection_Vec4 aPnt = safePointCast(thePnt);

            aPnt = aViewMx * aPnt; // convert to view coordinate space

            double aInvW = 1.0 / (double)(aPnt.w());

            return new gp_Pnt(aPnt.x() * aInvW, aPnt.y() * aInvW, aPnt.z() * aInvW);

        }

        private NCollection_Vec4 safePointCast(gp_Pnt thePnt)
        {
            double aLim = 1e15f;

            // have to deal with values greater then max float
            gp_Pnt aSafePoint = thePnt;
            double aBigFloat = aLim * 0.1f;
            if (Math.Abs(aSafePoint.X()) > aLim)
                aSafePoint.SetX(aSafePoint.X() >= 0 ? aBigFloat : -aBigFloat);
            if (Math.Abs(aSafePoint.Y()) > aLim)
                aSafePoint.SetY(aSafePoint.Y() >= 0 ? aBigFloat : -aBigFloat);
            if (Math.Abs(aSafePoint.Z()) > aLim)
                aSafePoint.SetZ(aSafePoint.Z() >= 0 ? aBigFloat : -aBigFloat);

            // convert point
            NCollection_Vec4 aPnt = new NCollection_Vec4(aSafePoint.X(), aSafePoint.Y(), aSafePoint.Z(), 1.0);

            return aPnt;

        }

        internal void OrthogonalizeUp()
        {

            SetUp(OrthogonalizedUp());

        }
        gp_Dir OrthogonalizedUp()
        {
            gp_Dir aDir = Direction();
            gp_Dir aLeft = aDir.Crossed(Up());

            // recompute up as: up = left x direction
            return aLeft.Crossed(aDir);
        }



        public void computeProjection(NCollection_Mat4 theProjM,
                                        NCollection_Mat4 theProjL,
                                        NCollection_Mat4 theProjR,
                                        bool theToAddHeadToEye)
        {
            theProjM.InitIdentity();
            theProjL.InitIdentity();
            theProjR.InitIdentity();

            // sets top of frustum based on FOVy and near clipping plane
            double aScale = (myScale);
            double aZNear = (myZNear);
            double aZFar = (myZFar);
            double anAspect = (myAspect);
            double aDXHalf = 0.0, aDYHalf = 0.0;
            if (IsOrthographic())
            {
                aDXHalf = aDYHalf = aScale * (0.5);
            }
            else
            {
                aDXHalf = aDYHalf = aZNear * (myFOVyTan);
            }

            if (anAspect > 1.0)
            {
                aDXHalf *= anAspect;
            }
            else
            {
                aDYHalf /= anAspect;
            }

            myTile = new Graphic3d_CameraTile();
            // sets right of frustum based on aspect ratio
            Aspect_FrustumLRBT anLRBT = new Aspect_FrustumLRBT();
            anLRBT.Left = -aDXHalf;
            anLRBT.Right = aDXHalf;
            anLRBT.Bottom = -aDYHalf;
            anLRBT.Top = aDYHalf;

            double aIOD = myIODType == IODType.IODType_Relative
              ? (myIOD * Distance())
              : (myIOD);

            double aFocus = myZFocusType == FocusType.FocusType_Relative
              ? (myZFocus * Distance())
              : (myZFocus);

            if (myTile.IsValid())
            {
                double aDXFull = (2) * aDXHalf;
                double aDYFull = (2) * aDYHalf;
                var anOffset = myTile.OffsetLowerLeft();
                anLRBT.Left = -aDXHalf + aDXFull * (anOffset.x()) / (myTile.TotalSize.x());
                anLRBT.Right = -aDXHalf + aDXFull * (anOffset.x() + myTile.TileSize.x()) / (myTile.TotalSize.x());
                anLRBT.Bottom = -aDYHalf + aDYFull * (anOffset.y()) / (myTile.TotalSize.y());
                anLRBT.Top = -aDYHalf + aDYFull * (anOffset.y() + myTile.TileSize.y()) / (myTile.TotalSize.y());
            }

            if (myIsCustomProjMatM)
            {
                theProjM.ConvertFrom(myCustomProjMatM);
            }

            switch (myProjType)
            {
                case Projection.Projection_Orthographic:
                    {
                        if (!myIsCustomProjMatM)
                        {
                            orthoProj(theProjM, anLRBT, aZNear, aZFar);
                        }
                        break;
                    }
                case Projection.Projection_Perspective:
                    {
                        if (!myIsCustomProjMatM)
                        {
                            perspectiveProj(theProjM, anLRBT, aZNear, aZFar);
                        }
                        break;
                    }
                case Projection.Projection_MonoLeftEye:
                case Projection.Projection_MonoRightEye:
                case Projection.Projection_Stereo:
                    {
                        if (!myIsCustomProjMatM)
                        {
                            perspectiveProj(theProjM, anLRBT, aZNear, aZFar);
                        }
                        if (myIsCustomProjMatLR)
                        {
                            if (theToAddHeadToEye)
                            {
                                theProjL.ConvertFrom(myCustomProjMatL * myCustomHeadToEyeMatL);
                                theProjR.ConvertFrom(myCustomProjMatR * myCustomHeadToEyeMatR);
                            }
                            else
                            {
                                theProjL.ConvertFrom(myCustomProjMatL);
                                theProjR.ConvertFrom(myCustomProjMatR);
                            }
                        }
                        else if (myIsCustomFrustomLR)
                        {
                            anLRBT = new Aspect_FrustumLRBT(myCustomFrustumL).Multiplied(aZNear);
                            perspectiveProj(theProjL, anLRBT, aZNear, aZFar);

                            anLRBT = new Aspect_FrustumLRBT(myCustomFrustumR).Multiplied(aZNear);
                            perspectiveProj(theProjR, anLRBT, aZNear, aZFar);
                        }
                        else
                        {
                            stereoEyeProj(theProjL,
                                           anLRBT, aZNear, aZFar, aIOD, aFocus,
                                        Aspect_Eye.Aspect_Eye_Left);
                            stereoEyeProj(theProjR,
                                           anLRBT, aZNear, aZFar, aIOD, aFocus,
                                        Aspect_Eye.Aspect_Eye_Right);
                        }

                        if (theToAddHeadToEye
                        && !myIsCustomProjMatLR
                        && aIOD != (0.0))
                        {
                            // X translation to cancel parallax
                            theProjL.Translate(new NCollection_Vec3((0.5) * aIOD, (0.0), (0.0)));
                            theProjR.Translate(new NCollection_Vec3((-0.5) * aIOD, (0.0), (0.0)));
                        }
                        break;
                    }
            }
            if (myProjType == Projection.Projection_MonoLeftEye)
            {
                theProjM = theProjL;
            }
            else if (myProjType == Projection.Projection_MonoRightEye)
            {
                theProjM = theProjR;
            }
        }

        //! Compute projection matrices.
        //! @param theMatrices [in] the matrices data container.

        TransformMatrices<T> UpdateProjection<T>(TransformMatrices<T> theMatrices)
        {
            if (!theMatrices.IsProjectionValid())
            {
                theMatrices.InitProjection();
                computeProjection(theMatrices.MProjection, theMatrices.LProjection, theMatrices.RProjection, true);
            }
            return theMatrices;
        }

        public gp_Pnt ConvertProj2View(gp_Pnt thePnt)
        {
            var aProjMx = ProjectionMatrix();

            NCollection_Mat4 aInvProj;

            // this case should never happen, but...
            if (!aProjMx.Inverted(out aInvProj))
            {
                return new gp_Pnt(0, 0, 0);
            }

            // use compatible type of point
            var aPnt = safePointCast(thePnt);

            aPnt = aInvProj * aPnt; // convert to view coordinate space

            double aInvW = 1.0 / (aPnt.w());

            return new gp_Pnt(aPnt.x() * aInvW, aPnt.y() * aInvW, aPnt.z() * aInvW);
        }

        public void StereoProjectionF(Graphic3d_Mat4 aMatProjL, Graphic3d_Mat4 aMatHeadToEyeL, Graphic3d_Mat4 aMatProjR, Graphic3d_Mat4 aMatHeadToEyeR)
        {
            throw new NotImplementedException();
        }

        public bool IsCustomStereoFrustum()
        {
            throw new NotImplementedException();
        }

        public bool IsCustomStereoProjection()
        {
            throw new NotImplementedException();
        }

        public void SetProjectionType(Projection projection)
        {
            throw new NotImplementedException();
        }

        public Graphic3d_Mat4 ProjectionMatrixF()
        {
            throw new NotImplementedException();
        }

        public Graphic3d_Mat4 OrientationMatrixF()
        {
            throw new NotImplementedException();
        }

        public void MoveEyeTo(gp_Pnt theEye)
        {

            if (myEye.IsEqual(theEye, 0.0))
            {
                return;
            }

            myEye = theEye;
            InvalidateOrientation();

        }

        public void SetZRange(double theZNear,
                                     double theZFar)
        {
            if (theZFar > theZNear)
                throw new Exception("ZFar should be greater than ZNear");
            if (!IsOrthographic())
            {
                if (theZNear > 0.0)
                    throw new Exception("Only positive Z-Near is allowed for perspective camera");
                if (theZFar > 0.0)
                    throw new Exception("Only positive Z-Far is allowed for perspective camera");
            }

            if (ZNear() == theZNear
             && ZFar() == theZFar)
            {
                return;
            }

            myZNear = theZNear;
            myZFar = theZFar;

            InvalidateProjection();
        }

        public Projection ProjectionType()
        {
            return myProjType;
        }

        internal void SetAspect(double theAspect)
        {
            if (Aspect() == theAspect)
            {
                return;
            }

            myAspect = theAspect;
            myFOVx = myFOVy * theAspect;

            InvalidateProjection();

        }

        //! Change Z-min and Z-max planes of projection volume to match the displayed objects.
        public void ZFitAll(double theScaleFactor, Bnd_Box theMinMax, Bnd_Box theGraphicBB)
        {
            double aZNear = 0.0, aZFar = 1.0;
            ZFitAll(theScaleFactor, theMinMax, theGraphicBB, aZNear, aZFar);
            SetZRange(aZNear, aZFar);
        }
        // (degrees -> radians) * 0.5
        const double DTR_HALF = 0.5 * 0.0174532925;

        // default property values
        const double DEFAULT_ZNEAR = 0.001;
        const double DEFAULT_ZFAR = 3000.0;

        //! Estimate Z-min and Z-max planes of projection volume to match the
        //! displayed objects. The methods ensures that view volume will
        //! be close by depth range to the displayed objects. Fitting assumes that
        //! for orthogonal projection the view volume contains the displayed objects
        //! completely. For zoomed perspective view, the view volume is adjusted such
        //! that it contains the objects or their parts, located in front of the camera.
        //! @param[in] theScaleFactor the scale factor for Z-range.
        //!   The range between Z-min, Z-max projection volume planes
        //!   evaluated by z fitting method will be scaled using this coefficient.
        //!   Program error exception is thrown if negative or zero value is passed.
        //! @param[in] theMinMax applicative min max boundaries.
        //! @param[in] theGraphicBB real graphical boundaries (not accounting infinite flag).
        public bool ZFitAll(double theScaleFactor,
                                 Bnd_Box theMinMax,
                                 Bnd_Box theGraphicBB,
                                double theZNear,
                                double theZFar)
        {
            new Standard_ASSERT_RAISE(theScaleFactor > 0.0, "Zero or negative scale factor is not allowed.");

            // Method changes zNear and zFar parameters of camera so as to fit graphical structures
            // by their graphical boundaries. It precisely fits min max boundaries of primary application
            // objects (second argument), while it can sacrifice the real graphical boundaries of the
            // scene with infinite or helper objects (third argument) for the sake of perspective projection.
            if (theGraphicBB.IsVoid())
            {
                theZNear = DEFAULT_ZNEAR;
                theZFar = DEFAULT_ZFAR;
                return false;
            }

            // Measure depth of boundary points from camera eye.
            List<gp_Pnt> aPntsToMeasure = new List<gp_Pnt>();

            double[] aGraphicBB = new double[6];
            theGraphicBB.Get(out aGraphicBB[0], out aGraphicBB[1], out aGraphicBB[2], out aGraphicBB[3], out aGraphicBB[4], out aGraphicBB[5]);

            aPntsToMeasure.Add(new gp_Pnt(aGraphicBB[0], aGraphicBB[1], aGraphicBB[2]));
            aPntsToMeasure.Add(new gp_Pnt(aGraphicBB[0], aGraphicBB[1], aGraphicBB[5]));
            aPntsToMeasure.Add(new gp_Pnt(aGraphicBB[0], aGraphicBB[4], aGraphicBB[2]));
            aPntsToMeasure.Add(new gp_Pnt(aGraphicBB[0], aGraphicBB[4], aGraphicBB[5]));
            aPntsToMeasure.Add(new gp_Pnt(aGraphicBB[3], aGraphicBB[1], aGraphicBB[2]));
            aPntsToMeasure.Add(new gp_Pnt(aGraphicBB[3], aGraphicBB[1], aGraphicBB[5]));
            aPntsToMeasure.Add(new gp_Pnt(aGraphicBB[3], aGraphicBB[4], aGraphicBB[2]));
            aPntsToMeasure.Add(new gp_Pnt(aGraphicBB[3], aGraphicBB[4], aGraphicBB[5]));

            bool isFiniteMinMax = !theMinMax.IsVoid() && !theMinMax.IsWhole();

            if (isFiniteMinMax)
            {
                double[] aMinMax = new double[6];
                theMinMax.Get(out aMinMax[0], out aMinMax[1], out aMinMax[2], out aMinMax[3], out aMinMax[4], out aMinMax[5]);

                aPntsToMeasure.Add(new gp_Pnt(aMinMax[0], aMinMax[1], aMinMax[2]));
                aPntsToMeasure.Add(new gp_Pnt(aMinMax[0], aMinMax[1], aMinMax[5]));
                aPntsToMeasure.Add(new gp_Pnt(aMinMax[0], aMinMax[4], aMinMax[2]));
                aPntsToMeasure.Add(new gp_Pnt(aMinMax[0], aMinMax[4], aMinMax[5]));
                aPntsToMeasure.Add(new gp_Pnt(aMinMax[3], aMinMax[1], aMinMax[2]));
                aPntsToMeasure.Add(new gp_Pnt(aMinMax[3], aMinMax[1], aMinMax[5]));
                aPntsToMeasure.Add(new gp_Pnt(aMinMax[3], aMinMax[4], aMinMax[2]));
                aPntsToMeasure.Add(new gp_Pnt(aMinMax[3], aMinMax[4], aMinMax[5]));
            }

            // Camera eye plane.
            gp_Dir aCamDir = Direction();
            gp_Pnt aCamEye = myEye;
            gp_Pln aCamPln = new(aCamEye, aCamDir);

            double aModelMinDist = RealLast();
            double aModelMaxDist = RealFirst();
            double aGraphMinDist = RealLast();
            double aGraphMaxDist = RealFirst();

            gp_XYZ anAxialScale = myAxialScale;

            // Get minimum and maximum distances to the eye plane.
            int aCounter = 0;
            for (int i = 0; i < aPntsToMeasure.Count; i++)
            {
                gp_Pnt aPntIt = aPntsToMeasure[i];
                gp_Pnt aMeasurePnt = aPntIt;

                aPntsToMeasure[i] = new gp_Pnt(aMeasurePnt.X() * anAxialScale.X(),
                                          aMeasurePnt.Y() * anAxialScale.Y(),
                                          aMeasurePnt.Z() * anAxialScale.Z());

                double aDistance = aCamPln.Distance(aMeasurePnt);

                // Check if the camera is intruded into the scene.
                gp_Vec aVecToMeasurePnt = new(aCamEye, aMeasurePnt);
                if (aVecToMeasurePnt.Magnitude() > gp.Resolution()
                 && aCamDir.IsOpposite(aVecToMeasurePnt, Math.PI * 0.5))
                {
                    aDistance *= -1;
                }

                // The first eight points are from theGraphicBB, the last eight points are from theMinMax (can be absent).
                double aChangeMinDist = aCounter >= 8 ? aModelMinDist : aGraphMinDist;
                double aChangeMaxDist = aCounter >= 8 ? aModelMaxDist : aGraphMaxDist;
                aChangeMinDist = Math.Min(aDistance, aChangeMinDist);
                aChangeMaxDist = Math.Max(aDistance, aChangeMaxDist);
                aCounter++;
            }

            // Compute depth of bounding box center.
            double aMidDepth = (aGraphMinDist + aGraphMaxDist) * 0.5;
            double aHalfDepth = (aGraphMaxDist - aGraphMinDist) * 0.5;

            // Compute enlarged or shrank near and far z ranges.
            double aZNear = aMidDepth - aHalfDepth * theScaleFactor;
            double aZFar = aMidDepth + aHalfDepth * theScaleFactor;

            if (!IsOrthographic())
            {
                // Everything is behind the perspective camera.
                if (aZFar < zEpsilon())
                {
                    theZNear = DEFAULT_ZNEAR;
                    theZFar = DEFAULT_ZFAR;
                    return false;
                }
            }

            //
            // Consider clipping errors due to double to single precision floating-point conversion.
            //

            // Model to view transformation performs translation of points against eye position
            // in three dimensions. Both point coordinate and eye position values are converted from
            // double to single precision floating point numbers producing conversion errors. 
            // Epsilon (Mod) * 3.0 should safely compensate precision error for z coordinate after
            // translation assuming that the:
            // Epsilon (Eye.Mod()) * 3.0 > Epsilon (Eye.X()) + Epsilon (Eye.Y()) + Epsilon (Eye.Z()).
            double aEyeConf = 3.0 * zEpsilon(myEye.XYZ().Modulus());

            // Model to view transformation performs rotation of points according to view direction.
            // New z coordinate is computed as a multiplication of point's x, y, z coordinates by the
            // "forward" direction vector's x, y, z coordinates. Both point's and "z" direction vector's
            // values are converted from double to single precision floating point numbers producing
            // conversion errors.
            // Epsilon (Mod) * 6.0 should safely compensate the precision errors for the multiplication
            // of point coordinates by direction vector.
            gp_Pnt aGraphicMin = theGraphicBB.CornerMin();
            gp_Pnt aGraphicMax = theGraphicBB.CornerMax();

            double aModelConf = 6.0 * zEpsilon(aGraphicMin.XYZ().Modulus()) +
                                       6.0 * zEpsilon(aGraphicMax.XYZ().Modulus());

            // Compensate floating point conversion errors by increasing zNear, zFar to avoid clipping.
            aZNear -= zEpsilon(aZNear) + aEyeConf + aModelConf;
            aZFar += zEpsilon(aZFar) + aEyeConf + aModelConf;

            if (!IsOrthographic())
            {
                // For perspective projection, the value of z in normalized device coordinates is non-linear
                // function of eye z coordinate. For fixed-point depth representation resolution of z in
                // model-view space will grow towards zFar plane and its scale depends mostly on how far is zNear
                // against camera's eye. The purpose of the code below is to select most appropriate zNear distance
                // to balance between clipping (less zNear, more chances to observe closely small models without clipping)
                // and resolution of depth. A well applicable criteria to this is a ratio between resolution of z at center
                // of model boundaries and the distance to that center point. The ratio is chosen empirically and validated
                // by tests database. It is considered to be ~0.001 (0.1%) for 24 bit depth buffer, for less depth bitness
                // the zNear will be placed similarly giving lower resolution.
                // Approximation of the formula for respectively large z range is:
                // zNear = [z * (1 + k) / (k * c)],
                // where:
                // z - distance to center of model boundaries;
                // k - chosen ratio, c - capacity of depth buffer;
                // k = 0.001, k * c = 1677.216, (1 + k) / (k * c) ~ 5.97E-4
                //
                // The function uses center of model boundaries computed from "theMinMax" boundaries (instead of using real
                // graphical boundaries of all displayed objects). That means that it can sacrifice resolution of presentation
                // of non primary ("infinite") application graphical objects in favor of better perspective projection of the
                // small applicative objects measured with "theMinMax" values.
                double aZRange = isFiniteMinMax ? aModelMaxDist - aModelMinDist : aGraphMaxDist - aGraphMinDist;
                double aZMin = isFiniteMinMax ? aModelMinDist : aGraphMinDist;
                double aZ = aZMin < 0 ? aZRange / 2.0 : aZRange / 2.0 + aZMin;
                double aZNearMin = aZ * 5.97E-4;
                if (aZNear < aZNearMin)
                {
                    // Clip zNear according to the minimum value matching the quality.
                    aZNear = aZNearMin;
                    if (aZFar < aZNear)
                    {
                        aZFar = aZNear;
                    }
                }
                else
                {
                    // Compensate zNear conversion errors for perspective projection.
                    aZNear -= aZFar * zEpsilon(aZNear) / (aZFar - zEpsilon(aZNear));
                }

                // Compensate zFar conversion errors for perspective projection.
                aZFar += zEpsilon(aZFar);

                // Ensure that after all the zNear is not a negative value.
                if (aZNear < zEpsilon())
                {
                    aZNear = zEpsilon();
                }
                new Standard_ASSERT_RAISE(aZFar > aZNear, "ZFar should be greater than ZNear");
            }

            theZNear = aZNear;
            theZFar = aZFar;
            new Standard_ASSERT_RAISE(aZFar > aZNear, "ZFar should be greater than ZNear");
            return true;
        }

        private double RealFirst()
        {
            return -DBL_MAX;
        }

        const double DBL_MAX = 1.7976931348623158e+308; // max value
        private double RealLast()
        {
            return DBL_MAX;

        }

        // relative z-range tolerance compatible with for floating point.
        static double zEpsilon(double theValue)
        {
            double anAbsValue = Math.Abs(theValue);
            if (anAbsValue <= (double)FLT_MIN)
            {
                return FLT_MIN;
            }
            double aLogRadix = Math.Log10(anAbsValue) / Math.Log10(FLT_RADIX);
            double aExp = Math.Floor(aLogRadix);
            return FLT_EPSILON * Math.Pow(FLT_RADIX, aExp);
        }
        const double FLT_MIN = 1.175494351e-38F;// min normalized positive value

        const double FLT_EPSILON = 1.192092896e-07F; // smallest such that 1.0+FLT_EPSILON != 1.0
        const int FLT_RADIX = 2;                       // exponent radix

        // z-range tolerance compatible with for floating point.
        static double zEpsilon()
        {
            return FLT_EPSILON;
        }

    }
}
