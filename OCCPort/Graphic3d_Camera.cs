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
			Graphic3d_Vec4d aPnt = safePointCast(thePnt);

			//aPnt = aViewMx * aPnt; // convert to view coordinate space

			double aInvW = 1.0 / (double)(aPnt.w());

			return new gp_Pnt(aPnt.x() * aInvW, aPnt.y() * aInvW, aPnt.z() * aInvW);

		}

		private Graphic3d_Vec4d safePointCast(gp_Pnt thePnt)
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
			Graphic3d_Vec4d aPnt = new Graphic3d_Vec4d(aSafePoint.X(), aSafePoint.Y(), aSafePoint.Z(), 1.0);

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
	}
}
