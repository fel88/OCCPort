namespace OCCPort
{
	//! An interface between the services provided by any
	//! surface from the package Geom and those required
	//! of the surface by algorithms which use it.
	//! Creation of the loaded surface the surface is C1 by piece
	//!
	//! Polynomial coefficients of BSpline surfaces used for their evaluation are
	//! cached for better performance. Therefore these evaluations are not
	//! thread-safe and parallel evaluations need to be prevented.
	public class GeomAdaptor_Surface : Adaptor3d_Surface
	{  //! Returns the type of the surface : Plane, Cylinder,
	   //! Cone,      Sphere,        Torus,    BezierSurface,
	   //! BSplineSurface,               SurfaceOfRevolution,
	   //! SurfaceOfExtrusion, OtherSurface
		public virtual new GeomAbs_SurfaceType GetType() { return mySurfaceType; }

		public GeomAdaptor_Surface()
		{
			/*myUFirst=(0.), myULast(0.),
	   myVFirst(0.), myVLast(0.),
	   myTolU(0.),   myTolV(0.),*/
			mySurfaceType = GeomAbs_SurfaceType.GeomAbs_OtherSurface;

		}
		GeomAbs_SurfaceType mySurfaceType;

	}
}