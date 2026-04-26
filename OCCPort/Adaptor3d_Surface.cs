namespace OCCPort
{
    public abstract class Adaptor3d_Surface
    {
        public abstract double FirstUParameter();
        //=======================================================================
        //function : UDegree
        //purpose  : 
        //=======================================================================

        public virtual int UDegree()
        {
            throw new Standard_NotImplemented("Adaptor3d_Surface::UDegree");
        }
        public abstract bool IsVPeriodic();
        public abstract bool IsUPeriodic();

        public abstract double UPeriod();
        public abstract double VPeriod();
        public abstract double VResolution(double v);
        public abstract double UResolution(double v);

        //=======================================================================
        //function : NbVKnots
        //purpose  : 
        //=======================================================================
        //! Computes the point  and the first derivatives on the surface.
        //! Raised if the continuity of the current intervals is not C1.
        //!
        //! Tip: use GeomLib::NormEstim() to calculate surface normal at specified (U, V) point.
        public abstract void D1(double U, double V, out gp_Pnt P, out gp_Vec D1U, out gp_Vec D1V);

        public abstract int NbVKnots();
        //=======================================================================
        //function : D0
        //purpose  : 
        //=======================================================================

        //void Adaptor3d_Surface::D0(const Standard_Real U, const Standard_Real V, gp_Pnt& P) const 
        public abstract void D0(double u, double v, ref gp_Pnt pnt);



        public abstract double LastUParameter();
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

        public abstract Adaptor3d_Surface BasisSurface();

        public abstract double FirstVParameter();
        public abstract double LastVParameter();

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
        public abstract gp_Pln Plane();
        //! Returns the type of the surface : Plane, Cylinder,
        //! Cone,      Sphere,        Torus,    BezierSurface,
        //! BSplineSurface,               SurfaceOfRevolution,
        //! SurfaceOfExtrusion, OtherSurface
        public abstract GeomAbs_SurfaceType _GetType();
    }
}