namespace OCCPort
{
    //! Modes of display of back faces in the view.
    public enum Graphic3d_TypeOfBackfacingModel
    {
        Graphic3d_TypeOfBackfacingModel_Auto,        //!< automatic back face culling enabled for opaque groups with closed flag
                                                     //!  (e.g. solids, see Graphic3d_Group::IsClosed())
        Graphic3d_TypeOfBackfacingModel_DoubleSided, //!< no culling (double-sided shading)
        Graphic3d_TypeOfBackfacingModel_BackCulled,  //!< back  face culling
        Graphic3d_TypeOfBackfacingModel_FrontCulled, //!< front face culling
                                                     // old aliases
        Graphic3d_TOBM_AUTOMATIC = Graphic3d_TypeOfBackfacingModel_Auto,
        Graphic3d_TOBM_FORCE = Graphic3d_TypeOfBackfacingModel_DoubleSided,
        Graphic3d_TOBM_DISABLE = Graphic3d_TypeOfBackfacingModel_BackCulled,
        V3d_TOBM_AUTOMATIC = Graphic3d_TypeOfBackfacingModel_Auto,
        V3d_TOBM_ALWAYS_DISPLAYED = Graphic3d_TypeOfBackfacingModel_DoubleSided,
        V3d_TOBM_NEVER_DISPLAYED = Graphic3d_TypeOfBackfacingModel_BackCulled
    };

}