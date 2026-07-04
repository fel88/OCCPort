using OCCPort;
using OCCPort.Common;
using OCCPort.OpenGL;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Intrinsics.Arm;
using TKernel;
using TKService;
using static System.Net.Mime.MediaTypeNames;
using ErrorCode = OpenTK.Graphics.OpenGL.ErrorCode;


namespace OCCPort.OpenGL
{
    //! This class generalize access to the GL context and available extensions.
    //!
    //! Functions related to specific OpenGL version or extension are grouped into structures which can be accessed as fields of this class.
    //! The most simple way to check that required functionality is available - is NULL check for the group:
    //! @code
    //!   if (myContext->core20 != NULL)
    //!   {
    //!     myGlProgram = myContext->core20->glCreateProgram();
    //!     .. do more stuff ..
    //!   }
    //!   else
    //!   {
    //!     .. compatibility with outdated configurations ..
    //!   }
    //! @endcode
    //!
    //! Current implementation provide access to OpenGL core functionality up to 4.6 version (core12, core13, core14, etc.)
    //! as well as several extensions (arbTBO, arbFBO, etc.).
    //!
    //! OpenGL context might be initialized in Core Profile. In this case deprecated functionality become unavailable.
    //! To select which core** function set should be used in specific case:
    //!  - Determine the minimal OpenGL version required for implemented functionality and use it to access all functions.
    //!    For example, if algorithm requires OpenGL 2.1+, it is better to write core20fwd->glEnable() rather than core11fwd->glEnable() for uniformity.
    //!  - Validate minimal requirements at initialization/creation time and omit checks within code where algorithm should be already initialized.
    //!    Properly escape code incompatible with Core Profile. The simplest way to check Core Profile is "if (core11ffp == NULL)".
    //!
    //! Simplified extensions classification:
    //!  - prefixed with NV, AMD, ATI are vendor-specific (however may be provided by other vendors in some cases);
    //!  - prefixed with EXT are accepted by 2+ vendors;
    //!  - prefixed with ARB are accepted by Architecture Review Board and are candidates
    //!    for inclusion into GL core functionality.
    //! Some functionality can be represented in several extensions simultaneously.
    //! In this case developer should be careful because different specification may differ
    //! in aspects (like enumeration values and error-handling).
    //!
    //! Notice that some systems provide mechanisms to simultaneously incorporate with GL contexts with different capabilities.
    //! For this reason OpenGl_Context should be initialized and used for each GL context independently.
    //!
    //! Matrices of OpenGl transformations:
    //! model -> world -> view -> projection
    //! These matrices might be changed for local transformation, transform persistent using direct access to
    //! current matrix of ModelWorldState, WorldViewState and ProjectionState
    //! After, these matrices should be applied using ApplyModelWorldMatrix, ApplyWorldViewMatrix,
    //! ApplyModelViewMatrix or ApplyProjectionMatrix.
    public class OpenGl_Context
    {
        public OpenGl_Context(OpenGl_Caps theCaps = null)
        {
            //core11ffp = null;

            core11fwd = new _core11fwd();
            core15fwd = new _core15fwd();
            core20fwd = new OpenGl_GlCore20();
            //hasPackRowLength=(Standard_True),
            //hasUnpackRowLength=(Standard_True),
            hasHighp = (true);
            hasUintIndex = true;
            //hasTexRGBA8=(Standard_True),
            //
            myFuncs = (new OpenGl_GlFunctions());
            myGapi = Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGL;
            /*
               mySupportedFormats (new Image_SupportedFormats()),*/
            myAnisoMax=(1);
  //myTexClamp=.(GL_CLAMP_TO_EDGE);
  myMaxTexDim = (1024);
            myMaxTexCombined = (1);/*
  myMaxTexUnitsFFP (1),
  myMaxDumpSizeX (1024),
  myMaxDumpSizeY (1024),
  myMaxClipPlanes (6)*/
            myMaxMsaaSamples = 0;
            myMaxDrawBuffers = 1;
            myMaxColorAttachments = (1);
            myGlVerMajor = (0);
            myGlVerMinor = (0);
            myIsInitialized = (false);
            myIsStereoBuffers = (false);
            myHasMsaaTextures = (false);
            myIsGlNormalizeEnabled = (false);
            mySpriteTexUnit = Graphic3d_TextureUnit.Graphic3d_TextureUnit_PointSprite;
            //myHasRayTracing = (false);

            //myHasRayTracingTextures = (false);
            //myHasRayTracingAdaptiveSampling = (false);
            //myHasRayTracingAdaptiveSamplingAtomic = false;
            myHasPBR = (false);
            /*
myPBREnvLUTTexUnit       (Graphic3d_TextureUnit_PbrEnvironmentLUT),
myPBRDiffIBLMapSHTexUnit (Graphic3d_TextureUnit_PbrIblDiffuseSH),
myPBRSpecIBLMapTexUnit   (Graphic3d_TextureUnit_PbrIblSpecular),
myShadowMapTexUnit       (Graphic3d_TextureUnit_ShadowMap),
myDepthPeelingDepthTexUnit (Graphic3d_TextureUnit_DepthPeelingDepth),
myDepthPeelingFrontColorTexUnit (Graphic3d_TextureUnit_DepthPeelingFrontColor),*/
            myFrameStats = new OpenGl_FrameStats();
            /*
myActiveMockTextures (0),
myActiveHatchType (Aspect_HS_SOLID),
myHatchIsEnabled (false),
myPointSpriteOrig (GL_UPPER_LEFT),*/
            myRenderMode = (int)All.Render;
            myShadeModel = (int)All.Smooth;
            myPolygonMode = (int)All.Fill;

            myFaceCulling = Graphic3d_TypeOfBackfacingModel.Graphic3d_TypeOfBackfacingModel_DoubleSided;
            myReadBuffer = (0);
            myDrawBuffers = new NCollection_Array1<int>(0, 7);

            myDefaultVao = (0);
            myColorMask = new NCollection_Vec4<bool>(true);

            myAlphaToCoverage = (false);
            /*
myIsGlDebugCtx (false),
myIsWindowDeepColor (false),
myIsSRgbWindow (false),
myIsSRgbActive (false),
myResolution (Graphic3d_RenderingParams::THE_DEFAULT_RESOLUTION),
myResolutionRatio (1.0f),
myLineWidthScale (1.0f),
myLineFeather (1.0f),*/
            myRenderScale = (1.0f);
            myRenderScaleInv = 1.0f;

            hasDrawBuffers = OpenGl_FeatureFlag.OpenGl_FeatureNotAvailable;
            caps = (theCaps != null ? theCaps : new OpenGl_Caps());
            myViewport[0] = 0;
            myViewport[1] = 0;
            myViewport[2] = 0;
            myViewport[3] = 0;
            myViewportVirt[0] = 0;
            myViewportVirt[1] = 0;
            myViewportVirt[2] = 0;
            myViewportVirt[3] = 0;


            /*myPolygonOffset.Mode = Aspect_POM_Off;
            myPolygonOffset.Factor = 0.0f;
            myPolygonOffset.Units = 0.0f;*/
            myShaderManager = new OpenGl_ShaderManager(this);
            mySharedResources = (new OpenGl_ResourcesMap());

        }

       public  bool arbTexFloat;        //!< GL_ARB_texture_float (on desktop OpenGL - since 3.0 or as extension GL_ARB_texture_float; on OpenGL ES - since 3.0); @sa hasTexFloatLinear for linear filtering support

