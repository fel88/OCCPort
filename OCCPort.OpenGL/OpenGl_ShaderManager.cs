using OCCPort.Enums;
using OpenTK.Compute.OpenCL;
using OpenTK.Graphics.Egl;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Linq;
using System.Reflection.Metadata;
using TKernel;
using TKMath;
using TKService;

namespace OCCPort.OpenGL
{
    internal class OpenGl_ShaderManager : Graphic3d_ShaderManager

    {
        //! Overwrites context
        public void SetContext(OpenGl_Context theCtx)
        {
            myContext = theCtx;
        }

        //! Creates new shader program or re-use shared instance.
        //! @param theProxy    [IN]  program definition
        //! @param theShareKey [OUT] sharing key
        //! @param theProgram  [OUT] OpenGL program
        //! @return true on success
        public bool Create(Graphic3d_ShaderProgram theProxy,
                ref string theShareKey,
                OpenGl_ShaderProgram theProgram)
        {
            theProgram = null;
            if (theProxy == null)
            {
                return false;
            }

            theShareKey = theProxy.GetId();
            if (myContext.GetResource<OpenGl_ShaderProgram>(theShareKey, ref theProgram))
            {
                if (theProgram.Share())
                {
                    myProgramList.Append(theProgram);
                }
                return true;
            }

            theProgram = new OpenGl_ShaderProgram(theProxy);
            if (!theProgram.Initialize(myContext, theProxy.ShaderObjects()))
            {
                theProgram.Release(myContext);
                theShareKey = string.Empty;                //theShareKey.Clear();
                theProgram = null;
                return false;
            }

            myProgramList.Append(theProgram);
            myContext.ShareResource(theShareKey, theProgram);
            return true;
        }

        //! Generates shader program to render correctly colored quad.
        public Graphic3d_ShaderProgram GetColoredQuadProgram()
        {
            if (myColoredQuadProgram == null)
            {
                myColoredQuadProgram = getColoredQuadProgram();
            }
            return myColoredQuadProgram;
        }


        // =======================================================================
        // function : Unregister
        // purpose  : Removes specified shader program from the manager
        // =======================================================================
        public void Unregister(ref string theShareKey,
                                                OpenGl_ShaderProgram theProgram)
        {
            //for (OpenGl_ShaderProgramList::Iterator anIt (myProgramList); anIt.More(); anIt.Next())
            foreach (var anIt in myProgramList)
            {
                if (anIt == theProgram)
                {
                    if (!theProgram.UnShare())
                    {
                        theShareKey = string.Empty; //theShareKey.Clear();
                        theProgram = null;
                        return;
                    }

                    myProgramList.Remove(anIt);
                    break;
                }
            }

            /*string anID = theProgram.myProxy.GetId();
           if (anID.IsEmpty())
           {
               myContext.DelayedRelease(theProgram);
               theProgram = null;
           }
           else
           {
               theProgram.Nullify();
               myContext->ReleaseResource(anID, Standard_True);
           }*/
        }

        Graphic3d_TypeOfShadingModel myShadingModel;       //!< lighting shading model
        OpenGl_ShaderProgramList myProgramList;        //!< The list of shader programs
        OpenGl_SetOfShaderPrograms myLightPrograms;      //!< pointer to active lighting programs matrix
        OpenGl_SetOfPrograms myUnlitPrograms;      //!< programs matrix without lighting
        OpenGl_SetOfPrograms myOutlinePrograms;    //!< programs matrix without lighting for outline presentation
        OpenGl_ShaderProgram myFontProgram;        //!< standard program for textured text
                                                   //NCollection_Array1<Handle(OpenGl_ShaderProgram)>
                                                   //    myBlitPrograms[2];    //!< standard program for FBO blit emulation
        OpenGl_ShaderProgram myBoundBoxProgram;    //!< standard program for bounding box
        OpenGl_ShaderProgram[] myOitCompositingProgram;//[2]; //!< standard program for OIT compositing (default and MSAA).
        OpenGl_ShaderProgram[] myOitDepthPeelingBlendProgram;//[2]; //!< standard program for OIT Depth Peeling blend (default and MSAA)
        OpenGl_ShaderProgram[] myOitDepthPeelingFlushProgram;//[2]; //!< standard program for OIT Depth Peeling flush (default and MSAA)
                                                             //OpenGl_MapOfShaderPrograms myMapOfLightPrograms; //!< map of lighting programs depending on lights configuration

