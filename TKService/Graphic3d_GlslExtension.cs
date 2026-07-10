namespace TKService
{
    //  enum
    // {
    //   Graphic3d_TextureUnit_NB = Graphic3d_TextureUnit_15 + 1,
    //  };
    //! GLSL syntax extensions.
    enum Graphic3d_GlslExtension
    {
        Graphic3d_GlslExtension_GL_OES_standard_derivatives, //!< OpenGL ES 2.0 extension GL_OES_standard_derivatives
        Graphic3d_GlslExtension_GL_EXT_shader_texture_lod,   //!< OpenGL ES 2.0 extension GL_EXT_shader_texture_lod
        Graphic3d_GlslExtension_GL_EXT_frag_depth,           //!< OpenGL ES 2.0 extension GL_EXT_frag_depth
        Graphic3d_GlslExtension_GL_EXT_gpu_shader4,          //!< OpenGL 2.0 extension GL_EXT_gpu_shader4

        Graphic3d_GlslExtension_NB = Graphic3d_GlslExtension_GL_EXT_gpu_shader4 + 1
    };
}
