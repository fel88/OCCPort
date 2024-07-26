namespace OCCPort
{
	//! Root class for 2D curves on which geometric
	//! algorithms work.
	//! An adapted curve is an interface between the
	//! services provided by a curve, and those required of
	//! the curve by algorithms, which use it.
	//! A derived concrete class is provided:
	//! Geom2dAdaptor_Curve for a curve from the Geom2d package.
	//!
	//! Polynomial coefficients of BSpline curves used for their evaluation are
	//! cached for better performance. Therefore these evaluations are not
	//! thread-safe and parallel evaluations need to be prevented.
	public class Adaptor2d_Curve2d
	{

	}
}