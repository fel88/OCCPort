namespace OCCPort
{
    internal class gp_Lin2d
    {
        private gp_Pnt2d gp_Pnt2d;
        private gp_Dir2d gp_Dir2d;

        public gp_Lin2d(gp_Pnt2d gp_Pnt2d, gp_Dir2d gp_Dir2d)
        {
            this.gp_Pnt2d = gp_Pnt2d;
            this.gp_Dir2d = gp_Dir2d;
        }
    }
}