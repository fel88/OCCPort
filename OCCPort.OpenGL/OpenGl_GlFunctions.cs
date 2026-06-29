using OpenTK.Graphics.OpenGL;
using System;

namespace OCCPort.OpenGL
{
    //! Mega structure defines the complete list of OpenGL functions.
    public class OpenGl_GlFunctions
    {
        internal static void readGlVersion(ref int theGlVerMajor, ref int theGlVerMinor)
        {
            // reset values
            theGlVerMajor = 0;
            theGlVerMinor = 0;
            bool toCheckVer3 = true;

            // Available since OpenGL 3.0 and OpenGL ES 3.0.
            if (toCheckVer3)
            {
                int aMajor = 0, aMinor = 0;
                GL.GetInteger(GetPName.MajorVersion, out aMajor);
                GL.GetInteger(GetPName.MinorVersion, out aMinor);

            }
            if (theGlVerMajor <= 0)
            {
                theGlVerMajor = 0;
                theGlVerMinor = 0;
            }
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

        internal void glBlitFramebuffer(GLint srcX0, GLint srcY0, GLint srcX1, GLint srcY1, GLint dstX0, GLint dstY0, GLint dstX1, GLint dstY1, ClearBufferMask mask, BlitFramebufferFilter filter)
        {
            GL.BlitFramebuffer(srcX0, srcY0, srcX1, srcY1, dstX0, dstY0, dstX1, dstY1, mask, filter);
        }
    }
}