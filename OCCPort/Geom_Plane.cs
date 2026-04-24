using System.Numerics;

namespace OCCPort
{
    internal class Geom_Plane : Geom_ElementarySurface
    {

        public gp_Pln Pln()
        {

            return new gp_Pln(Position());
        }

        public Geom_Plane(gp_Pln Pl)
        {
            pos = Pl.Position();

        }
        //! Applies the transformation T to this plane.
        public override void Transform(gp_Trsf T)
        {
            pos.Transform(T);
        }

        //! Creates a new object which is a copy of this plane.
        public override Geom_Geometry Copy()
        {
            Geom_Plane Pl = new Geom_Plane(pos);
            return Pl;
        }
        public Geom_Plane(gp_Ax3 A3)
        {

            pos = A3;
        }

        public override void Bounds(out double U1, out double U2, out double V1, out double V2)
        {
            U1 = -Precision.Infinite();
            U2 = Precision.Infinite();
            V1 = -Precision.Infinite();
            V2 = Precision.Infinite();
        }

        public override bool IsUPeriodic()
        {
            return true;
        }

        public override bool IsVPeriodic()
        {
            return false;
        }
    }
}