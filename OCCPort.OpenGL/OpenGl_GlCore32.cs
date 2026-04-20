using OpenTK.Graphics.OpenGL;
using System;

namespace OCCPort.OpenGL
{
    internal class OpenGl_GlCore32
    {
        internal void glBindVertexArray(uint myDefaultVao)
        {
            GL.BindVertexArray(myDefaultVao);
        }
    }
}