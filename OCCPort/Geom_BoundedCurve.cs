namespace OCCPort
{
	//! The abstract class BoundedCurve describes the
	//! common behavior of bounded curves in 3D space. A
	//! bounded curve is limited by two finite values of the
	//! parameter, termed respectively "first parameter" and
	//! "last parameter". The "first parameter" gives the "start
	//! point" of the bounded curve, and the "last parameter"
	//! gives the "end point" of the bounded curve.
	//! The length of a bounded curve is finite.
	//! The Geom package provides three concrete classes of bounded curves:
	//! - two frequently used mathematical formulations of complex curves:
	//! - Geom_BezierCurve,
	//! - Geom_BSplineCurve, and
	//! - Geom_TrimmedCurve to trim a curve, i.e. to only
	//! take part of the curve limited by two values of the
	//! parameter of the basis curve.
	public abstract class Geom_BoundedCurve : Geom_Curve
	{

	}

}