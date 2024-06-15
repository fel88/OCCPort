namespace OCCPort
{
    public enum OpenGl_LayerFilter
    {

        //Tool object to specify processed OpenGL layers for intermixed rendering of raytracable and non-raytracable layers.

        OpenGl_LF_All,

        //process all layers
        OpenGl_LF_Upper,

        //process only top non-raytracable layers
        OpenGl_LF_Bottom,

        //process only Graphic3d_ZLayerId_BotOSD
        OpenGl_LF_RayTracable

        //process only normal raytracable layers (save the bottom layer)
    }
}