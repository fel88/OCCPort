using OpenTK.Graphics.OpenGL;
using System;
using TKService;

namespace OCCPort.OpenGL
{
    //! Mega structure defines the complete list of OpenGL functions.
    public class OpenGl_GlFunctions : IOpenGl_ArbSamplerObject
    {
        void IOpenGl_ArbSamplerObject.glBindSampler(Graphic3d_TextureUnit theUnit, uint mySamplerID)
        {
            OpenGl_GlFunctions.glBindSampler(theUnit, mySamplerID);
        }
        public Func<All, int, string> glGetStringi;

        public static void glBindSampler(Graphic3d_TextureUnit theUnit, uint mySamplerID)
        {

            GL.BindSampler((int)theUnit, (int)mySamplerID);
        }

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

                // glGetError() sometimes does not report an error here even if
                // GL does not know GL_MAJOR_VERSION and GL_MINOR_VERSION constants.
                // This happens on some renderers like e.g. Cygwin MESA.
                // Thus checking additionally if GL has put anything to
                // the output variables.
                if (GL.GetError() == ErrorCode.NoError && aMajor != 0 && aMinor != 0)
                {
                    theGlVerMajor = aMajor;
                    theGlVerMinor = aMinor;
                    return;
                }

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

            //theCtx.hasTexRGBA8 = true;
            theCtx.hasTexSRGB = theCtx.IsGlGreaterEqual(2, 1);
            theCtx.hasFboSRGB = theCtx.IsGlGreaterEqual(2, 1);
            theCtx.hasSRGBControl = theCtx.hasFboSRGB;
            theCtx.hasFboRenderMipmap = true;

            theCtx.arbDepthClamp = theCtx.IsGlGreaterEqual(3, 2)
                  || theCtx.CheckExtension("GL_ARB_depth_clamp")
                  || theCtx.CheckExtension("NV_depth_clamp");

            theCtx.hasDrawBuffers = isGlGreaterEqualShort(theCtx, 2, 0) ? OpenGl_FeatureFlag.OpenGl_FeatureInCore :
                                    theCtx.arbDrawBuffers ? OpenGl_FeatureFlag.OpenGl_FeatureInExtensions
                                                          : OpenGl_FeatureFlag.OpenGl_FeatureNotAvailable;

            theCtx.arbDrawBuffers = theCtx.CheckExtension("GL_ARB_draw_buffers");
            theCtx.hasFloatBuffer = theCtx.hasHalfFloatBuffer =
  isGlGreaterEqualShort(theCtx, 3, 0) ? OpenGl_FeatureFlag.OpenGl_FeatureInCore :
  checkExtensionShort(theCtx,"GL_ARB_color_buffer_float") ? OpenGl_FeatureFlag.OpenGl_FeatureInExtensions
                                                    : OpenGl_FeatureFlag.OpenGl_FeatureNotAvailable;

            theCtx.hasGeometryStage = isGlGreaterEqualShort( theCtx, 3, 2)
                                    ? OpenGl_FeatureFlag.OpenGl_FeatureInCore
                                    : OpenGl_FeatureFlag.OpenGl_FeatureNotAvailable;

            theCtx.hasSampleVariables = isGlGreaterEqualShort(theCtx, 4, 0) ?OpenGl_FeatureFlag. OpenGl_FeatureInCore :
                                        theCtx.arbSampleShading ? OpenGl_FeatureFlag.OpenGl_FeatureInExtensions
                                                                : OpenGl_FeatureFlag.OpenGl_FeatureNotAvailable;

            bool hasSamplerObjects = (theCtx.IsGlGreaterEqual(3, 3) || theCtx.CheckExtension("GL_ARB_sampler_objects"))
      //&& FindProcShort(glGenSamplers)
      //&& FindProcShort(glDeleteSamplers)
      //&& FindProcShort(glIsSampler)
      //&& FindProcShort(glBindSampler)
      //&& FindProcShort(glSamplerParameteri)
      //&& FindProcShort(glSamplerParameteriv)
      //&& FindProcShort(glSamplerParameterf)
      //&& FindProcShort(glSamplerParameterfv)
      //&& FindProcShort(glSamplerParameterIiv)
      //&& FindProcShort(glSamplerParameterIuiv)
      //&& FindProcShort(glGetSamplerParameteriv)
      //&& FindProcShort(glGetSamplerParameterIiv)
      //&& FindProcShort(glGetSamplerParameterfv)
      //&& FindProcShort(glGetSamplerParameterIuiv);
      ;
            if (hasSamplerObjects)
            {
                theCtx.arbSamplerObject = this;
            }
        }

