using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;


namespace OCCPort.OpenGL
{
    //! OpenGL 2.0 core based on 1.5 version.
    public class OpenGl_GlCore20 : OpenGl_GlCore15
    {
        public int glGetUniformLocation(int myProgramID, string theName)
        {
           return GL.GetUniformLocation(myProgramID, theName);
        }

        internal void glClear(ClearBufferMask value)
        {
            GL.Clear(value);
        }

        internal void glClearDepth(double v)
        {
            GL.ClearDepth(v);
        }

        internal void glDepthFunc(DepthFunction always)
        {
            GL.DepthFunc(always);
        }

        internal void glDepthMask(bool v)
        {
            GL.DepthMask(v);
        }

        internal void glUniform1i(int theLocation, Graphic3d_TextureUnit theTextureUnit)
        {
            GL.Uniform1(theLocation, (int)theTextureUnit);
        }

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