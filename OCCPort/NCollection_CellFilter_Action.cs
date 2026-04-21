namespace OCCPort
{
    //! Auxiliary enumeration serving as responce from method Inspect
    public enum NCollection_CellFilter_Action
    {
        CellFilter_Keep = 0, //!< Target is needed and should be kept
        CellFilter_Purge = 1  //!< Target is not needed and can be removed from the current cell
    };
}