        private bool checkExtensionShort(OpenGl_Context ctx, string v)
        {
            return ctx.CheckExtension(v);

        }

        private bool isGlGreaterEqualShort(OpenGl_Context theCtx, int theMaj, int theMin)
        {
            return theCtx.IsGlGreaterEqual(theMaj, theMin);
        }

        internal void glBlitFramebuffer(GLint srcX0, GLint srcY0, GLint srcX1, GLint srcY1, GLint dstX0, GLint dstY0, GLint dstX1, GLint dstY1, ClearBufferMask mask, BlitFramebufferFilter filter)
        {
            GL.BlitFramebuffer(srcX0, srcY0, srcX1, srcY1, dstX0, dstY0, dstX1, dstY1, mask, filter);
        }

        internal FramebufferErrorCode glCheckFramebufferStatus(All framebuffer)
        {
            var ret = GL.CheckFramebufferStatus((FramebufferTarget)framebuffer);
            return ret;
        }

        internal void glFramebufferTexture2D(All target, All attachment, uint textarget, uint texture, int level)
        {
            GL.FramebufferTexture2D((FramebufferTarget)target, (FramebufferAttachment)attachment, (TextureTarget)textarget, texture, level);
        }

        internal void glGenRenderbuffers(int v, ref uint myGlDepthRBufferId)
        {
            if (v == 1)
            {
                GL.GenRenderbuffers(v, out myGlDepthRBufferId);

            }
            throw new NotImplementedException();
            //GL.GenRenderbuffers(v, )
        }

        internal void glBindRenderbuffer(All renderbuffer, uint myGlColorRBufferId)
        {
            GL.BindRenderbuffer((RenderbufferTarget)renderbuffer, myGlColorRBufferId);
        }

        internal void glRenderbufferStorage(All renderbuffer, int aDepthStencilFormat, int aSizeX, int aSizeY)
        {
            GL.RenderbufferStorage((RenderbufferTarget)renderbuffer, (RenderbufferStorage)aDepthStencilFormat, aSizeX, aSizeY);

        }
        internal void glRenderbufferStorage(All renderbuffer, All aDepthStencilFormat, int aSizeX, int aSizeY)
        {
            GL.RenderbufferStorage((RenderbufferTarget)renderbuffer, (RenderbufferStorage)aDepthStencilFormat, aSizeX, aSizeY);
        }

        internal void glRenderbufferStorageMultisample(All renderbuffer, int theNbSamples, int myDepthFormat, int aSizeX, int aSizeY)
        {
            GL.RenderbufferStorageMultisample((RenderbufferTarget)renderbuffer, theNbSamples, (RenderbufferStorage)myDepthFormat, aSizeX, aSizeY);
        }

        internal void glFramebufferRenderbuffer(All framebuffer, All colorAttachment0, All renderbuffer, uint myGlColorRBufferId)
        {
            GL.FramebufferRenderbuffer((FramebufferTarget)framebuffer, (FramebufferAttachment)colorAttachment0, (RenderbufferTarget)renderbuffer, myGlColorRBufferId);
        }

        public void glDeleteSamplers(int v, uint[] mySamplerID)
        {
            GL.DeleteSamplers(v, mySamplerID);
        }

        internal void glDeleteFramebuffers(int v, uint[] value)
        {
            GL.DeleteFramebuffers(v, value);
        }

        internal void glDeleteRenderbuffers(int v, uint[] value)
        {
            GL.DeleteRenderbuffers(v, value);
        }

        public void glGenSamplers(int v, uint[] mySamplerID)
        {
            GL.GenSamplers(v, mySamplerID);
        }

        public void glGenSampler(ref uint mySamplerID)
        {
            mySamplerID = (uint)GL.GenSampler();
        }



        internal Action<uint, int, int, int, int, bool> glTexImage2DMultisample;

        public Action<uint, int, int, int, int, bool> glTexStorage2DMultisample;
    }
}