        OpenGl_ShaderProgram[] myPBREnvBakingProgram;//[3]; //!< programs for IBL maps generation used in PBR pipeline (0 for Diffuse; 1 for Specular; 2 for fallback)
        Graphic3d_ShaderProgram myBgCubeMapProgram;       //!< program for background cubemap rendering
        Graphic3d_ShaderProgram myBgSkydomeProgram;       //!< program for background cubemap rendering
        Graphic3d_ShaderProgram myColoredQuadProgram;     //!< program for correct quad rendering

        OpenGl_ShaderProgram[] myStereoPrograms;//[Graphic3d_StereoMode_NB]; //!< standard stereo programs

        OpenGl_VertexBuffer myBoundBoxVertBuffer; //!< bounding box vertex buffer




        OpenGl_Context myContext;            //!< OpenGL context


        OpenGl_ProjectionState myProjectionState;    //!< State of OCCT projection  transformation
        OpenGl_ModelWorldState myModelWorldState;    //!< State of OCCT model-world transformation
        OpenGl_WorldViewState myWorldViewState;     //!< State of OCCT world-view  transformation
        //OpenGl_ClippingState myClippingState;      //!< State of OCCT clipping planes
        OpenGl_LightSourceState myLightSourceState;   //!< State of OCCT light sources
        OpenGl_MaterialState myMaterialState = new OpenGl_MaterialState();      //!< State of Front and Back materials
        //OpenGl_OitState myOitState;           //!< State of OIT uniforms

        gp_XYZ myLocalOrigin;        //!< local camera transformation
        bool myHasLocalOrigin;     //!< flag indicating that local camera transformation has been set

        public OpenGl_ShaderManager(OpenGl_Context theContext) : base(theContext.GraphicsLibrary())

        {
            myFfpProgram = new OpenGl_ShaderProgramFFP();
            myShadingModel = Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Gouraud;
            myUnlitPrograms = new OpenGl_SetOfPrograms();
            myContext = theContext;
            //myHasLocalOrigin(Standard_False)
            //mySRgbState = theContext->ToRenderSRGB();
        }



        // =======================================================================
        // function : prepareStdProgramUnlit
        // purpose  :
        // =======================================================================
        bool prepareStdProgramUnlit(ref OpenGl_ShaderProgram theProgram,
                                                                       int theBits,
                                                                       bool theIsOutline)
        {
            Graphic3d_ShaderProgram aProgramSrc = getStdProgramUnlit(theBits, theIsOutline);
            string aKey = "";
            if (!Create(aProgramSrc, ref aKey, theProgram))
            {
                theProgram = new OpenGl_ShaderProgram(); // just mark as invalid
                return false;
            }
            return true;
        }

        public bool BindFboBlitProgram(int theNbSamples,
                                                               bool theIsFallback_sRGB)
        {
            NCollection_Array1<OpenGl_ShaderProgram> aList = myBlitPrograms[theIsFallback_sRGB ? 1 : 0];
            int aNbSamples = Math.Max(theNbSamples, 1);
            if (aNbSamples > aList.Upper())
            {
                aList.Resize(1, aNbSamples, true);
            }

            OpenGl_ShaderProgram aProg = aList[aNbSamples];
            if (aProg != null)
            {
                return myContext.BindProgram(aProg);
            }

            Graphic3d_ShaderProgram aProgramSrc = getStdProgramFboBlit(aNbSamples, theIsFallback_sRGB);
            string aKey = "";
            if (!Create(aProgramSrc, ref aKey, aProg))
            {
                aProg = new OpenGl_ShaderProgram(); // just mark as invalid
                return false;
            }

            myContext.BindProgram(aProg);
            aProg.SetSampler(myContext, "uColorSampler", Graphic3d_TextureUnit.Graphic3d_TextureUnit_0);
            aProg.SetSampler(myContext, "uDepthSampler", Graphic3d_TextureUnit.Graphic3d_TextureUnit_1);
            return true;
        }

        NCollection_Array1<OpenGl_ShaderProgram>[]
                                     myBlitPrograms = new NCollection_Array1<OpenGl_ShaderProgram>[2];    //!< standard program for FBO blit emulation

