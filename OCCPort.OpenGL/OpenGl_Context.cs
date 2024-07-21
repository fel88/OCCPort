using OpenTK.Graphics.OpenGL;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
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
        OpenGl_ShaderProgram myActiveProgram;   //!< currently active GLSL program
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

        OpenGl_GlCore20 core20fwd;  //!< obsolete entry left for code portability; core20 should be used instead

        NCollection_Array1 myDrawBuffers = new NCollection_Array1();     //!< current draw buffers
        internal bool ShareResource(string theKey,
                OpenGl_ShaderProgram theResource)
        {
            if (theKey.IsEmpty() || theResource == null)
            {
                return false;
            }
            return mySharedResources.Bind(theKey, theResource);
        }
        int myPolygonMode;
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
        bool myIsStereoBuffers;      //!< context supports stereo buffering
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

        internal Aspect_GraphicsLibrary GraphicsLibrary()
        {
            throw new NotImplementedException();
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

        int[] myViewport = new int[4];     //!< current viewport
        int[] myViewportVirt = new int[4]; //!< virtual viewport
        int myPointSpriteOrig; //!< GL_POINT_SPRITE_COORD_ORIGIN state (

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

            caps = (theCaps != null ? theCaps : new OpenGl_Caps());

        }
    }
}