        bool myAlphaToCoverage; //!< flag indicating GL_SAMPLE_ALPHA_TO_COVERAGE state
        public bool extAnis;            //!< GL_EXT_texture_filter_anisotropic

        //! Either GL_CLAMP_TO_EDGE (1.2+) or GL_CLAMP (1.1).
        public int TextureWrapClamp() { return myTexClamp; }

        //! @return maximum degree of anisotropy texture filter
        public int MaxDegreeOfAnisotropy() { return myAnisoMax; }

        public bool SetSampleAlphaToCoverage(bool theToEnable)
        {
            bool toEnable = myAllowAlphaToCov && theToEnable;
            if (myAlphaToCoverage == toEnable)
            {
                return myAlphaToCoverage;
            }

            if (core15fwd != null)
            {
                if (toEnable)
                {
                    //core15fwd->core15fwd->glSampleCoverage (1.0f, GL_FALSE);
                    core15fwd.glEnable(All.SampleAlphaToCoverage);
                }
                else
                {
                    core15fwd.glDisable(All.SampleAlphaToCoverage);
                }
            }

            bool anOldValue = myAlphaToCoverage;
            myAlphaToCoverage = toEnable;
            return anOldValue;
        }


        OpenGl_Texture myTextureRgbaBlack;//!< mock black texture returning (0, 0, 0, 0)
        OpenGl_Texture myTextureRgbaWhite;//!< mock white texture returning (1, 1, 1, 1)

        //! Bind specified texture set to current context, or unbind previous one when NULL specified.
        //! @param theTextures [in] texture set to bind
        //! @param theProgram  [in] program attributes; when not NULL,
        //!                         mock textures will be bound to texture units expected by GLSL program, but undefined by texture set
        //! @return previous texture set
        public OpenGl_TextureSet BindTextures(OpenGl_TextureSet theTextures,
                                                        OpenGl_ShaderProgram theProgram)
        {
            int aTextureSetBits = theTextures != null ? theTextures.TextureSetBits() : 0;
            int aProgramBits = theProgram != null ? theProgram.TextureSetBits() : 0;
            int aMissingBits = aProgramBits & ~aTextureSetBits;
            if (aMissingBits != 0
             && myTextureRgbaBlack == null)
            {
                // allocate mock textures
                myTextureRgbaBlack = new OpenGl_Texture();
                myTextureRgbaWhite = new OpenGl_Texture();
                Image_PixMap anImage;
                //   anImage.InitZero(Image_Format_RGBA, 2, 2, 0, (Standard_Byte)0);
                //   if (!myTextureRgbaBlack->Init(this, OpenGl_TextureFormat::Create < GLubyte, 4 > (), Graphic3d_Vec2i(2, 2), Graphic3d_TypeOfTexture_2D, &anImage))
                {
                    //PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_PORTABILITY, 0, GL_DEBUG_SEVERITY_HIGH,
                    //     "Error: unable to create unit mock PBR texture map.");
                }
                //anImage.InitZero(Image_Format_RGBA, 2, 2, 0, (Standard_Byte)255);
                //  if (!myTextureRgbaWhite->Init(this, OpenGl_TextureFormat::Create < GLubyte, 4 > (), Graphic3d_Vec2i(2, 2), Graphic3d_TypeOfTexture_2D, &anImage))
                {
                    //PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_PORTABILITY, 0, GL_DEBUG_SEVERITY_HIGH,
                    //  "Error: unable to create normal mock PBR texture map.");
                }
            }

            OpenGl_TextureSet anOldTextures = myActiveTextures;
            if (myActiveTextures != theTextures)
            {
                OpenGl_Context aThisCtx = (this);
                for (OpenGl_TextureSetPairIterator aSlotIter = new OpenGl_TextureSetPairIterator(myActiveTextures, theTextures); aSlotIter.More(); aSlotIter.Next())
                {
                    Graphic3d_TextureUnit aTexUnit = aSlotIter.Unit();
                    OpenGl_Texture aTextureOld = aSlotIter.Texture1();
                    OpenGl_Texture aTextureNew = aSlotIter.Texture2();
                    if (aTextureNew == aTextureOld)
                    {
                        continue;
                    }

                    if (aTextureNew != null
                     && aTextureNew.IsValid())
                    {
                        if ((int)aTexUnit >= myMaxTexCombined)
                        {
                            //PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH,
                            //        TCollection_AsciiString("Texture unit ") + aTexUnit + " for " + aTextureNew->ResourceId() + " exceeds hardware limit " + myMaxTexCombined);
                            continue;
                        }

                        aTextureNew.Bind(aThisCtx, aTexUnit);
                        if (aTextureNew.Sampler().ToUpdateParameters())
                        {
                            if (aTextureNew.Sampler().IsImmutable())
                            {
                                aTextureNew.Sampler().Init(aThisCtx, aTextureNew);
                            }
                            else
                            {
                                //  OpenGl_Sampler.applySamplerParams(aThisCtx, aTextureNew->Sampler()->Parameters(), aTextureNew->Sampler().get(), aTextureNew->GetTarget(), aTextureNew->MaxMipmapLevel());
                            }
                        }
                        if (core11ffp != null)
                        {
                            // OpenGl_Sampler.applyGlobalTextureParams(aThisCtx, *aTextureNew, aTextureNew->Sampler()->Parameters());
                        }
                    }
                    else if (aTextureOld != null
                          && aTextureOld.IsValid())
                    {
                        aTextureOld.Unbind(aThisCtx, aTexUnit);
                        if (core11ffp != null)
                        {
                            //OpenGl_Sampler.resetGlobalTextureParams(aThisCtx, *aTextureOld, aTextureOld->Sampler()->Parameters());
                        }
                    }
                }
                myActiveTextures = theTextures;
            }

            //if (myActiveMockTextures != aMissingBits)
            //{
            //    myActiveMockTextures = aMissingBits;
            //    for (int aBitIter = 0; aMissingBits != 0; ++aBitIter)
            //    {
            //        int aUnitMask = 1 << aBitIter;
            //        if ((aUnitMask & aMissingBits) != 0)
            //        {
            //            aMissingBits = aMissingBits & ~aUnitMask;
            //            if (aBitIter == Graphic3d_TextureUnit_Normal)
            //            {
            //                myTextureRgbaBlack.Bind(this, static_cast<Graphic3d_TextureUnit>(aBitIter));
            //            }
            //            else
            //            {
            //                myTextureRgbaWhite.Bind(this, static_cast<Graphic3d_TextureUnit>(aBitIter));
            //            }
            //        }
            //    }
            //}

            return anOldTextures;
        }

