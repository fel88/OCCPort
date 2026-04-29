using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;

namespace OCCPort.OpenGL
{
    public class OpenGl_GlCore11
    {
        internal void glColor4fv(float[] v)
        {
            GL.Color4(v);
        }

        internal void glColor4fv(Vector4 theColor)
        {
            GL.Color4([theColor.X, theColor.Y, theColor.Z, theColor.W]);
        }

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