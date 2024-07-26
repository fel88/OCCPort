using OCCPort;
using OCCPort.Tester;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Policy;

namespace OCCPort
{
	public class BRep_Tool
	{
		//=======================================================================
		//function : CurveOnSurface
		//purpose  : Returns the curve  associated to the  edge in  the
		//           parametric  space of  the  face.  Returns   a NULL
		//           handle  if this curve  does not exist.  Returns in
		//           <First> and <Last> the parameter range.
		//=======================================================================

		public static Geom2d_Curve CurveOnSurface(TopoDS_Edge E,
											   TopoDS_Face F,
											   double First,
											   double Last,
											   ref bool theIsStored)
		{
			TopLoc_Location l = new TopLoc_Location();
			Geom_Surface S = BRep_Tool.Surface(F, ref l);
			TopoDS_Edge aLocalEdge = E;
			if (F.Orientation() == TopAbs_Orientation.TopAbs_REVERSED)
			{
				aLocalEdge.Reverse();
			}
			return CurveOnSurface(aLocalEdge, S, l, First, Last, ref theIsStored);
		}

		public static Geom2d_Curve CurveOnSurface(TopoDS_Edge E,
											   Geom_Surface S,
											   TopLoc_Location L,
											   double First,
											   double Last,
											   ref bool theIsStored)
		{
			TopLoc_Location loc = L.Predivided(E.Location());
			bool Eisreversed = (E.Orientation() == TopAbs_Orientation.TopAbs_REVERSED);
			if (theIsStored)
				theIsStored = true;

			// find the representation
			BRep_TEdge TE = E.TShape() as BRep_TEdge;
			BRep_ListIteratorOfListOfCurveRepresentation itcr = new BRep_ListIteratorOfListOfCurveRepresentation(TE.Curves());

			while (itcr.More())
			{
				BRep_CurveRepresentation cr = itcr.Value();
				if (cr.IsCurveOnSurface(S, loc))
				{
					BRep_GCurve GC = cr as BRep_GCurve;
					GC.Range(First, Last);
					if (GC.IsCurveOnClosedSurface() && Eisreversed)
						return GC.PCurve2();
					else
						return GC.PCurve();
				}
				itcr.Next();
			}

			// Curve is not found. Try projection on plane
			if (theIsStored)
				theIsStored = false;
			return CurveOnPlane(E, S, L, First, Last);
		}

		static Geom2d_Curve nullPCurve = null;

		//=======================================================================
		//function : CurveOnPlane
		//purpose  : For planar surface returns projection of the edge on the plane
		//=======================================================================
		public static Geom2d_Curve CurveOnPlane(TopoDS_Edge E,
											 Geom_Surface S,
											 TopLoc_Location L,
											 double First,
											 double Last)
		{
			First = Last = 0.0;

			// Check if the surface is planar
			Geom_Plane GP;
			Geom_RectangularTrimmedSurface GRTS;
			GRTS = S as Geom_RectangularTrimmedSurface;
			//if (GRTS != null)
			//	GP = GRTS.BasisSurface() as Geom_Plane;
			//else
			//	GP = S as Geom_Plane;

			//if (GP == null)
			//	// not a plane
			//	return nullPCurve;

			//// Check existence of 3d curve in edge
			//double f, l;
			//TopLoc_Location aCurveLocation;
			//Geom_Curve C3D = BRep_Tool.Curve(E, aCurveLocation, f, l);

			//if (C3D == null)
			//	// no 3d curve
			//	return nullPCurve;

			//aCurveLocation = aCurveLocation.Predivided(L);
			//First = f; Last = l;

			//// Transform curve and update parameters in account of scale factor
			//if (!aCurveLocation.IsIdentity())
			//{
			//	gp_Trsf aTrsf = aCurveLocation.Transformation();
			//	C3D = C3D.Transformed(aTrsf) as Geom_Curve;
			//	f = C3D.TransformedParameter(f, aTrsf);
			//	l = C3D.TransformedParameter(l, aTrsf);
			//}

			//// Perform projection
			//Geom_Curve ProjOnPlane =
			//GeomProjLib.ProjectOnPlane(new Geom_TrimmedCurve(C3D, f, l, true, false),
			//							GP,
			//							GP.Position().Direction(),
			//							true);

			//GeomAdaptor_Surface HS = new GeomAdaptor_Surface(GP);
			//GeomAdaptor_Curve HC = new GeomAdaptor_Curve(ProjOnPlane);

			//ProjLib_ProjectedCurve Proj = new ProjLib_ProjectedCurve(HS, HC);
			//Geom2d_Curve pc = Geom2dAdaptor.MakeCurve(Proj);

			//if (pc.DynamicType() == STANDARD_TYPE(Geom2d_TrimmedCurve))
			//{
			//	Geom2d_TrimmedCurve TC = pc as Geom2d_TrimmedCurve;
			//	pc = TC.BasisCurve();
			//}

			//return pc;
			throw new NotImplementedException();
		}

