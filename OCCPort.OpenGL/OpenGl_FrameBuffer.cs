using OCCPort;
using OCCPort.OpenGL;
using OpenTK.Graphics.OpenGL;
using System;
using System.Linq;
using System.Reflection.Metadata;
using TKernel;

namespace OCCPort
{
    //! Class implements FrameBuffer Object (FBO) resource
    //! intended for off-screen rendering.
    public class OpenGl_FrameBuffer : OpenGl_NamedResource
    {
        //! Number of multisampling samples.
        public int NbSamples() { return myNbSamples; }

        internal void BindBuffer(OpenGl_Context theGlCtx)
        {
            theGlCtx.arbFBO.glBindFramebuffer(All.Framebuffer, myGlFBufferId);
            theGlCtx.SetFrameBufferSRGB(true);
        }
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

        public bool Init(OpenGl_Context theGlContext,
                                           Graphic3d_Vec2i theSize,
                                           OpenGl_ColorFormats theColorFormats,
                                           int theDepthFormat,
                                           int theNbSamples)
        {

            /*
             more code 
            */
            // Build FBO and setup it as texture
            theGlContext.arbFBO.glGenFramebuffers(1, ref myGlFBufferId);
            theGlContext.arbFBO.glBindFramebuffer(OpenTK.Graphics.OpenGL.All.Framebuffer, myGlFBufferId);
            /*
             more code 
            */
            return true;

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