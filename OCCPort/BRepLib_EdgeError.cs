namespace OCCPort
{
    //! Errors that can occur at edge construction.
    //! no error
    enum BRepLib_EdgeError
    {
        BRepLib_EdgeDone,
        BRepLib_PointProjectionFailed,
        BRepLib_ParameterOutOfRange,
        BRepLib_DifferentPointsOnClosedCurve,
        BRepLib_PointWithInfiniteParameter,
        BRepLib_DifferentsPointAndParameter,
        BRepLib_LineThroughIdenticPoints
    };
}