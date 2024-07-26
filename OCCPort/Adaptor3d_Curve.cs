namespace OCCPort
{
	//! Root class for 3D curves on which geometric
	//! algorithms work.
	//! An adapted curve is an interface between the
	//! services provided by a curve and those required of
	//! the curve by algorithms which use it.
	//! Two derived concrete classes are provided:
	//! - GeomAdaptor_Curve for a curve from the Geom package
	//! - Adaptor3d_CurveOnSurface for a curve lying on
	//! a surface from the Geom package.
	//!
	//! Polynomial coefficients of BSpline curves used for their evaluation are
	//! cached for better performance. Therefore these evaluations are not
	//! thread-safe and parallel evaluations need to be prevented.
	public class Adaptor3d_Curve
	{
		//=======================================================================
		//function : GetType
		//purpose  : 
		//=======================================================================

		public new GeomAbs_CurveType GetType()
		{
			throw new Standard_NotImplemented("Adaptor3d_Curve::GetType");
		}

		//=======================================================================
		//function : Line
		//purpose  : 
		//=======================================================================

		public gp_Lin Line()
		{
			throw new Standard_NotImplemented("Adaptor3d_Curve::Line");
		}

		public double FirstParameter()
		{
			throw new Standard_NotImplemented("Adaptor3d_Curve::FirstParameter");
		}
		public double LastParameter()
		{
			throw new Standard_NotImplemented("Adaptor3d_Curve::LastParameter");
		}

		//void Adaptor3d_Curve::D0(const Standard_Real U, gp_Pnt& P) const 
		public virtual void D0(double d, ref gp_Pnt p)
		{
			throw new Standard_NotImplemented("Adaptor3d_Curve::D0");
		}
	}

	//! Identifies the type of a curve.
	public enum GeomAbs_CurveType
	{
		GeomAbs_Line,
		GeomAbs_Circle,
		GeomAbs_Ellipse,
		GeomAbs_Hyperbola,
		GeomAbs_Parabola,
		GeomAbs_BezierCurve,
		GeomAbs_BSplineCurve,
		GeomAbs_OffsetCurve,
		GeomAbs_OtherCurve
	};
}