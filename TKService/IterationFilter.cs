namespace TKService
{
    //! Iteration filter flags.
    public enum IterationFilter
    {
        IterationFilter_None = 0x0000, //!< no filter
        IterationFilter_ExcludeAmbient = 0x0002, //!< exclude ambient  light sources
        IterationFilter_ExcludeDisabled = 0x0004, //!< exclude disabled light sources
        IterationFilter_ExcludeNoShadow = 0x0008, //!< exclude light sources not casting shadow
        IterationFilter_ExcludeDisabledAndAmbient = IterationFilter_ExcludeAmbient | IterationFilter_ExcludeDisabled,
        IterationFilter_ActiveShadowCasters = IterationFilter_ExcludeDisabledAndAmbient | IterationFilter_ExcludeNoShadow,
    };
}
