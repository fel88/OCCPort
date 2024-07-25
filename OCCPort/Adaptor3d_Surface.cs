namespace OCCPort
{
	public class Adaptor3d_Surface
	{
		public double FirstUParameter()
		{
			throw new Standard_NotImplemented("Adaptor3d_Surface::FirstUParameter");
		}

		//=======================================================================
		//function : D0
		//purpose  : 
		//=======================================================================

		//void Adaptor3d_Surface::D0(const Standard_Real U, const Standard_Real V, gp_Pnt& P) const 
		public void D0(double u, double v, gp_Pnt pnt)
		{
			throw new Standard_NotImplemented("Adaptor3d_Surface::D0");
		}



		public double LastUParameter()
		{
			throw new Standard_NotImplemented("Adaptor3d_Surface::LastUParameter");
		}
		//=======================================================================
		//function : NbVPoles
		//purpose  : 
		//=======================================================================

		public int NbVPoles()
		{
			throw new Standard_NotImplemented("Adaptor3d_Surface::NbVPoles");
		}

		//=======================================================================
		//function : OffsetValue
		//purpose  : 
		//=======================================================================

		public double OffsetValue()
		{
			throw new Standard_NotImplemented("Adaptor3d_Surface::OffsetValue");
		}


		//=======================================================================
		//function : BasisSurface
		//purpose  : 
		//=======================================================================

		public Adaptor3d_Surface BasisSurface()
		{
			throw new Standard_NotImplemented("Adaptor3d_Surface::BasisSurface");
		}

		public double FirstVParameter()
		{
			throw new Standard_NotImplemented("Adaptor3d_Surface::FirstVParameter");
		}
		public double LastVParameter()
		{
			throw new Standard_NotImplemented("Adaptor3d_Surface::LastVParameter");
		}

		//=======================================================================
		//function : NbUPoles
		//purpose  : 
		//=======================================================================

		public int NbUPoles()
		{
			throw new Standard_NotImplemented("Adaptor3d_Surface::NbUPoles");
		}

		//=======================================================================
		//function : BSpline
		//purpose  : 
		//=======================================================================

		public Geom_BSplineSurface BSpline()
		{
			throw new Standard_NotImplemented("Adaptor3d_Surface::BSpline");
		}

		//! Returns the type of the surface : Plane, Cylinder,
		//! Cone,      Sphere,        Torus,    BezierSurface,
		//! BSplineSurface,               SurfaceOfRevolution,
		//! SurfaceOfExtrusion, OtherSurface
		public virtual new GeomAbs_SurfaceType GetType()
		{
			throw new Standard_NotImplemented("Adaptor3d_Surface::GetType");
		}
	}
}