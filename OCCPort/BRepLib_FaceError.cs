namespace OCCPort
{
    //! Errors that can occur at face construction.
    //! no error
    //! not initialised
    enum BRepLib_FaceError
    {
        BRepLib_FaceDone,
        BRepLib_NoFace,
        BRepLib_NotPlanar,
        BRepLib_CurveProjectionFailed,
        BRepLib_ParametersOutOfRange
    };
}
