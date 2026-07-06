using OpenTK.Graphics.OpenGL;
using System;

namespace OCCPort.OpenGL
{
    public  interface IOpenGl_GlCore32
    {

        void glBindVertexArray(uint myDefaultVao);

        void glGenVertexArrays(int v, uint[] myDefaultVao);
    }
}