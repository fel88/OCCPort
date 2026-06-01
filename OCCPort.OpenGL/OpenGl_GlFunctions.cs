using OpenTK.Graphics.OpenGL;
using System;

namespace OCCPort.OpenGL
{
    //! Mega structure defines the complete list of OpenGL functions.
    public class OpenGl_GlFunctions
    {
        internal static void readGlVersion(ref int myGlVerMajor, ref int myGlVerMinor)
        {
            
        }
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

        public void load(OpenGl_Context theCtx,
                                  bool theIsCoreProfile)
        {
            // load GL_ARB_framebuffer_object (added to OpenGL 3.0 core)

            bool hasFBO = true;// (isGlGreaterEqualShort(3, 0) || checkExtensionShort("GL_ARB_framebuffer_object"))
            //     && FindProcShort(glIsRenderbuffer)
            //     && FindProcShort(glBindRenderbuffer)
            //     && FindProcShort(glDeleteRenderbuffers)
            //     && FindProcShort(glGenRenderbuffers)
            //     && FindProcShort(glRenderbufferStorage)
            //     && FindProcShort(glGetRenderbufferParameteriv)
            //     && FindProcShort(glIsFramebuffer)
            //     && FindProcShort(glBindFramebuffer)
            //     && FindProcShort(glDeleteFramebuffers)
            //     && FindProcShort(glGenFramebuffers)
            //     && FindProcShort(glCheckFramebufferStatus)
            //     && FindProcShort(glFramebufferTexture1D)
            //     && FindProcShort(glFramebufferTexture2D)
            //     && FindProcShort(glFramebufferTexture3D)
            //     && FindProcShort(glFramebufferRenderbuffer)
            //     && FindProcShort(glGetFramebufferAttachmentParameteriv)
            //     && FindProcShort(glGenerateMipmap)
            //     && FindProcShort(glBlitFramebuffer)
            //     && FindProcShort(glRenderbufferStorageMultisample)
            //     && FindProcShort(glFramebufferTextureLayer);

            // initialize FBO extension (ARB)
            if (hasFBO)
            {
                theCtx.arbFBO = this;//theCtx.arbFBO = (OpenGl_ArbFBO)this;
                theCtx.arbFBOBlit = this;//theCtx.arbFBOBlit = (OpenGl_ArbFBOBlit)this;
                //theCtx.extPDS = true; // extension for EXT, but part of ARB
            }
        }

    }
}