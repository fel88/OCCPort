namespace OCCPort
{
    //! Type of the shader object.
    public enum Graphic3d_TypeOfShaderObject
    {
        // rendering shaders
        Graphic3d_TOS_VERTEX = 0x01, //!< vertex shader object, mandatory
        Graphic3d_TOS_TESS_CONTROL = 0x02, //!< tessellation control shader object, optional
        Graphic3d_TOS_TESS_EVALUATION = 0x04, //!< tessellation evaluation shader object, optional
        Graphic3d_TOS_GEOMETRY = 0x08, //!< geometry shader object, optional
        Graphic3d_TOS_FRAGMENT = 0x10, //!< fragment shader object, mandatory
                                       // general-purpose compute shader
        Graphic3d_TOS_COMPUTE = 0x20  //!< compute shader object, should be used as alternative to shader object types for rendering
    }

}
