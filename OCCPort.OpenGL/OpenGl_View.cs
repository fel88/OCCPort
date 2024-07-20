using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
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

            OpenGl_View aParentView = (OpenGl_View)(theParentVIew);
            if (theParentVIew != null)
            {
                /*if (aParentView == null
				 || aParentView.GlWindow().IsNull()
				 || aParentView.GlWindow()->GetGlContext().IsNull())
				{
					throw new Standard_ProgramError("OpenGl_View::SetWindow(), internal error");
				}

				myParentView = aParentView;
				myParentView.AddSubview(this);

				Aspect_NeutralWindow aSubWindow = (Aspect_NeutralWindow)(theWindow);
				SubviewResized(aSubWindow);

				OpenGl_Window aParentGlWindow = aParentView.GlWindow();
				Aspect_RenderingContext aRendCtx = aParentGlWindow.GetGlContext().RenderingContext();
				myWindow = myDriver.CreateRenderWindow(aParentGlWindow.PlatformWindow(), theWindow, aRendCtx);*/
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
        bool myWasRedrawnGL;

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

        public override void Redraw()
        {
            bool wasDisabledMSAA = myToDisableMSAA;
            bool hadFboBlit = myHasFboBlit;
            /*if (myRenderParams.Method == Graphic3d_RenderingMode.Graphic3d_RM_RAYTRACING
            && !myCaps.vboDisable
            && !myCaps.keepArrayData)
            {
                // caps are shared across all views, thus we need to invalidate all of them
                // if (myWasRedrawnGL) { myStructureManager->SetDeviceLost(); }
                myDriver.setDeviceLost();
                myCaps.keepArrayData = true;
            }*/

            OpenGl_FrameBuffer aFrameBuffer = myFBO;

            Graphic3d_Camera.Projection aProjectType = myCamera.ProjectionType();
            if (aProjectType == Graphic3d_Camera.Projection.Projection_Stereo)
            {
            }
            else
            {
                OpenGl_FrameBuffer aMainFbo = myMainSceneFbos[0].IsValid() ? myMainSceneFbos[0] : aFrameBuffer;
                OpenGl_FrameBuffer aMainFboOit = myMainSceneFbosOit[0].IsValid() ? myMainSceneFbosOit[0] : null;
                redraw(aProjectType, aMainFbo, aMainFboOit);
            }

        }

        public override void RedrawImmediate()
        {
            if (!myWorkspace.Activate())
                return;



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