		//=======================================================================
		//function : IsGeometric
		//purpose  : Returns True if <F> has a surface.
		//=======================================================================
		public bool IsGeometric(TopoDS_Face F)
		{
			BRep_TFace TF = F.TShape() as BRep_TFace;
			Geom_Surface S = TF.Surface();
			return S != null;
		}

		public static Poly_Polygon3D nullPolygon3D = new Poly_Polygon3D();
		//=======================================================================
		//function : IsGeometric
		//purpose  : Returns True if <E> is a 3d curve or a curve on
		//           surface.
		//=======================================================================

		public static bool IsGeometric(TopoDS_Edge E)
		{
			// find the representation
			BRep_TEdge TE = (BRep_TEdge)(E.TShape());
			BRep_ListIteratorOfListOfCurveRepresentation itcr = new BRep_ListIteratorOfListOfCurveRepresentation(TE.Curves());

			while (itcr.More())
			{
				BRep_CurveRepresentation cr = itcr.Value();
				if (cr.IsCurve3D())
				{
					BRep_Curve3D GC = cr as BRep_Curve3D;
					if (GC != null && GC.Curve3D() != null)
						return true;
				}
				else if (cr.IsCurveOnSurface()) return true;
				itcr.Next();
			}
			return false;
		}


		public static gp_Pnt Pnt(TopoDS_Vertex V)
		{
			BRep_TVertex TV = (V.TShape()) as BRep_TVertex;

			if (TV == null)
			{
				throw new Standard_NullObject("BRep_Tool:: TopoDS_Vertex hasn't gp_Pnt");
			}

			gp_Pnt P = TV.Pnt();
			if (V.Location().IsIdentity())
			{
				return P;
			}

			return P.Transformed(V.Location().Transformation());
		}

		public static Poly_Polygon3D Polygon3D(TopoDS_Edge E,
														  TopLoc_Location L)
		{
			// find the representation
			BRep_TEdge TE = (BRep_TEdge)E.TShape();
			//BRep_ListIteratorOfListOfCurveRepresentation itcr(TE->Curves());

			//while (itcr.More())
			//{
			//    const Handle(BRep_CurveRepresentation)&cr = itcr.Value();
			//    if (cr->IsPolygon3D())
			//    {
			//        const BRep_Polygon3D* GC = static_cast <const BRep_Polygon3D*> (cr.get());
			//        L = E.Location() * GC->Location();
			//        return GC->Polygon3D();
			//    }
			//    itcr.Next();
			//}
			//L.Identity();
			return nullPolygon3D;
		}
		//=======================================================================
		//function : Tolerance
		//purpose  : Returns the tolerance for <E>.
		//=======================================================================

		public static double Tolerance(TopoDS_Edge E)
		{
			BRep_TEdge TE = (BRep_TEdge)(E.TShape());
			double p = TE.Tolerance();
			double pMin = Precision.Confusion();
			if (p > pMin)
				return p;


			return pMin;
		}

		public static double Tolerance(TopoDS_Vertex V)
		{
			BRep_TVertex aTVert = (V.TShape()) as BRep_TVertex;

			if (aTVert == null)
			{
				throw new Standard_NullObject("BRep_Tool:: TopoDS_Vertex hasn't gp_Pnt");
			}

			double p = aTVert.Tolerance();
			double pMin = Precision.Confusion();
			if (p > pMin) return p;
			else return pMin;
		}

		//=======================================================================
		//function : Tolerance
		//purpose  : Returns the tolerance of the face.
		//=======================================================================

		public static double Tolerance(TopoDS_Face F)
		{
			BRep_TFace TF = (BRep_TFace)(F.TShape());
			double p = TF.Tolerance();
			double pMin = Precision.Confusion();
			if (p > pMin)
				return p;

			return pMin;
		}

