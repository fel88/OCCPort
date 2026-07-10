namespace OCCPort.OpenGL
{
    public enum OpenGl_FeatureFlag
    {
        OpenGl_FeatureNotAvailable = 0, //!< Feature is not supported by OpenGl implementation.
        OpenGl_FeatureInExtensions = 1, //!< Feature is supported as extension.
        OpenGl_FeatureInCore = 2  //!< Feature is supported as part of core profile.
    };
}