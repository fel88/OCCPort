using OCCPort.OpenGL;
using System;

namespace OCCPort
{
    //! Class implements FrameBuffer Object (FBO) resource
    //! intended for off-screen rendering.
    public class OpenGl_FrameBuffer : OpenGl_NamedResource
    {
        //! Number of multisampling samples.
        public int NbSamples() { return myNbSamples; }

        internal void BindBuffer(OpenGl_Context aCtx)
        {
            throw new NotImplementedException();
        }

        internal void BindDrawBuffer(OpenGl_Context aCtx)
        {
            throw new NotImplementedException();
        }

        internal void UnbindBuffer(OpenGl_Context aCtx)
        {
            throw new NotImplementedException();
        }

        internal Graphic3d_Vec2i GetVPSize()
        {
            throw new NotImplementedException();
        }

        internal void InitLazy(OpenGl_Context aCtx, Graphic3d_Vec2i aSizeXY, int myFboColorFormat, int myFboDepthFormat, int v)
        {
            throw new NotImplementedException();
        }

        int myInitVPSizeX;         //!< viewport width  specified during initialization (kept even on failure)
        int myInitVPSizeY;         //!< viewport height specified during initialization (kept even on failure)
        int myVPSizeX;             //!< viewport width  (should be <= texture width)
        int myVPSizeY;             //!< viewport height (should be <= texture height)
        int myNbSamples;           //!< number of MSAA samples
        OpenGl_ColorFormats myColorFormats;        //!< sized format for color         texture, GL_RGBA8 by default
        int myDepthFormat;         //!< sized format for depth-stencil texture, GL_DEPTH24_STENCIL8 by default
        uint myGlFBufferId;         //!< FBO object ID
        uint myGlColorRBufferId;    //!< color         Render Buffer object (alternative to myColorTexture)
        uint myGlDepthRBufferId;    //!< depth-stencil Render Buffer object (alternative to myDepthStencilTexture)
        bool myIsOwnBuffer;         //!< flag indicating that FBO should be deallocated by this class
        bool myIsOwnColor;          //!< flag indicating that color textures should be deallocated by this class
        bool myIsOwnDepth;          //!< flag indicating that depth texture  should be deallocated by this class
        OpenGl_TextureArray myColorTextures;       //!< color texture objects
        OpenGl_Texture myDepthStencilTexture; //!< depth-stencil texture object
    }
}