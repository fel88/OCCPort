using OpenTK.Compute.OpenCL;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace OCCPort.OpenGL
{
    public class OpenGl_View : Graphic3d_CView
    {
        OpenGl_FrameBuffer[] myMainSceneFbos;
        OpenGl_FrameBuffer[] myMainSceneFbosOit;
        public OpenGl_View(Graphic3d_StructureManager theMgr,
            OpenGl_GraphicDriver theDriver,
            OpenGl_Caps theCaps,
            OpenGl_StateCounter myStateCounter)
            : base(theMgr)
        {
            myDriver = theDriver;
            myCaps = theCaps;
            myMainSceneFbos = new OpenGl_FrameBuffer[2];
            myMainSceneFbosOit = new OpenGl_FrameBuffer[2];
            myZLayers = new OpenGl_LayerList();

            myMainSceneFbos[0] = new OpenGl_FrameBuffer("fbo0_main");
            myMainSceneFbos[1] = new OpenGl_FrameBuffer("fbo1_main");
            myMainSceneFbosOit[0] = new OpenGl_FrameBuffer("fbo0_main_oit");
            myMainSceneFbosOit[1] = new OpenGl_FrameBuffer("fbo1_main_oit");


            myWorkspace = new OpenGl_Workspace(this, null);

        }
        //! Return true if view content cache has been invalidated.
        public override bool IsInvalidated() { return !myBackBufferRestored; }
        public override void eraseStructure(Graphic3d_CStructure theStructure)
        {
            OpenGl_Structure aStruct = (OpenGl_Structure)(theStructure);
            myZLayers.RemoveStructure(aStruct);
        }

        // =======================================================================
        // function : SetWindow
        // purpose  :
        // =======================================================================
        public override void SetWindow(Graphic3d_CView theParentVIew,
                                  Aspect_Window theWindow,
                                  Aspect_RenderingContext theContext)
        {
            if (theContext != null
&& theParentVIew != null)
            {
                throw new Standard_ProgramError("OpenGl_View::SetWindow(), internal error");
            }

            if (myParentView != null)
            {
                myParentView.RemoveSubview(this);
                myParentView = null;
            }
            OpenGl_View aParentView = (OpenGl_View)(theParentVIew);
            if (theParentVIew != null)
            {
                if (aParentView == null
                 || aParentView.GlWindow() == null
                 || aParentView.GlWindow().GetGlContext() == null)
                {
                    throw new Standard_ProgramError("OpenGl_View::SetWindow(), internal error");
                }

                myParentView = aParentView;
                myParentView.AddSubview(this);

                Aspect_NeutralWindow aSubWindow = (Aspect_NeutralWindow)(theWindow);
                SubviewResized(aSubWindow);

                OpenGl_Window aParentGlWindow = aParentView.GlWindow();
                Aspect_RenderingContext aRendCtx = aParentGlWindow.GetGlContext().RenderingContext();
                myWindow = myDriver.CreateRenderWindow(aParentGlWindow.PlatformWindow(), theWindow, aRendCtx);
            }
            else
            {
                myWindow = myDriver.CreateRenderWindow(theWindow, theWindow, theContext);
            }
            if (myWindow == null)
            {
                throw new Standard_ProgramError("OpenGl_View::SetWindow, Failed to create OpenGl window");
            }
            myWorkspace = new OpenGl_Workspace(this, myWindow);

        }
        public override void Resized()
        {
            base.Resized();
            if (myWindow != null)
            {
                myWindow.Resize();
            }
        }

        //! Returns OpenGL window implementation.
        public OpenGl_Window GlWindow()
        {
            return myWindow;
        }

        //! Add a layer to the view.
        //! @param theNewLayerId  [in] id of new layer, should be > 0 (negative values are reserved for default layers).
        //! @param theSettings    [in] new layer settings
        //! @param theLayerBefore [in] id of layer to append new layer after
        public void InsertLayerAfter(Graphic3d_ZLayerId theLayerId,

                                                  Graphic3d_ZLayerSettings theSettings,
                                                  Graphic3d_ZLayerId theLayerBefore)
        {
            myZLayers.InsertLayerAfter(theLayerId, theSettings, theLayerBefore);

        }


        OpenGl_GraphicDriver myDriver;
        OpenGl_Window myWindow;
        OpenGl_Workspace myWorkspace;
        OpenGl_Caps myCaps;
        public bool myWasRedrawnGL;

        //GLint myFboColorFormat;        //!< sized format for color attachments
        //  GLint myFboDepthFormat;        //!< sized format for depth-stencil attachments
        OpenGl_ColorFormats myFboOitColorConfig;     //!< selected color format configuration for OIT color attachments
                                                     // Handle(OpenGl_FrameBuffer) myMainSceneFbos[2];
                                                     //  Handle(OpenGl_FrameBuffer) myMainSceneFbosOit[2];      //!< Additional buffers for transparent draw of main layer.
                                                     // Handle(OpenGl_FrameBuffer) myImmediateSceneFbos[2];    //!< Additional buffers for immediate layer in stereo mode.
                                                     //Handle(OpenGl_FrameBuffer) myImmediateSceneFbosOit[2]; //!< Additional buffers for transparency draw of immediate layer.
        OpenGl_FrameBuffer myXrSceneFbo;               //!< additional FBO (without MSAA) for submitting to XR
        OpenGl_DepthPeeling myDepthPeelingFbos;   //!< additional buffers for depth peeling
        OpenGl_ShadowMapArray myShadowMaps;         //!< additional FBOs for shadow map rendering
        OpenGl_VertexBuffer myFullScreenQuad;        //!< Vertices for full-screen quad rendering.
        OpenGl_VertexBuffer myFullScreenQuadFlip;
        bool myToFlipOutput;          //!< Flag to draw result image upside-down
        uint myFrameCounter;          //!< redraw counter, for debugging
        bool myHasFboBlit;            //!< disable FBOs on failure
        bool myToDisableOIT;          //!< disable OIT on failure
        bool myToDisableOITMSAA;      //!< disable OIT with MSAA on failure
        bool myToDisableMSAA;         //!< disable MSAA after failure
        bool myTransientDrawToFront; //!< optimization flag for immediate mode (to render directly to the front buffer)
        bool myBackBufferRestored;
        bool myIsImmediateDrawn;     //!< flag indicates that immediate mode buffer contains some data

        OpenGl_Aspects myTextureParams;                     //!< Stores texture and its parameters for textured background
        OpenGl_Aspects myCubeMapParams;                     //!< Stores cubemap and its parameters for cubemap background
        OpenGl_Aspects myColoredQuadParams;                 //!< Stores parameters for gradient (corner mode) background
        OpenGl_BackgroundArray[] myBackgrounds = new OpenGl_BackgroundArray[(int)Graphic3d_TypeOfBackground.Graphic3d_TypeOfBackground_NB]; //!< Array of primitive arrays of different background types
        OpenGl_TextureSet myTextureEnv;
        OpenGl_Texture mySkydomeTexture;

        //=======================================================================
        //function : drawBackground
        //purpose  :
        //=======================================================================
        void drawBackground(OpenGl_Workspace theWorkspace,
                                  Graphic3d_Camera.Projection theProjection)
        {
            var aCtx = theWorkspace.GetGlContext();
            bool wasUsedZBuffer = theWorkspace.SetUseZBuffer(false);
            if (wasUsedZBuffer)
            {
                GL.Disable(EnableCap.DepthTest);
                //aCtx->core11fwd->glDisable(GL_DEPTH_TEST);
            }

#if GL_DEPTH_CLAMP
            const bool wasDepthClamped = aCtx->arbDepthClamp && aCtx->core11fwd->glIsEnabled(GL_DEPTH_CLAMP);
            if (aCtx->arbDepthClamp && !wasDepthClamped)
            {
                // make sure background is always drawn (workaround skybox rendering on some hardware)
                aCtx->core11fwd->glEnable(GL_DEPTH_CLAMP);
            }
#endif

            if (myBackgroundType == Graphic3d_TypeOfBackground.Graphic3d_TOB_CUBEMAP)
            {
                updateSkydomeBg(aCtx);
                if (myCubeMapParams.Aspect().ShaderProgram() != null)
                {
                    myCubeMapParams.Aspect().ShaderProgram().PushVariableInt("uZCoeff", myCubeMapBackground.ZIsInverted() ? -1 : 1);
                    myCubeMapParams.Aspect().ShaderProgram().PushVariableInt("uYCoeff", myCubeMapBackground.IsTopDown() ? 1 : -1);
                    OpenGl_Aspects anOldAspectFace = theWorkspace.SetAspects(myCubeMapParams);

                    myBackgrounds[(int)Graphic3d_TypeOfBackground.Graphic3d_TOB_CUBEMAP].Render(theWorkspace, theProjection);

                    theWorkspace.SetAspects(anOldAspectFace);
                }
            }
            else if (myBackgroundType == Graphic3d_TypeOfBackground.Graphic3d_TOB_GRADIENT
                  || myBackgroundType == Graphic3d_TypeOfBackground.Graphic3d_TOB_TEXTURE)
            {
                // Drawing background gradient if:
                // - gradient fill type is not Aspect_GradientFillMethod_None and
                // - either background texture is no specified or it is drawn in Aspect_FM_CENTERED mode
                if (myBackgrounds[(int)Graphic3d_TypeOfBackground.Graphic3d_TOB_GRADIENT].IsDefined()
                  && (!myTextureParams.Aspect().ToMapTexture()
                    || myBackgrounds[(int)Graphic3d_TypeOfBackground.Graphic3d_TOB_TEXTURE].TextureFillMethod() == Aspect_FillMethod.Aspect_FM_CENTERED
                    || myBackgrounds[(int)Graphic3d_TypeOfBackground.Graphic3d_TOB_TEXTURE].TextureFillMethod() == Aspect_FillMethod.Aspect_FM_NONE))
                {
                    if (myBackgrounds[(int)Graphic3d_TypeOfBackground.Graphic3d_TOB_GRADIENT].GradientFillMethod() >= Aspect_GradientFillMethod.Aspect_GradientFillMethod_Corner1
                     && myBackgrounds[(int)Graphic3d_TypeOfBackground.Graphic3d_TOB_GRADIENT].GradientFillMethod() <= Aspect_GradientFillMethod.Aspect_GradientFillMethod_Corner4)
                    {
                        OpenGl_Aspects anOldAspectFace = theWorkspace.SetAspects(myColoredQuadParams);

                        myBackgrounds[(int)Graphic3d_TypeOfBackground.Graphic3d_TOB_GRADIENT].Render(theWorkspace, theProjection);

                        theWorkspace.SetAspects(anOldAspectFace);
                    }
                    else
                    {
                        myBackgrounds[(int)Graphic3d_TypeOfBackground.Graphic3d_TOB_GRADIENT].Render(theWorkspace, theProjection);
                    }
                }

                // Drawing background image if it is defined
                // (texture is defined and fill type is not Aspect_FM_NONE)
                if (myBackgrounds[(int)Graphic3d_TypeOfBackground.Graphic3d_TOB_TEXTURE].IsDefined()
                  && myTextureParams.Aspect().ToMapTexture())
                {
                    aCtx.core11fwd.glDisable((int)All.Blend);

                    OpenGl_Aspects anOldAspectFace = theWorkspace.SetAspects(myTextureParams);
                    myBackgrounds[(int)Graphic3d_TypeOfBackground.Graphic3d_TOB_TEXTURE].Render(theWorkspace, theProjection);
                    theWorkspace.SetAspects(anOldAspectFace);
                }
            }

            if (wasUsedZBuffer)
            {
                theWorkspace.SetUseZBuffer(true);
                aCtx.core11fwd.glEnable(All.DepthTest);
            }
#if GL_DEPTH_CLAMP
            if (aCtx->arbDepthClamp && !wasDepthClamped)
            {
                aCtx->core11fwd->glDisable(GL_DEPTH_CLAMP);
            }
#endif

        }

        private void updateSkydomeBg(OpenGl_Context aCtx)
        {
            throw new NotImplementedException();
        }
        //! Framebuffers for OpenGL output.
        OpenGl_FrameBuffer myOpenGlFBO;
        OpenGl_FrameBuffer myOpenGlFBO2;
        int myFboColorFormat;        //!< sized format for color attachments
        int myFboDepthFormat;        //!< sized format for depth-stencil 

        public enum RaytraceUpdateMode
        {

            //Describes update mode(state).

            OpenGl_GUM_CHECK,

            //check geometry state
            OpenGl_GUM_PREPARE,

            //collect unchanged objects
            OpenGl_GUM_REBUILD,

            //rebuild changed and new objects
        }
        public enum RaytraceInitStatus
        {
            //Result of OpenGL shaders initialization.
            OpenGl_RT_NONE,
            OpenGl_RT_INIT,
            OpenGl_RT_FAIL
        }
        //! @name fields related to ray-tracing

        //! Result of RT/PT shaders initialization.
        protected RaytraceInitStatus myRaytraceInitStatus;

        //! Is ray-tracing geometry data valid?
        protected bool myIsRaytraceDataValid;

        //! True if warning about missing extension GL_ARB_bindless_texture has been displayed.
        protected bool myIsRaytraceWarnTextures;

        //! 3D scene geometry data for ray-tracing.
        protected OpenGl_RaytraceGeometry myRaytraceGeometry;

        public override void displayStructure(Graphic3d_CStructure theStructure,
                                     Graphic3d_DisplayPriority thePriority)
        {
            OpenGl_Structure aStruct = (OpenGl_Structure)(theStructure);
            Graphic3d_ZLayerId aZLayer = aStruct.ZLayer();
            myZLayers.AddStructure(aStruct, aZLayer, thePriority);
        }

        //=======================================================================
        public void renderStructs(Graphic3d_Camera.Projection theProjection,
                                         OpenGl_FrameBuffer theReadDrawFbo,
                                         OpenGl_FrameBuffer theOitAccumFbo,
                                  bool theToDrawImmediate)
        {
            if (myIsSubviewComposer)
            {
                return;
            }

            myZLayers.UpdateCulling(myWorkspace, theToDrawImmediate);
            if (myZLayers.NbStructures() <= 0)
            {
                return;
            }

            OpenGl_Context aCtx = myWorkspace.GetGlContext();
            bool toRenderGL = theToDrawImmediate ||
              myRenderParams.Method != Graphic3d_RenderingMode.Graphic3d_RM_RAYTRACING ||
              myRaytraceInitStatus == RaytraceInitStatus.OpenGl_RT_FAIL ||
              aCtx.IsFeedback();

            if (!toRenderGL)
            {
                Graphic3d_Vec2i aSizeXY = theReadDrawFbo != null
                                              ? theReadDrawFbo.GetVPSize()
                                              : new Graphic3d_Vec2i(myWindow.Width(), myWindow.Height());

                toRenderGL = !initRaytraceResources(aSizeXY.x(), aSizeXY.y(), aCtx)
                          || !updateRaytraceGeometry(RaytraceUpdateMode.OpenGl_GUM_CHECK, myId, aCtx);

                toRenderGL |= !myIsRaytraceDataValid; // if no ray-trace data use OpenGL

                if (!toRenderGL)
                {
                    myOpenGlFBO.InitLazy(aCtx, aSizeXY, myFboColorFormat, myFboDepthFormat, 0);
                    if (theReadDrawFbo != null)
                    {
                        theReadDrawFbo.UnbindBuffer(aCtx);
                    }

                    // Prepare preliminary OpenGL output
                    if (aCtx.arbFBOBlit != null)
                    {
                        // Render bottom OSD layer
                        myZLayers.Render(myWorkspace, theToDrawImmediate, OpenGl_LayerFilter.OpenGl_LF_Bottom, theReadDrawFbo, theOitAccumFbo);

                        int aPrevFilter = myWorkspace.RenderFilter() & ~(int)(OpenGl_RenderFilter.OpenGl_RenderFilter_NonRaytraceableOnly);
                        myWorkspace.SetRenderFilter(aPrevFilter | (int)OpenGl_RenderFilter.OpenGl_RenderFilter_NonRaytraceableOnly);

                        {
                            if (theReadDrawFbo != null)
                            {
                                theReadDrawFbo.BindDrawBuffer(aCtx);
                            }
                            else
                            {
                                aCtx.arbFBO.glBindFramebuffer(All.DrawFramebuffer, 0);
                                aCtx.SetFrameBufferSRGB(false);
                            }

                            // Render non-polygonal elements in default layer
                            myZLayers.Render(myWorkspace, theToDrawImmediate, OpenGl_LayerFilter.OpenGl_LF_RayTracable, theReadDrawFbo, theOitAccumFbo);
                        }
                        myWorkspace.SetRenderFilter(aPrevFilter);
                    }

                    if (theReadDrawFbo != null)
                    {
                        theReadDrawFbo.BindBuffer(aCtx);
                    }
                    else
                    {
                        aCtx.arbFBO.glBindFramebuffer(All.Framebuffer, 0);
                        aCtx.SetFrameBufferSRGB(false);
                    }

                    // Reset OpenGl aspects state to default to avoid enabling of
                    // backface culling which is not supported in ray-tracing.
                    myWorkspace.ResetAppliedAspect();

                    // Ray-tracing polygonal primitive arrays
                    raytrace(aSizeXY.x(), aSizeXY.y(), theProjection, theReadDrawFbo, aCtx);

                    // Render upper (top and topmost) OpenGL layers
                    myZLayers.Render(myWorkspace, theToDrawImmediate, OpenGl_LayerFilter.OpenGl_LF_Upper, theReadDrawFbo, theOitAccumFbo);
                }
            }

            // Redraw 3D scene using OpenGL in standard
            // mode or in case of ray-tracing failure
            if (toRenderGL)
            {
                myZLayers.Render(myWorkspace, theToDrawImmediate, OpenGl_LayerFilter.OpenGl_LF_All, theReadDrawFbo, theOitAccumFbo);

                // Set flag that scene was redrawn by standard pipeline
                myWasRedrawnGL = true;
            }
        }


        private void raytrace(double v1, double v2, Graphic3d_Camera.Projection theProjection, OpenGl_FrameBuffer theReadDrawFbo, OpenGl_Context aCtx)
        {
            throw new NotImplementedException();
        }

        private bool updateRaytraceGeometry(object openGl_GUM_CHECK, int myId, OpenGl_Context aCtx)
        {
            throw new NotImplementedException();
        }

        private bool initRaytraceResources(double v1, double v2, OpenGl_Context aCtx)
        {
            throw new NotImplementedException();
        }

        void redraw(Graphic3d_Camera.Projection theProjection,
                          OpenGl_FrameBuffer theReadDrawFbo,
                          OpenGl_FrameBuffer theOitAccumFbo)
        {
            render(theProjection, theReadDrawFbo, theOitAccumFbo, false);
        }
        OpenGl_FrameBuffer myFBO;
        //=======================================================================
        //function : SetFBO
        //purpose  :
        //=======================================================================
        public void SetFBO(object theFbo)
        {
            myFBO = (OpenGl_FrameBuffer)theFbo;
        }//=======================================================================
         //function : FBOCreate
         //purpose  :
         //=======================================================================
        public object FBOCreate(int theWidth,
                                                   int theHeight)
        {
            return myWorkspace.FBOCreate(theWidth, theHeight);
        }

        public override void Redraw()
        {
            bool wasDisabledMSAA = myToDisableMSAA;
            bool hadFboBlit = myHasFboBlit;
            if (myRenderParams.Method == Graphic3d_RenderingMode.Graphic3d_RM_RAYTRACING
            && !myCaps.vboDisable
            && !myCaps.keepArrayData)
            {
                // caps are shared across all views, thus we need to invalidate all of them
                // if (myWasRedrawnGL) { myStructureManager->SetDeviceLost(); }
                myDriver.setDeviceLost();
                myCaps.keepArrayData = true;
            }

            if (!myWorkspace.Activate())
            {
                return;
            }

            //// implicitly disable VSync when using HMD composer (can be mirrored in window for debugging)
            myWindow.SetSwapInterval(IsActiveXR());

            ++myFrameCounter;
            OpenGl_Context aCtx = myWorkspace.GetGlContext();
            //aCtx->FrameStats()->FrameStart(myWorkspace->View(), false);
            //aCtx->SetLineFeather(myRenderParams.LineFeather);

            //const Standard_Integer anSRgbState = aCtx->ToRenderSRGB() ? 1 : 0;
            //if (mySRgbState != -1
            // && mySRgbState != anSRgbState)
            //{
            //    releaseSrgbResources(aCtx);
            //    initTextureEnv(aCtx);
            //}
            //mySRgbState = anSRgbState;
            //aCtx->ShaderManager()->UpdateSRgbState();

            //// release pending GL resources
            //aCtx->ReleaseDelayed();

            //// fetch OpenGl context state
            //aCtx->FetchState();

            Graphic3d_Camera.Projection aProjectType = myCamera.ProjectionType();

            if (!prepareFrameBuffers(aProjectType))
            {
                myBackBufferRestored = false;
                myIsImmediateDrawn = false;
                return;
            }
            OpenGl_FrameBuffer aFrameBuffer = myFBO;
            bool toSwap = aCtx.IsRender()
          && !aCtx.caps.buffersNoSwap
          && aFrameBuffer == null
          && (!IsActiveXR() || myRenderParams.ToMirrorComposer);
            if (aFrameBuffer == null
             && aCtx.DefaultFrameBuffer() != null
             && aCtx.DefaultFrameBuffer().IsValid())
            {
                aFrameBuffer = aCtx.DefaultFrameBuffer();
            }

            if (aProjectType == Graphic3d_Camera.Projection.Projection_Stereo)
            {
            }
            else
            {
                OpenGl_FrameBuffer aMainFbo = myMainSceneFbos[0].IsValid() ? myMainSceneFbos[0] : aFrameBuffer;
                OpenGl_FrameBuffer aMainFboOit = myMainSceneFbosOit[0].IsValid() ? myMainSceneFbosOit[0] : null;
                redraw(aProjectType, aMainFbo, aMainFboOit);
            }

            /*
             
             
             ............
             */

            // bind default FBO
            bindDefaultFbo();

            if (wasDisabledMSAA != myToDisableMSAA
             || hadFboBlit != myHasFboBlit)
            {
                // retry on error
                Redraw();
            }

            // reset state for safety
            aCtx.BindProgram(null);
            if (aCtx.caps.ffpEnable)
            {
                aCtx.ShaderManager().PushState(null);
            }

            // Swap the buffers
            if (toSwap
             && myParentView == null)
            {
                aCtx.SwapBuffers();
                if (!myMainSceneFbos[0].IsValid())
                {
                    myBackBufferRestored = false;
                }
            }
            else
            {
                aCtx.core11fwd.glFlush();
            }

            // reset render mode state
            aCtx.FetchState();
            aCtx.FrameStats().FrameEnd(myWorkspace.View(), false);

            myWasRedrawnGL = true;
        }

        //! Format Frame Buffer format for logging messages.
        static string printFboFormat(OpenGl_FrameBuffer theFbo)
        {
            return "" + theFbo.GetInitVPSizeX() + "x" + theFbo.GetInitVPSizeY() + "@" + theFbo.NbSamples();
        }


        bool prepareFrameBuffers(Graphic3d_Camera.Projection theProj)
        {
            theProj = myCamera.ProjectionType();
            OpenGl_Context aCtx = myWorkspace.GetGlContext();

            int aSizeX = 0, aSizeY = 0;
            OpenGl_FrameBuffer aFrameBuffer = myFBO;
            if (aFrameBuffer != null)
            {
                aSizeX = aFrameBuffer.GetVPSizeX();
                aSizeY = aFrameBuffer.GetVPSizeY();
            }
            else if (IsActiveXR())
            {
                aSizeX = myXRSession.RecommendedViewport().X;
                aSizeY = myXRSession.RecommendedViewport().Y;
            }
            else
            {
                aSizeX = myWindow.Width();
                aSizeY = myWindow.Height();
            }

            Graphic3d_Vec2i aRendSize = new Graphic3d_Vec2i((int)(myRenderParams.RenderResolutionScale * aSizeX + 0.5f),
                                  (int)(myRenderParams.RenderResolutionScale * aSizeY + 0.5f));
            if (aSizeX < 1
             || aSizeY < 1
             || aRendSize.x() < 1
             || aRendSize.y() < 1)
            {
                myBackBufferRestored = false;
                myIsImmediateDrawn = false;
                return false;
            }

            //// determine multisampling parameters
            //int aNbSamples = !myToDisableMSAA && aSizeX == aRendSize.x()
            //                            ? Max(Min(myRenderParams.NbMsaaSamples, aCtx.MaxMsaaSamples()), 0)
            //                            : 0;
            //if (aNbSamples != 0)
            //{
            //    aNbSamples = OpenGl_Context::GetPowerOfTwo(aNbSamples, aCtx.MaxMsaaSamples());
            //}
            //// Only MSAA textures can be blit into MSAA target,
            //// while render buffers could be resolved only into non-MSAA targets.
            //// As result, within obsolete OpenGL ES 3.0 context, we may create only one MSAA render buffer for main scene content
            //// and blit it into non-MSAA immediate FBO.
            //const bool hasTextureMsaa = aCtx.HasTextureMultisampling();

            //bool toUseOit = myRenderParams.TransparencyMethod != Graphic3d_RTM_BLEND_UNORDERED
            //             && !myIsSubviewComposer
            //             && checkOitCompatibility(aCtx, aNbSamples > 0);

            //const bool toInitImmediateFbo = myTransientDrawToFront && !myIsSubviewComposer
            //                             && (!aCtx->caps->useSystemBuffer || (toUseOit && HasImmediateStructures()));

            //if (aFrameBuffer == null
            // && !aCtx->DefaultFrameBuffer().IsNull()
            // && aCtx->DefaultFrameBuffer()->IsValid())
            //{
            //    aFrameBuffer = aCtx->DefaultFrameBuffer().operator->();
            //}

            //if (myHasFboBlit
            // && (myTransientDrawToFront
            //  || theProj == Graphic3d_Camera.Projection.Projection_Stereo
            //  || aNbSamples != 0
            //  || toUseOit
            //  || aSizeX != aRendSize.x()))
            //{
            //    if (myMainSceneFbos[0].GetVPSize() != aRendSize
            //     || myMainSceneFbos[0].NbSamples() != aNbSamples)
            //    {
            //        if (!myTransientDrawToFront)
            //        {
            //            myImmediateSceneFbos[0].Release(aCtx.operator->());
            //            myImmediateSceneFbos[1].Release(aCtx.operator->());
            //            myImmediateSceneFbos[0].ChangeViewport(0, 0);
            //            myImmediateSceneFbos[1].ChangeViewport(0, 0);
            //        }

            //        // prepare FBOs containing main scene
            //        // for further blitting and rendering immediate presentations on top
            //        if (aCtx.core20fwd != null)
            //        {
            //            const bool wasFailedMain0 = checkWasFailedFbo(myMainSceneFbos[0], aRendSize.x(), aRendSize.y(), aNbSamples);
            //            if (!myMainSceneFbos[0]->Init(aCtx, aRendSize, myFboColorFormat, myFboDepthFormat, aNbSamples)
            //             && !wasFailedMain0)
            //            {
            //                string aMsg = "Error! Main FBO "
            //                                                + printFboFormat(myMainSceneFbos[0]) + " initialization has failed";
            //                aCtx->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH, aMsg);
            //            }
            //        }
            //    }

            //    if (myMainSceneFbos[0]->IsValid() && (toInitImmediateFbo || myImmediateSceneFbos[0]->IsValid()))
            //    {
            //        const bool wasFailedImm0 = checkWasFailedFbo(myImmediateSceneFbos[0], myMainSceneFbos[0]);
            //        if (!myImmediateSceneFbos[0]->InitLazy(aCtx, *myMainSceneFbos[0], hasTextureMsaa)
            //         && !wasFailedImm0)
            //        {
            //            TCollection_ExtendedString aMsg = TCollection_ExtendedString() + "Error! Immediate FBO "
            //                                            + printFboFormat(myImmediateSceneFbos[0]) + " initialization has failed";
            //            aCtx->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH, aMsg);
            //        }
            //    }
            //}
            //else
            //{
            //    myMainSceneFbos[0]->Release(aCtx.operator->());
            //    myMainSceneFbos[1]->Release(aCtx.operator->());
            //    myImmediateSceneFbos[0]->Release(aCtx.operator->());
            //    myImmediateSceneFbos[1]->Release(aCtx.operator->());
            //    myXrSceneFbo->Release(aCtx.operator->());
            //    myMainSceneFbos[0]->ChangeViewport(0, 0);
            //    myMainSceneFbos[1]->ChangeViewport(0, 0);
            //    myImmediateSceneFbos[0]->ChangeViewport(0, 0);
            //    myImmediateSceneFbos[1]->ChangeViewport(0, 0);
            //    myXrSceneFbo->ChangeViewport(0, 0);
            //}

            //bool hasXRBlitFbo = false;
            //if (theProj == Graphic3d_Camera::Projection_Stereo
            // && IsActiveXR()
            // && myMainSceneFbos[0]->IsValid())
            //{
            //    if (aNbSamples != 0
            //     || aSizeX != aRendSize.x())
            //    {
            //        hasXRBlitFbo = myXrSceneFbo->InitLazy(aCtx, Graphic3d_Vec2i(aSizeX, aSizeY), myFboColorFormat, myFboDepthFormat, 0);
            //        if (!hasXRBlitFbo)
            //        {
            //            string aMsg = "Error! VR FBO "
            //                                            + printFboFormat(myXrSceneFbo) + " initialization has failed";
            //            aCtx.PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH, aMsg);
            //        }
            //    }
            //}
            //else if (theProj == Graphic3d_Camera.Projection.Projection_Stereo
            //      && myMainSceneFbos[0].IsValid())
            //{
            //    const bool wasFailedMain1 = checkWasFailedFbo(myMainSceneFbos[1], myMainSceneFbos[0]);
            //    if (!myMainSceneFbos[1]->InitLazy(aCtx, *myMainSceneFbos[0], true)
            //     && !wasFailedMain1)
            //    {
            //        TCollection_ExtendedString aMsg = TCollection_ExtendedString() + "Error! Main FBO (second) "
            //                                        + printFboFormat(myMainSceneFbos[1]) + " initialization has failed";
            //        aCtx->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH, aMsg);
            //    }
            //    if (!myMainSceneFbos[1]->IsValid())
            //    {
            //        // no enough memory?
            //        theProj = Graphic3d_Camera::Projection_Perspective;
            //    }
            //    else if (!myTransientDrawToFront)
            //    {
            //        //
            //    }
            //    else if (!aCtx->HasStereoBuffers()
            //           || myRenderParams.StereoMode != Graphic3d_StereoMode_QuadBuffer)
            //    {
            //        const bool wasFailedImm0 = checkWasFailedFbo(myImmediateSceneFbos[0], myMainSceneFbos[0]);
            //        const bool wasFailedImm1 = checkWasFailedFbo(myImmediateSceneFbos[1], myMainSceneFbos[0]);
            //        if (!myImmediateSceneFbos[0]->InitLazy(aCtx, *myMainSceneFbos[0], hasTextureMsaa)
            //         && !wasFailedImm0)
            //        {
            //            TCollection_ExtendedString aMsg = TCollection_ExtendedString() + "Error! Immediate FBO (first) "
            //                                            + printFboFormat(myImmediateSceneFbos[0]) + " initialization has failed";
            //            aCtx->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH, aMsg);
            //        }
            //        if (!myImmediateSceneFbos[1]->InitLazy(aCtx, *myMainSceneFbos[0], hasTextureMsaa)
            //         && !wasFailedImm1)
            //        {
            //            TCollection_ExtendedString aMsg = TCollection_ExtendedString() + "Error! Immediate FBO (first) "
            //                                            + printFboFormat(myImmediateSceneFbos[1]) + " initialization has failed";
            //            aCtx->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH, aMsg);
            //        }
            //        if (!myImmediateSceneFbos[0].IsValid()
            //         || !myImmediateSceneFbos[1].IsValid())
            //        {
            //            theProj = Graphic3d_Camera.Projection.Projection_Perspective;
            //        }
            //    }
            //}
            //if (!hasXRBlitFbo)
            //{
            //    myXrSceneFbo->Release(aCtx.get());
            //    myXrSceneFbo->ChangeViewport(0, 0);
            //}

            //// process PBR environment
            //if (myRenderParams.ShadingModel == Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Pbr
            // || myRenderParams.ShadingModel == Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_PbrFacet)
            //{
            //    if (myPBREnvironment != null
            //      && myPBREnvironment.SizesAreDifferent(myRenderParams.PbrEnvPow2Size,
            //                                              myRenderParams.PbrEnvSpecMapNbLevels))
            //    {
            //        myPBREnvironment.Release(aCtx);
            //        myPBREnvironment = null;
            //        myPBREnvState = PBREnvironmentState.OpenGl_PBREnvState_NONEXISTENT;
            //        myPBREnvRequest = true;
            //        ++myLightsRevision;
            //    }

            //    if (myPBREnvState == PBREnvironmentState.OpenGl_PBREnvState_NONEXISTENT
            //     && aCtx->HasPBR())
            //    {
            //        myPBREnvironment = OpenGl_PBREnvironment::Create(aCtx, myRenderParams.PbrEnvPow2Size, myRenderParams.PbrEnvSpecMapNbLevels);
            //        myPBREnvState = myPBREnvironment.IsNull() ? OpenGl_PBREnvState_UNAVAILABLE : OpenGl_PBREnvState_CREATED;
            //        if (myPBREnvState == OpenGl_PBREnvState_CREATED)
            //        {
            //            Handle(OpenGl_Texture) anEnvLUT;
            //            static const TCollection_AsciiString THE_SHARED_ENV_LUT_KEY("EnvLUT");
            //            if (!aCtx->GetResource(THE_SHARED_ENV_LUT_KEY, anEnvLUT))
            //            {
            //                bool toConvertHalfFloat = false;

            //                // GL_RG32F is not texture-filterable format in OpenGL ES without OES_texture_float_linear extension.
            //                // GL_RG16F is texture-filterable since OpenGL ES 3.0 or OpenGL ES 2.0 + OES_texture_half_float_linear.
            //                // OpenGL ES 3.0 allows initialization of GL_RG16F from 32-bit float data, but OpenGL ES 2.0 + OES_texture_half_float does not.
            //                // Note that it is expected that GL_RG16F has enough precision for this table, so that it can be used also on desktop OpenGL.
            //                const bool hasHalfFloat = aCtx->GraphicsLibrary() == Aspect_GraphicsLibrary_OpenGLES
            //                                      && (aCtx->IsGlGreaterEqual(3, 0) || aCtx->CheckExtension("GL_OES_texture_half_float_linear"));
            //                if (aCtx.GraphicsLibrary() == Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGLES)
            //                {
            //                    toConvertHalfFloat = !aCtx->IsGlGreaterEqual(3, 0) && hasHalfFloat;
            //                }

            //                Image_Format anImgFormat = Image_Format.Image_Format_UNKNOWN;
            //                if (aCtx.arbTexRG)
            //                {
            //                    anImgFormat = toConvertHalfFloat ? Image_Format.Image_Format_RGF_half : Image_Format.Image_Format_RGF;
            //                }
            //                else
            //                {
            //                    anImgFormat = toConvertHalfFloat ? Image_Format.Image_Format_RGBAF_half : Image_Format.Image_Format_RGBAF;
            //                }

            //                Image_PixMap aPixMap = new Image_PixMap();
            //                if (anImgFormat == Image_Format.Image_Format_RGF)
            //                {
            //                    aPixMap->InitWrapper(Image_Format_RGF, (Standard_Byte*)Textures_EnvLUT, Textures_EnvLUTSize, Textures_EnvLUTSize);
            //                }
            //                else
            //                {
            //                    aPixMap->InitZero(anImgFormat, Textures_EnvLUTSize, Textures_EnvLUTSize);
            //                    Image_PixMap aPixMapRG;
            //                    aPixMapRG.InitWrapper(Image_Format_RGF, (Standard_Byte*)Textures_EnvLUT, Textures_EnvLUTSize, Textures_EnvLUTSize);
            //                    for (Standard_Size aRowIter = 0; aRowIter < aPixMapRG.SizeY(); ++aRowIter)
            //                    {
            //                        for (Standard_Size aColIter = 0; aColIter < aPixMapRG.SizeX(); ++aColIter)
            //                        {
            //                            const Image_ColorRGF&aPixelRG = aPixMapRG.Value<Image_ColorRGF>(aRowIter, aColIter);
            //                            if (toConvertHalfFloat)
            //                            {
            //                                NCollection_Vec2<uint16_t> & aPixelRGBA = aPixMap->ChangeValue<NCollection_Vec2<uint16_t>>(aRowIter, aColIter);
            //                                aPixelRGBA.x() = Image_PixMap::ConvertToHalfFloat(aPixelRG.r());
            //                                aPixelRGBA.y() = Image_PixMap::ConvertToHalfFloat(aPixelRG.g());
            //                            }
            //                            else
            //                            {
            //                                Image_ColorRGBAF & aPixelRGBA = aPixMap->ChangeValue<Image_ColorRGBAF>(aRowIter, aColIter);
            //                                aPixelRGBA.r() = aPixelRG.r();
            //                                aPixelRGBA.g() = aPixelRG.g();
            //                            }
            //                        }
            //                    }
            //                }

            //                OpenGl_TextureFormat aTexFormat = OpenGl_TextureFormat::FindFormat(aCtx, aPixMap->Format(), false);
            //                if (aCtx->GraphicsLibrary() == Aspect_GraphicsLibrary_OpenGLES
            //                 && aTexFormat.IsValid()
            //                 && hasHalfFloat)
            //                {
            //                    aTexFormat.SetInternalFormat(aCtx->arbTexRG ? GL_RG16F : GL_RGBA16F);
            //                }

            //                Handle(Graphic3d_TextureParams) aParams = new Graphic3d_TextureParams();
            //                aParams->SetFilter(Graphic3d_TOTF_BILINEAR);
            //                aParams->SetRepeat(Standard_False);
            //                aParams->SetTextureUnit(aCtx->PBREnvLUTTexUnit());
            //                anEnvLUT = new OpenGl_Texture(THE_SHARED_ENV_LUT_KEY, aParams);
            //                if (!aTexFormat.IsValid()
            //                 || !anEnvLUT->Init(aCtx, aTexFormat, Graphic3d_Vec2i((Standard_Integer)Textures_EnvLUTSize), Graphic3d_TypeOfTexture_2D, aPixMap.get()))
            //                {
            //                    aCtx->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH, "Failed allocation of LUT for PBR");
            //                    anEnvLUT.Nullify();
            //                }
            //                aCtx->ShareResource(THE_SHARED_ENV_LUT_KEY, anEnvLUT);
            //            }
            //            if (!anEnvLUT.IsNull())
            //            {
            //                anEnvLUT->Bind(aCtx);
            //            }
            //            myWorkspace->ApplyAspects();
            //        }
            //    }
            //    updatePBREnvironment(aCtx);
            //}

            //// create color and coverage accumulation buffers required for OIT algorithm
            //if (toUseOit
            // && myRenderParams.TransparencyMethod == Graphic3d_RTM_DEPTH_PEELING_OIT)
            //{
            //    if (myDepthPeelingFbos->BlendBackFboOit()->GetSize() != aRendSize)
            //    {
            //        if (myDepthPeelingFbos->BlendBackFboOit()->Init(aCtx, aRendSize, GL_RGBA16F, 0))
            //        {
            //            for (int aPairIter = 0; aPairIter < 2; ++aPairIter)
            //            {
            //                OpenGl_ColorFormats aColorFormats;
            //                aColorFormats.Append(GL_RG32F);
            //                aColorFormats.Append(GL_RGBA16F);
            //                aColorFormats.Append(GL_RGBA16F);
            //                myDepthPeelingFbos->DepthPeelFbosOit()[aPairIter]->Init(aCtx, aRendSize, aColorFormats, 0);

            //                NCollection_Sequence < Handle(OpenGl_Texture) > anAttachments;
            //                anAttachments.Append(myDepthPeelingFbos->DepthPeelFbosOit()[aPairIter]->ColorTexture(1));
            //                anAttachments.Append(myDepthPeelingFbos->DepthPeelFbosOit()[aPairIter]->ColorTexture(2));
            //                myDepthPeelingFbos->FrontBackColorFbosOit()[aPairIter]->InitWrapper(aCtx, anAttachments);
            //            }
            //        }
            //        else
            //        {
            //            toUseOit = false;
            //            aCtx->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH,
            //                               "Initialization of float texture framebuffer for use with\n"


            //                               "  Depth-Peeling order-independent transparency rendering algorithm has failed.");
            //        }
            //    }
            //}
            //if (!toUseOit)
            //{
            //    myDepthPeelingFbos->Release(aCtx.operator->());
            //}

            //if (toUseOit
            // && myRenderParams.TransparencyMethod == Graphic3d_RTM_BLEND_OIT)
            //{
            //    Standard_Integer anFboIt = 0;
            //    for (; anFboIt < 2; ++anFboIt)
            //    {
            //        Handle(OpenGl_FrameBuffer) & aMainSceneFbo = myMainSceneFbos[anFboIt];
            //        Handle(OpenGl_FrameBuffer) & aMainSceneFboOit = myMainSceneFbosOit[anFboIt];
            //        Handle(OpenGl_FrameBuffer) & anImmediateSceneFbo = myImmediateSceneFbos[anFboIt];
            //        Handle(OpenGl_FrameBuffer) & anImmediateSceneFboOit = myImmediateSceneFbosOit[anFboIt];
            //        if (aMainSceneFbo->IsValid()
            //         && (aMainSceneFboOit->GetVPSize() != aRendSize
            //          || aMainSceneFboOit->NbSamples() != aNbSamples))
            //        {
            //            Standard_Integer aColorConfig = 0;
            //            for (; ; ) // seemly responding to driver limitation (GL_FRAMEBUFFER_UNSUPPORTED)
            //            {
            //                if (myFboOitColorConfig.IsEmpty())
            //                {
            //                    if (!chooseOitColorConfiguration(aCtx, aColorConfig++, myFboOitColorConfig))
            //                    {
            //                        break;
            //                    }
            //                }
            //                if (aMainSceneFboOit->Init(aCtx, aRendSize, myFboOitColorConfig, aMainSceneFbo->DepthStencilTexture(), aNbSamples))
            //                {
            //                    break;
            //                }
            //                myFboOitColorConfig.Clear();
            //            }
            //            if (!aMainSceneFboOit->IsValid())
            //            {
            //                break;
            //            }
            //        }
            //        else if (!aMainSceneFbo->IsValid())
            //        {
            //            aMainSceneFboOit->Release(aCtx.operator->());
            //            aMainSceneFboOit->ChangeViewport(0, 0);
            //        }

            //        if (anImmediateSceneFbo->IsValid()
            //         && (anImmediateSceneFboOit->GetVPSize() != aRendSize
            //          || anImmediateSceneFboOit->NbSamples() != aNbSamples))
            //        {
            //            if (!anImmediateSceneFboOit->Init(aCtx, aRendSize, myFboOitColorConfig,
            //                                               anImmediateSceneFbo->DepthStencilTexture(), aNbSamples))
            //            {
            //                break;
            //            }
            //        }
            //        else if (!anImmediateSceneFbo->IsValid())
            //        {
            //            anImmediateSceneFboOit->Release(aCtx.operator->());
            //            anImmediateSceneFboOit->ChangeViewport(0, 0);
            //        }
            //    }
            //    if (anFboIt == 0) // only the first OIT framebuffer is mandatory
            //    {
            //        aCtx->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH,
            //                           "Initialization of float texture framebuffer for use with\n"


            //                           "  blended order-independent transparency rendering algorithm has failed.\n"


            //                           "  Blended order-independent transparency will not be available.\n");
            //        if (aNbSamples > 0)
            //        {
            //            myToDisableOITMSAA = Standard_True;
            //        }
            //        else
            //        {
            //            myToDisableOIT = Standard_True;
            //        }
            //        toUseOit = false;
            //    }
            //}
            //if (!toUseOit && myMainSceneFbosOit[0]->IsValid())
            //{
            //    myDepthPeelingFbos->Release(aCtx.operator->());
            //    myMainSceneFbosOit[0]->Release(aCtx.operator->());
            //    myMainSceneFbosOit[1]->Release(aCtx.operator->());
            //    myImmediateSceneFbosOit[0]->Release(aCtx.operator->());
            //    myImmediateSceneFbosOit[1]->Release(aCtx.operator->());
            //    myMainSceneFbosOit[0]->ChangeViewport(0, 0);
            //    myMainSceneFbosOit[1]->ChangeViewport(0, 0);
            //    myImmediateSceneFbosOit[0]->ChangeViewport(0, 0);
            //    myImmediateSceneFbosOit[1]->ChangeViewport(0, 0);
            //}

            //// allocate shadow maps
            //const Handle(Graphic3d_LightSet)&aLights = myRenderParams.ShadingModel == Graphic3d_TypeOfShadingModel_Unlit ? myNoShadingLight : myLights;
            //if (!aLights.IsNull())
            //{
            //    aLights->UpdateRevision();
            //}
            //bool toUseShadowMap = myRenderParams.IsShadowEnabled
            //                   && myRenderParams.ShadowMapResolution > 0
            //                   && !myLights.IsNull()
            //                   && myLights->NbCastShadows() > 0
            //                   && myRenderParams.Method != Graphic3d_RM_RAYTRACING;
            //if (toUseShadowMap)
            //{
            //    if (myShadowMaps->Size() != myLights->NbCastShadows())
            //    {
            //        myShadowMaps->Release(aCtx.get());
            //        myShadowMaps->Resize(0, myLights->NbCastShadows() - 1, true);
            //    }

            //    const GLint aSamplFrom = GLint(aCtx->ShadowMapTexUnit()) - myLights->NbCastShadows() + 1;
            //    for (Standard_Integer aShadowIter = 0; aShadowIter < myShadowMaps->Size(); ++aShadowIter)
            //    {
            //        Handle(OpenGl_ShadowMap) & aShadow = myShadowMaps->ChangeValue(aShadowIter);
            //        if (aShadow.IsNull())
            //        {
            //            aShadow = new OpenGl_ShadowMap();
            //        }
            //        aShadow->SetShadowMapBias(myRenderParams.ShadowMapBias);
            //        aShadow->Texture()->Sampler()->Parameters()->SetTextureUnit((Graphic3d_TextureUnit)(aSamplFrom + aShadowIter));

            //        const Handle(OpenGl_FrameBuffer)&aShadowFbo = aShadow->FrameBuffer();
            //        if (aShadowFbo->GetVPSizeX() != myRenderParams.ShadowMapResolution
            //         && toUseShadowMap)
            //        {
            //            OpenGl_ColorFormats aDummy;
            //            if (!aShadowFbo->Init(aCtx, Graphic3d_Vec2i(myRenderParams.ShadowMapResolution), aDummy, myFboDepthFormat, 0))
            //            {
            //                toUseShadowMap = false;
            //            }
            //        }
            //    }
            //}
            //if (!toUseShadowMap && myShadowMaps->IsValid())
            //{
            //    myShadowMaps->Release(aCtx.get());
            //}

            return true;
        }

        public override void RedrawImmediate()
        {
            if (!myWorkspace.Activate())
                return;

            // no special handling of HMD display, since it will force full Redraw() due to no frame caching (myBackBufferRestored)
            OpenGl_Context aCtx = myWorkspace.GetGlContext();
            if (!myTransientDrawToFront
             || !myBackBufferRestored
             || (aCtx.caps.buffersNoSwap && !myMainSceneFbos[0].IsValid()))
            {
                Redraw();
                return;
            }

            Graphic3d_StereoMode aStereoMode = myRenderParams.StereoMode;
            Graphic3d_Camera.Projection aProjectType = myCamera.ProjectionType();
            OpenGl_FrameBuffer aFrameBuffer = myFBO;
            //aCtx.FrameStats()->FrameStart(myWorkspace.View(), true);
            bool toSwap = false;
            if (aProjectType == Graphic3d_Camera.Projection.Projection_Stereo)
            {
                OpenGl_FrameBuffer[] aMainFbos =
    {
      myMainSceneFbos[0].IsValid() ? myMainSceneFbos[0] : null,
      myMainSceneFbos[1].IsValid() ? myMainSceneFbos[1] : null
    };
                OpenGl_FrameBuffer[] anImmFbos =
    {
      myMainSceneFbos[0].IsValid() ? myMainSceneFbos[0] : null,
      myMainSceneFbos[1].IsValid() ? myMainSceneFbos[1] : null
    }; OpenGl_FrameBuffer[] anImmFbosOit =
    {
      myMainSceneFbos[0].IsValid() ? myMainSceneFbos[0] : null,
      myMainSceneFbos[1].IsValid() ? myMainSceneFbos[1] : null
    };

                toSwap = redrawImmediate(Graphic3d_Camera.Projection.Projection_MonoLeftEye,
                                     aMainFbos[0],
                                     anImmFbos[0],
                                     anImmFbosOit[0],
                                     true) || toSwap;


                if (anImmFbos[0] != null)
                {
                    //                    drawStereoPair(aFrameBuffer);
                }


            }
            else
            {
                //OpenGl_FrameBuffer aMainFbo = myMainSceneFbos[0].IsValid() ? myMainSceneFbos[0].operator->() : null;
                OpenGl_FrameBuffer aMainFbo = myMainSceneFbos[0].IsValid() ? myMainSceneFbos[0] : null;
                OpenGl_FrameBuffer anImmFbo = aFrameBuffer;
                OpenGl_FrameBuffer anImmFboOit = null;
                if (myImmediateSceneFbos[0].IsValid())
                {
                    //anImmFbo = myImmediateSceneFbos[0].operator->();
                    // anImmFboOit = myImmediateSceneFbosOit[0]->IsValid() ? myImmediateSceneFbosOit[0].operator->() : NULL;
                }
                if (aMainFbo == null)
                {
                    //aCtx.SetReadDrawBuffer(GL_BACK);
                }
                // aCtx->SetResolution(myRenderParams.Resolution, myRenderParams.ResolutionRatio(),
                //                    anImmFbo != aFrameBuffer ? myRenderParams.RenderResolutionScale : 1.0f);
                toSwap = redrawImmediate(aProjectType,
                                          aMainFbo,
                                          anImmFbo,
                                          anImmFboOit,
                                          true) || toSwap;
                if (anImmFbo != null
                 && anImmFbo != aFrameBuffer)
                {
                    blitBuffers(anImmFbo, aFrameBuffer, myToFlipOutput);
                }
            }

            // bind default FBO
            bindDefaultFbo();

            // reset state for safety
            aCtx.BindProgram(null);
            if (aCtx.caps.ffpEnable)
            {
                aCtx.ShaderManager().PushState(null);
            }

            if (toSwap
            && myFBO == null
            && !aCtx.caps.buffersNoSwap
            && myParentView == null)
            {
                aCtx.SwapBuffers();
            }
            else
            {
                aCtx.core11fwd.glFlush();
            }
            //aCtx->FrameStats()->FrameEnd(myWorkspace->View(), true);

            myWasRedrawnGL = true;


        }

        public void bindDefaultFbo(OpenGl_FrameBuffer theCustomFbo = null)
        {
            OpenGl_Context aCtx = myWorkspace.GetGlContext();
            OpenGl_FrameBuffer anFbo = (theCustomFbo != null && theCustomFbo.IsValid())
                                      ? theCustomFbo
                                      : (aCtx.DefaultFrameBuffer() != null
                                       && aCtx.DefaultFrameBuffer().IsValid()
                                        ? aCtx.DefaultFrameBuffer()
                              : null);
            if (anFbo != null)
            {
                anFbo.BindBuffer(aCtx);
                anFbo.SetupViewport(aCtx);
            }
            else
            {
                if (aCtx.GraphicsLibrary() != Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGLES)
                {
                    aCtx.SetReadDrawBuffer((int)All.Back);
                }
                else if (aCtx.arbFBO != null)
                {
                    aCtx.arbFBO.glBindFramebuffer(All.Framebuffer, OpenGl_FrameBuffer.NO_FRAMEBUFFER);
                }

                int[] aViewport = new int[4] { 0, 0, myWindow.Width(), myWindow.Height() };
                aCtx.ResizeViewport(aViewport);
            }
        }

        private bool redrawImmediate(Graphic3d_Camera.Projection theProjection,
                                   OpenGl_FrameBuffer theReadFbo,
                                   OpenGl_FrameBuffer theDrawFbo,
                                   OpenGl_FrameBuffer theOitAccumFbo,
                                    bool theIsPartialUpdate)
        {
            OpenGl_Context aCtx = myWorkspace.GetGlContext();
            bool toCopyBackToFront = false;
            if (theDrawFbo == theReadFbo
             && theDrawFbo != null
             && theDrawFbo.IsValid())
            {
                myBackBufferRestored = false;
                theDrawFbo.BindBuffer(aCtx);
            }
            else if (theReadFbo != null
                  && theReadFbo.IsValid()
                  && aCtx.IsRender())
            {
                if (!blitBuffers(theReadFbo, theDrawFbo))
                {
                    return true;
                }
            }
            else if (theDrawFbo == null)
            {
                if (aCtx.GraphicsLibrary() != Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGLES)
                {
                    aCtx.core11fwd.glGetBooleanv(GetPName.Doublebuffer, ref toCopyBackToFront);
                }
                if (toCopyBackToFront
                 && myTransientDrawToFront)
                {
                    if (!HasImmediateStructures()
                     && !theIsPartialUpdate)
                    {
                        // prefer Swap Buffers within Redraw in compatibility mode (without FBO)
                        return true;
                    }
                    if (!copyBackToFront())
                    {
                        toCopyBackToFront = false;
                        myBackBufferRestored = false;
                    }
                }
                else
                {
                    toCopyBackToFront = false;
                    myBackBufferRestored = false;
                }
            }
            else
            {
                myBackBufferRestored = false;
            }
            myIsImmediateDrawn = true;

            myWorkspace.SetUseZBuffer(true);
            myWorkspace.SetUseDepthWrite(true);
            aCtx.core11fwd.glDepthFunc(All.Lequal);
            aCtx.core11fwd.glDepthMask(true);
            aCtx.core11fwd.glEnable(All.DepthTest);
            aCtx.core11fwd.glClearDepth(1.0);

            render(theProjection, theDrawFbo, theOitAccumFbo, true);

            blitSubviews(theProjection, theDrawFbo);

            return !toCopyBackToFront;
        }

        private bool blitBuffers(OpenGl_FrameBuffer theReadFbo, OpenGl_FrameBuffer theDrawFbo, bool theToFlip = false)
        {
            OpenGl_Context aCtx = myWorkspace.GetGlContext();
            int aReadSizeX = theReadFbo != null ? theReadFbo.GetVPSizeX() : myWindow.Width();
            int aReadSizeY = theReadFbo != null ? theReadFbo.GetVPSizeY() : myWindow.Height();
            int aDrawSizeX = theDrawFbo != null ? theDrawFbo.GetVPSizeX() : myWindow.Width();
            int aDrawSizeY = theDrawFbo != null ? theDrawFbo.GetVPSizeY() : myWindow.Height();
            if (theReadFbo == null || aCtx.IsFeedback())
            {
                return false;
            }
            else if (theReadFbo == theDrawFbo)
            {
                return true;
            }

            // clear destination before blitting
            if (theDrawFbo != null && theDrawFbo.IsValid())
            {
                theDrawFbo.BindBuffer(aCtx);
            }
            else
            {
                aCtx.arbFBO.glBindFramebuffer(All.Framebuffer, OpenGl_FrameBuffer.NO_FRAMEBUFFER);
                aCtx.SetFrameBufferSRGB(false);
            }

            /*
             more code here
             */
            return true;
        }
        OpenGl_PBREnvironment myPBREnvironment; //!< manager of IBL maps used in PBR pipeline
        PBREnvironmentState myPBREnvState;    //!< state of PBR environment
                                              //! State of PBR environment.
        bool myPBREnvRequest;  //!< update PBR environment

        int myLightsRevision;

        enum PBREnvironmentState
        {
            OpenGl_PBREnvState_NONEXISTENT,
            OpenGl_PBREnvState_UNAVAILABLE, // indicates failed try to create PBR environment
            OpenGl_PBREnvState_CREATED
        };
        //! Returns true if there are immediate structures to display
        private bool HasImmediateStructures()
        {
            return myZLayers.NbImmediateStructures() != 0;
        }

        private void blitSubviews(Graphic3d_Camera.Projection theProjection, OpenGl_FrameBuffer theDrawFbo)
        {
            throw new NotImplementedException();
        }


        private bool copyBackToFront()
        {
            myIsImmediateDrawn = false;
            OpenGl_Context aCtx = myWorkspace.GetGlContext();
            if (aCtx.core11ffp == null)
            {
                return false;
            }

            /*OpenGl_Mat4 aProjectMat;
            Graphic3d_TransformUtils.Ortho2D(aProjectMat,
                                               0.0f, static_cast<GLfloat>(myWindow->Width()),
                                               0.0f, static_cast<GLfloat>(myWindow->Height()));
            */
            aCtx.WorldViewState.Push();
            aCtx.ProjectionState.Push();

            aCtx.WorldViewState.SetIdentity();
            //  aCtx.ProjectionState.SetCurrent(aProjectMat);

            aCtx.ApplyProjectionMatrix();
            //  aCtx.ApplyWorldViewMatrix();


            // synchronize FFP state before copying pixels
            aCtx.BindProgram(new OpenGl_ShaderProgram());
            //   aCtx.ShaderManager().PushState(new OpenGl_ShaderProgram());
            //  aCtx.DisableFeatures();

            switch (aCtx.DrawBuffer())
            {
                case GLConstants.GL_BACK_LEFT:
                    {
                        aCtx.SetReadBuffer(GLConstants.GL_BACK_LEFT);
                        aCtx.SetDrawBuffer(GLConstants.GL_FRONT_LEFT);
                        break;
                    }
                case GLConstants.GL_BACK_RIGHT:
                    {
                        aCtx.SetReadBuffer(GLConstants.GL_BACK_RIGHT);
                        aCtx.SetDrawBuffer(GLConstants.GL_FRONT_RIGHT);
                        break;
                    }
                default:
                    {
                        aCtx.SetReadBuffer(GLConstants.GL_BACK);
                        aCtx.SetDrawBuffer(GLConstants.GL_FRONT);
                        break;
                    }
            }

            /*aCtx->core11ffp->glRasterPos2i(0, 0);
            aCtx->core11ffp->glCopyPixels(0, 0, myWindow->Width() + 1, myWindow->Height() + 1, GL_COLOR);
            //aCtx->core11ffp->glCopyPixels  (0, 0, myWidth + 1, myHeight + 1, GL_DEPTH);

            aCtx->EnableFeatures();

            aCtx->WorldViewState.Pop();
            aCtx->ProjectionState.Pop();
            aCtx->ApplyProjectionMatrix();

            // read/write from front buffer now
            aCtx->SetReadBuffer(aCtx->DrawBuffer());*/
            return true;
        }

        public void renderScene(Graphic3d_Camera.Projection theProjection,
                             OpenGl_FrameBuffer theReadDrawFbo,
                             OpenGl_FrameBuffer theOitAccumFbo,
                              bool theToDrawImmediate)
        {
            OpenGl_Context aContext = myWorkspace.GetGlContext();
            renderStructs(theProjection, theReadDrawFbo, theOitAccumFbo, theToDrawImmediate);
            //aContext->BindTextures(Handle(OpenGl_TextureSet)(), Handle(OpenGl_ShaderProgram)());

        }
        //=======================================================================
        //function : Render
        //purpose  :
        //=======================================================================
        void render(Graphic3d_Camera.Projection theProjection,
                                      OpenGl_FrameBuffer theOutputFBO,
                                      OpenGl_FrameBuffer theOitAccumFbo,

                               bool theToDrawImmediate)
        {
            // ==================================
            //      Step 1: Prepare for render
            // ==================================

            OpenGl_Context aContext = myWorkspace.GetGlContext();

            aContext?.SetAllowSampleAlphaToCoverage(myRenderParams.ToEnableAlphaToCoverage
                                                  && theOutputFBO != null
                                                  && theOutputFBO.NbSamples() != 0);
            // ====================================
            //      Step 2: Redraw background
            // ====================================

            // Render background
            if (!theToDrawImmediate)
            {
                drawBackground(myWorkspace, theProjection);
            }

            // Switch off lighting by default
            /*if (aContext.core11ffp != null
             && aContext.caps.ffpEnable)
            {
                GL.Disable(EnableCap.Lighting);
                //aContext.core11fwd.glDisable(GL_LIGHTING);
            }*/

            // =================================
            //      Step 3: Redraw main plane
            // =================================

            renderScene(theProjection, theOutputFBO, theOitAccumFbo, theToDrawImmediate);

        }


        //! Returns True if the window associated to the view is defined.
        public override bool IsDefined()
        { return myWindow != null; }
        OpenGl_LayerList myZLayers; //!< main list of displayed structure, sorted by layers


        public override Graphic3d_Layer[] Layers()
        {
            return myZLayers.Layers();

        }


        public override Aspect_Window Window()
        {
            return myWindow.SizeWindow();

        }

        public override void Invalidate()
        {
            myBackBufferRestored = false;
        }

        OpenGl_FrameBuffer[] myImmediateSceneFbos = new OpenGl_FrameBuffer[2];    //!< Additional buffers for immediate layer in stereo mode.

        public override Graphic3d_Layer Layer(Graphic3d_ZLayerId theLayerId)
        {
            Graphic3d_Layer aLayer = null;
            if (theLayerId != Graphic3d_ZLayerId.Graphic3d_ZLayerId_UNKNOWN)
            {
                myZLayers.LayerIDs().Find(theLayerId, out aLayer);
            }
            return aLayer;

        }
    }
}