		public static bool IsClosed(TopoDS_Shape theShape)
		{
			if (theShape.ShapeType() == TopAbs_ShapeEnum.TopAbs_SHELL)
			{
				//Dictionary<TopoDS_Shape, TopTools_ShapeMapHasher> aMap(101, new NCollection_IncAllocator);
				NCollection_Map aMap = new NCollection_Map();
				TopExp_Explorer exp = new TopExp_Explorer(theShape.Oriented(TopAbs_Orientation.TopAbs_FORWARD), TopAbs_ShapeEnum.TopAbs_EDGE);
				bool hasBound = false;
				for (; exp.More(); exp.Next())
				{
					TopoDS_Edge E = TopoDS.Edge(exp.Current());
					if (BRep_Tool.Degenerated(E) || E.Orientation() == TopAbs_Orientation.TopAbs_INTERNAL || E.Orientation() == TopAbs_Orientation.TopAbs_EXTERNAL)
						continue;
					hasBound = true;
					if (!aMap.Add(E))
						aMap.Remove(E);
				}
				return hasBound && aMap.IsEmpty();
			}
			else if (theShape.ShapeType() == TopAbs_ShapeEnum.TopAbs_WIRE)
			{
				//NCollection_Map<TopoDS_Shape, TopTools_ShapeMapHasher> aMap(101, new NCollection_IncAllocator);
				NCollection_Map aMap = new NCollection_Map();
				TopExp_Explorer exp = new TopExp_Explorer(theShape.Oriented(TopAbs_Orientation.TopAbs_FORWARD), TopAbs_ShapeEnum.TopAbs_VERTEX);
				bool hasBound = false;
				for (; exp.More(); exp.Next())
				{
					TopoDS_Shape V = exp.Current();
					if (V.Orientation() == TopAbs_Orientation.TopAbs_INTERNAL || V.Orientation() == TopAbs_Orientation.TopAbs_EXTERNAL)
						continue;
					hasBound = true;
					if (!aMap.Add(V))
						aMap.Remove(V);
				}
				return hasBound && aMap.IsEmpty();
			}
			else if (theShape.ShapeType() == TopAbs_ShapeEnum.TopAbs_EDGE)
			{
				TopoDS_Vertex aVFirst, aVLast;
				TopExp.Vertices(TopoDS.Edge(theShape), out aVFirst, out aVLast);
				return !aVFirst.IsNull() && aVFirst.IsSame(aVLast);
			}
			return theShape.Closed();
		}

		private static bool Degenerated(TopoDS_Edge e)
		{
			BRep_TEdge TE = e.TShape() as BRep_TEdge;

			//const BRep_TEdge* TE = static_cast <const BRep_TEdge*> (E.TShape().get());
			return TE.Degenerated();
		}

		internal static double Surface(double f, TopLoc_Location l)
		{
			throw new NotImplementedException();
		}

		internal static Geom_Surface Surface(TopoDS_Face F,
		   ref TopLoc_Location L)
		{
			BRep_TFace TF = (BRep_TFace)(F.TShape());
			L = F.Location() * TF.Location();
			return TF.Surface();

		}

		public static Poly_Triangulation Triangulation(TopoDS_Face theFace,
			ref TopLoc_Location theLocation,
			Poly_MeshPurpose theMeshPurpose = Poly_MeshPurpose.Poly_MeshPurpose_NONE)
		{
			theLocation = theFace.Location();
			var aTFace = theFace.TShape() as BRep_TFace;
			//const BRep_TFace* aTFace = static_cast <const BRep_TFace*> (theFace.TShape().get());
			return aTFace.Triangulation(theMeshPurpose);
		}

		internal static Geom_Surface Surface(TopoDS_Face aFace, TopLoc_Location aDummyLoc)
		{
			throw new NotImplementedException();
		}

		internal static Poly_PolygonOnTriangulation PolygonOnTriangulation(TopoDS_Edge anEdge, Poly_Triangulation aTriangulation, TopLoc_Location aTrsf)
		{
			throw new NotImplementedException();
		}

		internal static GeomAbs_Shape MaxContinuity(TopoDS_Edge theEdge)
		{
			GeomAbs_Shape aMaxCont = GeomAbs_Shape.GeomAbs_C0;
			var curves = ((BRep_TEdge)theEdge.TShape()).ChangeCurves();
			//for (BRep_ListIteratorOfListOfCurveRepresentation aReprIter ((*((Handle(BRep_TEdge) *) & theEdge.TShape()))->ChangeCurves());
			//  aReprIter.More(); aReprIter.Next())
			foreach (var aReprIter in curves.list)
			{
				BRep_CurveRepresentation aRepr = aReprIter;
				if (aRepr.IsRegularity())
				{
					GeomAbs_Shape aCont = aRepr.Continuity();
					if ((int)aCont > (int)aMaxCont)
					{
						aMaxCont = aCont;
					}
				}
			}
			return aMaxCont;
		}
	}

	//! Describes a portion of a curve (termed the "basis
	//! curve") limited by two parameter values inside the
	//! parametric domain of the basis curve.
	//! The trimmed curve is defined by:
	//! - the basis curve, and
	//! - the two parameter values which limit it.
	//! The trimmed curve can either have the same
	//! orientation as the basis curve or the opposite orientation.
	public class Geom_TrimmedCurve : Geom_BoundedCurve

	{//=======================================================================
	 //function : BasisCurve
	 //purpose  : 
	 //=======================================================================

