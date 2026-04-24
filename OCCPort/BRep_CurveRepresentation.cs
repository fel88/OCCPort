using System.Reflection.Metadata;

namespace OCCPort
{
    //! Root class for the curve representations. Contains
    //! a location.
    public abstract class BRep_CurveRepresentation
    {
        public virtual Geom_Curve Curve3D()
        {
            throw new Standard_DomainError("BRep_CurveRepresentation");
        }

        public virtual bool IsRegularity()
        {
            return false;
        }
        public virtual Geom_Surface Surface()
        {
            throw new Standard_DomainError("BRep_CurveRepresentation");
        }

        public bool IsCurveOnClosedSurface()
        {
            return false;
        }

        public virtual Geom2d_Curve PCurve2()
        {
            throw new Standard_DomainError("BRep_CurveRepresentation");
        }

        public virtual Geom2d_Curve PCurve()
        {
            throw new Standard_DomainError("BRep_CurveRepresentation");
        }

        //! A 3D curve representation.
        public virtual bool IsCurve3D()
        {
            return false;
        }

        public virtual bool IsCurveOnSurface(Geom_Surface a, TopLoc_Location f)
        {
            return false;
        }

        //=======================================================================
        //function : Continuity
        //purpose  : 
        //=======================================================================

        public virtual GeomAbs_Shape Continuity()
        {
            throw new Standard_DomainError("BRep_CurveRepresentation");
        }

        public void Curve3D(Geom_Curve cc)
        {
            throw new Standard_DomainError("BRep_CurveRepresentation");
        }


        public virtual bool IsCurveOnSurface()
        {
            return false;
        }

        public TopLoc_Location Location()
        {
            return myLocation;
        }
        //Standard_EXPORT BRep_CurveRepresentation(const TopLoc_Location& L);
        public void Location(TopLoc_Location L)
        {
            myLocation = L;
        }

        TopLoc_Location myLocation = new TopLoc_Location();

        protected BRep_CurveRepresentation(TopLoc_Location L)
        {
            myLocation = (L);
        }
    }
}
