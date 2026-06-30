namespace TKV3d
{
    //! A framework to define a compound filter composed of
    //! two or more simple filters.
    public abstract class SelectMgr_CompositionFilter :
        SelectMgr_Filter
    {
     protected   SelectMgr_ListOfFilter myFilters;

    }
}

