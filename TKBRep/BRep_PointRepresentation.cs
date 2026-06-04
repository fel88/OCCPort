using System.Reflection.Metadata;
using TKG2d;
using TKG3d;
using TKMath;

namespace OCCPort
{
    //! Root  class     for   the points  representations.
    //! Contains a location and a parameter.
    public abstract class BRep_PointRepresentation
    {
        public bool IsPointOnCurve
  (Geom_Curve c,
    TopLoc_Location l)
        {
            return false;
        }

        public BRep_PointRepresentation(double P,
                            TopLoc_Location L)
        {
            myLocation = (L);
            myParameter = (P);

        }
        TopLoc_Location myLocation;

        public bool IsPointOnCurveOnSurface()
        {
            return false;
        }
        public void Parameter(double P)
        {
            myParameter = P;
        }
        double myParameter;

        public abstract double Parameter2();
        public virtual double Parameter()
        {
            return myParameter;
        }

        public bool IsPointOnSurface(Geom_Surface s,
   ref TopLoc_Location l)
        {
            return false;
        }

        public bool IsPointOnCurveOnSurface
  (Geom2d_Curve c,
   Geom_Surface s,
   TopLoc_Location l)
        {
            return false;
        }
    }
}

