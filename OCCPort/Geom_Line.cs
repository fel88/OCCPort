namespace OCCPort
{
    //! Describes an infinite line.
    //! A line is defined and positioned in space with an axis
    //! (gp_Ax1 object) which gives it an origin and a unit vector.
    //! The Geom_Line line is parameterized:
    //! P (U) = O + U*Dir, where:
    //! - P is the point of parameter U,
    //! - O is the origin and Dir the unit vector of its positioning axis.
    //! The parameter range is ] -infinite, +infinite [.
    //! The orientation of the line is given by the unit vector
    //! of its positioning axis.

    public class Geom_Line : Geom_Curve
    {

        gp_Ax1 pos;

        public Geom_Line(gp_Lin L)
        {
            pos = L.Position();
        }
    }
}