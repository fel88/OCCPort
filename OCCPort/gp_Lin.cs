using System;

namespace OCCPort
{
    //! Describes a line in 3D space.
    //! A line is positioned in space with an axis (a gp_Ax1
    //! object) which gives it an origin and a unit vector.
    //! A line and an axis are similar objects, thus, we can
    //! convert one into the other. A line provides direct access
    //! to the majority of the edit and query functions available
    //! on its positioning axis. In addition, however, a line has
    //! specific functions for computing distances and positions.
    //! See Also
    //! gce_MakeLin which provides functions for more complex
    //! line constructions
    //! Geom_Line which provides additional functions for
    //! constructing lines and works, in particular, with the
    //! parametric equations of lines

    public class gp_Lin
    {

        gp_Ax1 pos;
        //! Returns the direction of the line.
        public gp_Dir Direction() { return pos.Direction(); }

        //! Returns the location point (origin) of the line.
        public gp_Pnt Location() { return pos.Location(); }


        //! Returns the axis placement one axis with the same
        //! location and direction as <me>.
        public gp_Ax1 Position() { return pos; }


        //! Creates a line passing through point theP and parallel to
        //! vector theV (theP and theV are, respectively, the origin and
        //! the unit vector of the positioning axis of the line).
        public gp_Lin(gp_Pnt theP, gp_Dir theV)

        {
            pos = new gp_Ax1(theP, theV);
        }

        public gp_Lin(gp_Ax1 value)
        {
            pos = value;
        }
    }
}