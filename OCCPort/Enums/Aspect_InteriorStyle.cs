namespace OCCPort.Enums
{
    //! Interior types for primitive faces.
    public enum Aspect_InteriorStyle
    {
        Aspect_IS_EMPTY = -1, //!< no interior
        Aspect_IS_SOLID = 0, //!< normally filled surface interior
        Aspect_IS_HATCH,      //!< hatched surface interior
        Aspect_IS_HIDDENLINE, //!< interior is filled with viewer background color
        Aspect_IS_POINT,      //!< display only vertices of surface (obsolete)

        // obsolete aliases
        Aspect_IS_HOLLOW = Aspect_IS_EMPTY, //!< transparent surface interior
    };

}