		public Geom_Curve BasisCurve()
		{
			return basisCurve;
		}

		double uTrim1;
		double uTrim2;
		Geom_Curve basisCurve;

		public override double LastParameter()
		{
			return uTrim2;
		}

		public override double FirstParameter()
		{
			return uTrim1;
		}

	}


	//! Compute the 2d-curve.  Try to solve the particular
	//! case if possible.  Otherwise, an approximation  is
	//! done. For approximation some parameters are used, including 
	//! required tolerance of approximation.
	//! Tolerance is maximal possible value of 3d deviation of 3d projection of projected curve from
	//! "exact" 3d projection. Since algorithm searches 2d curve on surface, required 2d tolerance is computed
	//! from 3d tolerance with help of U,V resolutions of surface.
	//! 3d and 2d tolerances have sense only for curves on surface, it defines precision of projecting and approximation
	//! and have nothing to do with distance between the projected curve and the surface.
	public class ProjLib_ProjectedCurve : Adaptor2d_Curve2d
	{

		double myTolerance;
		Adaptor3d_Surface mySurface;
		Adaptor3d_Curve myCurve;
		//ProjLib_Projector myResult;
		int myDegMin;
		int myDegMax;
		int myMaxSegments;
		double myMaxDist;
		AppParCurves_Constraint myBndPnt;

		public ProjLib_ProjectedCurve
(Adaptor3d_Surface S,
 Adaptor3d_Curve C)
		{
			myTolerance = (Precision.Confusion());
			myDegMin = (-1);
			myDegMax = (-1);
			myMaxSegments = (-1);
			myMaxDist = (-1.0);
			myBndPnt = AppParCurves_Constraint.AppParCurves_TangencyPoint;
			Load(S);
			Perform(C);
		}
		public void Load(Adaptor3d_Surface S)
		{
			mySurface = S;
		}

