namespace OCCPort.Tester
{
    internal class Geom_Plane: Geom_ElementarySurface
    {
        

        public Geom_Plane(gp_Pln Pl)
        {
            pos = Pl.Position();

        }
    }
}