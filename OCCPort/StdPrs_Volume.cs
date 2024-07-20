namespace OCCPort.Tester
{
    //! defines the way how to interpret input shapes
    //! Volume_Autodetection to perform Autodetection (would split input shape into two groups)
    //! Volume_Closed as Closed volumes (to activate back-face culling and capping plane algorithms)
    //! Volume_Opened as Open volumes (shells or solids with holes)
    public enum StdPrs_Volume
    {
        StdPrs_Volume_Autodetection,
        StdPrs_Volume_Closed,
        StdPrs_Volume_Opened
    }
}