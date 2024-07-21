using OpenTK.Graphics.OpenGL;


namespace OCCPort.OpenGL
{
    //! OpenGL 2.0 core based on 1.5 version.
    public class OpenGl_GlCore20 : OpenGl_GlCore15
    {
        internal void glUseProgram(int v)
        {
            GL.UseProgram(v);
        }
    }
}