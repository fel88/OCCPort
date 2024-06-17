namespace OCCPort
{
    internal class Geom2d_Line : Geom2d_Curve

    {
        private gp_Lin2d l;

        public Geom2d_Line(gp_Lin2d l)
        {
            this.l = l;
        }


        public override double FirstParameter()
        { return -Precision.Infinite(); }

        //=======================================================================
        //function : LastParameter
        //purpose  : 
        //=======================================================================

        public override double LastParameter()
        { return Precision.Infinite(); }

    }
}