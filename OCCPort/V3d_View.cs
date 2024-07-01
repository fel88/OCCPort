using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OCCPort
{
	public class V3d_View
	{

		V3d_Viewer MyViewer;

		//=============================================================================
		//function : Redraw
		//purpose  :
		//=============================================================================
		public void Redraw()
		{
			/*if (!myView.IsDefined()
			 || !myView.IsActive())
			{
				return;
			}*/

			//myIsInvalidatedImmediate = false;
			Graphic3d_StructureManager aStructureMgr = MyViewer.StructureManager();
			//	for (Standard_Integer aRetryIter = 0; aRetryIter < 2; ++aRetryIter)
			{
				if (aStructureMgr.IsDeviceLost())
				{
					aStructureMgr.RecomputeStructures();
				}

				AutoZFit();

				myView.Redraw();

				if (!aStructureMgr.IsDeviceLost())
				{
					return;
				}
			}
		}

		private void AutoZFit()
		{
			//throw new NotImplementedException();
		}

		public void Rotation(int X,
						  int Y)
		{
			if (rx == 0.0 || ry == 0.0)
			{
				StartRotation(X, Y);
				return;
			}
			double dx = 0.0, dy = 0.0, dz = 0.0;
			if (myZRotation)
			{
				dz = Math.Atan2((double)(X) - rx / 2.0, ry / 2.0 - (double)(Y)) -
				  Math.Atan2(sx - rx / 2.0, ry / 2.0 - sy);
			}
			else
			{
				dx = ((double)(X) - sx) * Math.PI / rx;
				dy = (sy - (double)(Y)) * Math.PI / ry;
			}

			Rotate(dx, dy, dz,
					myRotateGravity.X(), myRotateGravity.Y(), myRotateGravity.Z(),
					false);
		}

		//public static Func<Graphic3d_CView> CreateView;
		public V3d_View()
		{
			//myView = theViewer->Driver()->CreateView(theViewer->StructureManager());
			//myView = CreateView();
			//myView.SetBackground(theViewer->GetBackgroundColor());
			//  myView->SetGradientBackground(theViewer->GetGradientBackground());

			// ChangeRenderingParams() = theViewer->DefaultRenderingParams();

			// camera init
			var aCamera = new Graphic3d_Camera();
			/*aCamera.SetFOVy(45.0);
            aCamera.SetIOD(Graphic3d_Camera::IODType_Relative, 0.05);
            aCamera.SetZFocus(Graphic3d_Camera::FocusType_Relative, 1.0);
            aCamera.SetProjectionType((theType == V3d_ORTHOGRAPHIC)
              ? Graphic3d_Camera::Projection_Orthographic
              : Graphic3d_Camera::Projection_Perspective);*/

			myDefaultCamera = new Graphic3d_Camera();

			myImmediateUpdate = false;
			/*  SetAutoZFitMode(true, 1.0);
              SetBackFacingModel(V3d_TOBM_AUTOMATIC);*/
			SetCamera(aCamera);/*
            SetAxis(0., 0., 0., 1., 1., 1.);
            SetVisualization(theViewer->DefaultVisualization());
            SetTwist(0.);
            SetAt(0.0, 0.0, 0.0);
            SetProj(theViewer->DefaultViewProj());
            SetSize(theViewer->DefaultViewSize());
            Standard_Real zsize = theViewer->DefaultViewSize();
            SetZSize(2.* zsize);
            SetDepth(theViewer->DefaultViewSize() / 2.0);
            SetViewMappingDefault();
            SetViewOrientationDefault();
            theViewer->AddView(this);*/
			Init();
			myImmediateUpdate = true;
		}
		public void SetWindow(Aspect_Window theWindow,
						  Aspect_RenderingContext theContext)
		{
			if (myView.IsRemoved())
			{
				return;
			}
			if (myParentView != null)
			{
				throw new Standard_ProgramError("V3d_View::SetWindow() called twice");
			}

			// method V3d_View::SetWindow() should assign the field MyWindow before calling Redraw()
			MyWindow = theWindow;
			myView.SetWindow(null, theWindow, theContext);
			MyViewer.SetViewOn(this);
			SetRatio();
			if (myImmediateUpdate)
			{
				Redraw();
			}
		}

		private void SetRatio()
		{
			if (MyWindow == null)
			{
				return;
			}

			int aWidth = 0;
			int aHeight = 0;
			MyWindow.Size(out aWidth, out aHeight);
			if (aWidth > 0 && aHeight > 0)
			{
				double aRatio = (double)(aWidth) /
									   (double)(aHeight);

				Camera().SetAspect(aRatio);
				myDefaultCamera.SetAspect(aRatio);
			}

		}

		public V3d_View(V3d_Viewer theViewer, V3d_TypeOfView theType)
		{
			//myIsInvalidatedImmediate = (true);
			MyViewer = theViewer;
			SwitchSetFront = (false);
			myZRotation = (false);
			//MyTrsf=new  (1, 4, 1, 4)

			myView = theViewer.Driver().CreateView(theViewer.StructureManager());
			//	myView.SetBackground(theViewer.GetBackgroundColor());
			//	myView.SetGradientBackground(theViewer.GetGradientBackground());

			//ChangeRenderingParams() = theViewer->DefaultRenderingParams();

			// camera init
			Graphic3d_Camera aCamera = new Graphic3d_Camera();
			//aCamera.SetFOVy(45.0);
			/*aCamera.SetIOD(Graphic3d_Camera::IODType_Relative, 0.05);
			aCamera.SetZFocus(Graphic3d_Camera::FocusType_Relative, 1.0);
			aCamera.SetProjectionType((theType == V3d_ORTHOGRAPHIC)
			  ? Graphic3d_Camera::Projection_Orthographic
			  : Graphic3d_Camera::Projection_Perspective);
			*/
			myDefaultCamera = new Graphic3d_Camera();

			myImmediateUpdate = false;/*
			SetAutoZFitMode(Standard_True, 1.0);
			SetBackFacingModel(V3d_TOBM_AUTOMATIC);*/
			SetCamera(aCamera);

			SetAxis(0.0, 0.0, 0.0, 1.0, 1.0, 1.0);
			//SetVisualization (theViewer->DefaultVisualization());
			SetTwist(0.0);
			SetAt(0.0, 0.0, 0.0);
			SetProj(theViewer.DefaultViewProj());
			/*
  SetSize (theViewer->DefaultViewSize());
  Standard_Real zsize = theViewer->DefaultViewSize();
  SetZSize (2.*zsize);
  SetDepth (theViewer->DefaultViewSize() / 2.0);
  SetViewMappingDefault();
  SetViewOrientationDefault();

			 */
			theViewer.AddView(this);
			Init();
			myImmediateUpdate = true;
		}



		public void SetAt(double X, double Y, double Z)
		{
			double aTwistBefore = Twist();

			bool wasUpdateEnabled = SetImmediateUpdate(false);

			Camera().SetCenter(new gp_Pnt(X, Y, Z));

			SetTwist(aTwistBefore);

			SetImmediateUpdate(wasUpdateEnabled);

			ImmediateUpdate();
		}

		double Twist()
		{
			gp_Vec Xaxis = new gp_Vec(), Yaxis = new gp_Vec(), Zaxis = new gp_Vec();
			gp_Dir aReferencePlane = new gp_Dir(Camera().Direction().Reversed());
			if (!screenAxis(aReferencePlane, gp.DZ(), ref Xaxis, ref Yaxis, ref Zaxis)
			 && !screenAxis(aReferencePlane, gp.DY(), ref Xaxis, ref Yaxis, ref Zaxis)
			 && !screenAxis(aReferencePlane, gp.DX(), ref Xaxis, ref Yaxis, ref Zaxis))
			{
				//
			}

			// Compute Cross Vector From Up & Origin
			gp_Dir aCameraUp = Camera().Up();
			gp_XYZ aP = Yaxis.XYZ().Crossed(aCameraUp.XYZ());

			// compute Angle
			double anAngle = Math.Asin(Math.Max(Math.Min(aP.Modulus(), 1.0), -1.0));
			if (Yaxis.Dot(aCameraUp.XYZ()) < 0.0)
			{
				anAngle = Math.PI - anAngle;
			}
			if (anAngle > 0.0
			 && anAngle < Math.PI)
			{
				gp_Dir aProjDir = Camera().Direction().Reversed();
				if (aP.Dot(aProjDir.XYZ()) < 0.0)
				{
					anAngle = DEUXPI - anAngle;
				}
			}
			return anAngle;
		}

		bool screenAxis(gp_Dir theVpn, gp_Dir theVup,
										   ref gp_Vec theXaxe, ref gp_Vec theYaxe, ref gp_Vec theZaxe)
		{
			theXaxe = new gp_Vec(theVup.XYZ().Crossed(theVpn.XYZ()));

			if (theXaxe.Magnitude() <= gp.Resolution())
			{
				return false;
			}
			theXaxe.Normalize();

			theYaxe = new gp_Vec(theVpn.XYZ().Crossed(theXaxe.XYZ()));
			if (theYaxe.Magnitude() <= gp.Resolution())
			{
				return false;
			}
			theYaxe.Normalize();

			theZaxe = new gp_Vec(theVpn.XYZ());
			theZaxe.Normalize();
			return true;
		}

		public void SetTwist(double angle)
		{
			double Angle = angle;

			if (Angle > 0.0) while (Angle > DEUXPI) Angle -= DEUXPI;
			else if (Angle < 0.0) while (Angle < -DEUXPI) Angle += DEUXPI;

			Graphic3d_Camera aCamera = Camera();

			gp_Dir aReferencePlane = new gp_Dir(aCamera.Direction().Reversed());
			if (!screenAxis(aReferencePlane, gp.DZ(), ref myXscreenAxis, ref myYscreenAxis, ref myZscreenAxis)
			 && !screenAxis(aReferencePlane, gp.DY(), ref myXscreenAxis, ref myYscreenAxis, ref myZscreenAxis)
			 && !screenAxis(aReferencePlane, gp.DX(), ref myXscreenAxis, ref myYscreenAxis, ref myZscreenAxis))
			{
				throw new V3d_BadValue("V3d_ViewSetTwist, alignment of Eye,At,Up,");
			}

			gp_Pnt aRCenter = aCamera.Center();
			gp_Dir aZAxis = new gp_Dir(aCamera.Direction().Reversed());

			gp_Trsf aTrsf = new gp_Trsf();
			aTrsf.SetRotation(new gp_Ax1(aRCenter, aZAxis), Angle);

			aCamera.SetUp(new gp_Dir(myYscreenAxis));
			aCamera.Transform(aTrsf);

			ImmediateUpdate();
		}

		void SetAxis(double theX, double theY, double theZ,

								double theVx, double theVy, double theVz)
		{
			myDefaultViewPoint.SetCoord(theX, theY, theZ);
			myDefaultViewAxis.SetCoord(theVx, theVy, theVz);
		}

		gp_Dir myDefaultViewAxis;
		gp_Pnt myDefaultViewPoint;


		public void Pan(int theDXp,
						 int theDYp,
						 double theZoomFactor = 1,
						 bool theToStart = true)
		{
			Panning(Convert(theDXp), Convert(theDYp), theZoomFactor, theToStart);
		}

		public void Panning(double theDXv,
					  double theDYv,
					  double theZoomFactor = 1,
					  bool theToStart = true)
		{
			//Standard_ASSERT_RAISE(theZoomFactor > 0.0, "Bad zoom factor");

			var aCamera = Camera();

			if (theToStart)
			{
				myCamStartOpDir = aCamera.Direction();
				myCamStartOpEye = aCamera.Eye();
				myCamStartOpCenter = aCamera.Center();
			}

			bool wasUpdateEnabled = SetImmediateUpdate(false);

			var aViewDims = aCamera.ViewDimensions();

			aCamera.SetEyeAndCenter(myCamStartOpEye, myCamStartOpCenter);
			aCamera.SetDirectionFromEye(myCamStartOpDir);
			Translate(aCamera, -theDXv, -theDYv);
			Scale(aCamera, aViewDims.X() / theZoomFactor, aViewDims.Y() / theZoomFactor);

			SetImmediateUpdate(wasUpdateEnabled);

			ImmediateUpdate();
		}

		private void Scale(Graphic3d_Camera theCamera, double theSizeXv, double theSizeYv)
		{
			var anAspect = theCamera.Aspect();
			if (anAspect > 1.0)
			{
				theCamera.SetScale(Math.Max(theSizeXv / anAspect, theSizeYv));
			}
			else
			{
				theCamera.SetScale(Math.Max(theSizeXv, theSizeYv * anAspect));
			}
			Invalidate();

		}



		private void Translate(Graphic3d_Camera theCamera, double theDXv, double theDYv)
		{
			gp_Pnt aCenter = theCamera.Center();
			gp_Dir aDir = theCamera.Direction();
			gp_Dir anUp = theCamera.Up();
			gp_Ax3 aCameraCS = new gp_Ax3(aCenter, aDir.Reversed(), aDir ^ anUp);

			gp_Vec aCameraPanXv = new gp_Vec(aCameraCS.XDirection()) * theDXv;
			gp_Vec aCameraPanYv = new gp_Vec(aCameraCS.YDirection()) * theDYv;
			gp_Vec aCameraPan = aCameraPanXv + aCameraPanYv;
			gp_Trsf aPanTrsf = new gp_Trsf();
			aPanTrsf.SetTranslation(aCameraPan);

			theCamera.Transform(aPanTrsf);
			Invalidate();
		}

		private void Invalidate()
		{
			if (!myView.IsDefined())
			{
				return;
			}

			myView.Invalidate();
		}

		public bool SetImmediateUpdate(bool theImmediateUpdate)
		{
			bool aPreviousMode = myImmediateUpdate;
			myImmediateUpdate = theImmediateUpdate;
			return aPreviousMode;
		}

		public void Zoom(int theXp1,
					  int theYp1,
					  int theXp2,
					  int theYp2)
		{
			int aDx = theXp2 - theXp1;
			int aDy = theYp2 - theYp1;
			if (aDx != 0 || aDy != 0)
			{
				double aCoeff = Math.Sqrt((double)(aDx * aDx + aDy * aDy)) / 100.0 + 1.0;
				aCoeff = (aDx > 0) ? aCoeff : 1.0 / aCoeff;
				SetZoom(aCoeff, true);
			}
		}

		private void SetZoom(double theCoef, bool theToStart)
		{
			//V3d_BadValue_Raise_if(theCoef <= 0., "V3d_View::SetZoom, bad coefficient");

			var aCamera = Camera();

			if (theToStart)
			{
				myCamStartOpEye = aCamera.Eye();
				myCamStartOpCenter = aCamera.Center();
			}

			var aViewWidth = aCamera.ViewDimensions().X();
			var aViewHeight = aCamera.ViewDimensions().Y();

			// ensure that zoom will not be too small or too big
			var aCoef = theCoef;
			if (aViewWidth < aCoef * Precision.Confusion())
			{
				aCoef = aViewWidth / Precision.Confusion();
			}
			else if (aViewWidth > aCoef * 1e12)
			{
				aCoef = aViewWidth / 1e12;
			}
			if (aViewHeight < aCoef * Precision.Confusion())
			{
				aCoef = aViewHeight / Precision.Confusion();
			}
			else if (aViewHeight > aCoef * 1e12)
			{
				aCoef = aViewHeight / 1e12;
			}

			aCamera.SetEye(myCamStartOpEye);
			aCamera.SetCenter(myCamStartOpCenter);
			aCamera.SetScale(aCamera.Scale() / aCoef);

			ImmediateUpdate();
		}

		private void SetCamera(Graphic3d_Camera aCamera)
		{
			_camera = aCamera;
		}

		public void Init()
		{
			myGravityReferencePoint = new Graphic3d_Vertex();
		}

		double myOldMouseX;
		double myOldMouseY;
		gp_Dir myCamStartOpUp;
		gp_Dir myCamStartOpDir;
		gp_Pnt myCamStartOpEye;
		gp_Pnt myCamStartOpCenter;
		Graphic3d_Camera myDefaultCamera;
		public Graphic3d_CView myView;
		bool myImmediateUpdate;
		//mutable Standard_Boolean myIsInvalidatedImmediate;

		//! Returns camera object of the view.
		//! @return: handle to camera object, or NULL if 3D view does not use
		//! the camera approach.
		Graphic3d_Camera _camera;
		public Graphic3d_Camera Camera()
		{
			return _camera;
		}

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
		public Aspect_Window MyWindow;
		int sx;
		int sy;
		double rx;
		double ry;
		gp_Pnt myRotateGravity;
		bool myComputedMode;
		bool SwitchSetFront;
		bool myZRotation;
		int MyZoomAtPointX;
		int MyZoomAtPointY;

		public void Convert(
						int Xp,
						int Yp,
						ref double Xv,
						ref double Yv)
		{
			int aDxw, aDyw;

			//V3d_UnMapped_Raise_if(!myView->IsDefined(), "view has no window");

			MyWindow.Size(out aDxw, out aDyw);

			gp_Pnt aPoint = new gp_Pnt(Xp * 2.0 / aDxw - 1.0, (aDyw - Yp) * 2.0 / aDyw - 1.0, 0.0);
			aPoint = Camera().ConvertProj2View(aPoint);

			Xv = aPoint.X();
			Yv = aPoint.Y();
		}


		//! Converts the PIXEL value
		//! to a value in the projection plane.
		public int Convert(double Vv)
		{
			int aDxw, aDyw;

			//V3d_UnMapped_Raise_if(!myView->IsDefined(), "view has no window");

			MyWindow.Size(out aDxw, out aDyw);


			var aViewDims = Camera().ViewDimensions();
			//aValue = aViewDims.X() * (float)Vp / (float)aDxw;
			var aValue = (aDxw * Vv / (aViewDims.X()));

			return (int)aValue;
		}
		public double Convert(int Vp)
		{
			int aDxw, aDyw;

			//V3d_UnMapped_Raise_if(!myView->IsDefined(), "view has no window");

			MyWindow.Size(out aDxw, out aDyw);

			var aViewDims = Camera().ViewDimensions();
			var aValue = aViewDims.X() * (float)Vp / (float)aDxw;

			return aValue;
		}
		public gp_Pnt Eye()
		{
			var r = Camera().Eye();
			return r;
		}
		public gp_Pnt At()
		{
			var r = Camera().Center();
			return r;
		}
		public gp_Dir Up()
		{
			var r = Camera().Up();
			return r;
		}
		public double ViewX()
		{
			var r = Camera().ViewDimensions().X();
			return r;
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

			var aCamera = Camera();

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
			for (int i = 0; i < 3; i++)
			{
				aRot[i] = new gp_Trsf();
			}
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
			Graphic3d_MapOfStructure[] aSetOfStructures;
			myView.DisplayedStructures(out aSetOfStructures);

			bool hasSelection = false;
			foreach (var aStructIter in aSetOfStructures)
			{
				if (aStructIter.Key().IsHighlighted()
				 && aStructIter.Key().IsVisible())
				{
					hasSelection = true;
					break;
				}
			}

			double Xmin, Ymin, Zmin, Xmax, Ymax, Zmax;
			int aNbPoints = 0;
			gp_XYZ aResult = new gp_XYZ(0.0, 0.0, 0.0);
			foreach (var aStructIter in aSetOfStructures)
			{
				var aStruct = aStructIter.Key();
				if (!aStruct.IsVisible()
				  || aStruct.IsInfinite()
				  || (hasSelection && !aStruct.IsHighlighted()))
				{
					continue;
				}

				Graphic3d_BndBox3d aBox = aStruct.CStructure().BoundingBox();
				if (!aBox.IsValid())
				{
					continue;
				}

				// skip transformation-persistent objects
				if (aStruct.TransformPersistence() != null)
				{
					continue;
				}

				// use camera projection to find gravity point
				Xmin = aBox.CornerMin().x();
				Ymin = aBox.CornerMin().y();
				Zmin = aBox.CornerMin().z();
				Xmax = aBox.CornerMax().x();
				Ymax = aBox.CornerMax().y();
				Zmax = aBox.CornerMax().z();
				gp_Pnt[] aPnts = new gp_Pnt[THE_NB_BOUND_POINTS]
				{
			 new gp_Pnt (Xmin, Ymin, Zmin),new  gp_Pnt (Xmin, Ymin, Zmax),
			new  gp_Pnt (Xmin, Ymax, Zmin), new gp_Pnt (Xmin, Ymax, Zmax),
			 new gp_Pnt (Xmax, Ymin, Zmin), new gp_Pnt (Xmax, Ymin, Zmax),
			 new gp_Pnt (Xmax, Ymax, Zmin), new gp_Pnt (Xmax, Ymax, Zmax)
		};

				for (int aPntIt = 0; aPntIt < THE_NB_BOUND_POINTS; ++aPntIt)
				{
					gp_Pnt aBndPnt = aPnts[aPntIt];
					gp_Pnt aProjected = Camera().Project(aBndPnt);
					if (Math.Abs(aProjected.X()) <= 1.0
					 && Math.Abs(aProjected.Y()) <= 1.0)
					{
						aResult += aBndPnt.XYZ();
						++aNbPoints;
					}
				}
			}

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
			//Standard_Real x,y;
			double x, y;

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

		private void Size(out double Width, out double Height)
		{
			var aViewDims = Camera().ViewDimensions();

			Width = aViewDims.X();
			Height = aViewDims.Y();
		}

		public void MoveTo(int x, int y)
		{
			throw new NotImplementedException();
		}

		public void SetProj(V3d_TypeOfOrientation theOrientation,

						 bool theIsYup = false)
		{
			Graphic3d_Vec3d anUp = theIsYup ? new Graphic3d_Vec3d(0.0, 1.0, 0.0) : new Graphic3d_Vec3d(0.0, 0.0, 1.0);
			if (theIsYup)
			{
				if (theOrientation == V3d_TypeOfOrientation.V3d_Ypos
				 || theOrientation == V3d_TypeOfOrientation.V3d_Yneg)
				{
					anUp.SetValues(0.0, 0.0, -1.0);
				}
			}
			else
			{
				if (theOrientation == V3d_TypeOfOrientation.V3d_Zpos)
				{
					anUp.SetValues(0.0, 1.0, 0.0);
				}
				else if (theOrientation == V3d_TypeOfOrientation.V3d_Zneg)
				{
					anUp.SetValues(0.0, -1.0, 0.0);
				}
			}

			gp_Dir aBck = V3d.GetProjAxis(theOrientation);

			// retain camera panning from origin when switching projection
			Graphic3d_Camera aCamera = Camera();
			gp_Pnt anOriginVCS = aCamera.ConvertWorld2View(gp.Origin());

			double aNewDist = aCamera.Eye().Distance(new gp_Pnt(0, 0, 0));
			aCamera.SetEyeAndCenter(new gp_Pnt(new gp_XYZ(0, 0, 0) + aBck.XYZ() * aNewDist),
				new gp_Pnt(new gp_XYZ(0, 0, 0)));
			aCamera.SetDirectionFromEye(-aBck);
			aCamera.SetUp(new gp_Dir(anUp.x(), anUp.y(), anUp.z()));
			aCamera.OrthogonalizeUp();

			Panning(anOriginVCS.X(), anOriginVCS.Y());

			ImmediateUpdate();
		}
		public void FrontView()
		{
			SetProj(V3d_TypeOfOrientation.V3d_Yneg);
		}

		public void TopView()
		{
			SetProj(V3d_TypeOfOrientation.V3d_Zpos);
		}

		public void StartZoomAtPoint(int theXp, int theYp)
		{
			MyZoomAtPointX = theXp;
			MyZoomAtPointY = theYp;
		}

		//=======================================================================
		//function : ZoomAtPoint
		//purpose  :
		//=======================================================================
		public void ZoomAtPoint(int theMouseStartX,
							 int theMouseStartY,
							 int theMouseEndX,
							 int theMouseEndY)
		{
			bool wasUpdateEnabled = SetImmediateUpdate(false);

			// zoom
			double aDxy = ((theMouseEndX + theMouseEndY) - (theMouseStartX + theMouseStartY));
			double aDZoom = Math.Abs(aDxy) / 100.0 + 1.0;
			aDZoom = (aDxy > 0.0) ? aDZoom : 1.0 / aDZoom;

			//V3d_BadValue_Raise_if(aDZoom <= 0.0, "V3d_View::ZoomAtPoint, bad coefficient");

			var aCamera = Camera();

			double aViewWidth = aCamera.ViewDimensions().X();
			double aViewHeight = aCamera.ViewDimensions().Y();

			// ensure that zoom will not be too small or too big.
			double aCoef = aDZoom;
			if (aViewWidth < aCoef * Precision.Confusion())
			{
				aCoef = aViewWidth / Precision.Confusion();
			}
			else if (aViewWidth > aCoef * 1e12)
			{
				aCoef = aViewWidth / 1e12;
			}
			if (aViewHeight < aCoef * Precision.Confusion())
			{
				aCoef = aViewHeight / Precision.Confusion();
			}
			else if (aViewHeight > aCoef * 1e12)
			{
				aCoef = aViewHeight / 1e12;
			}

			double aZoomAtPointXv = 0.0;
			double aZoomAtPointYv = 0.0;
			Convert(MyZoomAtPointX, MyZoomAtPointY, ref aZoomAtPointXv, ref aZoomAtPointYv);

			double aDxv = aZoomAtPointXv / aCoef;
			double aDyv = aZoomAtPointYv / aCoef;

			aCamera.SetScale(aCamera.Scale() / aCoef);
			Translate(aCamera, aZoomAtPointXv - aDxv, aZoomAtPointYv - aDyv);

			SetImmediateUpdate(wasUpdateEnabled);

			ImmediateUpdate();
		}

		V3d_View myParentView;
		//! Returns the Aspect Window associated with the view.
		public Aspect_Window Window() { return MyWindow; }

		//! Return TRUE if this is a subview of another view.
		public bool IsSubview() { return myParentView != null; }

		internal void SetLightOn(Graphic3d_CLight anActiveLightIter)
		{
			throw new NotImplementedException();
		}

		internal void SetGridActivity(bool v)
		{
			throw new NotImplementedException();
		}

		internal void SetGrid(gp_Ax3 myPrivilegedPlane, Aspect_Grid aGrid)
		{
			throw new NotImplementedException();
		}



		//! Returns the associated Graphic3d view.
		public Graphic3d_CView View() { return myView; }

	}
}
