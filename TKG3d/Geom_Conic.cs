using TKMath;

namespace TKG3d
{
    //! The abstract class Conic describes the common
    //! behavior of conic curves in 3D space and, in
    //! particular, their general characteristics. The Geom
    //! package provides four concrete classes of conics:
    //! Geom_Circle, Geom_Ellipse, Geom_Hyperbola and Geom_Parabola.
    //! A conic is positioned in space with a right-handed
    //! coordinate system (gp_Ax2 object), where:
    //! - the origin is the center of the conic (or the apex in
    //! the case of a parabola),
    //! - the origin, "X Direction" and "Y Direction" define the
    //! plane of the conic.
    //! This coordinate system is the local coordinate
    //! system of the conic.
    //! The "main Direction" of this coordinate system is the
    //! vector normal to the plane of the conic. The axis, of
    //! which the origin and unit vector are respectively the
    //! origin and "main Direction" of the local coordinate
    //! system, is termed the "Axis" or "main Axis" of the conic.
    //! The "main Direction" of the local coordinate system
    //! gives an explicit orientation to the conic, determining
    //! the direction in which the parameter increases along
    //! the conic. The "X Axis" of the local coordinate system
    //! also defines the origin of the parameter of the conic.
    public class Geom_Conic : Geom_Curve
    {
        public override Geom_Geometry Copy()
        {
            throw new NotImplementedException();
        }

        protected gp_Ax2 pos;

        public override void D0(double U, ref gp_Pnt P)
        {
            throw new NotImplementedException();
        }

        public override double FirstParameter()
        {
            throw new NotImplementedException();
        }

        public override bool IsPeriodic()
        {
            throw new NotImplementedException();
        }

        public override double LastParameter()
        {
            throw new NotImplementedException();
        }

        public override void Reverse()
        {
            throw new NotImplementedException();
        }

        public override double ReversedParameter(double U)
        {
            throw new NotImplementedException();
        }

        public override void Transform(gp_Trsf t)
        {
            throw new NotImplementedException();
        }

        public override GeomAbs_Shape Continuity()
        {
            throw new NotImplementedException();
        }
    }
}
