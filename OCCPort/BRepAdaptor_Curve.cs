using System.Security.Cryptography;

namespace OCCPort
{

	//! The Curve from BRepAdaptor  allows to use  an Edge
	//! of the BRep topology like a 3D curve.
	//!
	//! It has the methods the class Curve from Adaptor3d.
	//!
	//! It  is created or  Initialized  with  an Edge.  It
	//! takes  into account local  coordinate systems.  If
	//! the Edge has a 3D curve it is  use  with priority.
	//! If the edge  has no 3D curve one  of the curves on
	//! surface is used. It is possible to enforce using a
	//! curve on surface by creating  or initialising with
	//! an Edge and a Face.
	public class BRepAdaptor_Curve : Adaptor3d_Curve
	{
		gp_Trsf myTrsf;
		//GeomAdaptor_Curve myCurve;
		Adaptor3d_CurveOnSurface myConSurf;
		TopoDS_Edge myEdge;

		public void Initialize(TopoDS_Edge E)
		{
			myConSurf = null;
			myEdge = E;
			double pf, pl;

			TopLoc_Location L;
			//Geom_Curve C = BRep_Tool.Curve(E, L, pf, pl);

			//if (C != null)
			//{
			//	myCurve.Load(C, pf, pl);
			//}
			//else
			//{
			//	Geom2d_Curve PC;
			//	Geom_Surface S;
			//	BRep_Tool.CurveOnSurface(E, PC, S, L, pf, pl);
			//	if (PC != null)
			//	{
			//		GeomAdaptor_Surface HS = new GeomAdaptor_Surface();
			//		HS.Load(S);
			//		Geom2dAdaptor_Curve HC = new Geom2dAdaptor_Curve();
			//		HC.Load(PC, pf, pl);
			//		myConSurf = new Adaptor3d_CurveOnSurface();
			//		myConSurf.Load(HC, HS);
			//	}

			//	else
			//	{
			//		throw new Standard_NullObject("BRepAdaptor_Curve::No geometry");
			//	}
			//}
			//myTrsf = L.Transformation();
		}
	}
}