        //! Prepare standard GLSL program.
        public OpenGl_ShaderProgram getStdProgram(Graphic3d_TypeOfShadingModel theShadingModel,
                                                      int theBits)
        {
            if (theShadingModel == Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Unlit
   || ((theBits & (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_HasTextures) == (int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_TextureEnv))
            {
                // If environment map is enabled lighting calculations are
                // not needed (in accordance with default OCCT behavior)
                OpenGl_ShaderProgram aProgram = myUnlitPrograms.ChangeValue(theBits);
                if (aProgram == null)
                {
                    prepareStdProgramUnlit(ref aProgram, theBits, false);
                    myUnlitPrograms.ChangeValue(theBits, aProgram);// not original code
                }
                return aProgram;
            }
            {
                OpenGl_ShaderProgram aProgram = myLightPrograms.ChangeValue(theShadingModel, theBits);
                if (aProgram == null)
                {
                    prepareStdProgramLight(ref aProgram, theShadingModel, theBits);
                    myLightPrograms.ChangeValue(theShadingModel, theBits, aProgram);//not original code
                }
                return aProgram;
            }
        }

        //! Prepare standard GLSL program with lighting.
        bool prepareStdProgramLight(ref OpenGl_ShaderProgram theProgram,
                                                 Graphic3d_TypeOfShadingModel theShadingModel,
                                                 int theBits)
        {
            switch (theShadingModel)
            {
                case Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Unlit: return prepareStdProgramUnlit(ref theProgram, theBits, false);
                case Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_PhongFacet: return prepareStdProgramPhong(theProgram, theBits, true);
                case Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Gouraud: return prepareStdProgramGouraud(theProgram, theBits);
                case Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_DEFAULT:
                case Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Phong: return prepareStdProgramPhong(theProgram, theBits, false);
                case Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Pbr: return prepareStdProgramPhong(theProgram, theBits, false, true);
                case Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_PbrFacet: return prepareStdProgramPhong(theProgram, theBits, true, true);
            }
            return false;
        }

        private bool prepareStdProgramGouraud(OpenGl_ShaderProgram theProgram, int theBits)
        {
            throw new NotImplementedException();
        }

        // =======================================================================
        // function : prepareStdProgramPhong
        // purpose  :
        // =======================================================================
        bool prepareStdProgramPhong(OpenGl_ShaderProgram theProgram,
                                                                int theBits,
                                                                bool theIsFlatNormal = false,
                                                                bool theIsPBR = false)
        {
            int aNbShadowMaps = myLightSourceState.HasShadowMaps()
                                           ? myLightSourceState.LightSources().NbCastShadows()
                                           : 0;
            Graphic3d_ShaderProgram aProgramSrc = getStdProgramPhong(myLightSourceState.LightSources(), theBits, theIsFlatNormal, theIsPBR, aNbShadowMaps);
            string aKey = "";
            if (!Create(aProgramSrc, ref aKey, theProgram))
            {
                theProgram = new OpenGl_ShaderProgram(); // just mark as invalid
                return false;
            }
            return true;
        }
        //! Checks whether one of PBR shading models is set as default model.
        public bool IsPbrAllowed()
        {
            return myShadingModel == Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Pbr
                                              || myShadingModel == Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_PbrFacet;
        }



        //! Resets PBR shading models to corresponding non-PBR ones if PBR is not allowed.
        public static Graphic3d_TypeOfShadingModel PBRShadingModelFallback(Graphic3d_TypeOfShadingModel theShadingModel,
                                                                     bool theIsPbrAllowed = false)
        {
            if (theIsPbrAllowed)
            {
                return theShadingModel;
            }

            switch (theShadingModel)
            {
                case Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Pbr: return Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Phong;
                case Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_PbrFacet: return Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_PhongFacet;
                default: return theShadingModel;
            }
        }


        //! Choose Shading Model for filled primitives.
        //! Fallbacks to FACET model if there are no normal attributes.
        //! Fallbacks to corresponding non-PBR models if PBR is unavailable.

        internal Graphic3d_TypeOfShadingModel ChooseFaceShadingModel(
            Graphic3d_TypeOfShadingModel theCustomModel, bool theHasNodalNormals)
        {

            if (!myContext.ColorMask())
            {
                return Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Unlit;
            }

            Graphic3d_TypeOfShadingModel aModel = theCustomModel != Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_DEFAULT ? theCustomModel : myShadingModel;
            switch (aModel)
            {
                case Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_DEFAULT:
                case Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Unlit:
                case Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_PhongFacet:
                    return aModel;
                case Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Gouraud:
                case Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Phong:
                    return theHasNodalNormals ? aModel : Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_PhongFacet;
                case Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Pbr:
                    return PBRShadingModelFallback(theHasNodalNormals ? aModel : Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_PbrFacet, IsPbrAllowed());
                case Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_PbrFacet:
                    return PBRShadingModelFallback(aModel, IsPbrAllowed());
            }
            return Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Unlit;

        }

        //! Bind program for filled primitives rendering
        public bool BindFaceProgram(OpenGl_TextureSet theTextures,
                                    Graphic3d_TypeOfShadingModel theShadingModel,
                                    Graphic3d_AlphaMode theAlphaMode,
                                    bool theHasVertColor,
                                    bool theEnableEnvMap,
                                     OpenGl_ShaderProgram theCustomProgram)
        {
            return BindFaceProgram(theTextures, theShadingModel, theAlphaMode, Aspect_InteriorStyle.Aspect_IS_SOLID,
                                    theHasVertColor, theEnableEnvMap, false, theCustomProgram);
        }
        //! Bind program for filled primitives rendering
        public bool BindFaceProgram(OpenGl_TextureSet theTextures,
                                     Graphic3d_TypeOfShadingModel theShadingModel,
                                     Graphic3d_AlphaMode theAlphaMode,
                                     Aspect_InteriorStyle theInteriorStyle,
                                     bool theHasVertColor,
                                     bool theEnableEnvMap,
                                     bool theEnableMeshEdges,
                                      OpenGl_ShaderProgram theCustomProgram)
        {
            Graphic3d_TypeOfShadingModel aShadeModelOnFace = theShadingModel != Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Unlit
                                                       && (theTextures == null || theTextures.IsModulate())
                                                       ? theShadingModel
                                                       : Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Unlit;
            if (theCustomProgram != null
             || myContext.caps.ffpEnable)
            {
                return bindProgramWithState(theCustomProgram, aShadeModelOnFace);
            }

            int aBits = getProgramBits(theTextures, theAlphaMode, theInteriorStyle, theHasVertColor, theEnableEnvMap, theEnableMeshEdges);
            OpenGl_ShaderProgram aProgram = getStdProgram(aShadeModelOnFace, aBits);
            return bindProgramWithState(aProgram, aShadeModelOnFace);
        }

        //! Bind specified program to current context and apply state.
        bool bindProgramWithState(OpenGl_ShaderProgram theProgram,
                                                        Graphic3d_TypeOfShadingModel theShadingModel)
        {
            bool isBound = myContext.BindProgram(theProgram);
            if (isBound
            && theProgram != null)
            {
                theProgram.ApplyVariables(myContext);
            }
            PushState(theProgram, theShadingModel);
            return isBound;
        }
        OpenGl_ShaderProgramFFP myFfpProgram;
        //! Pushes current state of OCCT projection transform to specified program (only on state change).
        public void PushProjectionState(OpenGl_ShaderProgram theProgram)
        {
            if (myProjectionState.Index() != theProgram.ActiveState(OpenGl_UniformStateType.OpenGl_PROJECTION_STATE))
            {
                pushProjectionState(theProgram);
            }
        }

        private void pushProjectionState(OpenGl_ShaderProgram theProgram)
        {
            theProgram.UpdateState(OpenGl_UniformStateType.OpenGl_PROJECTION_STATE, myProjectionState.Index());
            if (theProgram == myFfpProgram)
            {
                if (myContext.core11ffp != null)
                {
                    myContext.core11ffp.glMatrixMode(All.Projection);
                    myContext.core11ffp.glLoadMatrixf(myProjectionState.ProjectionMatrix());
                }
                return;
            }
            /*
             
  theProgram->SetUniform (myContext,
                          theProgram->GetStateLocation (OpenGl_OCC_PROJECTION_MATRIX),
                          myProjectionState.ProjectionMatrix());

  GLint aLocation = theProgram->GetStateLocation (OpenGl_OCC_PROJECTION_MATRIX_INVERSE);
  if (aLocation != OpenGl_ShaderProgram::INVALID_LOCATION)
  {
    theProgram->SetUniform (myContext, aLocation, myProjectionState.ProjectionMatrixInverse());
  }

  theProgram->SetUniform (myContext,
                          theProgram->GetStateLocation (OpenGl_OCC_PROJECTION_MATRIX_TRANSPOSE),
                          myProjectionState.ProjectionMatrix(), true);

  aLocation = theProgram->GetStateLocation (OpenGl_OCC_PROJECTION_MATRIX_INVERSE_TRANSPOSE);
  if (aLocation != OpenGl_ShaderProgram::INVALID_LOCATION)
  {
    theProgram->SetUniform (myContext, aLocation, myProjectionState.ProjectionMatrixInverse(), true);
  }*/
        }

        // =======================================================================
        // function : PushState
        // purpose  : Pushes state of OCCT graphics parameters to the program
        // =======================================================================
        public void PushState(OpenGl_ShaderProgram theProgram,

                    Graphic3d_TypeOfShadingModel theShadingModel = Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Unlit)
        {
            OpenGl_ShaderProgram aProgram = theProgram != null ? theProgram : myFfpProgram;
            /*PushClippingState(aProgram);
            PushLightSourceState(aProgram); // should be before PushWorldViewState()
            PushWorldViewState(aProgram);
            PushModelWorldState(aProgram);*/
            PushProjectionState(aProgram);/*
            PushMaterialState(aProgram);
            PushOitState(aProgram);*/

            if (theProgram != null)

            {
                OpenGl_ShaderUniformLocation aLocViewPort = theProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCCT_VIEWPORT);
                if (aLocViewPort != null)
                {
                    theProgram.SetUniform(myContext, aLocViewPort, new Vector4((float)myContext.Viewport()[0], (float)myContext.Viewport()[1],
                                                                                (float)myContext.Viewport()[2], (float)myContext.Viewport()[3]));
                }
            }
            else if (myContext.core11ffp != null)
            {
                // manage FFP lighting
                myContext.SetShadeModel(theShadingModel);
                if (theShadingModel == Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Unlit)
                {
                    myContext.core11fwd.glDisable((int)All.Lighting);
                }
                else
                {
                    myContext.core11fwd.glEnable((int)All.Lighting);
                }
            }
        }

        //! Define clipping planes program bits.
        int getClipPlaneBits()
        {
            int aNbPlanes = myContext.Clipping().NbClippingOrCappingOn();
            if (aNbPlanes <= 0)
            {
                return 0;
            }

            Graphic3d_ShaderFlags aBits = 0;
            if (myContext.Clipping().HasClippingChains())
            {
                aBits |= Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_ClipChains;
            }

            if (aNbPlanes == 1)
            {
                aBits |= Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_ClipPlanes1;
            }
            else if (aNbPlanes == 2)
            {
                aBits |= Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_ClipPlanes2;
            }
            else
            {
                aBits |= Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_ClipPlanesN;
            }
            return (int)aBits;
        }

        //! Define program bits.
        int getProgramBits(OpenGl_TextureSet theTextures,
                                   Graphic3d_AlphaMode theAlphaMode,
                                   Aspect_InteriorStyle theInteriorStyle,
                                   bool theHasVertColor,
                                   bool theEnableEnvMap,
                                   bool theEnableMeshEdges)
        {
            Graphic3d_ShaderFlags aBits = 0;
            if (theAlphaMode == Graphic3d_AlphaMode.Graphic3d_AlphaMode_Mask
             || theAlphaMode == Graphic3d_AlphaMode.Graphic3d_AlphaMode_MaskBlend)
            {
                aBits |= Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_AlphaTest;
            }

            aBits |= (Graphic3d_ShaderFlags)getClipPlaneBits();
            if (theEnableMeshEdges
             && myContext.hasGeometryStage != OpenGl_FeatureFlag.OpenGl_FeatureNotAvailable)
            {
                aBits |= Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_MeshEdges;
                if (theInteriorStyle == Aspect_InteriorStyle.Aspect_IS_HOLLOW)
                {
                    aBits |= Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_AlphaTest;
                }
            }

            if (theEnableEnvMap)
            {
                // Environment map overwrites material texture
                aBits |= Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_TextureEnv;
            }
            else if (theTextures != null
                   && theTextures.HasNonPointSprite())
            {
                aBits |= Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_TextureRGB;
                if ((theTextures.TextureSetBits() & (int)Graphic3d_TextureSetBits.Graphic3d_TextureSetBits_Normal) != 0)
                {
                    aBits |= Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_TextureNormal;
                }
            }
            if (theHasVertColor
             && theInteriorStyle != Aspect_InteriorStyle.Aspect_IS_HIDDENLINE)
            {
                aBits |= Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_VertColor;
            }

            if (myOitState.ActiveMode() == Graphic3d_RenderTransparentMethod.Graphic3d_RTM_BLEND_OIT)
            {
                aBits |= Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_WriteOit;
            }
            else if (myOitState.ActiveMode() == Graphic3d_RenderTransparentMethod.Graphic3d_RTM_DEPTH_PEELING_OIT)
            {
                aBits |= Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_OitDepthPeeling;
            }
            return (int)aBits;
        }
        OpenGl_OitState myOitState = new OpenGl_OitState();           //!< State of OIT uniforms

        //! Returns list of registered shader programs.
        public override OpenGl_ShaderProgramList ShaderPrograms() { return myProgramList; }

        //! Returns true if no program objects are registered in the manager.
        public bool IsEmpty() { return myProgramList.IsEmpty(); }
        //! Returns current state of material.
        public OpenGl_MaterialState MaterialState() { return myMaterialState; }


    }
}