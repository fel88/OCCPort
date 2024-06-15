namespace OCCPort
{
    public enum OpenGl_RenderFilter
    {

        //Filter for rendering elements.

        OpenGl_RenderFilter_Empty,

        //disabled filter
        OpenGl_RenderFilter_OpaqueOnly,

        //render only opaque elements and any non-filling elements (conflicts with OpenGl_RenderFilter_TransparentOnly)
        OpenGl_RenderFilter_TransparentOnly,

        //render only semitransparent elements and OpenGl_AspectFace(conflicts with OpenGl_RenderFilter_OpaqueOnly)
        OpenGl_RenderFilter_NonRaytraceableOnly,

        //render only non-raytraceable elements
        OpenGl_RenderFilter_FillModeOnly,

        //render only filled elements
        OpenGl_RenderFilter_SkipTrsfPersistence,

        //render only normal 3D objects without transformation persistence
    }
}