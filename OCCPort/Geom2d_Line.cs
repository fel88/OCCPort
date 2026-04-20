namespace OCCPort
{
    internal class Geom2d_Line : Geom2d_Curve

    {
        private gp_Lin2d l;

        public Geom2d_Line(gp_Lin2d l)
        {
            this.l = l;
        }

        public override void D0(double U, ref gp_Pnt2d P)
        {
            P = ElCLib.LineValue(U, pos);
        }

        public override double FirstParameter()
        { return -Precision.Infinite(); }

        //=======================================================================
        //function : LastParameter
        //purpose  : 
        //=======================================================================
        gp_Ax2d pos;

        public override double LastParameter()
        { return Precision.Infinite(); }

    }
}