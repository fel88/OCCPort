namespace OCCPort
{
    //! Root  class     for   the points  representations.
    //! Contains a location and a parameter.
    public class BRep_PointRepresentation
    {
        public bool IsPointOnCurveOnSurface()
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

