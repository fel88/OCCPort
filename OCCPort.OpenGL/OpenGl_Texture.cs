using OCCPort.Common;
using OCCPort.OpenGL;
using OpenTK.Compute.OpenCL;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.IO.Compression;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using System.Security.Cryptography;
using TKernel;
using TKService;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OCCPort
{
    public class OpenGl_Texture : OpenGl_NamedResource
    {

        //! Return if 2D surface is defined top-down (TRUE) or bottom-up (FALSE).
        //! Normally set from Image_PixMap::IsTopDown() within texture initialization.
      public   bool IsTopDown()  { return myIsTopDown; }

        //! Return texture sampler.
        public OpenGl_Sampler Sampler() { return mySampler; }


        //! Return upper mipmap level index (0 means no mipmaps).
        public int MaxMipmapLevel() { return myMaxMipLevel; }

        //! Helpful constants
        const uint NO_TEXTURE = 0;
        public OpenGl_Texture(string theResourceId = null,
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
            if (aTexImgErr != OpenTK.Graphics.OpenGL.ErrorCode.NoError)
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
            Exceptions.Standard_ASSERT_RETURN(theGlCtx != null,
                "OpenGl_Texture destroyed without GL context! Possible GPU memory leakage...");

            if (theGlCtx.IsValid())
            {
                theGlCtx.core11fwd.glDeleteTextures(1, [myTextureId]);
            }
            myTextureId = NO_TEXTURE;
            mySize.SetValues(0, 0, 0);
        }

        //! Initialize the texture with Graphic3d_TextureMap.
        //! It is an universal way to initialize.
        //! Suitable initialization method will be chosen.
        public bool Init(OpenGl_Context theCtx,
                            Graphic3d_TextureRoot theTextureMap)
        {
            if (theTextureMap == null)
            {
                return false;
            }

            switch (theTextureMap.Type())
            {
                case Graphic3d_TypeOfTexture.Graphic3d_TypeOfTexture_CUBEMAP:
                    {
                        return true;
                        //return InitCubeMap(theCtx, (Graphic3d_CubeMap)(theTextureMap),
                                            //0, Image_Format_RGB, false, theTextureMap.IsColorMap());
                    }
                default:
                    {
        //                if (theCtx->SupportedTextureFormats()->HasCompressed()
        //                && !theCtx->caps->compressedTexturesDisable)
        //                {
        //                    if (Handle(Image_CompressedPixMap) aCompressed = theTextureMap->GetCompressedImage(theCtx->SupportedTextureFormats()))
        //{
        //                        return InitCompressed(theCtx, *aCompressed, theTextureMap->IsColorMap());
        //                    }
        //                }

        //                Handle(Image_PixMap) anImage = theTextureMap->GetImage(theCtx->SupportedTextureFormats());
        //                if (anImage.IsNull())
        //                {
        //                    return false;
        //                }
        //                if (!Init(theCtx, *anImage, theTextureMap->Type(), theTextureMap->IsColorMap()))
        //                {
        //                    return false;
        //                }
        //                if (theTextureMap->HasMipmaps())
        //                {
        //                    GenerateMipmaps(theCtx);
        //                }
                        return true;
                    }
            }
        }

        
        //! Initialize the 2D texture with specified format, size and texture type.
        //! If theImage is empty the texture data will contain trash.
        //! Notice that texture will be unbound after this call.
        public bool Init(OpenGl_Context theCtx,
            OpenGl_TextureFormat theFormat,
            Graphic3d_Vec2i theSizeXY,
            Graphic3d_TypeOfTexture theType,
            Image_PixMap theImage = null)
        {
            return Init(theCtx, theFormat, new Graphic3d_Vec3i(theSizeXY, 1), theType, theImage);
        }

        //! Apply default sampler parameters after texture creation.
        void applyDefaultSamplerParams(OpenGl_Context theCtx)
        {
            OpenGl_Sampler.applySamplerParams(theCtx, mySampler.Parameters(), null, myTarget, myMaxMipLevel);
            if (mySampler.IsValid() && !mySampler.IsImmutable())
            {
                OpenGl_Sampler.applySamplerParams(theCtx, mySampler.Parameters(), mySampler, myTarget, myMaxMipLevel);
            }
        }

        //! Initialize the texture with specified format, size and texture type.
        //! If theImage is empty the texture data will contain trash.
        //! Notice that texture will be unbound after this call.
        public bool Init(OpenGl_Context theCtx,
                                     OpenGl_TextureFormat theFormat,
                                     Graphic3d_Vec3i theSizeXYZ,
                                     Graphic3d_TypeOfTexture theType,
                                     Image_PixMap theImage = null)
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


            var aTarget = All.Texture2D;
            switch (theType)
            {
                case Graphic3d_TypeOfTexture.Graphic3d_TypeOfTexture_1D:
                    {
                        aTarget = theCtx.GraphicsLibrary() != Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGLES
                                ? All.Texture1D
                                : All.Texture2D;
                        break;
                    }
                case Graphic3d_TypeOfTexture.Graphic3d_TypeOfTexture_2D:
                case Graphic3d_TypeOfTexture.Graphic3d_TOT_2D_MIPMAP:
                    {
                        aTarget = All.Texture2D;
                        break;
                    }
                case Graphic3d_TypeOfTexture.Graphic3d_TypeOfTexture_3D:
                    {
                        aTarget = All.Texture3D;
                        break;
                    }
                case Graphic3d_TypeOfTexture.Graphic3d_TypeOfTexture_CUBEMAP:
                    {
                        aTarget = All.TextureCubeMap;
                        break;
                    }
            }

            bool toPatchExisting = IsValid()
                                     && myTextFormat == theFormat.PixelFormat()
                                     && myTarget == (int)aTarget
                                     && mySize.x() == theSizeXYZ.x()
                                     && (mySize.y() == theSizeXYZ.y() || theType == Graphic3d_TypeOfTexture.Graphic3d_TypeOfTexture_1D)
                                     && mySize.z() == theSizeXYZ.z();
            if (!Create(theCtx))
            {
                Release(theCtx);
                return false;
            }

            if (theImage != null)
            {
                myIsAlpha = theImage.Format() == Image_Format.Image_Format_Alpha
                         || theImage.Format() == Image_Format.Image_Format_AlphaF;
                myIsTopDown = theImage.IsTopDown();
            }
            else
            {
                myIsAlpha = theFormat.PixelFormat() == (int)All.Alpha;
            }

            myMaxMipLevel = 0;
            myTextFormat = (uint)theFormat.PixelFormat();
            mySizedFormat = theFormat.InternalFormat();
            myNbSamples = 1;

            // ES 2.0 does not support sized formats and format conversions - them detected from data type
            GLint anIntFormat = (theCtx.GraphicsLibrary() != Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGLES
                                     || theCtx.IsGlGreaterEqual(3, 0))
                                     ? theFormat.InternalFormat()
                                     : theFormat.PixelFormat();

            if (theFormat.DataType() == (int)All.Float
            && !theCtx.arbTexFloat)
            {
                //theCtx->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH,
                //           TCollection_AsciiString("Error: floating-point textures are not supported by hardware [") + myResourceId + "]");
                Release(theCtx);
                return false;
            }

            int aMaxSize = theCtx.MaxTextureSize();
            if (theSizeXYZ.maxComp() > aMaxSize)
            {
                //theCtx->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH,
                //     TCollection_AsciiString("Error: Texture dimension - ") + theSizeXYZ.x() + "x" + theSizeXYZ.y()
                //    + (theSizeXYZ.z() > 1 ? TCollection_AsciiString("x") + theSizeXYZ.z() : TCollection_AsciiString())
                //    + " exceeds hardware limits (" + aMaxSize + "x" + aMaxSize + ")"
                //   + " [" + myResourceId + "]");
                Release(theCtx);
                return false;
            }
            else if (theCtx.GraphicsLibrary() != Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGL
                 && !theCtx.IsGlGreaterEqual(3, 0)
                 && !theCtx.arbNPTW)
            {
                // Notice that formally general NPOT textures are required by OpenGL 2.0 specifications
                // however some hardware (NV30 - GeForce FX, RadeOn 9xxx and Xxxx) supports GLSL but not NPOT!
                // Trying to create NPOT textures on such hardware will not fail
                // but driver will fall back into software rendering,
                Graphic3d_Vec2i aSizeP2 = new NCollection_Vec2<int>(OpenGl_Context.GetPowerOfTwo(theSizeXYZ.x(), aMaxSize),
                                  OpenGl_Context.GetPowerOfTwo(theSizeXYZ.y(), aMaxSize));
                if (theSizeXYZ.x() != aSizeP2.x()
                 || (theType != Graphic3d_TypeOfTexture.Graphic3d_TypeOfTexture_1D && theSizeXYZ.y() != aSizeP2.y()))
                {
                    //theCtx->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_PORTABILITY, 0, GL_DEBUG_SEVERITY_HIGH,
                    //    TCollection_AsciiString("Error: NPOT Textures (") + theSizeXYZ.x() + "x" + theSizeXYZ.y() + ")"

                    //  " are not supported by hardware [" + myResourceId + "]");
                    Release(theCtx);
                    return false;
                }
            }

            GLint aTestWidth = 0, aTestHeight = 0;
            var aDataPtr = (theImage != null) ? theImage.Data() : null;

            // setup the alignment
            OpenGl_UnpackAlignmentSentry anUnpackSentry = new OpenGl_UnpackAlignmentSentry(theCtx);
            //(void)anUnpackSentry; // avoid compiler warning

            if (aDataPtr != null)
            {
                /*GLint anAligment =Math. Min((GLint)theImage.MaxRowAligmentBytes(), 8); // OpenGL supports alignment upto 8 bytes
               theCtx.core11fwd.glPixelStorei(GL_UNPACK_ALIGNMENT, anAligment);
               const GLint anExtraBytes = GLint(theImage->RowExtraBytes());
               const GLint aPixelsWidth = GLint(theImage->SizeRowBytes() / theImage->SizePixelBytes());
               if (theCtx.hasUnpackRowLength)
               {
                   theCtx.core11fwd.glPixelStorei(GL_UNPACK_ROW_LENGTH, (anExtraBytes >= anAligment) ? aPixelsWidth : 0);
               }*/
                //  else if (anExtraBytes >= anAligment)
                //{
                /*theCtx->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_PORTABILITY, 0, GL_DEBUG_SEVERITY_HIGH,
                                     TCollection_AsciiString("Error: unsupported image stride within OpenGL ES 2.0 [") + myResourceId + "]");*/
                //  Release(theCtx);
                //  return false;
                // }
            }

            myTarget = (uint)aTarget;
            switch (theType)
            {
                //case Graphic3d_TypeOfTexture_1D:
                //    {
                //        if (theCtx->GraphicsLibrary() == Aspect_GraphicsLibrary_OpenGLES)
                //        {
                //            theCtx->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH,
                //                                 TCollection_AsciiString("Error: 1D textures are not supported by hardware [") + myResourceId + "]");
                //            Release(theCtx.get());
                //            return false;
                //        }

                //        Bind(theCtx);
                //        applyDefaultSamplerParams(theCtx);
                //        if (toPatchExisting)
                //        {
                //            theCtx->core11fwd->glTexSubImage1D(GL_TEXTURE_1D, 0, 0,
                //                                                theSizeXYZ.x(), theFormat.PixelFormat(), theFormat.DataType(), aDataPtr);
                //            break;
                //        }

                //        // use proxy to check texture could be created or not
                //        theCtx->core11fwd->glTexImage1D(GL_PROXY_TEXTURE_1D, 0, anIntFormat,
                //                                         theSizeXYZ.x(), 0,
                //                                         theFormat.PixelFormat(), theFormat.DataType(), NULL);
                //        theCtx->core11fwd->glGetTexLevelParameteriv(GL_PROXY_TEXTURE_1D, 0, GL_TEXTURE_WIDTH, &aTestWidth);
                //        theCtx->core11fwd->glGetTexLevelParameteriv(GL_PROXY_TEXTURE_1D, 0, GL_TEXTURE_INTERNAL_FORMAT, &mySizedFormat);
                //        if (aTestWidth == 0)
                //        {
                //            // no memory or broken input parameters
                //            Unbind(theCtx);
                //            Release(theCtx.operator->());
                //            return false;
                //        }

                //        theCtx->core11fwd->glTexImage1D(GL_TEXTURE_1D, 0, anIntFormat,
                //                                         theSizeXYZ.x(), 0,
                //                                         theFormat.PixelFormat(), theFormat.DataType(), aDataPtr);
                //        if (theCtx->core11fwd->glGetError() != GL_NO_ERROR)
                //        {
                //            Unbind(theCtx);
                //            Release(theCtx.get());
                //            return false;
                //        }

                //        mySize.SetValues(theSizeXYZ.x(), 1, 1);
                //        break;
                //    }
                case Graphic3d_TypeOfTexture.Graphic3d_TypeOfTexture_2D:
                case Graphic3d_TypeOfTexture.Graphic3d_TOT_2D_MIPMAP:
                    {
                        Bind(theCtx);
                        applyDefaultSamplerParams(theCtx);
                        if (toPatchExisting)
                        {
                            theCtx.core11fwd.glTexSubImage2D(All.Texture2D, 0,
                                                                0, 0,
                                                                theSizeXYZ.x(), theSizeXYZ.y(),
                                                                theFormat.PixelFormat(), theFormat.DataType(), aDataPtr);
                            break;
                        }

                        if (theCtx.GraphicsLibrary() == Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGL)
                        {
                            // use proxy to check texture could be created or not
                            theCtx.core11fwd.glTexImage2D(All.ProxyTexture2D, 0, anIntFormat,
                                                             theSizeXYZ.x(), theSizeXYZ.y(), 0,
                                                             theFormat.PixelFormat(), theFormat.DataType(), null);
                            theCtx.core11fwd.glGetTexLevelParameteriv(All.ProxyTexture2D, 0, All.TextureWidth, ref aTestWidth);
                            theCtx.core11fwd.glGetTexLevelParameteriv(All.ProxyTexture2D, 0, All.TextureHeight, ref aTestHeight);
                            theCtx.core11fwd.glGetTexLevelParameteriv(All.ProxyTexture2D, 0, All.TextureInternalFormat, ref mySizedFormat);
                            if (aTestWidth == 0 || aTestHeight == 0)
                            {
                                // no memory or broken input parameters
                                Unbind(theCtx);
                                Release(theCtx);
                                return false;
                            }
                        }

                        theCtx.core11fwd.glTexImage2D(All.Texture2D, 0, anIntFormat,
                                                         theSizeXYZ.x(), theSizeXYZ.y(), 0,
                                                         theFormat.PixelFormat(), theFormat.DataType(), aDataPtr);
                        var anErr = theCtx.core11fwd.glGetError();
                        if (anErr != ErrorCode.NoError)
                        {
                            /*   theCtx->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH,
                                                    TCollection_AsciiString("Error: 2D texture ") + theSizeXYZ.x() + "x" + theSizeXYZ.y()
                                                                          + " IF: " + OpenGl_TextureFormat::FormatFormat(anIntFormat)
                                                                          + " PF: " + OpenGl_TextureFormat::FormatFormat(theFormat.PixelFormat())
                                                                          + " DT: " + OpenGl_TextureFormat::FormatDataType(theFormat.DataType())
                                                                          + " can not be created with error " + OpenGl_Context::FormatGlError(anErr)
                                                                          + " [" + myResourceId + "]");*/
                            Unbind(theCtx);
                            Release(theCtx);
                            return false;
                        }

                        mySize.SetValues(theSizeXYZ.xy(), 1);
                        break;
                    }
                //case Graphic3d_TypeOfTexture_3D:
                //    {
                //        if (theCtx->Functions()->glTexImage3D == nullptr)
                //        {
                //          /*  theCtx->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH,
                //                                 "Error: three-dimensional textures are not supported by hardware.");*/
                //            Unbind(theCtx);
                //            Release(theCtx.get());
                //            return false;
                //        }

                //        Bind(theCtx);
                //        applyDefaultSamplerParams(theCtx);
                //        if (theCtx->GraphicsLibrary() == Aspect_GraphicsLibrary_OpenGL)
                //        {
                //            theCtx->Functions()->glTexImage3D(GL_PROXY_TEXTURE_3D, 0, anIntFormat,
                //                                               theSizeXYZ.x(), theSizeXYZ.y(), theSizeXYZ.z(), 0,
                //                                               theFormat.PixelFormat(), theFormat.DataType(), nullptr);

                //            NCollection_Vec3<GLint> aTestSizeXYZ;
                //            theCtx->core11fwd->glGetTexLevelParameteriv(GL_PROXY_TEXTURE_3D, 0, GL_TEXTURE_WIDTH, &aTestSizeXYZ.x());
                //            theCtx->core11fwd->glGetTexLevelParameteriv(GL_PROXY_TEXTURE_3D, 0, GL_TEXTURE_HEIGHT, &aTestSizeXYZ.y());
                //            theCtx->core11fwd->glGetTexLevelParameteriv(GL_PROXY_TEXTURE_3D, 0, GL_TEXTURE_DEPTH, &aTestSizeXYZ.z());
                //            theCtx->core11fwd->glGetTexLevelParameteriv(GL_PROXY_TEXTURE_3D, 0, GL_TEXTURE_INTERNAL_FORMAT, &mySizedFormat);
                //            if (aTestSizeXYZ.x() == 0 || aTestSizeXYZ.y() == 0 || aTestSizeXYZ.z() == 0)
                //            {
                //                Unbind(theCtx);
                //                Release(theCtx.get());
                //                return false;
                //            }
                //        }

                //        theCtx->Functions()->glTexImage3D(GL_TEXTURE_3D, 0, anIntFormat,
                //                                           theSizeXYZ.x(), theSizeXYZ.y(), theSizeXYZ.z(), 0,
                //                                           theFormat.PixelFormat(), theFormat.DataType(), aDataPtr);
                //        GLenum anErr = theCtx->core11fwd->glGetError();
                //        if (anErr != GL_NO_ERROR)
                //        {
                //            /*theCtx->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH,
                //                                 TCollection_AsciiString("Error: 3D texture ") + theSizeXYZ.x() + "x" + theSizeXYZ.y() + "x" + theSizeXYZ.z()
                //                                                       + " IF: " + OpenGl_TextureFormat::FormatFormat(anIntFormat)
                //                                                       + " PF: " + OpenGl_TextureFormat::FormatFormat(theFormat.PixelFormat())
                //                                                       + " DT: " + OpenGl_TextureFormat::FormatDataType(theFormat.DataType())
                //                                                       + " can not be created with error " + OpenGl_Context::FormatGlError(anErr)
                //                                                       + " [" + myResourceId + "]");*/
                //            Unbind(theCtx);
                //            Release(theCtx);
                //            return false;
                //        }

                //        mySize = theSizeXYZ;
                //        break;
                //    }
                case Graphic3d_TypeOfTexture.Graphic3d_TypeOfTexture_CUBEMAP:
                    {
                        Unbind(theCtx);
                        Release(theCtx);
                        return false;
                    }
                default:
                    throw new NotImplementedException();
                    break;
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
        protected Graphic3d_Vec3i mySize = new NCollection_Vec3<int>();       //!< texture width x height x depth
        protected uint myTextFormat; //!< texture format - GL_RGB, GL_RGBA,...
        protected int mySizedFormat;//!< internal (sized) texture format
        protected int myNbSamples;  //!< number of MSAA samples
        protected int myMaxMipLevel;//!< upper mipmap level index (0 means no mipmaps)
        protected bool myIsAlpha;    //!< indicates alpha format
        protected bool myIsTopDown;  //!< indicates if 2D surface is defined top-down (TRUE) or bottom-up (FALSE)
    }
    //! Simple class to reset unpack alignment settings
    struct OpenGl_UnpackAlignmentSentry
    {
        public OpenGl_UnpackAlignmentSentry(OpenGl_Context theCtx)
        {
            myCtx = (theCtx);
        }
        OpenGl_Context myCtx;

    }
}