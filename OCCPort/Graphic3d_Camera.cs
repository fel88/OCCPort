using System;
using System.Runtime.InteropServices.ComTypes;

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

		public void stereoEyeProj(NCollection_Mat4 theOutMx,

											   Aspect_FrustumLRBT theLRBT,
											   double theNear,

											   double theFar,

											   double theIOD,

											   double theZFocus,

											   Aspect_Eye theEyeIndex)
		{
			var aDx = theEyeIndex ==Aspect_Eye.  Aspect_Eye_Left ? (0.5) * theIOD : (-0.5) * theIOD;
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
			theOutMx.ChangeValue(0, 0) .Assign( (2.0) / (theLRBT.Right - theLRBT.Left));
			theOutMx.ChangeValue(0, 1) .Assign (0.0);
			theOutMx.ChangeValue(0, 2).Assign(0.0);
			theOutMx.ChangeValue(0, 3).Assign ( -(theLRBT.Right + theLRBT.Left) / (theLRBT.Right - theLRBT.Left));

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
		Graphic3d_Mat4d OrientationMatrix()
		{
			return UpdateOrientation(myMatricesD).Orientation;
		}
		NCollection_Mat4 ProjectionMatrix()
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

	}

	//! Camera eye index within stereoscopic pair.
	public enum Aspect_Eye
	{
		Aspect_Eye_Left,
		Aspect_Eye_Right
	};

}
