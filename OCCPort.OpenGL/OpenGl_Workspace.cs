using OCCPort.Common;
using OpenTK.Graphics.OpenGL;
using System;
using System.Reflection.Metadata;
using TKService;

namespace OCCPort.OpenGL
{

    //! Rendering workspace.
    //! Provides methods to render primitives and maintain GL state.
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
        public void UseZBuffer(bool v) { myUseZBuffer = v; }

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
            return myAspectsSet;
        }

        //! Get rendering filter.
        //! @sa ShouldRender()
        internal int RenderFilter()
        {
            return myRenderFilter;
        }

        internal void ResetAppliedAspect()
        {
            myGlContext.BindDefaultVao();

            myHighlightStyle = null;
            myToAllowFaceCulling = false;
            // myAspectsSet = &myDefaultAspects;
            // myAspectsApplied = null;
            // myGlContext.SetPolygonOffset(Graphic3d_PolygonOffset());

            ApplyAspects();
            //   myGlContext.SetLineStipple(myDefaultAspects.Aspect().LinePattern());
            //  myGlContext.SetLineWidth(myDefaultAspects.Aspect().LineWidth());
            if (myGlContext.core15fwd != null)
            {
                myGlContext.core15fwd.glActiveTexture(All.Texture0);
            }
        }
        Graphic3d_Aspects myAspectsApplied;

        public Graphic3d_PolygonOffset SetDefaultPolygonOffset(Graphic3d_PolygonOffset theOffset)
        {
            Graphic3d_PolygonOffset aPrev = myDefaultAspects.Aspect().PolygonOffset();
            myDefaultAspects.Aspect().SetPolygonOffset(theOffset);
            if (myAspectsApplied == myDefaultAspects.Aspect()
             || myAspectsApplied==null
             || (myAspectsApplied.PolygonOffset().Mode & Aspect_PolygonOffsetMode.Aspect_POM_None) == Aspect_PolygonOffsetMode.Aspect_POM_None)
            {
                myGlContext.SetPolygonOffset(theOffset);
            }
            return aPrev;
        }

        //! Apply aspects.
        //! @param theToBindTextures flag to bind texture set defined by applied aspect
        //! @return aspect set by SetAspects()
        public OpenGl_Aspects ApplyAspects(bool theToBindTextures = true)
        {
            //bool toSuppressBackFaces = myView->BackfacingModel() == Graphic3d_TypeOfBackfacingModel_BackCulled;
            Graphic3d_TypeOfBackfacingModel aCullFacesMode = myView.BackfacingModel();
            if (aCullFacesMode == Graphic3d_TypeOfBackfacingModel.Graphic3d_TypeOfBackfacingModel_Auto)
            {
                aCullFacesMode = myAspectsSet.Aspect().FaceCulling();
                if (aCullFacesMode == Graphic3d_TypeOfBackfacingModel.Graphic3d_TypeOfBackfacingModel_Auto)
                {
                    aCullFacesMode = Graphic3d_TypeOfBackfacingModel.Graphic3d_TypeOfBackfacingModel_DoubleSided;
                    if (myToAllowFaceCulling)
                    {
                        if (myAspectsSet.Aspect().InteriorStyle() == Aspect_InteriorStyle.Aspect_IS_HATCH
                         || myAspectsSet.Aspect().AlphaMode() == Graphic3d_AlphaMode.Graphic3d_AlphaMode_Blend
                         || myAspectsSet.Aspect().AlphaMode() == Graphic3d_AlphaMode.Graphic3d_AlphaMode_Mask
                         || myAspectsSet.Aspect().AlphaMode() == Graphic3d_AlphaMode.Graphic3d_AlphaMode_MaskBlend
                         || (myAspectsSet.Aspect().AlphaMode() == Graphic3d_AlphaMode.Graphic3d_AlphaMode_BlendAuto
                          && myAspectsSet.Aspect().FrontMaterial().Transparency() != 0.0f))
                        {
                            // disable culling in case of translucent shading aspect
                            aCullFacesMode = Graphic3d_TypeOfBackfacingModel.Graphic3d_TypeOfBackfacingModel_DoubleSided;
                        }
                        else
                        {
                            aCullFacesMode = Graphic3d_TypeOfBackfacingModel.Graphic3d_TypeOfBackfacingModel_BackCulled;
                        }
                    }
                }
            }
            myGlContext.SetFaceCulling(aCullFacesMode);

          //  if (myAspectsSet.Aspect() == myAspectsApplied
           //  && myHighlightStyle == myAspectFaceAppliedWithHL)
            {
            //    return myAspectsSet;
            }
           // myAspectFaceAppliedWithHL = myHighlightStyle;

            // Aspect_POM_None means: do not change current settings
            if ((myAspectsSet.Aspect().PolygonOffset().Mode & Aspect_PolygonOffsetMode.Aspect_POM_None) != Aspect_PolygonOffsetMode.Aspect_POM_None)
            {
                myGlContext.SetPolygonOffset(myAspectsSet.Aspect().PolygonOffset());
            }

            Aspect_InteriorStyle anIntstyle = myAspectsSet.Aspect().InteriorStyle();
            if (myAspectsApplied == null
             || myAspectsApplied.InteriorStyle() != anIntstyle)
            {
                myGlContext.SetPolygonMode(anIntstyle == Aspect_InteriorStyle.Aspect_IS_POINT ? All.Point : All.Fill);
                myGlContext.SetPolygonHatchEnabled(anIntstyle == Aspect_InteriorStyle.Aspect_IS_HATCH);
            }

            if (anIntstyle == Aspect_InteriorStyle.Aspect_IS_HATCH)
            {
               // myGlContext.SetPolygonHatchStyle(myAspectsSet.Aspect().HatchStyle());
            }

            // Case of hidden line
            if (anIntstyle == Aspect_InteriorStyle.Aspect_IS_HIDDENLINE)
            {
                // copy all values including line edge aspect
                /**myAspectFaceHl.Aspect() = *myAspectsSet->Aspect();
                myAspectFaceHl.Aspect()->SetShadingModel(Graphic3d_TypeOfShadingModel_Unlit);
                myAspectFaceHl.Aspect()->SetInteriorColor(myView->BackgroundColor().GetRGB());
                myAspectFaceHl.Aspect()->SetDistinguish(false);
                myAspectFaceHl.SetNoLighting();
                myAspectsSet = &myAspectFaceHl;*/
            }
            else
            {
                myGlContext.SetShadingMaterial(myAspectsSet, myHighlightStyle);
            }

            if (theToBindTextures)
            {
                OpenGl_TextureSet aTextureSet = TextureSet();
                myGlContext.BindTextures(aTextureSet, null);
            }

            //if ((myView.ShadingModel() ==Graphic3d_TypeOfShadingModel. Graphic3d_TypeOfShadingModel_Pbr
            //  || myView.ShadingModel() ==Graphic3d_TypeOfShadingModel. Graphic3d_TypeOfShadingModel_PbrFacet)
            // && !myView.myPBREnvironment.IsNull()
            // && myView.myPBREnvironment->IsNeededToBeBound())
            //{
            //    myView->myPBREnvironment->Bind(myGlContext);
            //}

            myAspectsApplied = myAspectsSet.Aspect();
            return myAspectsSet;
        }
        OpenGl_Aspects myAspectFaceHl; //!< Hiddenline aspect

        internal void SetAllowFaceCulling(object value)
        {
            //throw new NotImplementedException();
        }

        internal OpenGl_Aspects SetAspects(OpenGl_Aspects theAspect)
        {
            OpenGl_Aspects aPrevAspects = myAspectsSet;
            myAspectsSet = theAspect;
            return aPrevAspects;
        }

        //! Set filter for restricting rendering of particular elements.

        internal void SetRenderFilter(int theFilter)
        {
            myRenderFilter = theFilter;

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

        //! Return TextureSet from set Aspects or Environment texture.
        internal OpenGl_TextureSet TextureSet()
        {
            OpenGl_TextureSet aTextureSet = myAspectsSet.TextureSet(myGlContext, ToHighlight());
            return aTextureSet != null
                  || myAspectsSet.Aspect().ToMapTexture()
                  ? aTextureSet
                  : myEnvironmentTexture;
        }
        OpenGl_TextureSet myEnvironmentTexture;

        internal OpenGl_TextureSet EnvironmentTexture()
        {
            throw new NotImplementedException();
        }

        //! Return true if following structures should apply highlight color.
        internal bool ToHighlight()
        {
            return myHighlightStyle != null;
        }


        //! @return true if depth writing is enabled.
        public bool UseDepthWrite() { return myUseDepthWrite; }
        public void UseDepthWrite(bool v) { myUseDepthWrite = v; }

        //! Sets a new environment texture.
        public void SetEnvironmentTexture(OpenGl_TextureSet theTexture)
        {
            myEnvironmentTexture = theTexture;
        }


        //! Reset skipped transparent elements counter.
        //! @sa OpenGl_LayerList::Render()
        public void ResetSkippedCounter() { myNbSkippedTranspElems = 0; }

        //! Return the number of skipped transparent elements within active OpenGl_RenderFilter_OpaqueOnly filter.
        //! @sa OpenGl_LayerList::Render()
        public int NbSkippedTransparentElements() { return myNbSkippedTranspElems; }


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

            //
            myAspectsSet = (myDefaultAspects);
            //
            myToAllowFaceCulling = (false);
        }
        OpenGl_Aspects myDefaultAspects = new OpenGl_Aspects();

        bool myToAllowFaceCulling; //!< allow back face culling

    }
}