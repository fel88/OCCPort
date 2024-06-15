using OpenTK.Graphics.OpenGL;
using System;

namespace OCCPort.OpenGL
{
    public class OpenGl_View : Graphic3d_CView
    {

        public OpenGl_View()
        {
            myZLayers = new OpenGl_LayerList();
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
                    || myBackgrounds[(int)Graphic3d_TypeOfBackground.Graphic3d_TOB_TEXTURE].TextureFillMethod() == Aspect_FillMethod. Aspect_FM_CENTERED
                    || myBackgrounds[(int)Graphic3d_TypeOfBackground.Graphic3d_TOB_TEXTURE].TextureFillMethod() == Aspect_FillMethod.Aspect_FM_NONE))
                {
                    if (myBackgrounds[(int)Graphic3d_TypeOfBackground.Graphic3d_TOB_GRADIENT].GradientFillMethod() >= Aspect_GradientFillMethod.Aspect_GradientFillMethod_Corner1
                     && myBackgrounds[(int)Graphic3d_TypeOfBackground.Graphic3d_TOB_GRADIENT].GradientFillMethod() <= Aspect_GradientFillMethod. Aspect_GradientFillMethod_Corner4)
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

        //=======================================================================
        public void renderStructs(Graphic3d_Camera.Projection theProjection,
                                         OpenGl_FrameBuffer theReadDrawFbo,
                                         OpenGl_FrameBuffer theOitAccumFbo,
                                  bool theToDrawImmediate)
        {

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

            aContext.SetAllowSampleAlphaToCoverage(myRenderParams.ToEnableAlphaToCoverage
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
            throw new System.NotImplementedException();
        }
    }



}