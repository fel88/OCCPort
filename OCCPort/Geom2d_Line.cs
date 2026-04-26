namespace OCCPort
{
    internal class Geom2d_Line : Geom2d_Curve

    {


        public Geom2d_Line(gp_Lin2d L)
        {
            pos = (L.Position());
        }

        public override Geom2d_Geometry Copy()
        {
            Geom2d_Line L;
            L = new Geom2d_Line(pos);
            return L;
        }
        public gp_Lin2d Lin2d() { return new gp_Lin2d(pos); }

        //! Computes the parameter on the reversed line for the
        //! point of parameter U on this line.
        //! For a line, the returned value is -U.
        public override double ReversedParameter(double U)
        {
            return (-U);
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
        public Geom2d_Line(gp_Ax2d A) { pos = (A); }

        public override double LastParameter()
        { return Precision.Infinite(); }

        public override void Reverse()
        {
            pos.Reverse();
        }

        public override bool IsPeriodic()
        {
            return false;
        }

        public override void D1(double U, out gp_Pnt2d P, out gp_Vec2d V1)
        {
            P = new gp_Pnt2d();
            V1 = new gp_Vec2d ();
            ElCLib.LineD1(U, pos, ref P, ref V1);

        }
    }
}