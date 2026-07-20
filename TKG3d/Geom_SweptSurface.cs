using TKMath;

namespace TKG3d
{
    //! Describes the common behavior for surfaces
    //! constructed by sweeping a curve with another curve.
    //! The Geom package provides two concrete derived
    //! surfaces: surface of revolution (a revolved surface),
    //! and surface of linear extrusion (an extruded surface).
    public  abstract class Geom_SweptSurface : Geom_Surface
    {
        

        public  gp_Dir Direction()   { return direction; }

        //! Returns the referenced curve of the surface.
        //! For a surface of revolution it is the revolution curve,
        //! for a surface of linear extrusion it is the extruded curve.
        public Geom_Curve BasisCurve()
        {
            return basisCurve;
        }
       public  GeomAbs_Shape Continuity()  { return smooth; }

        protected Geom_Curve basisCurve;
        protected gp_Dir direction;
        protected GeomAbs_Shape smooth;
        

        
    }
}
