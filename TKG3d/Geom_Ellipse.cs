using TKMath;

namespace TKG3d
{
    //! Describes an ellipse in 3D space.
    //! An ellipse is defined by its major and minor radii and,
    //! as with any conic curve, is positioned in space with a
    //! right-handed coordinate system (gp_Ax2 object) where:
    //! - the origin is the center of the ellipse,
    //! - the "X Direction" defines the major axis, and
    //! - the "Y Direction" defines the minor axis.
    //! The origin, "X Direction" and "Y Direction" of this
    //! coordinate system define the plane of the ellipse. The
    //! coordinate system is the local coordinate system of the ellipse.
    //! The "main Direction" of this coordinate system is the
    //! vector normal to the plane of the ellipse. The axis, of
    //! which the origin and unit vector are respectively the
    //! origin and "main Direction" of the local coordinate
    //! system, is termed the "Axis" or "main Axis" of the ellipse.
    //! The "main Direction" of the local coordinate system
    //! gives an explicit orientation to the ellipse (definition of
    //! the trigonometric sense), determining the direction in
    //! which the parameter increases along the ellipse.
    //! The Geom_Ellipse ellipse is parameterized by an angle:
    //! P(U) = O + MajorRad*Cos(U)*XDir + MinorRad*Sin(U)*YDir
    //! where:
    //! - P is the point of parameter U,
    //! - O, XDir and YDir are respectively the origin, "X
    //! Direction" and "Y Direction" of its local coordinate system,
    //! - MajorRad and MinorRad are the major and minor radii of the ellipse.
    //! The "X Axis" of the local coordinate system therefore
    //! defines the origin of the parameter of the ellipse.
    //! An ellipse is a closed and periodic curve. The period
    //! is 2.*Pi and the parameter range is [ 0, 2.*Pi [.
    public class Geom_Ellipse : Geom_Conic
    {
        public override void D1(double U, out gp_Pnt P, out gp_Vec V1)
        {
            throw new NotImplementedException();
        }

        public override double Eccentricity()
        {
            throw new NotImplementedException();
        }

        public override bool IsClosed()
        {
            throw new NotImplementedException();
        }
    }
}
