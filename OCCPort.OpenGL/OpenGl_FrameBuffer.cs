using OCCPort;
using OCCPort.Common;
using OCCPort.OpenGL;
using OpenTK.Graphics.OpenGL;
using System;
using System.Linq;
using System.Reflection.Metadata;
using TKernel;
using TKService;

namespace OCCPort
{
    //! Class implements FrameBuffer Object (FBO) resource
    //! intended for off-screen rendering.
    public class OpenGl_FrameBuffer : OpenGl_NamedResource
    {
        //! Number of multisampling samples.
        public int NbSamples() { return myNbSamples; }

        //! Return true if FBO has been created with color attachment.
        public bool HasColor() { return !myColorFormats.IsEmpty(); }

        //! Return true if FBO has been created with depth attachment.
        public bool HasDepth() { return myDepthFormat != 0; }

        internal void BindBuffer(OpenGl_Context theGlCtx)
        {
            theGlCtx.arbFBO.glBindFramebuffer(All.Framebuffer, myGlFBufferId);
            theGlCtx.SetFrameBufferSRGB(true);
        }
        //! Returns TRUE if color Render Buffer is defined.
        public bool IsColorRenderBuffer() { return myGlColorRBufferId != NO_RENDERBUFFER; }

        public void SetupViewport(OpenGl_Context theGlCtx)
        {
            int[] aViewport = new int[4] { 0, 0, myVPSizeX, myVPSizeY };
            theGlCtx.ResizeViewport(aViewport);
        }

        internal void BindDrawBuffer(OpenGl_Context theGlCtx)
        {
            theGlCtx.arbFBO.glBindFramebuffer(All.DrawFramebuffer, myGlFBufferId);
            theGlCtx.SetFrameBufferSRGB(true);
        }

        internal void UnbindBuffer(OpenGl_Context theGlCtx)
        {
            if (theGlCtx.DefaultFrameBuffer() != null && theGlCtx.DefaultFrameBuffer() != this)
            {
                theGlCtx.DefaultFrameBuffer().BindBuffer(theGlCtx);
            }
            else
            {
                theGlCtx.arbFBO.glBindFramebuffer(All.Framebuffer, NO_FRAMEBUFFER);
                theGlCtx.SetFrameBufferSRGB(false);
            }
        }



        internal void InitLazy(OpenGl_Context aCtx, Graphic3d_Vec2i aSizeXY, int myFboColorFormat, int myFboDepthFormat, int v)
        {
            throw new NotImplementedException();
        }

        public bool IsValid()
        {
            return isValidFrameBuffer();
        }


        //! Helpful constants
        public const int NO_FRAMEBUFFER = 0;
        public const int NO_RENDERBUFFER = 0;

        //! Return viewport width x height.
        public Graphic3d_Vec2i GetVPSize() { return new Graphic3d_Vec2i(myVPSizeX, myVPSizeY); }

        //! Viewport width.
        public int GetVPSizeX() { return myVPSizeX; }

        //! Viewport height.
        public int GetVPSizeY() { return myVPSizeY; }

        //! Return viewport width x height.
        public Graphic3d_Vec2i GetInitVPSize()
        {
            return new Graphic3d_Vec2i(myInitVPSizeX, myInitVPSizeY);
        }

        //! Viewport width.
        public int GetInitVPSizeX() { return myInitVPSizeX; }

        //! Viewport height.
        public int GetInitVPSizeY() { return myInitVPSizeY; }
        bool isValidFrameBuffer()
        {
            return myGlFBufferId != NO_FRAMEBUFFER;
        }
        public bool Init(OpenGl_Context theGlContext,
                                           Graphic3d_Vec2i theSize,
                                           int theColorFormat,
                                           int theDepthFormat,
                                           int theNbSamples)
        {
            OpenGl_ColorFormats aColorFormats = new OpenGl_ColorFormats();
            if (theColorFormat != 0)
            {
                aColorFormats.Add(theColorFormat);
            }
            return Init(theGlContext, theSize, aColorFormats, theDepthFormat, theNbSamples);
        }


