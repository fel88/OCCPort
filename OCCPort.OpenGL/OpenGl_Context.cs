using OCCPort;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Intrinsics.Arm;
using static System.Net.Mime.MediaTypeNames;


namespace OCCPort.OpenGL
{
    public class OpenGl_Context
    {
        public OpenGl_ArbFBO arbFBO;             //!< GL_ARB_framebuffer_object
        public OpenGl_ArbFBOBlit arbFBOBlit;         //!< glBlitFramebuffer function, moved out from OpenGl_ArbFBO structure for compatibility with OpenGL ES 2.0
        bool arbSampleShading;   //!< GL_ARB_sample_shading
        bool arbDepthClamp;      //!< GL_ARB_depth_clamp (on desktop 
        public OpenGl_GlCore11 core11ffp;  //!< OpenGL 1.1 core functionality
        //! @name public properties tracking current state

        public OpenGl_MatrixState<float> ModelWorldState; //!< state of orientation matrix
        public OpenGl_MatrixState<float> WorldViewState;  //!< state of orientation matrix
        public OpenGl_MatrixState<float> ProjectionState; //!< state of projection  matrix
        OpenGl_GlCore32 core32;     //!< OpenGL 3.2 core profile
        internal bool GetResource<T>(string theShareKey, T theProgram)
        {
            throw new NotImplementedException();
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
                OpenGl_ShaderProgram theResource)
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

        internal Graphic3d_Camera Camera()
        {
            throw new NotImplementedException();
        }

        internal void ApplyProjectionMatrix()
        {
            throw new NotImplementedException();
        }

        internal void ApplyModelViewMatrix()
        {
            throw new NotImplementedException();
        }

        //! Return cached viewport definition (x, y, width, height).
        public int[] Viewport() { return myViewport; }

        internal bool IsFeedback()
        {
            throw new NotImplementedException();
        }

        internal void SetFrameBufferSRGB(bool v)
        {
            throw new NotImplementedException();
        }

        internal object Clipping()
        {
            throw new NotImplementedException();
        }

        OpenGl_ShaderManager myShaderManager; //! support object for managing shader programs

        internal OpenGl_ShaderManager ShaderManager()
        {
            return myShaderManager;
        }

        internal bool ColorMask()
        {
            throw new NotImplementedException();
        }

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
            throw new NotImplementedException();
        }

        internal void PushMessage(All debugSourceApplication, All debugTypePerformance, int v, All debugSeverityLow, string aMsg)
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

        public void SwapBuffers()
        {

            if (myDisplay != null)
            {
                //??SwapBuffers((HDC)myDisplay);
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
        Vector4 Vec4FromQuantityColor(Vector4 theColor)
        {
            return myIsSRgbActive
                 ? Vec4LinearFromQuantityColor(theColor)
                 : Vec4sRGBFromQuantityColor(theColor);
        }
        //! Convert Quantity_ColorRGBA into vec4.
        //! Quantity_Color is expected to be linear RGB, hence conversion is NOT required
        Vector4 Vec4LinearFromQuantityColor(Vector4 theColor) { return theColor; }
        //! Convert Quantity_ColorRGBA (linear RGB) into non-linear sRGB vec4.
        public Vector4 Vec4sRGBFromQuantityColor(Vector4 theColor)
        {
            return Quantity_ColorRGBA.Convert_LinearRGB_To_sRGB(theColor);
        }


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




        OpenGl_ResourcesMap mySharedResources; //!< shared resources with unique identification key
        public OpenGl_Caps caps; //!< context options
        internal _core11fwd core11fwd;
        internal _core15fwd core15fwd;


        //! Empty constructor. You should call Init() to perform initialization with bound GL context.

        public OpenGl_Context(OpenGl_Caps theCaps = null)
        {
            //core11ffp = null;

            core11fwd = new _core11fwd();
            core15fwd = new _core15fwd();
            /*
             *  mySupportedFormats (new Image_SupportedFormats()),
  myAnisoMax   (1),
  myTexClamp   (GL_CLAMP_TO_EDGE),
  myMaxTexDim  (1024),
  myMaxTexCombined (1),
  myMaxTexUnitsFFP (1),
  myMaxDumpSizeX (1024),
  myMaxDumpSizeY (1024),
  myMaxClipPlanes (6),
  myMaxMsaaSamples(0),
  myMaxDrawBuffers (1),
  myMaxColorAttachments (1),
  myGlVerMajor (0),
  myGlVerMinor (0),
  myIsInitialized (Standard_False),
  myIsStereoBuffers (Standard_False),
  myHasMsaaTextures (Standard_False),
  myIsGlNormalizeEnabled (Standard_False),
  mySpriteTexUnit (Graphic3d_TextureUnit_PointSprite),
  myHasRayTracing (Standard_False),
  myHasRayTracingTextures (Standard_False),
  myHasRayTracingAdaptiveSampling (Standard_False),
  myHasRayTracingAdaptiveSamplingAtomic (Standard_False),
  myHasPBR (Standard_False),
  myPBREnvLUTTexUnit       (Graphic3d_TextureUnit_PbrEnvironmentLUT),
  myPBRDiffIBLMapSHTexUnit (Graphic3d_TextureUnit_PbrIblDiffuseSH),
  myPBRSpecIBLMapTexUnit   (Graphic3d_TextureUnit_PbrIblSpecular),
  myShadowMapTexUnit       (Graphic3d_TextureUnit_ShadowMap),
  myDepthPeelingDepthTexUnit (Graphic3d_TextureUnit_DepthPeelingDepth),
  myDepthPeelingFrontColorTexUnit (Graphic3d_TextureUnit_DepthPeelingFrontColor),
  myFrameStats (new OpenGl_FrameStats()),
  myActiveMockTextures (0),
  myActiveHatchType (Aspect_HS_SOLID),
  myHatchIsEnabled (false),
  myPointSpriteOrig (GL_UPPER_LEFT),
  myRenderMode (GL_RENDER),
  myShadeModel (GL_SMOOTH),
  myPolygonMode (GL_FILL),
  myFaceCulling (Graphic3d_TypeOfBackfacingModel_DoubleSided),
  myReadBuffer (0),
  myDrawBuffers (0, 7),
  myDefaultVao (0),
  myColorMask (true),
  myAlphaToCoverage (false),
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


        }
    }
}