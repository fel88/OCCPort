using System;

namespace OCCPort
{
    public class gp_Lin2d
    {


        //! Creates a line located with theA.
        public gp_Lin2d(gp_Ax2d theA)

        { pos = theA; }

        //! Returns the axis placement one axis with the same
        //! location and direction as <me>.
        public gp_Ax2d Position() { return pos; }


        //! Returns the direction of the line.
        public gp_Dir2d Direction() { return pos.Direction(); }

        //! Returns the location point (origin) of the line.
        public gp_Pnt2d Location() { return pos.Location(); }

        //! Computes the distance between <me> and the point <theP>.
        internal double Distance(gp_Pnt2d theP)
        {
            gp_XY aCoord = theP.XY();
            aCoord.Subtract((pos.Location()).XY());
            double aVal = aCoord.Crossed(pos.Direction().XY());
            if (aVal < 0)
            {
                aVal = -aVal;
            }
            return aVal;
        }

        gp_Ax2d pos;

        //! <theP> is the location point (origin) of the line and
        //! <theV> is the direction of the line.
        public gp_Lin2d(gp_Pnt2d theP, gp_Dir2d theV)
        {
            pos = new gp_Ax2d(theP, theV);
        }
    }
}