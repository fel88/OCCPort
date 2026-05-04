namespace OCCPort.Enums
{
    //! Errors that can occur at wire construction.
    //! no error
    enum BRepLib_WireError
    {
        BRepLib_WireDone,
        BRepLib_EmptyWire,
        BRepLib_DisconnectedWire,
        BRepLib_NonManifoldWire
    };
}
