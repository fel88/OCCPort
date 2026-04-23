namespace OCCPort
{
    //! different state of a Selection in a ViewerSelector...
    public enum SelectMgr_StateOfSelection
    {
        SelectMgr_SOS_Any = -2, //!< ANY selection state (for querying selections)
        SelectMgr_SOS_Unknown = -1, //!< selection, which has  never been in SelectMgr_SOS_Activated state (almost the same thing as SelectMgr_SOS_Deactivated)
        SelectMgr_SOS_Deactivated = 0, //!< deactivated selection, once been in SelectMgr_SOS_Activated state
        SelectMgr_SOS_Activated,        //!< activated selection
    };
}