        public void DisableFeatures()
        {
            // Disable stuff that's likely to slow down glDrawPixels.
            core11fwd.glDisable(All.Dither);
            core11fwd.glDisable(All.Blend);
            core11fwd.glDisable(All.DepthTest);
            core11fwd.glDisable(All.StencilTest);

            if (core11ffp == null)
            {
                return;
            }

            core11fwd.glDisable(All.Texture1D);
            core11fwd.glDisable(All.Texture2D);

            core11fwd.glDisable(All.Lighting);
            core11fwd.glDisable(All.AlphaTest);
            core11fwd.glDisable(All.Fog);
            core11fwd.glDisable(All.LogicOp);

            core11ffp.glPixelTransferi(All.MapColor, 0);//false
            core11ffp.glPixelTransferi(All.RedScale, 1);
            core11ffp.glPixelTransferi(All.RedBias, 0);
            core11ffp.glPixelTransferi(All.GreenScale, 1);
            core11ffp.glPixelTransferi(All.GreenBias, 0);
            core11ffp.glPixelTransferi(All.BlueScale, 1);
            core11ffp.glPixelTransferi(All.BlueBias, 0);
            core11ffp.glPixelTransferi(All.AlphaScale, 1);
            core11ffp.glPixelTransferi(All.AlphaBias, 0);

            if (IsGlGreaterEqual(1, 2))
            {
                if (CheckExtension("GL_CONVOLUTION_1D_EXT"))
                {
                    core11fwd.glDisable(All.Convolution1DExt);
                }
                if (CheckExtension("GL_CONVOLUTION_2D_EXT"))
                {
                    core11fwd.glDisable(All.Convolution2DExt);
                }
                if (CheckExtension("GL_SEPARABLE_2D_EXT"))
                {
                    core11fwd.glDisable(All.Separable2DExt);
                }
                if (CheckExtension("GL_SEPARABLE_2D_EXT"))
                {
                    core11fwd.glDisable(All.HistogramExt);
                }
                if (CheckExtension("GL_MINMAX_EXT"))
                {
                    core11fwd.glDisable(All.MinmaxExt);
                }
                if (CheckExtension("GL_TEXTURE_3D_EXT"))
                {
                    core11fwd.glDisable(All.Texture3DExt);
                }
            }
        }
        public bool arbNPTW;            //!< GL_ARB_texture_non_power_of_two

        //! Get maximum number of clip planes supported by OpenGl.
        //! This value is implementation dependent. At least 6
        //! planes should be supported by OpenGl (see specs).
        //! @return value for GL_MAX_CLIP_PLANES
        public int MaxClipPlanes() { return myMaxClipPlanes; }

        //! Returns TRUE if PBR shading model is supported.
        //! Basically, feature requires OpenGL 3.0+ / OpenGL ES 3.0+ hardware; more precisely:
        //! - Graphics hardware with moderate capabilities for compiling long enough GLSL program.
        //! - FBO (e.g. for baking environment).
        //! - Multi-texturing with >= 4 units (LUT and IBL textures).
        //! - GL_RG32F texture format (arbTexRG + arbTexFloat)
        //! - Cubemap texture lookup textureCubeLod()/textureLod() with LOD index within Fragment Shader,
        //!   which requires GLSL OpenGL 3.0+ / OpenGL ES 3.0+ or OpenGL 2.1 + GL_EXT_gpu_shader4 extension.
        public bool HasPBR() { return myHasPBR; }
        bool myHasPBR;                      //!< indicates whether PBR shading model is supported


        bool myIsGlNormalizeEnabled; //!< GL_NORMALIZE flag
                                     //!< Used to tell OpenGl that normals should be normalized
        public bool SetGlNormalizeEnabled(bool isEnabled)
        {
            if (isEnabled == myIsGlNormalizeEnabled)
            {
                return myIsGlNormalizeEnabled;
            }

            bool anOldGlNormalize = myIsGlNormalizeEnabled;
            myIsGlNormalizeEnabled = isEnabled;

            if (core11ffp != null)
            {
                if (isEnabled)
                {
                    core11fwd.glEnable(All.Normalize);
                }
                else
                {
                    core11fwd.glDisable(All.Normalize);
                }
            }

            return anOldGlNormalize;
        }


        public bool extPDS;             //!< GL_EXT_packed_depth_stencil

        //! @return true if MSAA textures are supported.
        public bool HasTextureMultisampling() { return myHasMsaaTextures; }



        public IOpenGl_ArbSamplerObject arbSamplerObject; //!< GL_ARB_sampler_objects (on desktop OpenGL - since 3.3 or as extension GL_ARB_sampler_objects; on OpenGL ES - since 3.0)

        public bool hasHighp;           //!< highp in GLSL ES fragment shader is supported
        public bool arbClipControl;     //!< GL_ARB_clip_control, in core since 4.5


        public bool extFragDepth;       //!< GL_EXT_frag_depth on OpenGL ES 2.0 (gl_FragDepthEXT built-in variable, before OpenGL ES 3.0)

        public void SetColorMaskRGBA(NCollection_Vec4<bool> theVal)
        {
            core11fwd.glColorMask(theVal.r() ? true : false,
                         theVal.g() ? true : false,
                         theVal.b() ? true : false,
                         theVal.a() ? true : false);
            myColorMask = theVal;
        }
        //! @return value for GL_MAX_COMBINED_TEXTURE_IMAGE_UNITS
        public int MaxCombinedTextureUnits() { return myMaxTexCombined; }

        Graphic3d_TextureUnit mySpriteTexUnit;   //!< sampler2D occSamplerPointSprite, texture unit for point sprite texture

        //! Return texture unit to be used for sprites (Graphic3d_TextureUnit_PointSprite by default).
        public Graphic3d_TextureUnit SpriteTextureUnit() { return mySpriteTexUnit; }


        //! @return true if detected GL version is greater or equal to requested one.
        public bool IsGlGreaterEqual(int theVerMajor,
                                             int theVerMinor)
        {
            return (myGlVerMajor > theVerMajor)
                || (myGlVerMajor == theVerMajor && myGlVerMinor >= theVerMinor);
        }


        public static string FormatGlError(int theGlError)
        {
            switch ((All)theGlError)
            {
                case All.InvalidEnum: return "GL_INVALID_ENUM";
                case All.InvalidValue: return "GL_INVALID_VALUE";
                case All.InvalidOperation: return "GL_INVALID_OPERATION";
                case All.StackOverflow: return "GL_STACK_OVERFLOW";
                case All.StackUnderflow: return "GL_STACK_UNDERFLOW";
                case All.OutOfMemory: return "GL_OUT_OF_MEMORY";
                case All.InvalidFramebufferOperation: return "GL_INVALID_FRAMEBUFFER_OPERATION";
            }
            return FormatGlEnumHex(theGlError);
        }

        public static string FormatGlError(ErrorCode theGlError)
        {
            switch (theGlError)
            {
                case ErrorCode.InvalidEnum: return "GL_INVALID_ENUM";
                case ErrorCode.InvalidValue: return "GL_INVALID_VALUE";
                case ErrorCode.InvalidOperation: return "GL_INVALID_OPERATION";
                case ErrorCode.StackOverflow: return "GL_STACK_OVERFLOW";
                case ErrorCode.StackUnderflow: return "GL_STACK_UNDERFLOW";
                case ErrorCode.OutOfMemory: return "GL_OUT_OF_MEMORY";
                case ErrorCode.InvalidFramebufferOperation: return "GL_INVALID_FRAMEBUFFER_OPERATION";
            }
            return FormatGlEnumHex((int)theGlError);
        }

        static string FormatGlEnumHex(int theGlEnum)
        {
            return $"0x{theGlEnum:X}";
            //char[] aBuff = new char[16];
            //Sprintf(aBuff, theGlEnum < (int)std::numeric_limits < uint16_t >::max()
            //              ? "0x%04X"
            //              : "0x%08X", theGlEnum);
            //return aBuff;
        }

        Graphic3d_TypeOfBackfacingModel myFaceCulling;   //!< back face culling mode enabled state (glIsEnabled (GL_CULL_FACE))

        //! Returns TRUE if sRGB rendering is supported and permitted.
        public bool ToRenderSRGB()
        {
            return HasSRGB()
               && !caps.sRGBDisable
               && !caps.ffpEnable;
        }
        //! @return value for GL_MAX_SAMPLES
        public int MaxMsaaSamples() { return myMaxMsaaSamples; }

