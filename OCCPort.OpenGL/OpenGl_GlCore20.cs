using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;


namespace OCCPort.OpenGL
{
    //! OpenGL 2.0 core based on 1.5 version.
    public class OpenGl_GlCore20 : OpenGl_GlCore15
    {
        internal void glUniform4fv(int location, int v2, Vector4 theValue)
        {            
            GL.Uniform4(location, v2, theValue.ToFloatArray());
        }

        internal void glUseProgram(int v)
        {
            GL.UseProgram(v);
        }
    }
}