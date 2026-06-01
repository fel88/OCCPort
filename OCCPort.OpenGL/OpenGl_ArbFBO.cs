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
        public void glBindFramebuffer(All framebuffer, uint v)
        {
            GL.BindFramebuffer((FramebufferTarget)framebuffer, v);
        }
        internal void glGenFramebuffers(int v, ref uint myGlFBufferId)
        {
            if (v == 1)
            {
                myGlFBufferId = (uint)GL.GenFramebuffer();
            }
            //GL.GenFramebuffers(v,)
        }
    }
}