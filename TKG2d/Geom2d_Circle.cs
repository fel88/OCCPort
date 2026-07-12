using OCCPort.Common;
using TKMath;

namespace TKG2d
{
    //! Describes a circle in the plane (2D space).
    //! A circle is defined by its radius and, as with any conic
    //! curve, is positioned in the plane with a coordinate
    //! system (gp_Ax22d object) where the origin is the
    //! center of the circle.
    //! The coordinate system is the local coordinate
    //! system of the circle.
    //! The orientation (direct or indirect) of the local
    //! coordinate system gives an explicit orientation to the
    //! circle, determining the direction in which the
    //! parameter increases along the circle.
    //! The Geom2d_Circle circle is parameterized by an angle:
    //! P(U) = O + R*Cos(U)*XDir + R*Sin(U)*YDir
    //! where:
    //! - P is the point of parameter U,
    //! - O, XDir and YDir are respectively the origin, "X
    //! Direction" and "Y Direction" of its local coordinate system,
    //! - R is the radius of the circle.
    //! The "X Axis" of the local coordinate system therefore
    //! defines the origin of the parameter of the circle. The
    //! parameter is the angle with this "X Direction".
    //! A circle is a closed and periodic curve. The period is
    //! 2.*Pi and the parameter range is [ 0,2.*Pi [.
    //! See Also
    //! GCE2d_MakeCircle which provides functions for
    //! more complex circle constructions
    //! gp_Ax22d and  gp_Circ2d for an equivalent, non-parameterized data structure.
    public class Geom2d_Circle : Geom2d_Conic
    {
        public Geom2d_Circle(gp_Circ2d C)
        {
            radius = (C.Radius());
            pos = C.Axis();
        }
        public gp_Circ2d Circ2d()
        {
            return new gp_Circ2d(pos, radius);
        }

        double radius;

    }
}
