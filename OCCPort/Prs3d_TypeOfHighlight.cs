namespace OCCPort
{
    //! Type of highlighting to apply specific style.
    enum Prs3d_TypeOfHighlight
    {
        Prs3d_TypeOfHighlight_None = 0,       //!< no highlighting
        Prs3d_TypeOfHighlight_Selected,       //!< entire object is selected
        Prs3d_TypeOfHighlight_Dynamic,        //!< entire object is dynamically highlighted
        Prs3d_TypeOfHighlight_LocalSelected,  //!< part of the object is selected
        Prs3d_TypeOfHighlight_LocalDynamic,   //!< part of the object is dynamically highlighted
        Prs3d_TypeOfHighlight_SubIntensity,   //!< sub-intensity style
        Prs3d_TypeOfHighlight_NB
    };



}
