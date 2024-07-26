namespace OCCPort
{
	public class Adaptor3d_Surface
	{
		public double FirstUParameter()
		{
			throw new Standard_NotImplemented("Adaptor3d_Surface::FirstUParameter");
		}
		//=======================================================================
		//function : UDegree
		//purpose  : 
		//=======================================================================

		public virtual int UDegree()
		{
			throw new Standard_NotImplemented("Adaptor3d_Surface::UDegree");
		}


		//=======================================================================
		//function : NbVKnots
		//purpose  : 
		//=======================================================================

		public virtual int NbVKnots()
		{
			throw new Standard_NotImplemented("Adaptor3d_Surface::NbVKnots");
		}
		//=======================================================================
		//function : D0
		//purpose  : 
		//=======================================================================

		//void Adaptor3d_Surface::D0(const Standard_Real U, const Standard_Real V, gp_Pnt& P) const 
		public virtual  void D0(double u, double v, ref gp_Pnt pnt)
		{
			throw new Standard_NotImplemented("Adaptor3d_Surface::D0");
		}



		public virtual  double LastUParameter()
		{
			throw new Standard_NotImplemented("Adaptor3d_Surface::LastUParameter");
		}
		//=======================================================================
		//function : NbVPoles
		//purpose  : 
		//=======================================================================

		public virtual int NbVPoles()
		{
			throw new Standard_NotImplemented("Adaptor3d_Surface::NbVPoles");
		}

		//=======================================================================
		//function : OffsetValue
		//purpose  : 
		//=======================================================================

		public virtual double OffsetValue()
		{
			throw new Standard_NotImplemented("Adaptor3d_Surface::OffsetValue");
		}


		//=======================================================================
		//function : BasisSurface
		//purpose  : 
		//=======================================================================

		public virtual Adaptor3d_Surface BasisSurface()
		{
			throw new Standard_NotImplemented("Adaptor3d_Surface::BasisSurface");
		}

		public virtual double FirstVParameter()
		{
			throw new Standard_NotImplemented("Adaptor3d_Surface::FirstVParameter");
		}
		public virtual double LastVParameter()
		{
			throw new Standard_NotImplemented("Adaptor3d_Surface::LastVParameter");
		}

		//=======================================================================
		//function : NbUPoles
		//purpose  : 
		//=======================================================================

		public virtual int NbUPoles()
		{
			throw new Standard_NotImplemented("Adaptor3d_Surface::NbUPoles");
		}

		//=======================================================================
		//function : BSpline
		//purpose  : 
		//=======================================================================

		public virtual Geom_BSplineSurface BSpline()
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