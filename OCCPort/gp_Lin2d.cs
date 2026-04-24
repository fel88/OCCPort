namespace OCCPort
{
    public class gp_Lin2d
    {


        //! Creates a line located with theA.
        public gp_Lin2d(gp_Ax2d theA)

        { pos = theA; }

        //! Returns the direction of the line.
        public gp_Dir2d Direction() { return pos.Direction(); }

        //! Returns the location point (origin) of the line.
        public gp_Pnt2d Location() { return pos.Location(); }


        gp_Ax2d pos;

        //! <theP> is the location point (origin) of the line and
        //! <theV> is the direction of the line.
        public gp_Lin2d(gp_Pnt2d theP, gp_Dir2d theV)
        {
            pos = new gp_Ax2d(theP, theV);
        }
    }
}