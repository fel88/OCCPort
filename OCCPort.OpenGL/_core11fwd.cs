using OpenTK.Graphics.OpenGL;
using System;

namespace OCCPort.OpenGL
{
    public class _core11fwd
    {
        internal void glClearDepth(double v)
        {
            GL.ClearDepth(v);
        }

        internal void glDepthFunc(All lequal)
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

        internal void glGetBooleanv(GetPName n, ref bool v)
        {
            v = GL.GetBoolean(n);
        }

        internal void glGetIntegerv(GetPName drawBuffer, ref int aDrawBuffer)
        {            
            GL.GetInteger(drawBuffer, out aDrawBuffer);
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