namespace TKG3d
{
    //! Identifies the position of a vertex or a set of
    //! vertices relative to a region of a shape.
    //! The figure shown above illustrates the states of
    //! vertices found in various parts of the edge relative
    //! to the face which it intersects.
    public enum TopAbs_State
    {
        TopAbs_IN,
        TopAbs_OUT,
        TopAbs_ON,
        TopAbs_UNKNOWN
    };
}
