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

        internal void glGenVertexArrays(int v, uint [] myDefaultVao)
        {
            GL.GenVertexArrays(v, myDefaultVao);
        }
    }
}