        //! Function for getting power of to number larger or equal to input number.
        //! @param theNumber    number to 'power of two'
        //! @param theThreshold upper threshold
        //! @return power of two number
        public static int GetPowerOfTwo(int theNumber,
                                                 int theThreshold)
        {
            for (int p2 = 2; p2 <= theThreshold; p2 <<= 1)
            {
                if (theNumber <= p2)
                {
                    return p2;
                }
            }
            return theThreshold;
        }
        //! Returns cached GL_FRAMEBUFFER_SRGB state.
        //! If TRUE, GLSL program is expected to write linear RGB color.
        //! Otherwise, GLSL program might need manually converting result color into sRGB color space.
        public bool IsFrameBufferSRGB() { return myIsSRgbActive; }

        //! Returns TRUE if sRGB rendering is supported.
        public bool HasSRGB()
        {
            return hasTexSRGB
               && hasFboSRGB;
        }

        bool hasTexSRGB;         //!< sRGB texture    formats (desktop OpenGL 2.1, OpenGL ES 3.0 or OpenGL ES 2.0 + GL_EXT_sRGB)
        bool hasFboSRGB;         //!< sRGB FBO render targets (desktop OpenGL 2.1, OpenGL ES 3.0)

        public OpenGl_FeatureFlag hasGeometryStage;   //!< Complex flag indicating support of Geometry shader (desktop OpenGL 3.2, OpenGL ES 3.2, GL_EXT_geometry_shader)

        //! @return true if this context is valid (has been initialized)
        public bool IsValid()
        {
            return myIsInitialized;
        }

        //! Set GL_SHADE_MODEL value.
        public void SetShadeModel(Graphic3d_TypeOfShadingModel theModel)
        {
            if (core11ffp != null)
            {
                int aModel = theModel == Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_PhongFacet
                                            || theModel == Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_PbrFacet ? (int)All.Flat : (int)All.Smooth;
                if (myShadeModel == aModel)
                {
                    return;
                }
                myShadeModel = aModel;
                core11ffp.glShadeModel(aModel);
            }
        }
        public OpenGl_FeatureFlag hasSampleVariables; //!< Complex flag indicating support of MSAA variables in GLSL shader (desktop OpenGL 4.0, GL_ARB_sample_shading)
        public bool oesSampleVariables; //!< GL_OES_sample_variables

        public OpenGl_GlFunctions /*OpenGl_ArbFBO */arbFBO;             //!< GL_ARB_framebuffer_object
        public OpenGl_GlFunctions /*OpenGl_ArbFBOBlit */arbFBOBlit;         //!< glBlitFramebuffer function, moved out from OpenGl_ArbFBO structure for compatibility with OpenGL ES 2.0
        public bool arbSampleShading;   //!< GL_ARB_sample_shading
        public bool arbDepthClamp;      //!< GL_ARB_depth_clamp (on desktop 
        public OpenGl_GlCore11 core11ffp;  //!< OpenGL 1.1 core functionality
                                           //! @name public properties tracking current state
        public bool hasUintIndex;       //!< GLuint for index buffer is supported (always available on desktop; on OpenGL ES - since 3.0 or as extension GL_OES_element_index_uint)

        public OpenGl_MatrixState<float> ModelWorldState = new OpenGl_MatrixState<float>(); //!< state of orientation matrix
        public OpenGl_MatrixState<float> WorldViewState = new OpenGl_MatrixState<float>();  //!< state of orientation matrix
        public OpenGl_MatrixState<float> ProjectionState = new OpenGl_MatrixState<float>(); //!< state of projection  matrix
        OpenGl_GlCore32 core32;     //!< OpenGL 3.2 core profile

        internal bool GetResource<TheHandleType>(string theKey, ref TheHandleType theValue) where TheHandleType : class
        {
            OpenGl_Resource aResource = GetResource(theKey);
            if (aResource == null)
            {
                return false;
            }


            theValue = aResource as TheHandleType;
            return theValue != null;
        }

        //! Set resolution ratio.
        //! Note that this method rounds @theRatio to nearest integer.
        public void SetResolution(uint theResolution,
                            float theRatio,
                            float theScale)
        {
            myResolution = (uint)(theScale * theResolution + 0.5f);
            myRenderScale = theScale;
            myRenderScaleInv = 1.0f / theScale;
            SetResolutionRatio(theRatio * theScale);
        }

        //! Set resolution ratio.
        //! Note that this method rounds @theRatio to nearest integer.
        public void SetResolutionRatio(float theRatio)
        {
            myResolutionRatio = theRatio;
            myLineWidthScale = (float)Math.Max(1.0f, Math.Floor(theRatio + 0.5f));
        }

        float myResolutionRatio; //!< scaling factor for parameters like text size
                                 //!  to be properly displayed on device (screen / printer)
        float myLineWidthScale;  //!< scaling factor for line width

        uint myResolution;      //!< Pixels density (PPI), defines scaling factor for parameters like text size
        public OpenGl_Resource GetResource(string theKey)
        {
            return mySharedResources.IsBound(theKey) ? mySharedResources.Find(theKey) : null;
        }

        //! Return active draw buffer attached to a render target referred by index (layout location).
        public int DrawBuffer(int theIndex = 0)
        {
            return theIndex >= myDrawBuffers.Lower()
                && theIndex <= myDrawBuffers.Upper()
                 ? myDrawBuffers.Value(theIndex)
                 : 0; // GL_NONE
        }

        Graphic3d_Camera myCamera;          //!< active camera object
        OpenGl_FrameStats myFrameStats;      //!< structure accumulating frame statistics
        OpenGl_ShaderProgram myActiveProgram;   //!< currently active GLSL program
        OpenGl_TextureSet myActiveTextures;  //!< currently bound textures
                                             //!< currently active sampler objects

        //! Return structure holding frame statistics.
        public OpenGl_FrameStats FrameStats() { return myFrameStats; }

        //! @return active GLSL program
        public OpenGl_ShaderProgram ActiveProgram()
        {
            return myActiveProgram;
        }

        // =======================================================================
        // function : BindProgram
        // purpose  :
        // =======================================================================
        public bool BindProgram(OpenGl_ShaderProgram theProgram)
        {
            if (core20fwd == null)
            {
                return false;
            }
            else if (myActiveProgram == theProgram)
            {
                return true;
            }

            if (theProgram == null
            || !theProgram.IsValid())
            {
                if (myActiveProgram != null)
                {
                    core20fwd.glUseProgram(OpenGl_ShaderProgram.NO_PROGRAM);
                    myActiveProgram = null;
                }
                return false;
            }

            myActiveProgram = theProgram;
            core20fwd.glUseProgram(theProgram.ProgramId());
            return true;
        }

        //! Sets camera object to the context and update matrices.
        public void SetCamera(Graphic3d_Camera theCamera)
        {
            myCamera = theCamera;
            if (theCamera != null)
            {
                ProjectionState.SetCurrent(theCamera.ProjectionMatrixF());
                WorldViewState.SetCurrent(theCamera.OrientationMatrixF());
                ApplyProjectionMatrix();
                ApplyWorldViewMatrix();
            }
        }

        readonly OpenGl_Mat4 THE_IDENTITY_MATRIX = new NCollection_Mat4<float>();

