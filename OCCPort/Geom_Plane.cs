using System.Numerics;

namespace OCCPort
{
    internal class Geom_Plane : Geom_ElementarySurface
    {


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

        public override void Bounds(ref double U1, ref double U2, ref double V1, ref double V2)
        {
            U1 = -Precision.Infinite();
            U2 = Precision.Infinite();
            V1 = -Precision.Infinite();
            V2 = Precision.Infinite();
        }
    }
}