        //! Return TRUE if GL_DEPTH_STENCIL_ATTACHMENT can be used.
        static bool hasDepthStencilAttach(OpenGl_Context theCtx)
        {
            //# ifdef __EMSCRIPTEN__
            //            // supported since WebGL 2.0,
            //            // while WebGL 1.0 + GL_WEBGL_depth_texture needs GL_DEPTH_STENCIL_ATTACHMENT
            //            // and NOT separate GL_DEPTH_ATTACHMENT+GL_STENCIL_ATTACHMENT calls which is different to OpenGL ES 2.0 + extension
            //            return theCtx->IsGlGreaterEqual(3, 0) || theCtx->extPDS;
            //#else
            // supported since OpenGL ES 3.0,
            // while OpenGL ES 2.0 + GL_EXT_packed_depth_stencil needs separate GL_DEPTH_ATTACHMENT+GL_STENCIL_ATTACHMENT calls
            //
            // available on desktop since OpenGL 3.0
            // or OpenGL 2.0 + GL_ARB_framebuffer_object (GL_EXT_framebuffer_object is unsupported by OCCT)
            return theCtx.GraphicsLibrary() != Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGLES
                || theCtx.IsGlGreaterEqual(3, 0);
            //#endif
        }

        //! (Re-)initialize FBO with specified dimensions.
        //! The Render Buffer Objects will be used for Color, Depth and Stencil attachments (as opposite to textures).
        //! @param theGlCtx        currently bound OpenGL context
        //! @param theSize         render buffer width x height
        //! @param theColorFormats list of color render buffer sized format, e.g. GL_RGBA8; list should define only one element
        //! @param theDepthFormat  depth-stencil render buffer sized format, e.g. GL_DEPTH24_STENCIL8
        //! @param theNbSamples    MSAA number of samples (0 means normal render buffer)
        public bool InitRenderBuffer(OpenGl_Context theGlCtx,
                                      Graphic3d_Vec2i theSize,
                                      OpenGl_ColorFormats theColorFormats,
                                      int theDepthFormat,
                                      int theNbSamples = 0)
        {
            return initRenderBuffer(theGlCtx, theSize, theColorFormats, theDepthFormat, theNbSamples, 0);
        }

