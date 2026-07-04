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
    }
}