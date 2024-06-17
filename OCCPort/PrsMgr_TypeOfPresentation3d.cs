namespace OCCPort
{
    //! The type of presentation.
    public enum PrsMgr_TypeOfPresentation3d
    {
        //! Presentation display involves no recalculation for new projectors (points of view) in hidden line removal mode.
        PrsMgr_TOP_AllView,
        //! Every new point of view entails recalculation of the display in hidden line removal mode.
        PrsMgr_TOP_ProjectorDependent
    };


}