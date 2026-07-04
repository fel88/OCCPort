using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL;
using System;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using TKMath;
using TKService;

namespace OCCPort.OpenGL
{

    //! Stores parameters of OpenGL texture format.    
    public class OpenGl_TextureFormat
    {


        //! Returns OpenGL data type of the pixel data (example: GL_FLOAT).
       public  GLint DataType()  { return myDataType; }

        //! Returns OpenGL format of the pixel data (example: GL_RED).
        public int PixelFormat() { return myPixelFormat; }


        //! Returns OpenGL internal format of the pixel data (example: GL_R32F).
        public int InternalFormat() { return myInternalFormat; }

        //! Find texture format suitable to specified internal (sized) texture format.
        //! @param theCtx [in] OpenGL context defining supported texture formats
        //! @param theSizedFormat [in] sized (internal) texture format (example: GL_RGBA8)
        //! @return found format or invalid format
        public static OpenGl_TextureFormat FindSizedFormat(OpenGl_Context theCtx,
                                                              GLint theSizedFormat)
        {
            var subFormat = (SizedInternalFormat)theSizedFormat;
            OpenGl_TextureFormat aFormat = new OpenGl_TextureFormat();
            switch (subFormat)
            {

                case SizedInternalFormat.Rgba32f:
                    {
                        aFormat.SetNbComponents(4);
                        aFormat.SetInternalFormat(theSizedFormat);
                        aFormat.SetPixelFormat(All.Rgba);
                        aFormat.SetDataType(All.Float);
                        aFormat.SetImageFormat(Image_Format.Image_Format_RGBAF);
                        return aFormat;
                    }
                case SizedInternalFormat.R32f:
                    {
                        aFormat.SetNbComponents(1);
                        aFormat.SetInternalFormat(theSizedFormat);
                        aFormat.SetPixelFormat(All.Red);
                        aFormat.SetDataType(All.Float);
                        aFormat.SetImageFormat(Image_Format.Image_Format_GrayF);
                        return aFormat;
                    }
                case SizedInternalFormat.Rg32f:
                    {
                        aFormat.SetNbComponents(1);
                        aFormat.SetInternalFormat(theSizedFormat);
                        aFormat.SetPixelFormat(All.Rg);
                        aFormat.SetDataType(All.Float);
                        aFormat.SetImageFormat(Image_Format.Image_Format_RGF);
                        return aFormat;
                    }
                //case GL_RGBA16F:
                //    {
                //        aFormat.SetNbComponents(4);
                //        aFormat.SetInternalFormat(theSizedFormat);
                //        aFormat.SetPixelFormat(GL_RGBA);
                //        aFormat.SetDataType(GL_HALF_FLOAT);
                //        aFormat.SetImageFormat(Image_Format_RGBAF_half);
                //        if (theCtx->hasHalfFloatBuffer == OpenGl_FeatureInExtensions)
                //        {
                //            aFormat.SetDataType(theCtx->GraphicsLibrary() == Aspect_GraphicsLibrary_OpenGLES
                //                               ? GL_HALF_FLOAT_OES
                //                               : GL_FLOAT);
                //        }
                //        return aFormat;
                //    }
                //case GL_R16F:
                //    {
                //        aFormat.SetNbComponents(1);
                //        aFormat.SetInternalFormat(theSizedFormat);
                //        aFormat.SetPixelFormat(GL_RED);
                //        aFormat.SetDataType(GL_HALF_FLOAT);
                //        aFormat.SetImageFormat(Image_Format_GrayF_half);
                //        if (theCtx->hasHalfFloatBuffer == OpenGl_FeatureInExtensions)
                //        {
                //            aFormat.SetDataType(theCtx->GraphicsLibrary() == Aspect_GraphicsLibrary_OpenGLES
                //                               ? GL_HALF_FLOAT_OES
                //                               : GL_FLOAT);
                //        }
                //        return aFormat;
                //    }
                //case GL_RG16F:
                //    {
                //        aFormat.SetNbComponents(2);
                //        aFormat.SetInternalFormat(theSizedFormat);
                //        aFormat.SetPixelFormat(GL_RG);
                //        aFormat.SetDataType(GL_HALF_FLOAT);
                //        aFormat.SetImageFormat(Image_Format_RGF_half);
                //        if (theCtx->hasHalfFloatBuffer == OpenGl_FeatureInExtensions)
                //        {
                //            aFormat.SetDataType(theCtx->GraphicsLibrary() == Aspect_GraphicsLibrary_OpenGLES
                //                               ? GL_HALF_FLOAT_OES
                //                               : GL_FLOAT);
                //        }
                //        return aFormat;
                //    }
                case SizedInternalFormat.Srgb8Alpha8:
                //case SizedInternalFormat.Srgb8Alpha8Ext GL_SRGB_ALPHA_EXT:
                case SizedInternalFormat.Rgba8:
                    //case SizedInternalFormat.rgbaGL_RGBA:
                    {
                        aFormat.SetNbComponents(4);
                        aFormat.SetInternalFormat(theSizedFormat);
                        aFormat.SetPixelFormat(All.Rgba);
                        aFormat.SetDataType(All.UnsignedByte);
                        aFormat.SetImageFormat(Image_Format.Image_Format_RGBA);
                        if ((theSizedFormat == (int)All.Srgb8Alpha8 || theSizedFormat == (int)All.SrgbAlphaExt))
                        {
                            if (theCtx.ToRenderSRGB())
                            {
                                if (theCtx.GraphicsLibrary() == Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGLES
                                && !theCtx.IsGlGreaterEqual(3, 0))
                                {
                                    aFormat.SetPixelFormat(All.SrgbAlphaExt);
                                }
                            }
                            else
                            {
                                aFormat.SetInternalFormat((int)All.Rgba8); // fallback format
                            }
                        }
                        return aFormat;
                    }
                //case GL_SRGB8:
                //case GL_SRGB_EXT:
                //case GL_RGB8:
                //case GL_RGB:
                //    {
                //        aFormat.SetNbComponents(3);
                //        aFormat.SetInternalFormat(theSizedFormat);
                //        aFormat.SetPixelFormat(GL_RGB);
                //        aFormat.SetDataType(GL_UNSIGNED_BYTE);
                //        aFormat.SetImageFormat(Image_Format_RGB);
                //        if ((theSizedFormat == GL_SRGB8 || theSizedFormat == GL_SRGB_EXT))
                //        {
                //            if (theCtx->ToRenderSRGB())
                //            {
                //                if (theCtx->GraphicsLibrary() == Aspect_GraphicsLibrary_OpenGLES
                //                && !theCtx->IsGlGreaterEqual(3, 0))
                //                {
                //                    aFormat.SetPixelFormat(GL_SRGB_EXT);
                //                }
                //            }
                //            else
                //            {
                //                aFormat.SetInternalFormat(GL_RGB8); // fallback format
                //            }
                //        }
                //        return aFormat;
                //    }
                //case GL_RGB10_A2:
                //    {
                //        aFormat.SetNbComponents(4);
                //        aFormat.SetInternalFormat(theSizedFormat);
                //        aFormat.SetPixelFormat(GL_RGBA);
                //        aFormat.SetDataType(GL_UNSIGNED_INT_2_10_10_10_REV);
                //        aFormat.SetImageFormat(Image_Format_RGBA);
                //        return aFormat;
                //    }
                //// integer types
                //case GL_R32I:
                //    {
                //        aFormat.SetNbComponents(1);
                //        aFormat.SetInternalFormat(theSizedFormat);
                //        aFormat.SetPixelFormat(GL_RED_INTEGER);
                //        aFormat.SetDataType(GL_INT);
                //        return aFormat;
                //    }
                //case GL_RG32I:
                //    {
                //        aFormat.SetNbComponents(2);
                //        aFormat.SetInternalFormat(theSizedFormat);
                //        aFormat.SetPixelFormat(GL_RG_INTEGER);
                //        aFormat.SetDataType(GL_INT);
                //        return aFormat;
                //    }
                // depth formats
                case  SizedInternalFormat.Depth24Stencil8:
                    {
                        aFormat.SetNbComponents(2);
                        aFormat.SetInternalFormat(theSizedFormat);
                        aFormat.SetPixelFormat(All.DepthStencil );
                        aFormat.SetDataType(All.UnsignedInt248);
                        return aFormat;
                    }
                    //case GL_DEPTH32F_STENCIL8:
                    //    {
                    //        aFormat.SetNbComponents(2);
                    //        aFormat.SetInternalFormat(theSizedFormat);
                    //        aFormat.SetPixelFormat(GL_DEPTH_STENCIL);
                    //        aFormat.SetDataType(GL_FLOAT_32_UNSIGNED_INT_24_8_REV);
                    //        return aFormat;
                    //    }
                    //case GL_DEPTH_COMPONENT16:
                    //    {
                    //        aFormat.SetNbComponents(1);
                    //        aFormat.SetInternalFormat(theSizedFormat);
                    //        aFormat.SetPixelFormat(GL_DEPTH_COMPONENT);
                    //        aFormat.SetDataType(GL_UNSIGNED_SHORT);
                    //        return aFormat;
                    //    }
                    //case GL_DEPTH_COMPONENT24:
                    //    {
                    //        aFormat.SetNbComponents(1);
                    //        aFormat.SetInternalFormat(theSizedFormat);
                    //        aFormat.SetPixelFormat(GL_DEPTH_COMPONENT);
                    //        aFormat.SetDataType(GL_UNSIGNED_INT);
                    //        return aFormat;
                    //    }
                    //case GL_DEPTH_COMPONENT32F:
                    //    {
                    //        aFormat.SetNbComponents(1);
                    //        aFormat.SetInternalFormat(theSizedFormat);
                    //        aFormat.SetPixelFormat(GL_DEPTH_COMPONENT);
                    //        aFormat.SetDataType(GL_FLOAT);
                    //        return aFormat;
                    //    }
            }
            return aFormat;
        }

        //! Sets image format.
        private void SetImageFormat(Image_Format theFormat)
        {
            myImageFormat = theFormat;
        }

        //! Sets OpenGL data type of the pixel data.
        private void SetDataType(All theType)
        {
            myDataType = (int)theType;
        }

        private void SetPixelFormat(All theFormat)
        {
            myPixelFormat = (int)theFormat;
        }

        //! Sets texture internal format.
        private void SetInternalFormat(int theSizedFormat)
        {
            myInternalFormat = theSizedFormat;
        }

        Image_Format myImageFormat; //!< image format
        GLint myInternalFormat; //!< OpenGL internal format of the pixel data
        int myPixelFormat;    //!< OpenGL pixel format
        GLint myDataType;       //!< OpenGL data type of input pixel data
        GLint myNbComponents;   //!< number of channels for each pixel (from 1 to 4)
        private void SetNbComponents(int theNbComponents)
        {
            myNbComponents = theNbComponents;
        }


        //! Return TRUE if format is defined.
        public bool IsValid()
        {
            return myInternalFormat != 0
                && myPixelFormat != 0
                && myDataType != 0;
        }
    }
}