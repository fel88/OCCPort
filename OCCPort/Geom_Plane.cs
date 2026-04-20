namespace OCCPort
{
    internal class Geom_Plane : Geom_ElementarySurface
    {


        public Geom_Plane(gp_Pln Pl)
        {
            pos = Pl.Position();

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