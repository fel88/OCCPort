using System;
using System.Reflection.Metadata;
using System.Threading;

namespace OCCPort
{
    //! The ProjLib package first provides projection of curves on a plane along a given Direction.
    //! The result will be a 3D curve.
    //!
    //! The ProjLib package provides projection of curves on surfaces to compute the curve in the parametric space.
    //! It is assumed that the curve is on the surface.
    //!
    //! It provides:
    //!
    //! * Package methods to handle the easiest cases:
    //!  - Line, Circle, Ellipse, Parabola, Hyperbola on plane.
    //!  - Line, Circle on cylinder.
    //!  - Line, Circle on cone.
    //!
    //! * Classes to handle the general cases:
    //!  - Plane.
    //!  - Cylinder.
    //!  - Cone.
    //!  - Sphere.
    //!  - Torus.
    //!
    //! * A generic class to handle a Adaptor3d_Curve on a Adaptor3d_Surface.
    public class ProjLib
    {
        internal static bool IsAnaSurf(Adaptor3d_Surface theAS)
        {

            switch (theAS._GetType())
            {

                case GeomAbs_SurfaceType. GeomAbs_Plane:
                case GeomAbs_SurfaceType.GeomAbs_Cylinder:
                case GeomAbs_SurfaceType.GeomAbs_Cone:
                case GeomAbs_SurfaceType.GeomAbs_Sphere:
                case GeomAbs_SurfaceType.GeomAbs_Torus:
                    return true;                    
                default:
                    return false;                    
            }


        }
    }
}