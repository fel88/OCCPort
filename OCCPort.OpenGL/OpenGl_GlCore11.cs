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

        internal void glColorMaterial(TriangleFace frontAndBack, ColorMaterialParameter ambientAndDiffuse)
        {
            GL.ColorMaterial(frontAndBack, ambientAndDiffuse);
        }

        internal void glColorPointer(int theNbComp, int theDataType, int theStride, int theOffset)
        {
            GL.ColorPointer(theNbComp, (ColorPointerType)theDataType, theStride, theOffset);
        }

        internal void glCopyPixels(int v1, int v2, int v3, int v4, All color)
        {
            GL.CopyPixels(v1, v2, v3, v4, (PixelCopyType)color);
        }

        
        internal void glDisableClientState(ArrayCap vertexArray)
        {
            GL.DisableClientState(vertexArray);
        }
        internal void glEnableClientState(ArrayCap vertexArray)
        {
            GL.EnableClientState(vertexArray);
        }
        internal void glEnableClientState(All vertexArray)
        {
            GL.EnableClientState((ArrayCap)vertexArray);
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

        internal void glNormalPointer(int theDataType, int theStride, int theOffset)
        {
            GL.NormalPointer((NormalPointerType)theDataType, theStride, theOffset);
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

        internal void glTexCoordPointer(int theNbComp, int theDataType, int theStride, int theOffset)
        {
            GL.TexCoordPointer(theNbComp, (TexCoordPointerType)theDataType, theStride, theOffset);
        }

        internal void glVertexPointer(int theNbComp, int theDataType, int theStride, int theOffset)
        {
            GL.VertexPointer(theNbComp, (VertexPointerType) theDataType, theStride, theOffset);
        }
    }
}