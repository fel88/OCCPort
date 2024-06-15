namespace OCCPort
{
    //! Class implements FrameBuffer Object (FBO) resource
    //! intended for off-screen rendering.
    public class OpenGl_FrameBuffer : OpenGl_NamedResource
    {
        //! Number of multisampling samples.
        public int NbSamples() { return myNbSamples; }

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