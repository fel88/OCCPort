using OpenTK.Graphics.OpenGL;
using System;

namespace OCCPort.OpenGL
{
    public class _core11fwd
    {
        internal void glBindTexture(uint myTarget, uint myTextureId)
        {
            GL.BindTexture((TextureTarget)myTarget, myTextureId);
        }

        internal void glClear(uint toClear)
        {
            GL.Clear((ClearBufferMask)toClear);
        }
        internal void glClear(All toClear)
        {
            GL.Clear((ClearBufferMask)toClear);
        }

        internal void glClearColor(float v1, float v2, float v3, float v4)
        {
            GL.ClearColor(v1, v2, v3, v4);
        }

        internal void glClearDepth(double v)
        {
            GL.ClearDepth(v);
        }

        internal void glColorMask(bool v1, bool v2, bool v3, bool v4)
        {
            GL.ColorMask(v1, v2, v3, v4);
        }

        internal void glDeleteTextures(int v, uint[] value)
        {
            GL.DeleteTextures(v, value);
        }

        internal void glDepthFunc(All lequal)
        {
            GL.DepthFunc((DepthFunction)lequal);
        }
        internal void glDepthFunc(int lequal)
        {
            GL.DepthFunc((DepthFunction)lequal);
        }

        internal void glDepthMask(bool t)
        {
            GL.DepthMask(t);
        }

        internal void glDisable(int v)
        {
            GL.Disable((EnableCap)v);
        }
        internal void glDisable(EnableCap v)
        {
            GL.Disable(v);
        }
        internal void glDisable(All v)
        {
            GL.Disable((EnableCap)v);
        }


        internal void glDrawArrays(int aDrawMode, int aFirstElem, int aNbElemsInGroup)
        {
            glDrawArrays((PrimitiveType)aDrawMode, aFirstElem, aNbElemsInGroup);
        }
        internal void glDrawArrays(PrimitiveType aDrawMode, int aFirstElem, int aNbElemsInGroup)
        {
            GL.DrawArrays(aDrawMode, aFirstElem, aNbElemsInGroup);
        }

        internal void glDrawBuffer(int aDrawBuffer)
        {
            GL.DrawBuffer((DrawBufferMode)aDrawBuffer);
        }

        internal void glDrawElements(int aDrawMode, int v, All all, int  anOffset)
        {
            GL.DrawElements((PrimitiveType)aDrawMode, v, (DrawElementsType)all, anOffset);
        }

        internal void glEnable(All v)
        {
            GL.Enable((EnableCap)v);
        }
        internal void glEnable(EnableCap v)
        {
            GL.Enable(v);
        }
        internal void glEnable(int v)
        {
            GL.Enable((EnableCap)v);
        }

        internal void glFlush()
        {
            GL.Flush();
        }

        internal void glGenTextures(int v, ref uint myTextureId)
        {
            GL.GenTextures(v, out myTextureId);
        }

        internal void glGetBooleanv(GetPName n, ref bool v)
        {
            v = GL.GetBoolean(n);
        }
        internal void glGetBooleanv(All n, ref bool v)
        {
            v = GL.GetBoolean((GetPName)n);
        }

        internal ErrorCode  glGetError()
        {
           return  GL.GetError();
        }

        internal void glGetIntegerv(All drawBuffer, ref int aDrawBuffer)
        {
            GL.GetInteger((GetPName)drawBuffer, out aDrawBuffer);
        }

        internal void glGetIntegerv(All drawBuffer,  int []aDrawBuffer)
        {
            GL.GetInteger((GetPName)drawBuffer,  aDrawBuffer);
        }
        internal void glGetIntegerv(GetPName drawBuffer, ref int aDrawBuffer)
        {            
            GL.GetInteger(drawBuffer, out aDrawBuffer);
        }

        internal string glGetString(All extensions)
        {
            return GL.GetString((StringName)extensions);
        }

        public bool glIsEnabled(All depthClamp)
        {
         return    GL.IsEnabled((EnableCap)depthClamp);
        }

        internal void glReadBuffer(int myReadBuffer)
        {
            GL.ReadBuffer((ReadBufferMode)myReadBuffer);
        }

        internal void glViewport(int x, int y, int w, int h)
        {
            GL.Viewport(x, y, w, h);
        }

        internal void glTexSubImage2D(All texture2D, int v1, int v2, int v3, int v4, int v5, int v6, int v7, byte[] aDataPtr)
        {
            GL.TexSubImage2D((TextureTarget)texture2D, v1, v2, v3, v4, v5, (PixelFormat)v6, (PixelType)v7, aDataPtr);
        }

        internal void glTexImage2D(All texture2D, int v1, int anIntFormat, int v2, int v3, int v4, int v5, int v6, byte[] aDataPtr)
        {
            GL.TexImage2D((TextureTarget)texture2D, v1,(PixelInternalFormat) anIntFormat, v2, v3, v4, (PixelFormat)v5, (PixelType)v6, aDataPtr);
        }

        internal void glGetTexLevelParameteriv(All proxyTexture2D, int v, All textureWidth, ref int aTestWidth)
        {
            GL.GetTexLevelParameter((TextureTarget)proxyTexture2D, v, (GetTextureParameter)textureWidth, out aTestWidth);
        }

        internal void glPolygonOffset(float factor, float units)
        {
            GL.PolygonOffset(factor, units);
        }

        internal void glCullFace(TriangleFace front)
        {
            GL.CullFace(front);
        }
    }
}