        //! (Re-)initialize FBO with specified dimensions.
        //! The Render Buffer Objects will be used for Color, Depth and Stencil attachments (as opposite to textures).
        //! @param theGlCtx        currently bound OpenGL context
        //! @param theSize         render buffer width x height
        //! @param theColorFormats list of color render buffer sized format, e.g. GL_RGBA8
        //! @param theDepthFormat  depth-stencil render buffer sized format, e.g. GL_DEPTH24_STENCIL8
        //! @param theNbSamples    MSAA number of samples (0 means normal render buffer)
        //! @param theColorRBufferFromWindow when specified - should be ID of already initialized RB object, which will be released within this class
        bool initRenderBuffer(OpenGl_Context theGlCtx,
                                                       Graphic3d_Vec2i theSize,
                                                       OpenGl_ColorFormats theColorFormats,
                                                       int theDepthFormat,
                                                       int theNbSamples,
                                                       uint theColorRBufferFromWindow)
        {
            myColorFormats = theColorFormats;
            if (!myColorTextures.IsEmpty())
            {
                OpenGl_Texture aTexutre = myColorTextures.First();
                for (OpenGl_TextureArray.Iterator aTextureIt = new NCollection_Vector<OpenGl_Texture>.Iterator(myColorTextures); aTextureIt.More(); aTextureIt.Next())
                {
                    aTextureIt.Value().Release(theGlCtx);
                }
                myColorTextures.Clear();
                myColorTextures.Append(aTexutre);
            }

            myDepthFormat = theDepthFormat;
            myNbSamples = theNbSamples;
            myInitVPSizeX = theSize.x();
            myInitVPSizeY = theSize.y();
            if (theGlCtx.arbFBO == null)
            {
                return false;
            }

            // clean up previous state
            Release(theGlCtx);
            if (theNbSamples > theGlCtx.MaxMsaaSamples()
             || theNbSamples < 0)
            {
                //theGlCtx.PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH,
                //   TCollection_AsciiString("Error: FBO creation failed - MSAA") + theNbSamples
                //    + " render buffer exceeds samples limit: " + theGlCtx->MaxMsaaSamples() + ").");
                return false;
            }

            myIsOwnColor = true;
            myIsOwnBuffer = true;
            myIsOwnDepth = true;

            // setup viewport sizes as is
            myVPSizeX = theSize.x();
            myVPSizeY = theSize.y();
            int aSizeX = theSize.x() > 0 ? theSize.x() : 2;
            int aSizeY = theSize.y() > 0 ? theSize.y() : 2;

            // Create the render-buffers
            if (theColorRBufferFromWindow != NO_RENDERBUFFER)
            {
                myGlColorRBufferId = theColorRBufferFromWindow;
            }
            else if (!theColorFormats.IsEmpty())
            {
                if (theColorFormats.Size() != 1)
                {
                    throw new Standard_NotImplemented("Multiple color attachments as FBO render buffers are not implemented");
                }

                GLint aColorFormat = theColorFormats.First();
                OpenGl_TextureFormat aFormat = OpenGl_TextureFormat.FindSizedFormat(theGlCtx, aColorFormat);
                if (!aFormat.IsValid())
                {
                    Release(theGlCtx);
                    return false;
                }

                theGlCtx.arbFBO.glGenRenderbuffers(1, ref myGlColorRBufferId);
                theGlCtx.arbFBO.glBindRenderbuffer(All.Renderbuffer, myGlColorRBufferId);
                if (theNbSamples != 0)
                {
                    theGlCtx.Functions().glRenderbufferStorageMultisample(All.Renderbuffer, theNbSamples, aFormat.InternalFormat(), aSizeX, aSizeY);
                }
                else
                {
                    theGlCtx.arbFBO.glRenderbufferStorage(All.Renderbuffer, aFormat.InternalFormat(), aSizeX, aSizeY);
                }

                theGlCtx.arbFBO.glBindRenderbuffer(All.Renderbuffer, NO_RENDERBUFFER);

                var aRendImgErr = theGlCtx.core11fwd.glGetError();
                if (aRendImgErr != ErrorCode.NoError)
                {
                    /*     theGlCtx.PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH,
                                                TCollection_AsciiString("Error: FBO color render buffer ") + aSizeX + "x" + aSizeY + "@" + theNbSamples
                                                + " IF: " + OpenGl_TextureFormat::FormatFormat(aFormat.InternalFormat())
                                                + " can not be created with error " + OpenGl_Context::FormatGlError(aRendImgErr) + ".");*/
                    Release(theGlCtx);
                    return false;
                }
            }

            bool hasStencilRB = false;
            if (myDepthFormat != 0)
            {
                OpenGl_TextureFormat aDepthFormat = OpenGl_TextureFormat.FindSizedFormat(theGlCtx, myDepthFormat);
                hasStencilRB = aDepthFormat.PixelFormat() == (int)All.DepthStencil;

                theGlCtx.arbFBO.glGenRenderbuffers(1, ref myGlDepthRBufferId);
                theGlCtx.arbFBO.glBindRenderbuffer(All.Renderbuffer, myGlDepthRBufferId);
                if (theNbSamples != 0)
                {
                    theGlCtx.Functions().glRenderbufferStorageMultisample(All.Renderbuffer, theNbSamples, myDepthFormat, aSizeX, aSizeY);
                }
                else
                {
                    theGlCtx.arbFBO.glRenderbufferStorage(All.Renderbuffer, myDepthFormat, aSizeX, aSizeY);
                }
                theGlCtx.arbFBO.glBindRenderbuffer(All.Renderbuffer, NO_RENDERBUFFER);

                var aRendImgErr = theGlCtx.core11fwd.glGetError();
                if (aRendImgErr != ErrorCode.NoError)
                {
                    /*theGlCtx.PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH,
                                           ("Error: FBO depth render buffer ") + aSizeX + "x" + aSizeY + "@" + theNbSamples
                                           + " IF: " + OpenGl_TextureFormat::FormatFormat(myDepthFormat)
                                           + " can not be created with error " + OpenGl_Context::FormatGlError(aRendImgErr) + ".");*/
                    Release(theGlCtx);
                    return false;
                }
            }

            // create FBO
            theGlCtx.arbFBO.glGenFramebuffers(1, ref myGlFBufferId);
            theGlCtx.arbFBO.glBindFramebuffer(All.Framebuffer, myGlFBufferId);
            if (myGlColorRBufferId != NO_RENDERBUFFER)
            {
                theGlCtx.arbFBO.glFramebufferRenderbuffer(All.Framebuffer, All.ColorAttachment0,
                                                             All.Renderbuffer, myGlColorRBufferId);
            }
            if (myGlDepthRBufferId != NO_RENDERBUFFER)
            {
                if (hasDepthStencilAttach(theGlCtx) && hasStencilRB)
                {
                    theGlCtx.arbFBO.glFramebufferRenderbuffer(All.Framebuffer, All.DepthStencilAttachment,
                                                                 All.Renderbuffer, myGlDepthRBufferId);
                }
                else
                {
                    theGlCtx.arbFBO.glFramebufferRenderbuffer(All.Framebuffer, All.DepthAttachment,
                                                                 All.Renderbuffer, myGlDepthRBufferId);
                    if (hasStencilRB)
                    {
                        theGlCtx.arbFBO.glFramebufferRenderbuffer(All.Framebuffer, All.StencilAttachment,
                                                                     All.Renderbuffer, myGlDepthRBufferId);
                    }
                }
            }
            if (theGlCtx.arbFBO.glCheckFramebufferStatus(All.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                UnbindBuffer(theGlCtx);
                Release(theGlCtx);
                return false;
            }

            UnbindBuffer(theGlCtx);
            return true;
        }

        //! Initialize FBO for rendering into single/multiple color buffer and depth textures.
        //! @param theGlCtx        currently bound OpenGL context
        //! @param theSize         texture width x height
        //! @param theColorFormats list of color texture sized format (0 means no color attachment), e.g. GL_RGBA8
        //! @param theDepthFormat  depth-stencil texture sized format (0 means no depth attachment), e.g. GL_DEPTH24_STENCIL8
        //! @param theNbSamples    MSAA number of samples (0 means normal texture)
        //! @return true on success
        public bool Init(OpenGl_Context theGlContext,
                                                   Graphic3d_Vec2i theSize,
                                                   OpenGl_ColorFormats theColorFormats,
                                                   int theDepthFormat,
                                                   int theNbSamples = 0)
        {

            myColorFormats = theColorFormats;

            OpenGl_TextureArray aTextures = new NCollection_Vector<OpenGl_Texture>(myColorTextures);
            if (!myColorTextures.IsEmpty())
            {
                for (OpenGl_TextureArray.Iterator aTextureIt = new NCollection_Vector<OpenGl_Texture>.Iterator(myColorTextures); aTextureIt.More(); aTextureIt.Next())
                {
                    aTextureIt.Value().Release(theGlContext);
                }
                myColorTextures.Clear();
            }
            for (int aLength = 0; aLength < myColorFormats.Length(); ++aLength)
            {
                myColorTextures.Append(aLength < aTextures.Length()
                                      ? aTextures.Value(aLength)
                                      : new OpenGl_Texture(myResourceId + ":color" + aLength));
            }

            myDepthFormat = theDepthFormat;
            myNbSamples = theNbSamples;
            myInitVPSizeX = theSize.x();
            myInitVPSizeY = theSize.y();
            if (theGlContext.arbFBO == null)
                return false;


            // clean up previous state
            Release(theGlContext);
            if (myColorFormats.IsEmpty()
             && myDepthFormat == 0)
                return false;


            if (theNbSamples != 0
            && !theGlContext.HasTextureMultisampling()
             && theGlContext.MaxMsaaSamples() > 1)
            {
                return InitRenderBuffer(theGlContext, theSize, theColorFormats, theDepthFormat, theNbSamples);
            }

            myIsOwnColor = true;
            myIsOwnBuffer = true;
            myIsOwnDepth = true;

            // setup viewport sizes as is
            myVPSizeX = theSize.x();
            myVPSizeY = theSize.y();
            int aSizeX = theSize.x() > 0 ? theSize.x() : 2;
            int aSizeY = theSize.y() > 0 ? theSize.y() : 2;
            bool hasStencilRB = false;

            // Create the textures (will be used as color buffer and depth-stencil buffer)
            if (theNbSamples != 0)
            {
                for (int aColorBufferIdx = 0; aColorBufferIdx < myColorTextures.Length(); ++aColorBufferIdx)
                {
                    OpenGl_Texture aColorTexture = myColorTextures[aColorBufferIdx];
                    GLint aColorFormat = myColorFormats[aColorBufferIdx];
                    if (aColorFormat == 0
                    || !aColorTexture.Init2DMultisample(theGlContext, theNbSamples,
                                                          aColorFormat, aSizeX, aSizeY))
                    {
                        Release(theGlContext);
                        return false;
                    }
                }
                if (myDepthFormat != 0
    && !myDepthStencilTexture.Init2DMultisample(theGlContext, theNbSamples, myDepthFormat, aSizeX, aSizeY))
                {
                    Release(theGlContext);
                    return false;
                }
            }
            else
            {
                for (int aColorBufferIdx = 0; aColorBufferIdx < myColorTextures.Length(); ++aColorBufferIdx)
                {
                    OpenGl_Texture aColorTexture = myColorTextures[aColorBufferIdx];
                    GLint aColorFormat = myColorFormats[aColorBufferIdx];
                    OpenGl_TextureFormat aFormat = OpenGl_TextureFormat.FindSizedFormat(theGlContext, aColorFormat);
                    if (!aFormat.IsValid()
                     || !aColorTexture.Init(theGlContext, aFormat, new Graphic3d_Vec2i(aSizeX, aSizeY), Graphic3d_TypeOfTexture.Graphic3d_TypeOfTexture_2D))
                    {
                        Release(theGlContext);
                        return false;
                    }
                }

                // extensions (GL_OES_packed_depth_stencil, GL_OES_depth_texture) + GL version might be used to determine supported formats
                // instead of just trying to create such texture
                OpenGl_TextureFormat aDepthFormat = OpenGl_TextureFormat.FindSizedFormat(theGlContext, myDepthFormat);
                if (aDepthFormat.IsValid()
                && !myDepthStencilTexture.Init(theGlContext, aDepthFormat, new Graphic3d_Vec2i(aSizeX, aSizeY), Graphic3d_TypeOfTexture.Graphic3d_TypeOfTexture_2D))
                {
                    //theGlContext.PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_PORTABILITY, 0, GL_DEBUG_SEVERITY_HIGH,
                    //   "Warning! Depth textures are not supported by hardware!");

                    hasStencilRB = aDepthFormat.PixelFormat() == (int)All.DepthStencil
                                && theGlContext.extPDS;
                    var aDepthStencilFormat = hasStencilRB
                                              ? All.Depth24Stencil8
                                              : All.DepthComponent16;

                    theGlContext.arbFBO.glGenRenderbuffers(1, ref myGlDepthRBufferId);
                    theGlContext.arbFBO.glBindRenderbuffer(All.Renderbuffer, myGlDepthRBufferId);
                    theGlContext.arbFBO.glRenderbufferStorage(All.Renderbuffer, aDepthStencilFormat, aSizeX, aSizeY);
                    theGlContext.arbFBO.glBindRenderbuffer(All.Renderbuffer, NO_RENDERBUFFER);

                    var aRendImgErr = theGlContext.core11fwd.glGetError();
                    if (aRendImgErr != (int)All.NoError)
                    {
                        //theGlContext.PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH,
                        //        ("Error: FBO depth render buffer ") + aSizeX + "x" + aSizeY
                        //     + " IF: " + OpenGl_TextureFormat::FormatFormat(aDepthStencilFormat)
                        //   + " can not be created with error " + OpenGl_Context::FormatGlError(aRendImgErr) + ".");
                        Release(theGlContext);
                        return false;
                    }
                }

            }

            // Build FBO and setup it as texture
            theGlContext.arbFBO.glGenFramebuffers(1, ref myGlFBufferId);
            theGlContext.arbFBO.glBindFramebuffer(OpenTK.Graphics.OpenGL.All.Framebuffer, myGlFBufferId);

            for (int aColorBufferIdx = 0; aColorBufferIdx < myColorTextures.Length(); ++aColorBufferIdx)
            {
                OpenGl_Texture aColorTexture = myColorTextures[aColorBufferIdx];
                if (aColorTexture.IsValid())
                {
                    theGlContext.arbFBO.glFramebufferTexture2D(All.Framebuffer, All.ColorAttachment0 + aColorBufferIdx,
                                                                  aColorTexture.GetTarget(), aColorTexture.TextureId(), 0);
                }
            }
            if (myDepthStencilTexture.IsValid())
            {
                if (hasDepthStencilAttach(theGlContext))
                {
                    theGlContext.arbFBO.glFramebufferTexture2D(All.Framebuffer, All.DepthStencilAttachment,
                                                                  myDepthStencilTexture.GetTarget(), myDepthStencilTexture.TextureId(), 0);
                }
                else
                {
                    theGlContext.arbFBO.glFramebufferTexture2D(All.Framebuffer, All.DepthAttachment,
                                                                  myDepthStencilTexture.GetTarget(), myDepthStencilTexture.TextureId(), 0);
                    theGlContext.arbFBO.glFramebufferTexture2D(All.Framebuffer, All.StencilAttachment,
                                                                  myDepthStencilTexture.GetTarget(), myDepthStencilTexture.TextureId(), 0);
                }
            }
            if (theGlContext.arbFBO.glCheckFramebufferStatus(All.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                Release(theGlContext);
                return false;
            }

            UnbindBuffer(theGlContext);
            return true;

        }

        private void Release(OpenGl_Context theGlCtx)
        {
            if (isValidFrameBuffer())
            {
                // application can not handle this case by exception - this is bug in code
                Exceptions.Standard_ASSERT_RETURN(!myIsOwnBuffer || theGlCtx != null,
                         "OpenGl_FrameBuffer destroyed without GL context! Possible GPU memory leakage...");
                if (myIsOwnBuffer
                 && theGlCtx.IsValid())
                {
                    theGlCtx.arbFBO.glDeleteFramebuffers(1, [myGlFBufferId]);
                    if (myGlColorRBufferId != NO_RENDERBUFFER)
                    {
                        theGlCtx.arbFBO.glDeleteRenderbuffers(1, [myGlColorRBufferId]);
                    }
                    if (myGlDepthRBufferId != NO_RENDERBUFFER)
                    {
                        theGlCtx.arbFBO.glDeleteRenderbuffers(1, [myGlDepthRBufferId]);
                    }
                }
                myGlFBufferId = NO_FRAMEBUFFER;
                myGlColorRBufferId = NO_RENDERBUFFER;
                myGlDepthRBufferId = NO_RENDERBUFFER;
                myIsOwnBuffer = false;
            }

            if (myIsOwnColor)
            {
                for (int aColorBufferIdx = 0; aColorBufferIdx < myColorTextures.Length(); ++aColorBufferIdx)
                {
                    myColorTextures[aColorBufferIdx].Release(theGlCtx);
                }
                myIsOwnColor = false;
            }

            if (myIsOwnDepth)
            {
                myDepthStencilTexture.Release(theGlCtx);
                myIsOwnDepth = false;
            }

            myVPSizeX = 0;
            myVPSizeY = 0;
        }

        public void ChangeViewport(int theVPSizeX,
                                         int theVPSizeY)
        {
            myVPSizeX = theVPSizeX;
            myVPSizeY = theVPSizeY;
        }

        internal void BindReadBuffer(OpenGl_Context theGlCtx)
        {
            theGlCtx.arbFBO.glBindFramebuffer(All.ReadFramebuffer, myGlFBufferId);

        }

        int myInitVPSizeX;         //!< viewport width  specified during initialization (kept even on failure)
        int myInitVPSizeY;         //!< viewport height specified during initialization (kept even on failure)
        int myVPSizeX;             //!< viewport width  (should be <= texture width)
        int myVPSizeY;             //!< viewport height (should be <= texture height)
        int myNbSamples;           //!< number of MSAA samples
        OpenGl_ColorFormats myColorFormats = new NCollection_Vector<int>();        //!< sized format for color         texture, GL_RGBA8 by default
        int myDepthFormat;         //!< sized format for depth-stencil texture, GL_DEPTH24_STENCIL8 by default
        uint myGlFBufferId;         //!< FBO object ID
        uint myGlColorRBufferId;    //!< color         Render Buffer object (alternative to myColorTexture)
        uint myGlDepthRBufferId;    //!< depth-stencil Render Buffer object (alternative to myDepthStencilTexture)
        bool myIsOwnBuffer;         //!< flag indicating that FBO should be deallocated by this class
        bool myIsOwnColor;          //!< flag indicating that color textures should be deallocated by this class
        bool myIsOwnDepth;          //!< flag indicating that depth texture  should be deallocated by this class
        OpenGl_TextureArray myColorTextures = new NCollection_Vector<OpenGl_Texture>();       //!< color texture objects
        OpenGl_Texture myDepthStencilTexture; //!< depth-stencil texture object

        public OpenGl_FrameBuffer(string theResourceId = null) : base(theResourceId)
        {
            myInitVPSizeX = (0);
            myInitVPSizeY = (0);
            myVPSizeX = (0);
            myVPSizeY = (0);
            myNbSamples = (0);
            myDepthFormat = (int)(All.Depth24Stencil8);
            myGlFBufferId = (NO_FRAMEBUFFER);
            myGlColorRBufferId = (NO_RENDERBUFFER);
            myGlDepthRBufferId = (NO_RENDERBUFFER);
            myIsOwnBuffer = (false);
            myIsOwnColor = (false);
            myIsOwnDepth = (false);
            myDepthStencilTexture = (new OpenGl_Texture(theResourceId + ":depth_stencil"));

            myColorFormats.Append((int)All.Rgba8);
            myColorTextures.Append(new OpenGl_Texture(theResourceId + ":color"));

        }
    }
}