        // =======================================================================
        // function : ApplyWorldViewMatrix
        // purpose  :
        // =======================================================================
        public void ApplyWorldViewMatrix()
        {
            if (myShaderManager.ModelWorldState().ModelWorldMatrix() != THE_IDENTITY_MATRIX)
            {
                myShaderManager.UpdateModelWorldStateTo(THE_IDENTITY_MATRIX);
            }
            if (myShaderManager.WorldViewState().WorldViewMatrix() != WorldViewState.Current())
            {
                myShaderManager.UpdateWorldViewStateTo(WorldViewState.Current());
            }
        }
        //! Switch read/draw buffers.
        public void SetReadDrawBuffer(int theBuffer)
        {
            SetReadBuffer(theBuffer);
            SetDrawBuffer(theBuffer);
        }

        //! Default Frame Buffer Object.
        public OpenGl_FrameBuffer DefaultFrameBuffer()
        {
            return myDefaultFbo;
        }
        OpenGl_FrameBuffer myDefaultFbo;      //!< default Frame Buffer Object

        public OpenGl_GlCore20 core20fwd;  //!< obsolete entry left for code portability; core20 should be used instead

        NCollection_Array1<int> myDrawBuffers = new();     //!< current draw buffers
        internal bool ShareResource(string theKey,
                OpenGl_Resource theResource)
        {
            if (theKey.IsEmpty() || theResource == null)
            {
                return false;
            }
            return mySharedResources.Bind(theKey, theResource);
        }

        internal int SetPolygonMode(int theMode)
        {
            if (myPolygonMode == theMode)
            {
                return myPolygonMode;
            }

            int anOldPolygonMode = myPolygonMode;
            myPolygonMode = theMode;
            //if (myGapi != Aspect_GraphicsLibrary_OpenGLES)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, (PolygonMode)theMode);
                //core11fwd->glPolygonMode(GL_FRONT_AND_BACK, (GLenum)theMode);
            }
            return anOldPolygonMode;

        }

        int myReadBuffer;      //!< current read buffer
        Aspect_GraphicsLibrary myGapi;           //!< GAPI name
        // =======================================================================
        // function : SetReadBuffer
        // purpose  :
        // =======================================================================
        public void SetReadBuffer(int theReadBuffer)
        {
            if (myGapi == Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGLES)
            {
                return;
            }

            myReadBuffer = !myIsStereoBuffers ? stereoToMonoBuffer(theReadBuffer) : theReadBuffer;
            if (myReadBuffer < GLConstants.GL_COLOR_ATTACHMENT0
             && arbFBO != null)
            {
                arbFBO.glBindFramebuffer(All.Framebuffer, OpenGl_FrameBuffer.NO_FRAMEBUFFER);
            }
            core11fwd.glReadBuffer(myReadBuffer);
        }

        // =======================================================================
        // function : SetDrawBuffer
        // purpose  :
        // =======================================================================
        public void SetDrawBuffer(int theDrawBuffer)
        {
            if (myGapi == Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGLES)
            {
                return;
            }

            int aDrawBuffer = !myIsStereoBuffers ? stereoToMonoBuffer(theDrawBuffer) : theDrawBuffer;
            if (aDrawBuffer < (int)All.ColorAttachment0
             && arbFBO != null)
            {
                arbFBO.glBindFramebuffer(All.Framebuffer, OpenGl_FrameBuffer.NO_FRAMEBUFFER);
            }
            core11fwd.glDrawBuffer(aDrawBuffer);

            myDrawBuffers.Init((int)All.None);
            myDrawBuffers.SetValue(0, aDrawBuffer);
        }

        public static int stereoToMonoBuffer(int theBuffer)
        {
            switch (theBuffer)
            {
                case GLConstants.GL_BACK_LEFT:
                case GLConstants.GL_BACK_RIGHT:
                    return GLConstants.GL_BACK;
                case GLConstants.GL_FRONT_LEFT:
                case GLConstants.GL_FRONT_RIGHT:
                    return GLConstants.GL_FRONT;
                default:
                    return theBuffer;
            }
        }
        internal bool ToUseVbo()
        {
            return core15fwd != null
               && !caps.vboDisable;
        }

        //! Return active graphics library.
        internal Aspect_GraphicsLibrary GraphicsLibrary()
        {
            return myGapi;
        }

        //! Allow GL_SAMPLE_ALPHA_TO_COVERAGE usage.
        public void SetAllowSampleAlphaToCoverage(bool theToEnable) { myAllowAlphaToCov = theToEnable; }
        bool myAllowAlphaToCov; //!< flag allowing   GL_SAMPLE_ALPHA_TO_COVERAGE usage


        //! Returns camera object.
        public Graphic3d_Camera Camera() { return myCamera; }


        internal void ApplyProjectionMatrix()
        {
            if (myShaderManager.ProjectionState().ProjectionMatrix() != ProjectionState.Current())
            {
                myShaderManager.UpdateProjectionStateTo(ProjectionState.Current());
            }
        }

        internal void ApplyModelViewMatrix()
        {

            if (myShaderManager.ModelWorldState().ModelWorldMatrix() != ModelWorldState.Current())
            {
                myShaderManager.UpdateModelWorldStateTo(ModelWorldState.Current());
            }
            if (myShaderManager.WorldViewState().WorldViewMatrix() != WorldViewState.Current())
            {
                myShaderManager.UpdateWorldViewStateTo(WorldViewState.Current());
            }
        }

        //! Return cached viewport definition (x, y, width, height).
        public int[] Viewport() { return myViewport; }

        internal bool IsFeedback()
        {
            return myRenderMode == (int)All.Feedback;

        }
        //! Enables/disables GL_FRAMEBUFFER_SRGB flag.
        //! This flag can be set to:
        //! - TRUE when writing into offscreen FBO (always expected to be in sRGB or RGBF formats).
        //! - TRUE when writing into sRGB-ready window buffer (might require choosing proper pixel format on window creation).
        //! - FALSE if sRGB rendering is not supported or sRGB-not-ready window buffer is used for drawing.
        //! @param[in] theIsFbo flag indicating writing into offscreen FBO (always expected sRGB-ready when sRGB FBO is supported)
        //!                     or into window buffer (FALSE, sRGB-readiness might vary).
        //! @param[in] theIsFboSRgb flag indicating off-screen FBO is sRGB-ready
        internal void SetFrameBufferSRGB(bool theIsFbo, bool theIsFboSRgb = true)
        {
            if (!hasFboSRGB)
            {
                myIsSRgbActive = false;
                return;
            }
            myIsSRgbActive = ToRenderSRGB()
                         && (theIsFbo || myIsSRgbWindow)
                         && theIsFboSRgb;
            if (!hasSRGBControl)
            {
                return;
            }

            if (myIsSRgbActive)
            {
                core11fwd.glEnable(All.FramebufferSrgb);
            }
            else
            {
                core11fwd.glDisable(All.FramebufferSrgb);
            }
        }

        bool myIsSRgbWindow;    //!< indicates that window buffer is sRGB-ready
        bool hasSRGBControl;     //!< sRGB write control (any desktop OpenGL, OpenGL ES + GL_EXT_sRGB_write_control extension)
        //! @return tool for management of clippings within this context.
        internal OpenGl_Clipping Clipping()
        {
            return myClippingState;
        }
        OpenGl_Clipping myClippingState = new OpenGl_Clipping(); //!< state of clip planes


        OpenGl_ShaderManager myShaderManager; //! support object for managing shader programs

        internal OpenGl_ShaderManager ShaderManager()
        {
            return myShaderManager;
        }

        //! Return cached flag indicating writing into color buffer is enabled or disabled (glColorMask).
        internal bool ColorMask()
        {
            return myColorMask.r();
        }

