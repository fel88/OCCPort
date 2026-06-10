namespace TKV3d
{
    //! The mode specifying how multiple active Selection Modes should be treated during activation of new one.
    public enum AIS_SelectionModesConcurrency
    {
        AIS_SelectionModesConcurrency_Single,        //!< only one selection mode can be activated at the same moment - previously activated should be deactivated
        AIS_SelectionModesConcurrency_GlobalOrLocal, //!< either Global (AIS_InteractiveObject::GlobalSelectionMode() or Local (multiple) selection modes can be active at the same moment
        AIS_SelectionModesConcurrency_Multiple,      //!< any combination of selection modes can be activated
    };
}

