namespace OCCPort
{

    //! Describes a torus.
    //! A torus is defined by its major and minor radii, and
    //! positioned in space with a coordinate system (a
    //! gp_Ax3 object) as follows:
    //! - The origin is the center of the torus.
    //! - The surface is obtained by rotating a circle around
    //! the "main Direction". This circle has a radius equal
    //! to the minor radius, and is located in the plane
    //! defined by the origin, "X Direction" and "main
    //! Direction". It is centered on the "X Axis", on its
    //! positive side, and positioned at a distance from the
    //! origin equal to the major radius. This circle is the
    //! "reference circle" of the torus.
    //! - The plane defined by the origin, the "X Direction"
    //! and the "Y Direction" is called the "reference plane" of the torus.
    //! This coordinate system is the "local coordinate
    //! system" of the torus. The following apply:
    //! - Rotation around its "main Axis", in the trigonometric
    //! sense given by "X Direction" and "Y Direction",
    //! defines the u parametric direction.
    //! - The "X Axis" gives the origin for the u parameter.
    //! - Rotation around an axis parallel to the "Y Axis" and
    //! passing through the center of the "reference circle"
    //! gives the v parameter on the "reference circle".
    //! - The "X Axis" gives the origin of the v parameter on
    //! the "reference circle".
    //! - The v parametric direction is oriented by the
    //! inverse of the "main Direction", i.e. near 0, as v
    //! increases, the Z coordinate decreases. (This
    //! implies that the "Y Direction" orients the reference
    //! circle only when the local coordinate system is direct.)
    //! - The u isoparametric curve is a circle obtained by
    //! rotating the "reference circle" of the torus through
    //! an angle u about the "main Axis".
    //! The parametric equation of the torus is :
    //! P(u, v) = O + (R + r*cos(v)) * (cos(u)*XDir +
    //! sin(u)*YDir ) + r*sin(v)*ZDir, where:
    //! - O, XDir, YDir and ZDir are respectively the
    //! origin, the "X Direction", the "Y Direction" and the "Z
    //! Direction" of the local coordinate system,
    //! - r and R are, respectively, the minor and major radius.
    //! The parametric range of the two parameters is:
    //! - [ 0, 2.*Pi ] for u
    //! - [ 0, 2.*Pi ] for v
    public class Geom_ToroidalSurface : Geom_ElementarySurface
    {
        public override void Bounds(out double U1, out double U2, out double V1, out double V2)
        {
            throw new System.NotImplementedException();
        }
        public double MajorRadius()
        {

            return majorRadius;
        }


        double  majorRadius;
        double  minorRadius;
        //=======================================================================

        //function : MinorRadius
        //purpose  : 
        //=======================================================================

        public double MinorRadius()
        {

            return minorRadius;
        }

        public override Geom_Geometry Copy()
        {
            throw new System.NotImplementedException();
        }

        public override bool IsUPeriodic()
        {
            throw new System.NotImplementedException();
        }

        public override bool IsVPeriodic()
        {
            throw new System.NotImplementedException();
        }

        public override void Transform(gp_Trsf t)
        {
            throw new System.NotImplementedException();
        }

        public override void D1(double U, double V, out gp_Pnt P, out gp_Vec D1U, out gp_Vec D1V)
        {
            throw new System.NotImplementedException();
        }

        public override void D0(double U, double V, ref gp_Pnt P)
        {
            throw new System.NotImplementedException();
        }

        public override void D2(double U, double V, out gp_Pnt P, out gp_Vec D1U, out gp_Vec D1V, out gp_Vec D2U, out gp_Vec D2V, out gp_Vec D2UV)
        {
            throw new System.NotImplementedException();
        }
    }
}