using OpenTK.Graphics.OpenGL;
using System;

namespace OCCPort.OpenGL
{
    public class _core15fwd
    {
        internal void glBufferData<T>(BufferTarget target, int size, T[] data, BufferUsageHint usage) where T : struct
        {
            GL.BufferData(target, size, data, usage);
        }
    }
}