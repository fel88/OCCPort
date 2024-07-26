namespace OCCPort
{
	//! An interface between the services provided by any
	//! curve from the package Geom2d and those required
	//! of the curve by algorithms which use it.
	//!
	//! Polynomial coefficients of BSpline curves used for their evaluation are
	//! cached for better performance. Therefore these evaluations are not
	//! thread-safe and parallel evaluations need to be prevented.
	public class Geom2dAdaptor_Curve : Adaptor2d_Curve2d
	{
	}

}