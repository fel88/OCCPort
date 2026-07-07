namespace OCCPort
{

    //! Filter for rendering elements.
 public   enum OpenGl_RenderFilter
    {
        OpenGl_RenderFilter_Empty = 0x000, //!< disabled filter

        OpenGl_RenderFilter_OpaqueOnly = 0x001, //!< render only opaque elements and any non-filling elements   (conflicts with OpenGl_RenderFilter_TransparentOnly)
        OpenGl_RenderFilter_TransparentOnly = 0x002, //!< render only semitransparent elements and OpenGl_AspectFace (conflicts with OpenGl_RenderFilter_OpaqueOnly)

        OpenGl_RenderFilter_NonRaytraceableOnly = 0x004, //!< render only non-raytraceable elements
        OpenGl_RenderFilter_FillModeOnly = 0x008, //!< render only filled elements

        OpenGl_RenderFilter_SkipTrsfPersistence = 0x010, //!< render only normal 3D objects without transformation persistence
    };

}