using OpenTK.Graphics.OpenGL;
using System;

namespace OCCPort.OpenGL
{
    public class _core15fwd
    {
        internal void glActiveTexture(All texture0)
        {
            GL.ActiveTexture((TextureUnit)texture0);
        }

        internal void glBindBuffer(BufferTarget bufferTarget, uint myBufferId)
        {
            GL.BindBuffer(bufferTarget, myBufferId);
        }

        internal void glBufferData<T>(BufferTarget target, int size, T[] data, BufferUsageHint usage) where T : struct
        {
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
        }

        internal void glGenBuffer(ref int myBufferId)
        {
            myBufferId = GL.GenBuffer();
        }
    }
}