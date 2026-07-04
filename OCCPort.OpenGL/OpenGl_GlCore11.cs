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

        internal void glCopyPixels(int v1, int v2, int v3, int v4, All color)
        {
            GL.CopyPixels(v1, v2, v3, v4, (PixelCopyType)color);
        }

        internal void glLoadMatrixf(Matrix4 matrix4)
        {
            GL.LoadMatrix(ref matrix4);
        }
        internal void glLoadMatrixf(float[] matrix4)
        {
            GL.LoadMatrix( matrix4);
        }

        internal void glMatrixMode(All projection)
        {
            GL.MatrixMode((MatrixMode)projection);
        }

        internal void glPixelTransferi(All mapColor, int v)
        {
            GL.PixelTransfer((PixelTransferParameter)mapColor, v);
        }        

        internal void glRasterPos2i(int v1, int v2)
        {
            GL.RasterPos2(v1, v2);
        }

        internal void glShadeModel(int aModel)
        {
            GL.ShadeModel((ShadingModel)aModel);
        }
    }
}