        NCollection_Vec4<bool> myColorMask = new NCollection_Vec4<bool>();       //!< flag indicating writing into color buffer is enabled or disabled (glColorMask)

        internal static bool CheckIsTransparent(OpenGl_Aspects theAspect,
            Graphic3d_PresentationAttributes theHighlight)
        {

            float anAlphaFront = 1.0f, anAlphaBack = 1.0f;
            return CheckIsTransparent(theAspect, theHighlight, anAlphaFront, anAlphaBack);
        }

        private static bool CheckIsTransparent(OpenGl_Aspects theAspect,
            Graphic3d_PresentationAttributes theHighlight,
            float theAlphaFront, float theAlphaBack)
        {
            Graphic3d_Aspects anAspect = (theHighlight != null && theHighlight.BasicFillAreaAspect() != null)
                                         ? (Graphic3d_Aspects)theHighlight.BasicFillAreaAspect()
                                           : theAspect.Aspect();

            bool toDistinguish = anAspect.Distinguish();
            Graphic3d_MaterialAspect aMatFrontSrc = anAspect.FrontMaterial();
            Graphic3d_MaterialAspect aMatBackSrc = toDistinguish
                                                        ? anAspect.BackMaterial()
                                                        : aMatFrontSrc;

            // handling transparency
            if (theHighlight != null
              && theHighlight.BasicFillAreaAspect() == null)
            {
                theAlphaFront = theHighlight.ColorRGBA().Alpha();
                theAlphaBack = theHighlight.ColorRGBA().Alpha();
            }
            else
            {
                theAlphaFront = aMatFrontSrc.Alpha();
                theAlphaBack = aMatBackSrc.Alpha();
            }

            if (anAspect.AlphaMode() == Graphic3d_AlphaMode.Graphic3d_AlphaMode_BlendAuto)
            {
                return theAlphaFront < 1.0f
                    || theAlphaBack < 1.0f;
            }
            // Graphic3d_AlphaMode_Mask and Graphic3d_AlphaMode_MaskBlend are not considered transparent here
            return anAspect.AlphaMode() == Graphic3d_AlphaMode.Graphic3d_AlphaMode_Blend;

        }

        internal bool IsCurrent()
        {
            /*if (myDisplay == null || myGContext == null)
            {
                return false;
            }
            return (((HDC)myDisplay == wglGetCurrentDC())
                && ((HGLRC)myGContext == wglGetCurrentContext()));*/
            return true;
        }

