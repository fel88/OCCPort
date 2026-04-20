using OpenTK.Graphics.ES11;
using System;
using System.Reflection.Metadata;

namespace OCCPort.OpenGL
{
    public class OpenGl_Workspace
    {



        public OpenGl_Context GetGlContext() { return myGlContext; }

        internal bool Activate()
        {
            if (myWindow == null || !myWindow.Activate())
            {
                return false;
            }

            if (myGlContext.core11ffp == null)
            {
                if (myGlContext.caps.ffpEnable)
                {
                    Message.SendWarning(myGlContext.GraphicsLibrary() != Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGLES
                                        ? "Warning: FFP is unsupported by OpenGL ES"
                                        : "Warning: FFP is unsupported by OpenGL Core Profile");
                    myGlContext.caps.ffpEnable = false;
                }
            }

            //if (myGlContext->caps->useZeroToOneDepth
            //&& !myGlContext->arbClipControl)
            //{
            //    Message::SendWarning("Warning: glClipControl() requires OpenGL 4.5 or GL_ARB_clip_control extension");
            //    myGlContext->caps->useZeroToOneDepth = false;
            //}
            //myView->Camera()->SetZeroToOneDepth(myGlContext->caps->useZeroToOneDepth);
            //if (myGlContext->arbClipControl)
            //{
            //    myGlContext->Functions()->glClipControl(GL_LOWER_LEFT, myGlContext->caps->useZeroToOneDepth ? GL_ZERO_TO_ONE : GL_NEGATIVE_ONE_TO_ONE);
            //}

            ResetAppliedAspect();

            // reset state for safety
            myGlContext.BindProgram(null);
            if (myGlContext.core20fwd != null)
            {
                myGlContext.core20fwd.glUseProgram(OpenGl_ShaderProgram.NO_PROGRAM);
            }
            if (myGlContext.caps.ffpEnable)
            {
                myGlContext.ShaderManager().PushState(null);
            }
            return true;
        }

        //! @return true if usage of Z buffer is enabled.
        public bool UseZBuffer() { return myUseZBuffer; }

        // =======================================================================
        // function : FBOCreate
        // purpose  :
        // =======================================================================
        public OpenGl_FrameBuffer FBOCreate(int theWidth,
                                                        int theHeight)
        {
            // activate OpenGL context
            if (!Activate())
                return null;

            // create the FBO
            OpenGl_Context aCtx = GetGlContext();
            //aCtx.BindTextures(Handle(OpenGl_TextureSet)(), Handle(OpenGl_ShaderProgram)());
            OpenGl_FrameBuffer aFrameBuffer = new OpenGl_FrameBuffer();
            if (!aFrameBuffer.Init(aCtx, new Graphic3d_Vec2i(theWidth, theHeight), (int)All.Srgb8Alpha8, (int)All.Depth24Stencil8, 0))
            {
                //aFrameBuffer.Release(aCtx.operator->());
                return null;
            }
            return aFrameBuffer;
        }

        internal OpenGl_Aspects Aspects()
        {
            throw new NotImplementedException();
        }

        internal int RenderFilter()
        {
            throw new NotImplementedException();
        }

        internal void ResetAppliedAspect()
        {
            throw new NotImplementedException();
        }

        internal void SetAllowFaceCulling(object value)
        {
            throw new NotImplementedException();
        }

        internal OpenGl_Aspects SetAspects(OpenGl_Aspects myCubeMapParams)
        {
            throw new NotImplementedException();
        }

        internal void SetRenderFilter(int aPrevFilter)
        {
            throw new NotImplementedException();
        }

        internal bool SetUseZBuffer(bool theToUse)
        {
            bool wasUsed = myUseZBuffer;
            myUseZBuffer = theToUse;
            return wasUsed;

        }
        int myRenderFilter;         //!< active filter for skipping rendering of elements by some criteria (multiple render passes)

        internal bool ShouldRender(OpenGl_Element theElement, OpenGl_Group theGroup)
        {
            if ((myRenderFilter & (int)OpenGl_RenderFilter.OpenGl_RenderFilter_SkipTrsfPersistence) != 0)
            {
                if (theGroup.HasPersistence())
                {
                    return false;
                }
            }

            // render only non-raytracable elements when RayTracing is enabled
            if ((myRenderFilter & (int)OpenGl_RenderFilter.OpenGl_RenderFilter_NonRaytraceableOnly) != 0)
            {
                if (!theGroup.HasPersistence() && OpenGl_Raytrace.IsRaytracedElement(theElement))
                {
                    return false;
                }
            }
            else if ((myRenderFilter & (int)OpenGl_RenderFilter.OpenGl_RenderFilter_FillModeOnly) != 0)
            {
                if (!theElement.IsFillDrawMode())
                {
                    return false;
                }
            }
            // handle opaque/transparency render passes
            if ((myRenderFilter & (int)OpenGl_RenderFilter.OpenGl_RenderFilter_OpaqueOnly) != 0)
            {
                if (!theElement.IsFillDrawMode())
                {
                    return true;
                }

                if (OpenGl_Context.CheckIsTransparent(myAspectsSet, myHighlightStyle))
                {
                    ++myNbSkippedTranspElems;
                    return false;
                }
            }
            else if ((myRenderFilter & (int)OpenGl_RenderFilter.OpenGl_RenderFilter_TransparentOnly) != 0)
            {
                if (!theElement.IsFillDrawMode())
                {
                    if ((OpenGl_Aspects)(theElement) == null)
                    {
                        return false;
                    }
                }
                else if (!OpenGl_Context.CheckIsTransparent(myAspectsSet, myHighlightStyle))
                {
                    return false;
                }
            }
            return true;


        }
        Graphic3d_PresentationAttributes myHighlightStyle; //!< active highlight style

        OpenGl_Aspects myAspectsSet;
        int myNbSkippedTranspElems; //!< counter of skipped transparent elements for OpenGl_LayerList two rendering passes method

        internal OpenGl_View View()
        {
            return myView;
        }

        internal void SetUseDepthWrite(bool v)
        {
            myUseDepthWrite = v;
        }

        internal OpenGl_TextureSet TextureSet()
        {
            throw new NotImplementedException();
        }

        internal OpenGl_TextureSet EnvironmentTexture()
        {
            throw new NotImplementedException();
        }

        internal bool ToHighlight()
        {
            throw new NotImplementedException();
        }

        OpenGl_View myView;
        OpenGl_Window myWindow;
        OpenGl_Context myGlContext;
        bool myUseZBuffer;
        bool myUseDepthWrite;
        OpenGl_Aspects myNoneCulling;
        OpenGl_Aspects myFrontCulling;

        public OpenGl_Workspace(OpenGl_View theView, OpenGl_Window theWindow)
        {
            myView = (theView);
            myWindow = (theWindow);
            myGlContext = (theWindow != null ? theWindow.GetGlContext() : null);
            myUseZBuffer = (true);
            myUseDepthWrite = (true);
        }
    }
}