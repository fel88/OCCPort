using TKService;


namespace OCCPort.OpenGL
{
    //! Provide Sampler Object functionality (texture parameters stored independently from texture itself).
    //! Available since OpenGL 3.3+ (GL_ARB_sampler_objects extension) and OpenGL ES 3.0+.
    public interface IOpenGl_ArbSamplerObject
    {
        void glBindSampler(Graphic3d_TextureUnit theUnit, uint mySamplerID);
        void glDeleteSamplers(int v, uint[] mySamplerID);
        void glGenSampler(ref uint mySamplerID);
        void glGenSamplers(int v,  uint[] mySamplerID);
    }

}