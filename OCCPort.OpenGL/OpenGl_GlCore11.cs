using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;

namespace OCCPort.OpenGL
{
    public class OpenGl_GlCore11
    {
        internal void glLoadMatrixf(Matrix4 matrix4)
        {
            GL.LoadMatrix(ref matrix4);
        }

        internal void glMatrixMode(All projection)
        {
            GL.MatrixMode((MatrixMode)projection);
        }
    }
}