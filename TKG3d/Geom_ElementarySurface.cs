using TKMath;

namespace TKG3d
{
    //! Describes the common behavior of surfaces which
    //! have a simple parametric equation in a local
    //! coordinate system. The Geom package provides
    //! several implementations of concrete elementary surfaces:
    //! - the plane, and
    //! - four simple surfaces of revolution: the cylinder, the
    //! cone, the sphere and the torus.
    //! An elementary surface inherits the common behavior
    //! of Geom_Surface surfaces. Furthermore, it is located
    //! in 3D space by a coordinate system (a gp_Ax3
    //! object) which is also its local coordinate system.
    //! Any elementary surface is oriented, i.e. the normal
    //! vector is always defined, and gives the same
    //! orientation to the surface, at any point on the surface.
    //! In topology this property is referred to as the "outside
    //! region of the surface". This orientation is related to
    //! the two parametric directions of the surface.
    //! Rotation of a surface around the "main Axis" of its
    //! coordinate system, in the trigonometric sense given
    //! by the "X Direction" and the "Y Direction" of the
    //! coordinate system, defines the u parametric direction
    //! of that elementary surface of revolution. This is the
    //! default construction mode.
    //! It is also possible, however, to change the orientation
    //! of a surface by reversing one of the two parametric
    //! directions: use the UReverse or VReverse functions
    //! to change the orientation of the normal at any point on the surface.
    //! Warning
    //! The local coordinate system of an elementary surface
    //! is not necessarily direct:
    //! - if it is direct, the trigonometric sense defined by its
    //! "main Direction" is the same as the trigonometric
    //! sense defined by its two vectors "X Direction" and "Y Direction":
    //! "main Direction" = "X Direction" ^ "Y Direction"
    //! - if it is indirect, the two definitions of trigonometric
    //! sense are opposite:
    //! "main Direction" = - "X Direction" ^ "Y Direction"
    public abstract class Geom_ElementarySurface : Geom_Surface
    {
        protected gp_Ax3 pos;

        //! Returns the local coordinates system of the surface.
        public gp_Ax3 Position() { return pos; }


    }
}
