namespace OCCPort
{
    //! Rebuilds a Shape by making pre-defined substitutions on some
    //! of its components
    //!
    //! In a first phase, it records requests to replace or remove
    //! some individual shapes
    //! For each shape, the last given request is recorded
    //! Requests may be applied "Oriented" (i.e. only to an item with
    //! the SAME orientation) or not (the orientation of replacing
    //! shape is respectful of that of the original one)
    //!
    //! Then, these requests may be applied to any shape which may
    //! contain one or more of these individual shapes
    //!
    //! Supports the 'BRepTools_History' history by method 'History'.
    public class BRepTools_ReShape 
    {
    }
}