		public void Perform(Adaptor3d_Curve C)
		{
			myTolerance = Math.Max(myTolerance, Precision.Confusion());
			myCurve = C;
			double FirstPar = C.FirstParameter();
			double LastPar = C.LastParameter();
			GeomAbs_SurfaceType SType = mySurface.GetType();
			GeomAbs_CurveType CType = myCurve.GetType();
			bool isAnalyticalSurf = true;
			bool[] IsTrimmed = { false, false };
			int[] SingularCase = new int[2];
			double eps = 0.01;
			double TolConf = Precision.Confusion();
			double dt = (LastPar - FirstPar) * eps;
			double U1 = 0.0, U2 = 0.0, V1 = 0.0, V2 = 0.0;
			U1 = mySurface.FirstUParameter();
			U2 = mySurface.LastUParameter();
			V1 = mySurface.FirstVParameter();
			V2 = mySurface.LastVParameter();

			switch (SType)
			{
				case GeomAbs_SurfaceType.GeomAbs_Plane:
					{
						//ProjLib_Plane P = new ProjLib_Plane(mySurface.Plane());
						//Project(P, myCurve);
						//myResult = P;
					}
					break;

					//case GeomAbs_Cylinder:
					//	{
					//		ProjLib_Cylinder P(mySurface->Cylinder());
					//		Project(P, myCurve);
					//		myResult = P;
					//	}
					//	break;


					//case GeomAbs_Cone:
					//	{
					//		ProjLib_Cone P(mySurface->Cone());
					//		Project(P, myCurve);
					//		myResult = P;
					//	}
					//	break;

					//case GeomAbs_Sphere:
					//	{
					//		ProjLib_Sphere P(mySurface->Sphere());
					//		Project(P, myCurve);
					//		if (P.IsDone())
					//		{
					//			// on met dans la pseudo-periode ( car Sphere n'est pas
					//			// periodique en V !)
					//			P.SetInBounds(myCurve->FirstParameter());
					//		}
					//		else
					//		{
					//			const Standard_Real Vmax = M_PI / 2.;
					//			const Standard_Real Vmin = -Vmax;
					//			const Standard_Real minang = 1.e - 5 * M_PI;
					//			gp_Sphere aSph = mySurface->Sphere();
					//			Standard_Real anR = aSph.Radius();
					//			Standard_Real f = myCurve->FirstParameter();
					//			Standard_Real l = myCurve->LastParameter();

					//			gp_Pnt Pf = myCurve->Value(f);
					//			gp_Pnt Pl = myCurve->Value(l);
					//			gp_Pnt aLoc = aSph.Position().Location();
					//			Standard_Real maxdist = Max(Pf.Distance(aLoc), Pl.Distance(aLoc));
					//			TolConf = Max(anR * minang, Abs(anR - maxdist));

					//			//Surface has pole at V = Vmin and Vmax
					//			gp_Pnt Pole = mySurface->Value(U1, Vmin);
					//			TrimC3d(myCurve, IsTrimmed, dt, Pole, SingularCase, 3, TolConf);
					//			Pole = mySurface->Value(U1, Vmax);
					//			TrimC3d(myCurve, IsTrimmed, dt, Pole, SingularCase, 4, TolConf);
					//		}
					//		myResult = P;
					//	}
					//	break;

					//case GeomAbs_Torus:
					//	{
					//		ProjLib_Torus P(mySurface->Torus());
					//		Project(P, myCurve);
					//		myResult = P;
					//	}
					//	break;

					//case GeomAbs_BezierSurface:
					//case GeomAbs_BSplineSurface:
					//	{
					//		isAnalyticalSurf = Standard_False;
					//		Standard_Real f, l;
					//		f = myCurve->FirstParameter();
					//		l = myCurve->LastParameter();
					//		dt = (l - f) * eps;

					//		const Adaptor3d_Surface&S = *mySurface;
					//		U1 = S.FirstUParameter();
					//		U2 = S.LastUParameter();
					//		V1 = S.FirstVParameter();
					//		V2 = S.LastVParameter();

					//		if (IsoIsDeg(S, U1, GeomAbs_IsoU, 0., myTolerance))
					//		{
					//			//Surface has pole at U = Umin
					//			gp_Pnt Pole = mySurface->Value(U1, V1);
					//			TrimC3d(myCurve, IsTrimmed, dt, Pole, SingularCase, 1, TolConf);
					//		}

					//		if (IsoIsDeg(S, U2, GeomAbs_IsoU, 0., myTolerance))
					//		{
					//			//Surface has pole at U = Umax
					//			gp_Pnt Pole = mySurface->Value(U2, V1);
					//			TrimC3d(myCurve, IsTrimmed, dt, Pole, SingularCase, 2, TolConf);
					//		}

					//		if (IsoIsDeg(S, V1, GeomAbs_IsoV, 0., myTolerance))
					//		{
					//			//Surface has pole at V = Vmin
					//			gp_Pnt Pole = mySurface->Value(U1, V1);
					//			TrimC3d(myCurve, IsTrimmed, dt, Pole, SingularCase, 3, TolConf);
					//		}

					//		if (IsoIsDeg(S, V2, GeomAbs_IsoV, 0., myTolerance))
					//		{
					//			//Surface has pole at V = Vmax
					//			gp_Pnt Pole = mySurface->Value(U1, V2);
					//			TrimC3d(myCurve, IsTrimmed, dt, Pole, SingularCase, 4, TolConf);
					//		}

					//		ProjLib_ComputeApproxOnPolarSurface polar;
					//		polar.SetTolerance(myTolerance);
					//		polar.SetDegree(myDegMin, myDegMax);
					//		polar.SetMaxSegments(myMaxSegments);
					//		polar.SetBndPnt(myBndPnt);
					//		polar.SetMaxDist(myMaxDist);
					//		polar.Perform(myCurve, mySurface);

					//		Handle(Geom2d_BSplineCurve) aRes = polar.BSpline();

					//		if (!aRes.IsNull())
					//		{
					//			myTolerance = polar.Tolerance();
					//			if ((IsTrimmed[0] || IsTrimmed[1]))
					//			{
					//				if (IsTrimmed[0])
					//				{
					//					//Add segment before start of curve
					//					f = myCurve->FirstParameter();
					//					ExtendC2d(aRes, f, -dt, U1, U2, V1, V2, 0, SingularCase[0]);
					//				}
					//				if (IsTrimmed[1])
					//				{
					//					//Add segment after end of curve
					//					l = myCurve->LastParameter();
					//					ExtendC2d(aRes, l, dt, U1, U2, V1, V2, 1, SingularCase[1]);
					//				}
					//				Handle(Geom2d_Curve) NewCurve2d;
					//				GeomLib::SameRange(Precision::PConfusion(), aRes,
					//				  aRes->FirstParameter(), aRes->LastParameter(),
					//				  FirstPar, LastPar, NewCurve2d);
					//				aRes = Handle(Geom2d_BSplineCurve)::DownCast(NewCurve2d);
					//			}
					//			myResult.SetBSpline(aRes);
					//			myResult.Done();
					//			myResult.SetType(GeomAbs_BSplineCurve);
					//		}
					//	}
					//	break;

					//default:
					//	{
					//		isAnalyticalSurf = Standard_False;
					//		Standard_Real Vsingular[2] = { 0.0, 0.0 }; //for surfaces of revolution
					//		Standard_Real f = 0.0, l = 0.0;
					//		dt = 0.0;

					//		if (mySurface->GetType() == GeomAbs_SurfaceOfRevolution)
					//		{
					//			//Check possible singularity

					//			gp_Pnt P = mySurface->AxeOfRevolution().Location();
					//			gp_Dir N = mySurface->AxeOfRevolution().Direction();
					//			gp_Lin L(P, N);

					//			f = myCurve->FirstParameter();
					//			l = myCurve->LastParameter();
					//			dt = (l - f) * eps;

					//			P = myCurve->Value(f);
					//			if (L.Distance(P) < Precision::Confusion())
					//			{
					//				IsTrimmed[0] = Standard_True;
					//				f = f + dt;
					//				myCurve = myCurve->Trim(f, l, Precision::Confusion());
					//				// Searching the parameter on the basis curve for surface of revolution
					//				Extrema_ExtPC anExtr(P, * mySurface->BasisCurve(), myTolerance);
					//				if (anExtr.IsDone())
					//				{
					//					Standard_Real aMinDist = RealLast();
					//					for (Standard_Integer anIdx = 1; anIdx <= anExtr.NbExt(); anIdx++)
					//					{
					//						if (anExtr.IsMin(anIdx) &&
					//							anExtr.SquareDistance(anIdx) < aMinDist)
					//						{
					//							aMinDist = anExtr.SquareDistance(anIdx);
					//							Vsingular[0] = anExtr.Point(anIdx).Parameter();
					//						}
					//					}
					//				}
					//				else
					//					Vsingular[0] = ElCLib::Parameter(L, P);
					//				//SingularCase[0] = 3;
					//			}

					//			P = myCurve->Value(l);
					//			if (L.Distance(P) < Precision::Confusion())
					//			{
					//				IsTrimmed[1] = Standard_True;
					//				l = l - dt;
					//				myCurve = myCurve->Trim(f, l, Precision::Confusion());
					//				// Searching the parameter on the basis curve for surface of revolution
					//				Extrema_ExtPC anExtr(P, * mySurface->BasisCurve(), myTolerance);
					//				if (anExtr.IsDone())
					//				{
					//					Standard_Real aMinDist = RealLast();
					//					for (Standard_Integer anIdx = 1; anIdx <= anExtr.NbExt(); anIdx++)
					//					{
					//						if (anExtr.IsMin(anIdx) &&
					//							anExtr.SquareDistance(anIdx) < aMinDist)
					//						{
					//							aMinDist = anExtr.SquareDistance(anIdx);
					//							Vsingular[1] = anExtr.Point(anIdx).Parameter();
					//						}
					//					}
					//				}
					//				else
					//					Vsingular[1] = ElCLib::Parameter(L, P);
					//				//SingularCase[1] = 4;
					//			}
					//		}

					//		Standard_Real aTolU = Max(ComputeTolU(mySurface, myTolerance), Precision::Confusion());
					//		Standard_Real aTolV = Max(ComputeTolV(mySurface, myTolerance), Precision::Confusion());
					//		Standard_Real aTol2d = Sqrt(aTolU * aTolU + aTolV * aTolV);

					//		Standard_Real aMaxDist = 100. * myTolerance;
					//		if (myMaxDist > 0.)
					//		{
					//			aMaxDist = myMaxDist;
					//		}
					//		Handle(ProjLib_HCompProjectedCurve) HProjector = new ProjLib_HCompProjectedCurve(mySurface, myCurve, aTolU, aTolV, aMaxDist);

					//		// Normalement, dans le cadre de ProjLib, le resultat 
					//		// doit etre une et une seule courbe !!!
					//		// De plus, cette courbe ne doit pas etre Single point
					//		Standard_Integer NbCurves = HProjector->NbCurves();
					//		Standard_Real Udeb = 0.0, Ufin = 0.0;
					//		if (NbCurves > 0)
					//		{
					//			HProjector->Bounds(1, Udeb, Ufin);
					//		}
					//		else
					//		{
					//			return;
					//		}
					//		// Approximons cette courbe algorithmique.
					//		Standard_Boolean Only3d = Standard_False;
					//		Standard_Boolean Only2d = Standard_True;
					//		GeomAbs_Shape Continuity = GeomAbs_C1;
					//		if (myBndPnt == AppParCurves_PassPoint)
					//		{
					//			Continuity = GeomAbs_C0;
					//		}
					//		Standard_Integer MaxDegree = 14;
					//		if (myDegMax > 0)
					//		{
					//			MaxDegree = myDegMax;
					//		}
					//		Standard_Integer MaxSeg = 16;
					//		if (myMaxSegments > 0)
					//		{
					//			MaxSeg = myMaxSegments;
					//		}

					//		Approx_CurveOnSurface appr(HProjector, mySurface, Udeb, Ufin, myTolerance);
					//		appr.Perform(MaxSeg, MaxDegree, Continuity, Only3d, Only2d);

					//		Handle(Geom2d_BSplineCurve) aRes = appr.Curve2d();

					//		if (!aRes.IsNull())
					//		{
					//			aTolU = appr.MaxError2dU();
					//			aTolV = appr.MaxError2dV();
					//			Standard_Real aNewTol2d = Sqrt(aTolU * aTolU + aTolV * aTolV);
					//			myTolerance *= (aNewTol2d / aTol2d);
					//			if (IsTrimmed[0] || IsTrimmed[1])
					//			{
					//				// Treatment only for surface of revolution
					//				Standard_Real u1, u2, v1, v2;
					//				u1 = mySurface->FirstUParameter();
					//				u2 = mySurface->LastUParameter();
					//				v1 = mySurface->FirstVParameter();
					//				v2 = mySurface->LastVParameter();

					//				if (IsTrimmed[0])
					//				{
					//					//Add segment before start of curve
					//					ExtendC2d(aRes, f, -dt, u1, u2, Vsingular[0], v2, 0, 3);
					//				}
					//				if (IsTrimmed[1])
					//				{
					//					//Add segment after end of curve
					//					ExtendC2d(aRes, l, dt, u1, u2, v1, Vsingular[1], 1, 4);
					//				}
					//				Handle(Geom2d_Curve) NewCurve2d;
					//				GeomLib::SameRange(Precision::PConfusion(), aRes,
					//				  aRes->FirstParameter(), aRes->LastParameter(),
					//				  FirstPar, LastPar, NewCurve2d);
					//				aRes = Handle(Geom2d_BSplineCurve)::DownCast(NewCurve2d);
					//				if (Continuity == GeomAbs_C0)
					//				{
					//					// try to smoother the Curve GeomAbs_C1.
					//					Standard_Integer aDeg = aRes->Degree();
					//					Standard_Boolean OK = Standard_True;
					//					Standard_Real aSmoothTol = Max(Precision::Confusion(), aNewTol2d);
					//					for (Standard_Integer ij = 2; ij < aRes->NbKnots(); ij++)
					//					{
					//						OK = OK && aRes->RemoveKnot(ij, aDeg - 1, aSmoothTol);
					//					}
					//				}
					//			}

					//			myResult.SetBSpline(aRes);
					//			myResult.Done();
					//			myResult.SetType(GeomAbs_BSplineCurve);
					//		}
					//	}
			}

			//if (!myResult.IsDone() && isAnalyticalSurf)
			//{
			//	// Use advanced analytical projector if base analytical projection failed.
			//	ProjLib_ComputeApprox Comp;
			//	Comp.SetTolerance(myTolerance);
			//	Comp.SetDegree(myDegMin, myDegMax);
			//	Comp.SetMaxSegments(myMaxSegments);
			//	Comp.SetBndPnt(myBndPnt);
			//	Comp.Perform(myCurve, mySurface);
			//	if (Comp.Bezier().IsNull() && Comp.BSpline().IsNull())
			//		return; // advanced projector has been failed too
			//	myResult.Done();
			//	Handle(Geom2d_BSplineCurve) aRes;
			//	if (Comp.BSpline().IsNull())
			//	{
			//		aRes = Geom2dConvert::CurveToBSplineCurve(Comp.Bezier());
			//	}
			//	else
			//	{
			//		aRes = Comp.BSpline();
			//	}
			//	if ((IsTrimmed[0] || IsTrimmed[1]))
			//	{
			//		if (IsTrimmed[0])
			//		{
			//			//Add segment before start of curve
			//			Standard_Real f = myCurve->FirstParameter();
			//			ExtendC2d(aRes, f, -dt, U1, U2, V1, V2, 0, SingularCase[0]);
			//		}
			//		if (IsTrimmed[1])
			//		{
			//			//Add segment after end of curve
			//			Standard_Real l = myCurve->LastParameter();
			//			ExtendC2d(aRes, l, dt, U1, U2, V1, V2, 1, SingularCase[1]);
			//		}
			//		Handle(Geom2d_Curve) NewCurve2d;
			//		GeomLib::SameRange(Precision::PConfusion(), aRes,
			//		  aRes->FirstParameter(), aRes->LastParameter(),
			//		  FirstPar, LastPar, NewCurve2d);
			//		aRes = Handle(Geom2d_BSplineCurve)::DownCast(NewCurve2d);
			//		myResult.SetBSpline(aRes);
			//		myResult.SetType(GeomAbs_BSplineCurve);
			//	}
			//	else
			//	{
			//		// set the type
			//		if (SType == GeomAbs_Plane && CType == GeomAbs_BezierCurve)
			//		{
			//			myResult.SetType(GeomAbs_BezierCurve);
			//			myResult.SetBezier(Comp.Bezier());
			//		}
			//		else
			//		{
			//			myResult.SetType(GeomAbs_BSplineCurve);
			//			myResult.SetBSpline(Comp.BSpline());
			//		}
			//	}
			//	// set the periodicity flag
			//	if (SType == GeomAbs_Plane &&
			//	  CType == GeomAbs_BSplineCurve &&
			//	  myCurve->IsPeriodic())
			//	{
			//		myResult.SetPeriodic();
			//	}
			//	myTolerance = Comp.Tolerance();
			//}

			//bool[] isPeriodic = {mySurface.IsUPeriodic(),
			//				   mySurface.IsVPeriodic()};
			//if (myResult.IsDone() && (isPeriodic[0] || isPeriodic[1]))
			{
				// Check result curve to be in params space.

				// U and V parameters space correspondingly.
				//double[] aSurfFirstPar = {mySurface->FirstUParameter(),
				//							mySurface->FirstVParameter()};
				//double[] aSurfPeriod = { 0.0, 0.0 };
				//if (isPeriodic[0])
				//	aSurfPeriod[0] = mySurface.UPeriod();
				//if (isPeriodic[1])
				//	aSurfPeriod[1] = mySurface.VPeriod();

				//for (int anIdx = 1; anIdx <= 2; anIdx++)
				//{
				//	if (!isPeriodic[anIdx - 1])
				//		continue;

				//	if (myResult.GetType() == GeomAbs_BSplineCurve)
				//	{
				//		NCollection_DataMap<Standard_Integer, Standard_Integer> aMap;
				//		Handle(Geom2d_BSplineCurve) aRes = myResult.BSpline();
				//		const Standard_Integer aDeg = aRes->Degree();

				//		for (Standard_Integer aKnotIdx = aRes->FirstUKnotIndex();
				//							 aKnotIdx < aRes->LastUKnotIndex();
				//							 aKnotIdx++)
				//		{
				//			const Standard_Real aFirstParam = aRes->Knot(aKnotIdx);
				//			const Standard_Real aLastParam = aRes->Knot(aKnotIdx + 1);

				//			for (Standard_Integer anIntIdx = 0; anIntIdx <= aDeg; anIntIdx++)
				//			{
				//				const Standard_Real aCurrParam = aFirstParam + (aLastParam - aFirstParam) * anIntIdx / (aDeg + 1.0);
				//				gp_Pnt2d aPnt2d;
				//				aRes->D0(aCurrParam, aPnt2d);

				//				Standard_Integer aMapKey = Standard_Integer((aPnt2d.Coord(anIdx) - aSurfFirstPar[anIdx - 1]) / aSurfPeriod[anIdx - 1]);

				//				if (aPnt2d.Coord(anIdx) - aSurfFirstPar[anIdx - 1] < 0.0)
				//					aMapKey--;

				//				if (aMap.IsBound(aMapKey))
				//					aMap.ChangeFind(aMapKey)++;
				//				else
				//					aMap.Bind(aMapKey, 1);
				//			}
				//		}

				//		Standard_Integer aMaxPoints = 0, aMaxIdx = 0;
				//		NCollection_DataMap<Standard_Integer, Standard_Integer>::Iterator aMapIter(aMap);
				//		for (; aMapIter.More(); aMapIter.Next())
				//		{
				//			if (aMapIter.Value() > aMaxPoints)
				//			{
				//				aMaxPoints = aMapIter.Value();
				//				aMaxIdx = aMapIter.Key();
				//			}
				//		}
				//		if (aMaxIdx != 0)
				//		{
				//			gp_Pnt2d aFirstPnt = aRes->Value(aRes->FirstParameter());
				//			gp_Pnt2d aSecondPnt = aFirstPnt;
				//			aSecondPnt.SetCoord(anIdx, aFirstPnt.Coord(anIdx) - aSurfPeriod[anIdx - 1] * aMaxIdx);
				//			aRes->Translate(gp_Vec2d(aFirstPnt, aSecondPnt));
				//		}
				//	}

				//	if (myResult.GetType() == GeomAbs_Line)
				//	{
				//		Standard_Real aT1 = myCurve->FirstParameter();
				//		Standard_Real aT2 = myCurve->LastParameter();

				//		if (anIdx == 1)
				//		{
				//			// U param space.
				//			myResult.UFrame(aT1, aT2, aSurfFirstPar[anIdx - 1], aSurfPeriod[anIdx - 1]);
				//		}
				//		else
				//		{
				//			// V param space.
				//			myResult.VFrame(aT1, aT2, aSurfFirstPar[anIdx - 1], aSurfPeriod[anIdx - 1]);
				//		}
				//	}
				//}
			}
		}
	}


	//! Projects elementary curves on a plane.
	public class ProjLib_Plane : ProjLib_Projector
	{
	}

}