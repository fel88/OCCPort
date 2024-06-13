using OpenTK.Graphics.OpenGL;

namespace OCCPort.OpenGL
{
    public class OpenGl_View : Graphic3d_CView
    {

        public OpenGl_View()
        {
            myZLayers = new OpenGl_LayerList();
        }

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

            /*const Handle(OpenGl_Context)& aContext = myWorkspace->GetGlContext();
                  aContext->SetAllowSampleAlphaToCoverage(myRenderParams.ToEnableAlphaToCoverage
                                                        && theOutputFBO != NULL
                                                        && theOutputFBO->NbSamples() != 0);*/
            // ====================================
            //      Step 2: Redraw background
            // ====================================

            // Render background
            if (!theToDrawImmediate)
            {
                //drawBackground(myWorkspace, theProjection);
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
        OpenGl_Window myWindow;

        public override Aspect_Window Window()
        {
            return myWindow.SizeWindow();

        }

    }

}