using OCCPort;
using System;

namespace OCCPort
{
	public class BndLib_Add3dCurve
	{
		public static void Add(Adaptor3d_Curve C,
			   double U1,
			   double U2,
			   double Tol,
					 Bnd_Box B)
		{
			double weakness = 1.5;  //OCC566(apo)
			double tol = 0.0;
			switch (C.GetType())
			{

				case GeomAbs_CurveType.GeomAbs_Line:
					{
						BndLib.Add(C.Line(), U1, U2, Tol, B);
						break;
					}
				//case GeomAbs_CurveType.GeomAbs_Circle:
				//	{
				//		BndLib.Add(C.Circle(), U1, U2, Tol, B);
				//		break;
				//	}
				//case GeomAbs_CurveType.GeomAbs_Ellipse:
				//	{
				//		BndLib::Add(C.Ellipse(), U1, U2, Tol, B);
				//		break;
				//	}
				//case GeomAbs_CurveType.GeomAbs_Hyperbola:
				//	{
				//		BndLib::Add(C.Hyperbola(), U1, U2, Tol, B);
				//		break;
				//	}
				//case GeomAbs_CurveType.GeomAbs_Parabola:
				//	{
				//		BndLib::Add(C.Parabola(), U1, U2, Tol, B);
				//		break;
				//	}
				//case GeomAbs_CurveType.GeomAbs_BezierCurve:
				//	{
				//		Handle(Geom_BezierCurve) Bz = C.Bezier();
				//		Standard_Integer N = Bz->Degree();
				//		GeomAdaptor_Curve GACurve(Bz);
				//		Bnd_Box B1;
				//		tol = FillBox(B1, GACurve, U1, U2, N);
				//		B1.Enlarge(weakness * tol);
				//		reduceSplineBox(C, B1, B);
				//		B.Enlarge(Tol);
				//		break;
				//	}
				//case GeomAbs_CurveType.GeomAbs_BSplineCurve:
				//	{
				//		Handle(Geom_BSplineCurve) Bs = C.BSpline();
				//		if (Abs(Bs->FirstParameter() - U1) > Precision::Parametric(Tol) ||
				//	   Abs(Bs->LastParameter() - U2) > Precision::Parametric(Tol))
				//		{

				//			Handle(Geom_Geometry) G = Bs->Copy();
				//			Handle(Geom_BSplineCurve) Bsaux(Handle(Geom_BSplineCurve)::DownCast(G));
				//			Standard_Real u1 = U1, u2 = U2;
				//			//// modified by jgv, 24.10.01 for BUC61031 ////
				//			if (Bsaux->IsPeriodic())
				//				ElCLib::AdjustPeriodic(Bsaux->FirstParameter(), Bsaux->LastParameter(), Precision::PConfusion(), u1, u2);
				//			else
				//			{
				//				////////////////////////////////////////////////
				//				//  modified by NIZHNY-EAP Fri Dec  3 14:29:14 1999 ___BEGIN___
				//				// To avoid exception in Segment
				//				if (Bsaux->FirstParameter() > U1) u1 = Bsaux->FirstParameter();
				//				if (Bsaux->LastParameter() < U2) u2 = Bsaux->LastParameter();
				//				//  modified by NIZHNY-EAP Fri Dec  3 14:29:18 1999 ___END___
				//			}
				//			Standard_Real aSegmentTol = 2. * Precision::PConfusion();
				//			if (Abs(u2 - u1) < aSegmentTol)
				//				aSegmentTol = Abs(u2 - u1) * 0.01;
				//			Bsaux->Segment(u1, u2, aSegmentTol);
				//			Bs = Bsaux;
				//		}
				//		//OCC566(apo)->
				//		Bnd_Box B1;
				//		Standard_Integer k, k1 = Bs->FirstUKnotIndex(), k2 = Bs->LastUKnotIndex(),
				//						 N = Bs->Degree(), NbKnots = Bs->NbKnots();
				//		TColStd_Array1OfReal Knots(1,NbKnots);
				//		Bs->Knots(Knots);
				//		GeomAdaptor_Curve GACurve(Bs);
				//		Standard_Real first = Knots(k1), last;
				//		for (k = k1 + 1; k <= k2; k++)
				//		{
				//			last = Knots(k);
				//			tol = Max(FillBox(B1, GACurve, first, last, N), tol);
				//			first = last;
				//		}
				//		if (!B1.IsVoid())
				//		{
				//			B1.Enlarge(weakness * tol);
				//			reduceSplineBox(C, B1, B);
				//			B.Enlarge(Tol);
				//		}
				//		//<-OCC566(apo)
				//		break;
				//	}
				//default:
				//	{
				//		Bnd_Box B1;
				//		static Standard_Integer N = 33;
				//		tol = FillBox(B1, C, U1, U2, N);
				//		B1.Enlarge(weakness * tol);
				//		double x, y, z, X, Y, Z;
				//		B1.Get(out x, out y, out z, out X, out Y, out Z);
				//		B.Update(x, y, z, X, Y, Z);
				//		B.Enlarge(Tol);
				//	}
				//	break;
			}
		}
		//=======================================================================
		//function : Add
		//purpose  : 
		//=======================================================================
		public void Add(Adaptor3d_Curve C,
			   double Tol,
					 Bnd_Box B)
		{
			BndLib_Add3dCurve.Add(C,
					   C.FirstParameter(),
					   C.LastParameter(),
					   Tol, B);
		}

		//OCC566(apo)->
		public static double FillBox(Bnd_Box B, Adaptor3d_Curve C,
					 double first, double last,
					 int N)
		{
			gp_Pnt P1 = new gp_Pnt(), P2 = new gp_Pnt(), P3 = new gp_Pnt();
			C.D0(first, ref P1); B.Add(P1);
			double p = first, dp = last - first, tol = 0.0;
			if (Math.Abs(dp) > Precision.PConfusion())
			{
				int i;
				dp /= 2 * N;
				for (i = 1; i <= N; i++)
				{
					p += dp; C.D0(p, ref P2); B.Add(P2);
					p += dp; C.D0(p, ref P3); B.Add(P3);
					gp_Pnt Pc = new gp_Pnt((P1.XYZ() + P3.XYZ()) / 2.0);
					tol = Math.Max(tol, Pc.Distance(P2));
					P1 = P3;
				}
			}
			else
			{
				C.D0(first, ref P1); B.Add(P1);
				C.D0(last, ref P3); B.Add(P3);
				tol = 0.0;
			}
			return tol;
		}
	}

}