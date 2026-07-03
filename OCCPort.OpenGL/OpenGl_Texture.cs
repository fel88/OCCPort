using OCCPort.Common;
using OCCPort.OpenGL;
using OpenTK.Compute.OpenCL;
using OpenTK.Graphics.ES11;
using System;
using System.Drawing;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using System.Security.Cryptography;
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
        public bool Create(OpenGl_Context theCtx)
        {
            if (myTextureId != NO_TEXTURE)
                return true;


            theCtx.core11fwd.glGenTextures(1, ref myTextureId);
            if (myTextureId == NO_TEXTURE)
                return false;

            //mySampler->Create (theCtx); // do not create sampler object by default
            return true;
        }

        //! Bind this Texture to the unit specified in sampler parameters.
        //! Also binds Sampler Object if it is allocated.
        public void Bind(OpenGl_Context theCtx)
        {
            Bind(theCtx, mySampler.Parameters().TextureUnit());
        }

        public void Bind(OpenGl_Context theCtx,
                             Graphic3d_TextureUnit theTextureUnit)
        {
            if (theCtx.core15fwd != null)
            {
                theCtx.core15fwd.glActiveTexture(OpenTK.Graphics.OpenGL.All.Texture0 + (int)theTextureUnit);
            }
            mySampler.Bind(theCtx, theTextureUnit);
            theCtx.core11fwd.glBindTexture(myTarget, myTextureId);
        }

        //! Initialize the 2D multisampling texture using glTexImage2DMultisample().
        public bool Init2DMultisample(OpenGl_Context theCtx,
                                                  int theNbSamples,
                                                  int theTextFormat,
                                                  int theSizeX,
                                                  int theSizeY)
        {
            if (!Create(theCtx)
             || theNbSamples > theCtx.MaxMsaaSamples()
             || theNbSamples < 1)
            {
                //theCtx.PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH,
                //      TCollection_AsciiString("Error: MSAA texture ") + theSizeX + "x" + theSizeY + "@" + myNbSamples
                //     + " exceeds samples limit: " + theCtx->MaxMsaaSamples() + ".");
                return false;
            }

            myNbSamples = OpenGl_Context.GetPowerOfTwo(theNbSamples, theCtx.MaxMsaaSamples());
            myTarget = (int)All.Texture2DMultisample;
            myMaxMipLevel = 0;
            if (theSizeX > theCtx.MaxTextureSize()
            || theSizeY > theCtx.MaxTextureSize())
            {
                //theCtx.PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH,
                //     TCollection_AsciiString("Error: MSAA texture ") + theSizeX + "x" + theSizeY + "@" + myNbSamples
                //   + " exceeds size limit: " + theCtx->MaxTextureSize() + "x" + theCtx->MaxTextureSize() + ".");
                return false;
            }

            Bind(theCtx);
            //myTextFormat = theTextFormat;
            mySizedFormat = theTextFormat;
            if (theCtx.HasTextureMultisampling()
             && theCtx.Functions().glTexStorage2DMultisample != null)   // OpenGL 4.3
            {
                theCtx.Functions().glTexStorage2DMultisample(myTarget, myNbSamples, theTextFormat, theSizeX, theSizeY, false);
            }
            else if (theCtx.HasTextureMultisampling()
                  && theCtx.Functions().glTexImage2DMultisample != null) // OpenGL 3.2
            {
                theCtx.Functions().glTexImage2DMultisample(myTarget, myNbSamples, theTextFormat, theSizeX, theSizeY, false);
            }
            else
            {
                //theCtx.PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH,
                //      "Error: MSAA textures are not supported by hardware.");
                Unbind(theCtx);
                return false;
            }

            var aTexImgErr = theCtx.core11fwd.glGetError();
            if (aTexImgErr != OpenTK.Graphics.OpenGL.ErrorCode.NoError )
            {
                //theCtx.PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH,
                //  TCollection_AsciiString("Error: MSAA texture ") + theSizeX + "x" + theSizeY + "@" + myNbSamples
                //    + " IF: " + OpenGl_TextureFormat::FormatFormat(theTextFormat)
                //    + " cannot be created with error " + OpenGl_Context::FormatGlError(aTexImgErr) + ".");
                Unbind(theCtx);
                return false;
            }

            mySize.SetValues(theSizeX, theSizeY, 1);

            Unbind(theCtx);
            return true;
        }

        //! @return target to which the texture is bound (GL_TEXTURE_1D, GL_TEXTURE_2D)
        public uint GetTarget() { return myTarget; }


        //! @return true if current object was initialized
        public virtual bool IsValid() { return myTextureId != NO_TEXTURE; }


        //! @return texture ID
        public uint TextureId() { return myTextureId; }

        internal void Release(OpenGl_Context theGlCtx)
        {
            mySampler.Release(theGlCtx);
            if (myTextureId == NO_TEXTURE)
            {
                return;
            }

            // application can not handle this case by exception - this is bug in code
          Exceptions.  Standard_ASSERT_RETURN(theGlCtx != null,
              "OpenGl_Texture destroyed without GL context! Possible GPU memory leakage...");

            if (theGlCtx.IsValid())
            {
                theGlCtx.core11fwd.glDeleteTextures(1, [ myTextureId]);
            }
            myTextureId = NO_TEXTURE;
            mySize.SetValues(0, 0, 0);
        }

        //! Initialize the 2D texture with specified format, size and texture type.
        //! If theImage is empty the texture data will contain trash.
        //! Notice that texture will be unbound after this call.
        internal bool Init(OpenGl_Context theCtx,
            OpenGl_TextureFormat theFormat,
            Graphic3d_Vec2i theSizeXY,
            Graphic3d_TypeOfTexture theType,
            Image_PixMap theImage = null)
        {
            return Init(theCtx, theFormat, new Graphic3d_Vec3i(theSizeXY, 1), theType, theImage);

        }
        public bool Init(OpenGl_Context theCtx,
                             OpenGl_TextureFormat theFormat,
                             Graphic3d_Vec3i theSizeXYZ,
                             Graphic3d_TypeOfTexture theType,
                             Image_PixMap theImage)
        {
            if (theSizeXYZ.x() < 1
             || theSizeXYZ.y() < 1
             || theSizeXYZ.z() < 1)
            {
                //theCtx.PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH,
                //    ("Error: texture of 0 size cannot be created [") + myResourceId +"]");
                Release(theCtx);
                return false;
            }


            Unbind(theCtx);
            return true;
        }

        //! Unbind texture from the unit specified in sampler parameters.
        //! Also unbinds Sampler Object if it is allocated.
        public void Unbind(OpenGl_Context theCtx)
        {
            Unbind(theCtx, mySampler.Parameters().TextureUnit());
        }

        public void Unbind(OpenGl_Context theCtx,
                              Graphic3d_TextureUnit theTextureUnit)
        {
            if (theCtx.core15fwd != null)            
                theCtx.core15fwd.glActiveTexture(OpenTK.Graphics.OpenGL.All.Texture0 + (int)theTextureUnit);
            
            mySampler.Unbind(theCtx, theTextureUnit);
            theCtx.core11fwd.glBindTexture(myTarget, NO_TEXTURE);
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