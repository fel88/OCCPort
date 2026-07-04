namespace TKService
{
    //! Definition of all the type of light source.
    public enum Graphic3d_TypeOfLightSource
    {
        Graphic3d_TypeOfLightSource_Ambient,     //!< ambient light
        Graphic3d_TypeOfLightSource_Directional, //!< directional light
        Graphic3d_TypeOfLightSource_Positional,  //!< positional light
        Graphic3d_TypeOfLightSource_Spot,        //!< spot light

        // obsolete aliases
        Graphic3d_TOLS_AMBIENT = Graphic3d_TypeOfLightSource_Ambient,
        Graphic3d_TOLS_DIRECTIONAL = Graphic3d_TypeOfLightSource_Directional,
        Graphic3d_TOLS_POSITIONAL = Graphic3d_TypeOfLightSource_Positional,
        Graphic3d_TOLS_SPOT = Graphic3d_TypeOfLightSource_Spot,
        //
        V3d_AMBIENT = Graphic3d_TypeOfLightSource_Ambient,
        V3d_DIRECTIONAL = Graphic3d_TypeOfLightSource_Directional,
        V3d_POSITIONAL = Graphic3d_TypeOfLightSource_Positional,
        V3d_SPOT = Graphic3d_TypeOfLightSource_Spot,


        //! Auxiliary value defining the overall number of values in enumeration Graphic3d_TypeOfLightSource
        Graphic3d_TypeOfLightSource_NB = Graphic3d_TypeOfLightSource_Spot + 1
    };

}
