namespace OCCPort
{
	//! The Surface from BRepAdaptor allows to  use a Face
	//! of the BRep topology look like a 3D surface.
	//!
	//! It  has  the methods  of  the class   Surface from
	//! Adaptor3d.
	//!
	//! It is created or initialized with a Face. It takes
	//! into account the local coordinates system.
	//!
	//! The  u,v parameter range is   the minmax value for
	//! the  restriction,  unless  the flag restriction is
	//! set to false.
	public class BRepAdaptor_Surface : Adaptor3d_Surface
	{
		//! Returns the type of the surface : Plane, Cylinder,
		//! Cone,      Sphere,        Torus,    BezierSurface,
		//! BSplineSurface,               SurfaceOfRevolution,
		//! SurfaceOfExtrusion, OtherSurface
		public virtual new GeomAbs_SurfaceType GetType() { return mySurf.GetType(); }

		GeomAdaptor_Surface mySurf;
	}
}