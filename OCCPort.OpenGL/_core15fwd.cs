using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OCCPort.OpenGL
{
    public class _core15fwd
    {
        internal void glActiveTexture(All texture0)
        {
            ILog.Log(System.Reflection.MethodBase.GetCurrentMethod().Name, [texture0]);

            GL.ActiveTexture((TextureUnit)texture0);
        }
        internal void glActiveTexture(int texture0)
        {
            ILog.Log(System.Reflection.MethodBase.GetCurrentMethod().Name, [texture0]);

            GL.ActiveTexture((TextureUnit)texture0);
        }
        internal void glBindBuffer(BufferTarget bufferTarget, uint myBufferId)
        {
            ILog.Log(System.Reflection.MethodBase.GetCurrentMethod().Name, [bufferTarget, myBufferId]);

            GL.BindBuffer(bufferTarget, myBufferId);
        }

        internal void glBufferData<T>(BufferTarget target, int size, T[] data, BufferUsageHint usage) where T : struct
        {
            ILog.Log(System.Reflection.MethodBase.GetCurrentMethod().Name, [target, size,data,usage]);

            GL.BufferData(target, size, data, usage);
        }

        internal void glGenBuffers(int v, ref uint myBufferId)
        {
            if (v == 1)
            {
                int t = 0;
                glGenBuffer(ref t);
                myBufferId = (uint)t;
            }
            else
            {

                //GL.GenBuffers(v, ref)
            }
            ILog.Log(System.Reflection.MethodBase.GetCurrentMethod().Name, [v, myBufferId]);

        }

        internal void glGenBuffer(ref int myBufferId)
        {
            myBufferId = GL.GenBuffer();
            ILog.Log(System.Reflection.MethodBase.GetCurrentMethod().Name, [ myBufferId]);

        }

        internal void glDeleteBuffers(int v,ref uint myBufferId)
        {
            ILog.Log(System.Reflection.MethodBase.GetCurrentMethod().Name, [v, myBufferId]);

            GL.DeleteBuffers(v,ref myBufferId);
        }

        internal int glGetError()
        {
            return (int)GL.GetError();

        }

        internal void glDisable(All cap)
        {
            ILog.Log(System.Reflection.MethodBase.GetCurrentMethod().Name, [ cap]);

            GL.Disable((EnableCap)cap);

        }

        internal void glEnable(All cap)
        {
            ILog.Log(System.Reflection.MethodBase.GetCurrentMethod().Name, [cap]);

            GL.Enable((EnableCap)cap);
        }
    }
}