        // Import wglGetCurrentContext from opengl32.dll
        [DllImport("opengl32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr wglGetCurrentContext();

        [DllImport("opengl32.dll", SetLastError = true)]
        public static extern IntPtr wglGetCurrentDC();

        //! Initialize class from currently bound OpenGL context. Method should be called only once.
        //! @return false if no GL context is bound to the current thread
        public bool Init(bool theIsCoreProfile = false)
        {
            if (myIsInitialized)
            {
                return true; ;
            }
            myDisplay = wglGetCurrentDC();
            myGContext = (Aspect_RenderingContext)wglGetCurrentContext();

            if (myGContext == null)
            {
                return false;
            }

            init(theIsCoreProfile);
            myIsInitialized = true;
            return true;
        }

        //! Initialize class from specified surface and rendering context. Method should be called only once.
        //! The meaning of parameters is platform-specific.
        //!
        //! EGL:
        //! @code
        //!   Handle(Aspect_Window) theAspWin;
        //!   EGLSurface theEglSurf = eglCreateWindowSurface (theEglDisp, anEglConfig, (EGLNativeWindowType )theAspWin->NativeHandle(), NULL);
        //!   EGLDisplay theEglDisp = eglGetDisplay (EGL_DEFAULT_DISPLAY);
        //!   EGLContext theEglCtx  = eglCreateContext ((EGLDisplay )theEglDisp, anEglConfig, EGL_NO_CONTEXT, anEglCtxAttribs);
        //!   Handle(OpenGl_Context) aGlCtx = new OpenGl_Context();
        //!   aGlCtx->Init ((Aspect_Drawable )theEglSurf, (Aspect_Display )theEglDisp,  (Aspect_RenderingContext )theEglCtx);
        //! @endcode
        //!
        //! Windows (Win32):
        //! @code
        //!   Handle(WNT_Window) theAspWin;
        //!   HWND  theWindow   = (HWND )theAspWin->NativeHandle();
        //!   HDC   theDevCtx   = GetDC(theWindow);
        //!   HGLRC theGContext = wglCreateContext (theDevCtx);
        //!   Handle(OpenGl_Context) aGlCtx = new OpenGl_Context();
        //!   aGlCtx->Init ((Aspect_Drawable )theWindow, (Aspect_Display )theDevCtx, (Aspect_RenderingContext )theGContext);
        //! @endcode
        //!
        //! Linux (Xlib):
        //! @code
        //!   Handle(Xw_Window) theAspWin;
        //!   Window     theXWindow = (Window )theAspWin->NativeHandle();
        //!   Display*   theXDisp   = (Display* )theAspWin->DisplayConnection()->GetDisplayAspect();
        //!   GLXContext theGlxCtx  = glXCreateContext (theXDisp, aVis.get(), NULL, GL_TRUE);
        //!   Handle(OpenGl_Context) aGlCtx = new OpenGl_Context();
        //!   aGlCtx->Init ((Aspect_Drawable )theXWindow, (Aspect_Display )theXDisp,  (Aspect_RenderingContext )theGlxCtx);
        //! @endcode
        //!
        //! @param theSurface [in] surface / window          (EGLSurface | HWND  | GLXDrawable/Window)
        //! @param theDisplay [in] display or device context (EGLDisplay | HDC   | Display*)
        //! @param theContext [in] rendering context         (EGLContext | HGLRC | GLXContext | EAGLContext* | NSOpenGLContext*)
        //! @param theIsCoreProfile [in] flag indicating that passed GL rendering context has been created with Core Profile
        //! @return false if OpenGL context can not be bound to specified surface
        public bool Init(IntPtr theSurface,
                                       Aspect_Display theDisplay,
                                       Aspect_RenderingContext theContext,
                                       bool theIsCoreProfile = false)
        {
            Exceptions.Standard_ProgramError_Raise_if(myIsInitialized, "OpenGl_Context::Init() should be called only once!");
            myWindow = theSurface;
            myDisplay = theDisplay;
            myGContext = theContext;
            if (myGContext == null || !MakeCurrent())
            {
                return false;
            }

            init(theIsCoreProfile);
            myIsInitialized = true;
            return true;
        }

        IntPtr myWindow;   //!< surface           EGLSurface | HWND  | GLXDrawable

        //! Private initialization function that should be called only once.
        public void init(bool theIsCoreProfile)
        {
            // read version
            myGlVerMajor = 0;
            myGlVerMinor = 0;
            myHasMsaaTextures = false;
            myMaxMsaaSamples = 0;
            myMaxDrawBuffers = 1;
            myMaxColorAttachments = 1;
            myDefaultVao = 0;
            OpenGl_GlFunctions.readGlVersion(ref myGlVerMajor, ref myGlVerMinor);
            //mySupportedFormats.Clear();


            // setup shader generator
            myShaderManager.SetGapiVersion(myGlVerMajor, myGlVerMinor);
            myShaderManager.SetEmulateDepthClamp(!arbDepthClamp);


            myFuncs.load(this, theIsCoreProfile);


            if (hasDrawBuffers != OpenGl_FeatureFlag.OpenGl_FeatureNotAvailable)
            {
                core11fwd.glGetIntegerv(All.MaxDrawBuffers, ref myMaxDrawBuffers);
                core11fwd.glGetIntegerv(All.MaxColorAttachments, ref myMaxColorAttachments);
                if (myDrawBuffers.Length() < myMaxDrawBuffers)
                {
                    myDrawBuffers.Resize(0, myMaxDrawBuffers - 1, false);
                }
            }

            if (myGapi != Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGLES)
            {
                if (core32 != null && core11ffp == null)
                {
                    uint[] ar = [myDefaultVao];
                    core32.glGenVertexArrays(1, ar);
                    myDefaultVao = ar[0];
                }

                myTexClamp = (int)(IsGlGreaterEqual(1, 2) ? All.ClampToEdge : All.Clamp);

                GLint aStereo = 0;
                core11fwd.glGetIntegerv(All.Stereo, ref aStereo);
                myIsStereoBuffers = aStereo == 1;

                // get number of maximum clipping planes
                core11fwd.glGetIntegerv(All.MaxClipPlanes, ref myMaxClipPlanes);
            }

        }
        OpenGl_GlFunctions myFuncs;                //!< mega structure for all GL functions


        internal bool MakeCurrent()
        {
            /*if (myDisplay == null || myGContext == null)
            {
                Standard_ProgramError_Raise_if(myIsInitialized, "OpenGl_Context::Init() should be called before!");
                return false;
            }*/

            // technically it should be safe to activate already bound GL context
            // however some drivers (Intel etc.) may FAIL doing this for unknown reason
            if (IsCurrent())
            {
                myShaderManager.SetContext(this);
                return true;
            }
            /*else if (wglMakeCurrent((HDC)myDisplay, (HGLRC)myGContext) != TRUE)
            {
                // notice that glGetError() couldn't be used here!
                wchar_t* aMsgBuff = NULL;
                DWORD anErrorCode = GetLastError();
                FormatMessageW(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
                                NULL, anErrorCode, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), (wchar_t*)&aMsgBuff, 0, NULL);
                TCollection_ExtendedString aMsg("wglMakeCurrent() has failed. ");
                if (aMsgBuff != NULL)
                {
                    aMsg += (Standard_ExtString)aMsgBuff;
                    LocalFree(aMsgBuff);
                }
                PushMessage(GL_DEBUG_SOURCE_WINDOW_SYSTEM, GL_DEBUG_TYPE_ERROR, (unsigned int)anErrorCode, GL_DEBUG_SEVERITY_HIGH, aMsg);
                myIsInitialized = Standard_False;
                return Standard_False;
            }*/

            myShaderManager.SetContext(this);
            return true;

        }

        internal bool IsRender()
        {
            return myRenderMode == (int)All.Render;
            //#define GL_RENDER                         0x1C00
        }

        internal void PushMessage(All debugSourceApplication, All debugTypePerformance, int v, All debugSeverityLow, string aMsg)
        {
            //throw new NotImplementedException();
        }
        internal void PushMessage(int debugSourceApplication, int debugTypePerformance, int v, int debugSeverityLow, string aMsg)
        {
            //throw new NotImplementedException();
        }
        Aspect_RenderingContext myGContext; //!< rendering context EGLContext | HGLRC | GLXContext | EAGLContext* | NSOpenGLContext*


        //! Return rendering context (EGLContext | HGLRC | GLXContext | EAGLContext* | NSOpenGLContext*).
        public Aspect_RenderingContext RenderingContext() { return myGContext; }
        uint myDefaultVao;      //!< default Vertex Array Object

        internal void BindDefaultVao()
        {
            if (myDefaultVao == 0
        || core32 == null)
            {
                return;
            }

            core32.glBindVertexArray(myDefaultVao);
        }
        [DllImport("gdi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SwapBuffers(IntPtr hdc);

        public unsafe void SwapBuffers()
        {

            if (myDisplay != null)
            {
                GLFW.SwapBuffers((Window*)myDisplay.ToPointer());
                GLFW.PollEvents();

                SwapBuffers(myDisplay);
                core11fwd.glFlush();
            }

        }

        float myRenderScale;     //!< scaling factor for rendering resolution

        float myRenderScaleInv;  //!< scaling factor for rendering resolution (inverted value)
                                 //! Return TRUE if rendering scale factor is not 1.
        public bool HasRenderScale() { return Math.Abs(myRenderScale - 1.0f) > 0.0001f; }

        public void ResizeViewport(int[] theRect)
        {
            core11fwd.glViewport(theRect[0], theRect[1], theRect[2], theRect[3]);
            myViewport[0] = theRect[0];
            myViewport[1] = theRect[1];
            myViewport[2] = theRect[2];
            myViewport[3] = theRect[3];
            if (HasRenderScale())
            {
                myViewportVirt[0] = (int)(theRect[0] * myRenderScaleInv);
                myViewportVirt[1] = (int)(theRect[1] * myRenderScaleInv);
                myViewportVirt[2] = (int)(theRect[2] * myRenderScaleInv);
                myViewportVirt[3] = (int)(theRect[3] * myRenderScaleInv);
            }
            else
            {
                myViewportVirt[0] = theRect[0];
                myViewportVirt[1] = theRect[1];
                myViewportVirt[2] = theRect[2];
                myViewportVirt[3] = theRect[3];
            }
        }

        internal void SetSwapInterval(int mySwapInterval)
        {
            //??
        }
        int[] myViewport = new int[4];     //!< current viewport
        int[] myViewportVirt = new int[4]; //!< virtual viewport
        int myPointSpriteOrig; //!< GL_POINT_SPRITE_COORD_ORIGIN state (GL_UPPER_LEFT by default)
        int myRenderMode;      //!< value for active rendering mode
        int myShadeModel;      //!< currently used shade model (glShadeModel)
        int myPolygonMode;     //!< currently used polygon rasterization mode (glPolygonMode)
        public void FetchState()
        {
            if (myGapi == Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGLES)
            {
                return;
            }

            // cache feedback mode state
            if (core11ffp != null)
            {
                core11fwd.glGetIntegerv(GetPName.RenderMode, ref myRenderMode);
                core11fwd.glGetIntegerv(GetPName.ShadeModel, ref myShadeModel);
            }

            // cache read buffers state
            core11fwd.glGetIntegerv(GetPName.ReadBuffer, ref myReadBuffer);

            // cache draw buffers state
            if (myDrawBuffers.Length() < myMaxDrawBuffers)
            {
                myDrawBuffers.Resize(0, myMaxDrawBuffers - 1, false);
            }
            myDrawBuffers.Init((int)All.None);

            int aDrawBuffer = (int)All.None;
            if (myMaxDrawBuffers == 1)
            {
                core11fwd.glGetIntegerv(GetPName.DrawBuffer, ref aDrawBuffer);
                myDrawBuffers.SetValue(0, aDrawBuffer);
            }
            else
            {
                for (int anI = 0; anI < myMaxDrawBuffers; ++anI)
                {
                    core11fwd.glGetIntegerv(GetPName.DrawBuffer0 + anI, ref aDrawBuffer);
                    myDrawBuffers.SetValue(anI, aDrawBuffer);
                }
            }
        }

        internal void SetColor4fv(Graphic3d_Vec4 theColor)
        {
            SetColor4fv(new Vector4(theColor.x(), theColor.y(), theColor.z(), theColor.a()));
        }
        internal void SetColor4fv(Vector4 theColor)
        {
            if (myActiveProgram != null)
            {
                OpenGl_ShaderUniformLocation aLoc = myActiveProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCCT_COLOR);
                if (aLoc != null)
                {
                    myActiveProgram.SetUniform(this, aLoc, Vec4FromQuantityColor(theColor));
                }
            }
            else if (core11ffp != null)
            {
                core11ffp.glColor4fv(theColor);
            }
        }
        //! Convert Quantity_ColorRGBA into vec4
        //! with conversion or no conversion into non-linear sRGB
        //! basing on ToRenderSRGB() flag.
        public OpenGl_Vec4 Vec4FromQuantityColor(OpenGl_Vec4 theColor)
        {
            return myIsSRgbActive
                 ? Vec4LinearFromQuantityColor(theColor)
                 : Vec4sRGBFromQuantityColor(theColor);
        }

        public OpenGl_Vec4 Vec4FromQuantityColor(Vector4 theColor)
        {
            return Vec4FromQuantityColor(theColor.ToOpenGl_Vec4());

        }
        //! Convert Quantity_ColorRGBA into vec4.
        //! Quantity_Color is expected to be linear RGB, hence conversion is NOT required
        OpenGl_Vec4 Vec4LinearFromQuantityColor(OpenGl_Vec4 theColor) { return theColor; }

        //! Convert Quantity_ColorRGBA (linear RGB) into non-linear sRGB vec4.
        public OpenGl_Vec4 Vec4sRGBFromQuantityColor(OpenGl_Vec4 theColor)
        {
            return Quantity_ColorRGBA.Convert_LinearRGB_To_sRGB(theColor);
        }

        internal void SetColorMask(bool v)
        {

        }

        internal void Share(OpenGl_Context theShareCtx)
        {
            if (theShareCtx != null)
            {
                mySharedResources = theShareCtx.mySharedResources;
                //myDelayed = theShareCtx.myDelayed;
                //myUnusedResources = theShareCtx.myUnusedResources;
                myShaderManager = theShareCtx.myShaderManager;
            }
        }


        //! @return value for GL_MAX_TEXTURE_SIZE
        public int MaxTextureSize() { return myMaxTexDim; }
        public bool CheckExtension(string theExtString,
                                                 string theExtName)
        {
            if (theExtString == null)
            {
                return false;
            }
            return theExtString.Contains(theExtName);
            throw new NotImplementedException();
            // Search for theExtName in the extensions string.
            // Use of strstr() is not sufficient because extension names can be prefixes of other extension names.
            //char* aPtrIter = (char*)theExtString;
            //const char* aPtrEnd = aPtrIter + strlen(theExtString);
            //const size_t anExtNameLen = strlen(theExtName);
            //while (aPtrIter < aPtrEnd)
            //{
            //    const size_t n = strcspn(aPtrIter, " ");
            //    if ((n == anExtNameLen) && (strncmp(aPtrIter, theExtName, anExtNameLen) == 0))
            //    {
            //        return true;
            //    }
            //    aPtrIter += (n + 1);
            //}
            return false;
        }


        internal bool CheckExtension(string theExtName)
        {

            if (theExtName == null)
            {
                //# ifdef OCCT_DEBUG
                //                    std::cerr << "CheckExtension called with NULL string!\n";
                //#endif
                return false;
            }
            else if (caps.contextNoExtensions)
            {
                return false;
            }

            // available since OpenGL 3.0
            // and the ONLY way to check extensions with OpenGL 3.1+ core profile
            if (myGapi == Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGL
             && IsGlGreaterEqual(3, 0)
             && myFuncs.glGetStringi != null)
            {
                GLint anExtNb = 0;
                core11fwd.glGetIntegerv(All.NumExtensions, ref anExtNb);
                int anExtNameLen = theExtName.Length;
                for (GLint anIter = 0; anIter < anExtNb; ++anIter)
                {
                    string anExtension = myFuncs.glGetStringi(All.Extensions, (int)anIter);
                    int aTestExtNameLen = anExtension.Length;
                    if (aTestExtNameLen == anExtNameLen
                     && string.Compare(anExtension, theExtName) == 0)
                    {
                        return true;
                    }
                }
                return false;
            }

            // use old way with huge string for all extensions
            string anExtString = core11fwd.glGetString(All.Extensions);
            if (anExtString == null)
            {
                //Messenger()->Send("TKOpenGL: glGetString (GL_EXTENSIONS) has returned NULL! No GL context?", Message_Warning);
                return false;
            }
            if (!CheckExtension(anExtString, theExtName))
            {
                return false;
            }


            return true;

        }



        //! Access entire map of loaded OpenGL functions.
        public OpenGl_GlFunctions Functions() { return myFuncs; }

        internal void EnableFeatures()
        {

        }

        //! @return true if texture parameters GL_TEXTURE_BASE_LEVEL/GL_TEXTURE_MAX_LEVEL are supported.
        public bool HasTextureBaseLevel()
        {
            return myGapi == Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGLES
                 ? IsGlGreaterEqual(3, 0)
                 : IsGlGreaterEqual(1, 2);
        }


        //! @return tool for management of clippings within this context.
        public OpenGl_Clipping ChangeClipping() { return myClippingState; }


        bool myIsSRgbActive;    //!< flag indicating GL_FRAMEBUFFER_SRGB state

        int myAnisoMax;             //!< maximum level of anisotropy texture filter
        int myTexClamp;             //!< either GL_CLAMP_TO_EDGE (1.2+) or GL_CLAMP (1.1)
        int myMaxTexDim;            //!< value for GL_MAX_TEXTURE_SIZE
        int myMaxTexCombined;       //!< value for GL_MAX_COMBINED_TEXTURE_IMAGE_UNITS
        int myMaxTexUnitsFFP;       //!< value for GL_MAX_TEXTURE_UNITS (fixed-function pipeline only)
        int myMaxDumpSizeX;         //!< maximum FBO width  for image dump
        int myMaxDumpSizeY;         //!< maximum FBO height for image dump
        int myMaxClipPlanes;        //!< value for GL_MAX_CLIP_PLANES
        int myMaxMsaaSamples;       //!< value for GL_MAX_SAMPLES
        int myMaxDrawBuffers;       //!< value for GL_MAX_DRAW_BUFFERS
        int myMaxColorAttachments;  //!< value for GL_MAX_COLOR_ATTACHMENTS
        int myGlVerMajor;           //!< cached GL version major number
        int myGlVerMinor;           //!< cached GL version minor number
        bool myIsInitialized;        //!< flag indicates initialization state
        bool myIsStereoBuffers;      //!< context supports stereo buffering
        bool myHasMsaaTextures;      //!< context supports MSAA textures
        Aspect_Display myDisplay;  //!< display           EGLDisplay | HDC   | Display*
        public OpenGl_FeatureFlag hasDrawBuffers;     //!< Complex flag indicating support of multiple draw buffers (desktop OpenGL 2.0, OpenGL ES 3.0, GL_ARB_draw_buffers, GL_EXT_draw_buffers)
        public bool arbDrawBuffers;     //!< GL_ARB_draw_buffers
        public bool extDrawBuffers;     //!< GL_EXT_draw_buffers



        OpenGl_ResourcesMap mySharedResources; //!< shared resources with unique identification key
        public OpenGl_Caps caps; //!< context options
        internal _core11fwd core11fwd;
        internal _core15fwd core15fwd;


        //! Empty constructor. You should call Init() to perform initialization with bound GL context.


    }

}