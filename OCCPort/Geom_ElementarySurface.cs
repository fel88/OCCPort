namespace OCCPort
{
    internal class Geom_ElementarySurface: Geom_Surface
    {
      protected  gp_Ax3 pos;

        public override void Bounds(ref double U1, ref double U2, ref double V1, ref double V2)
        {
            throw new System.NotImplementedException();
        }
    }
}