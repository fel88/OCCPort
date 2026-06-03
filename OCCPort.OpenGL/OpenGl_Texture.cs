using OCCPort.OpenGL;
using OpenTK.Graphics.ES11;
using System.Reflection.Metadata;
using TKService;

namespace OCCPort
{
    public class OpenGl_Texture : OpenGl_NamedResource
    {
        //! Helpful constants
        const uint NO_TEXTURE = 0;
        public OpenGl_Texture(string theResourceId,
                                 Graphic3d_TextureParams theParams = null) : base(theResourceId)
        {
            mySampler = new OpenGl_Sampler(theParams);
            myRevision = (0);
            myTextureId = (NO_TEXTURE);
            myTarget = (uint)All.Texture2D;
            myTextFormat = (uint)All.Rgba;
            mySizedFormat = (int)All.Rgba8;
            myNbSamples = (1);
            myMaxMipLevel = (0);
            myIsAlpha = (false);
            myIsTopDown = true;

            //
        }

        protected OpenGl_Sampler mySampler; //!< texture sampler
        protected int myRevision;   //!< revision of associated data source
        protected uint myTextureId;  //!< GL resource ID
        protected uint myTarget;     //!< GL_TEXTURE_1D/GL_TEXTURE_2D/GL_TEXTURE_3D
        protected Graphic3d_Vec3i mySize;       //!< texture width x height x depth
        protected uint myTextFormat; //!< texture format - GL_RGB, GL_RGBA,...
        protected int mySizedFormat;//!< internal (sized) texture format
        protected int myNbSamples;  //!< number of MSAA samples
        protected int myMaxMipLevel;//!< upper mipmap level index (0 means no mipmaps)
        protected bool myIsAlpha;    //!< indicates alpha format
        protected bool myIsTopDown;  //!< indicates if 2D surface is defined top-down (TRUE) or bottom-up (FALSE)
    }
}