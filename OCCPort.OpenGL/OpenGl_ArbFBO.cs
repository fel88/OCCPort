using OpenTK.Graphics.OpenGL;
using System;

namespace OCCPort.OpenGL
{
    public class OpenGl_ArbFBO
    {
        public void glBindFramebuffer(All framebuffer, int v)
        {
            GL.BindFramebuffer((FramebufferTarget)framebuffer, v);
        }
    }
}