//! List of shader programs.
global using OpenGl_MapOfShaderPrograms = TKernel.NCollection_DataMap<string, OCCPort.OpenGL.OpenGl_SetOfShaderPrograms>;
global using OpenGl_ShaderProgramList = TKernel.NCollection_Sequence<OCCPort.OpenGL.OpenGl_ShaderProgram>;
global using OpenGl_Vec4d = TKernel.NCollection_Vec4<double>;
using OCCPort.Common;
using OCCPort.Enums;
using OCCPort.OpenGL;
using OpenTK.Audio.OpenAL;
using OpenTK.Compute.OpenCL;
using OpenTK.Graphics.Egl;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using TKernel;
using TKMath;
using TKService;

namespace OCCPort.OpenGL
{
    //! This class is responsible for managing shader programs.
    public class OpenGl_ShaderManager : Graphic3d_ShaderManager
    {
        //! Updates state of material.
        public void UpdateMaterialStateTo(OpenGl_Material theMat,
                                   float theAlphaCutoff,
                                   bool theToDistinguish,
                                   bool theToMapTexture)
        {
            myMaterialState.Set(theMat, theAlphaCutoff, theToDistinguish, theToMapTexture);
            myMaterialState.Update();
        }

        public void UpdateSRgbState()
        {
            if (mySRgbState == myContext.ToRenderSRGB())
            {
                return;
            }

            mySRgbState = myContext.ToRenderSRGB();

            // special cases - GLSL programs dealing with sRGB/linearRGB internally
            //myStereoPrograms[(int)Graphic3d_StereoMode.Graphic3d_StereoMode_Anaglyph] = null;
        }

        public void UpdateLightSourceStateTo(Graphic3d_LightSet theLights,
                                                     uint theSpecIBLMapLevels,
                                                     OpenGl_ShadowMapArray theShadowMaps)
        {
            myLightSourceState.Set(theLights);
            myLightSourceState.SetSpecIBLMapLevels((int)theSpecIBLMapLevels);
            myLightSourceState.SetShadowMaps(theShadowMaps);
            myLightSourceState.Update();
            switchLightPrograms();
        }


        // =======================================================================
        // function : UpdateClippingState
        // purpose  : Updates state of OCCT clipping planes
        // =======================================================================
        public void UpdateClippingState()
        {
            myClippingState.Update();
        }

        OpenGl_ClippingState myClippingState = new OpenGl_ClippingState();      //!< State of OCCT clipping planes


        //! Returns current state of OCCT light sources.
        public OpenGl_LightSourceState LightSourceState() { return myLightSourceState; }

        //! Returns current state of OCCT projection transform.
        public OpenGl_ProjectionState ProjectionState() { return myProjectionState; }

        // =======================================================================
        // function : SetProjectionState
        // purpose  : Sets new state of OCCT projection transform
        // =======================================================================
        public void UpdateProjectionStateTo(OpenGl_Mat4 theProjectionMatrix)
        {
            myProjectionState.Set(theProjectionMatrix);
            myProjectionState.Update();
        }

        //! Pushes current state of OCCT model-world transform to specified program (only on state change).
        public void PushModelWorldState(OpenGl_ShaderProgram theProgram)
        {
            if (myModelWorldState.Index() != theProgram.ActiveState(OpenGl_UniformStateType.OpenGl_MODEL_WORLD_STATE))
            {
                pushModelWorldState(theProgram);
            }
        }

        //! Returns current state of OCCT world-view transform.
        public OpenGl_WorldViewState WorldViewState() { return myWorldViewState; }

        //! Updates state of OCCT world-view transform.
        public void UpdateWorldViewStateTo(OpenGl_Mat4 theWorldViewMatrix)
        {
            myWorldViewState.Set(theWorldViewMatrix);
            myWorldViewState.Update();
        }

        //! Returns current state of OCCT model-world transform.
        public OpenGl_ModelWorldState ModelWorldState() { return myModelWorldState; }

        // =======================================================================
        // function : SetModelWorldState
        // purpose  : Sets new state of OCCT model-world transform
        // =======================================================================
        public void UpdateModelWorldStateTo(OpenGl_Mat4 theModelWorldMatrix)
        {
            myModelWorldState.Set(theModelWorldMatrix);
            myModelWorldState.Update();
        }
        // =======================================================================
        // function : pushProjectionState
        // purpose  :
        // =======================================================================
        void pushProjectionState(OpenGl_ShaderProgram theProgram)
        {
            theProgram.UpdateState(OpenGl_UniformStateType.OpenGl_PROJECTION_STATE, myProjectionState.Index());
            if (theProgram == myFfpProgram)
            {
                if (myContext.core11ffp != null)
                {
                    myContext.core11ffp.glMatrixMode(All.Projection);
                    myContext.core11ffp.glLoadMatrixf(myProjectionState.ProjectionMatrix().GetData());
                }
                return;
            }

            theProgram.SetUniform(myContext,
                                    theProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCC_PROJECTION_MATRIX),
                                    myProjectionState.ProjectionMatrix());

            GLint aLocation = theProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCC_PROJECTION_MATRIX_INVERSE);
            if (aLocation != OpenGl_ShaderProgram.INVALID_LOCATION)
            {
                theProgram.SetUniform(myContext, aLocation, myProjectionState.ProjectionMatrixInverse());
            }

            theProgram.SetUniform(myContext,
                                    theProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCC_PROJECTION_MATRIX_TRANSPOSE),
                                    myProjectionState.ProjectionMatrix(), true);

            aLocation = theProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCC_PROJECTION_MATRIX_INVERSE_TRANSPOSE);
            if (aLocation != OpenGl_ShaderProgram.INVALID_LOCATION)
            {
                theProgram.SetUniform(myContext, aLocation, myProjectionState.ProjectionMatrixInverse(), true);
            }
        }

        //! Pushes current state of OCCT model-world transform to specified program.
        public void pushModelWorldState(OpenGl_ShaderProgram theProgram)
        {
            theProgram.UpdateState(OpenGl_UniformStateType.OpenGl_MODEL_WORLD_STATE, myModelWorldState.Index());
            if (theProgram == myFfpProgram)
            {
                if (myContext.core11ffp != null)
                {
                    OpenGl_Mat4 aModelView = myWorldViewState.WorldViewMatrix() * myModelWorldState.ModelWorldMatrix();
                    myContext.core11ffp.glMatrixMode(All.Modelview);
                    myContext.core11ffp.glLoadMatrixf(aModelView.GetData());
                    theProgram.UpdateState(OpenGl_UniformStateType.OpenGl_WORLD_VIEW_STATE, myWorldViewState.Index());
                }
                return;
            }

            theProgram.SetUniform(myContext,
                                    theProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCC_MODEL_WORLD_MATRIX),
                                    myModelWorldState.ModelWorldMatrix());

            GLint aLocation = theProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCC_MODEL_WORLD_MATRIX_INVERSE);
            if (aLocation != OpenGl_ShaderProgram.INVALID_LOCATION)
            {
                theProgram.SetUniform(myContext, aLocation, myModelWorldState.ModelWorldMatrixInverse());
            }

            theProgram.SetUniform(myContext,
                                    theProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCC_MODEL_WORLD_MATRIX_TRANSPOSE),
                                    myModelWorldState.ModelWorldMatrix(), true);

            aLocation = theProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCC_MODEL_WORLD_MATRIX_INVERSE_TRANSPOSE);
            if (aLocation != OpenGl_ShaderProgram.INVALID_LOCATION)
            {
                theProgram.SetUniform(myContext, aLocation, myModelWorldState.ModelWorldMatrixInverse(), true);
            }
        }

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
              ref OpenGl_ShaderProgram theProgram)
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
        OpenGl_ShaderProgramList myProgramList = new OpenGl_ShaderProgramList();        //!< The list of shader programs
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


        OpenGl_ProjectionState myProjectionState = new OpenGl_ProjectionState();    //!< State of OCCT projection  transformation
        OpenGl_ModelWorldState myModelWorldState = new OpenGl_ModelWorldState();    //!< State of OCCT model-world transformation
        OpenGl_WorldViewState myWorldViewState = new OpenGl_WorldViewState();     //!< State of OCCT world-view  transformation
        //OpenGl_ClippingState myClippingState;      //!< State of OCCT clipping planes
        OpenGl_LightSourceState myLightSourceState = new OpenGl_LightSourceState();   //!< State of OCCT light sources
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
            myHasLocalOrigin = (false);
            mySRgbState = theContext.ToRenderSRGB();

            // not origin code here
            for (int i = 0; i < myBlitPrograms.Length; i++)
            {
                myBlitPrograms[i] = new NCollection_Array1<OpenGl_ShaderProgram>();
            }
            //end of not origin code
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
            if (!Create(aProgramSrc, ref aKey, ref theProgram))
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
            if (!Create(aProgramSrc, ref aKey, ref aProg))
            {
                aProg = new OpenGl_ShaderProgram(); // just mark as invalid
                return false;
            }
            else
            {
                aList[aNbSamples] = aProg;
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
                case Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_PhongFacet: return prepareStdProgramPhong(ref theProgram, theBits, true);
                case Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Gouraud: return prepareStdProgramGouraud(theProgram, theBits);
                case Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_DEFAULT:
                case Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Phong: return prepareStdProgramPhong(ref theProgram, theBits, false);
                case Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Pbr: return prepareStdProgramPhong(ref theProgram, theBits, false, true);
                case Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_PbrFacet: return prepareStdProgramPhong(ref theProgram, theBits, true, true);
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
        bool prepareStdProgramPhong(ref OpenGl_ShaderProgram theProgram,
                                                                int theBits,
                                                                bool theIsFlatNormal = false,
                                                                bool theIsPBR = false)
        {
            int aNbShadowMaps = myLightSourceState.HasShadowMaps()
                                           ? myLightSourceState.LightSources().NbCastShadows()
                                           : 0;
            Graphic3d_ShaderProgram aProgramSrc = getStdProgramPhong(myLightSourceState.LightSources(), theBits, theIsFlatNormal, theIsPBR, aNbShadowMaps);
            string aKey = "";
            if (!Create(aProgramSrc, ref aKey, ref theProgram))
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

        public void SetShadingModel(Graphic3d_TypeOfShadingModel theModel)
        {
            if (theModel == Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_DEFAULT)
            {
                throw new Standard_ProgramError("OpenGl_ShaderManager::SetShadingModel() - attempt to set invalid Shading Model!");
            }

            myShadingModel = theModel;
            switchLightPrograms();
        }

        OpenGl_MapOfShaderPrograms myMapOfLightPrograms = new OpenGl_MapOfShaderPrograms(); //!< map of lighting programs depending on lights configuration

        void switchLightPrograms()
        {
            Graphic3d_LightSet aLights = myLightSourceState.LightSources();
            if (aLights == null)
            {
                if (!myMapOfLightPrograms.Find("unlit", out myLightPrograms))
                {
                    myLightPrograms = new OpenGl_SetOfShaderPrograms(myUnlitPrograms);
                    myMapOfLightPrograms.Bind("unlit", myLightPrograms);
                }
                return;
            }

            string aKey = genLightKey(aLights, myLightSourceState.HasShadowMaps());
            if (!myMapOfLightPrograms.Find(aKey, out myLightPrograms))
            {
                myLightPrograms = new OpenGl_SetOfShaderPrograms();
                myMapOfLightPrograms.Bind(aKey, myLightPrograms);
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


        // =======================================================================
        // function : pushWorldViewState
        // purpose  :
        // =======================================================================
        void pushWorldViewState(OpenGl_ShaderProgram theProgram)
        {
            if (myWorldViewState.Index() == theProgram.ActiveState(OpenGl_UniformStateType.OpenGl_WORLD_VIEW_STATE))
            {
                return;
            }

            theProgram.UpdateState(OpenGl_UniformStateType.OpenGl_WORLD_VIEW_STATE, myWorldViewState.Index());
            if (theProgram == myFfpProgram)
            {
                if (myContext.core11ffp != null)
                {
                    OpenGl_Mat4 aModelView = myWorldViewState.WorldViewMatrix() * myModelWorldState.ModelWorldMatrix();
                    myContext.core11ffp.glMatrixMode(All.Modelview);
                    myContext.core11ffp.glLoadMatrixf(aModelView.GetData());
                    theProgram.UpdateState(OpenGl_UniformStateType.OpenGl_MODEL_WORLD_STATE, myModelWorldState.Index());
                }
                return;
            }

            theProgram.SetUniform(myContext,
                                    theProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCC_WORLD_VIEW_MATRIX),
                                    myWorldViewState.WorldViewMatrix());

            GLint aLocation = theProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCC_WORLD_VIEW_MATRIX_INVERSE);
            if (aLocation != OpenGl_ShaderProgram.INVALID_LOCATION)
            {
                theProgram.SetUniform(myContext, aLocation, myWorldViewState.WorldViewMatrixInverse());
            }

            theProgram.SetUniform(myContext,
                                    theProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCC_WORLD_VIEW_MATRIX_TRANSPOSE),
                                    myWorldViewState.WorldViewMatrix(), true);

            aLocation = theProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCC_WORLD_VIEW_MATRIX_INVERSE_TRANSPOSE);
            if (aLocation != OpenGl_ShaderProgram.INVALID_LOCATION)
            {
                theProgram.SetUniform(myContext, aLocation, myWorldViewState.WorldViewMatrixInverse(), true);
            }
        }


        //! Pushes current state of OCCT world-view transform to specified program (only on state change).
        void PushWorldViewState(OpenGl_ShaderProgram theProgram)
        {
            if (myWorldViewState.Index() != theProgram.ActiveState(OpenGl_UniformStateType.OpenGl_WORLD_VIEW_STATE))
            {
                pushWorldViewState(theProgram);
            }
        }

        //! Pushes current state of OCCT light sources to specified program (only on state change).
        //! Note that light sources definition depends also on WorldViewState.
        public void PushLightSourceState(OpenGl_ShaderProgram theProgram)
        {
            if (myLightSourceState.Index() != theProgram.ActiveState(OpenGl_UniformStateType.OpenGl_LIGHT_SOURCES_STATE)
             || myWorldViewState.Index() != theProgram.ActiveState(OpenGl_UniformStateType.OpenGl_WORLD_VIEW_STATE))
            {
                pushLightSourceState(theProgram);
            }

        }
        static float[] THE_DEFAULT_AMBIENT = [0.0f, 0.0f, 0.0f, 1.0f];
        static float[] THE_DEFAULT_SPOT_DIR = { 0.0f, 0.0f, -1.0f };
        static float THE_DEFAULT_SPOT_EXPONENT = 0.0f;
        static float THE_DEFAULT_SPOT_CUTOFF = 180.0f;
        NCollection_Array1<int> myLightTypeArray = new NCollection_Array1<int>();

        //! Bind FFP light source.
        static void bindLight(Graphic3d_CLight theLight,
                          GLenum theLightGlId,
                          OpenGl_Mat4 theModelView,
                         OpenGl_Context theCtx)
        {
            // the light is a headlight?
            if (theLight.IsHeadlight())
            {
                theCtx.core11ffp.glMatrixMode(All.Modelview);
                theCtx.core11ffp.glLoadIdentity();
            }

            // setup light type
            Graphic3d_Vec4 aLightColor = theLight.PackedColor();
            switch (theLight.Type())
            {
                case Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Ambient:
                    {
                        break; // handled by separate if-clause at beginning of method
                    }
                case Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Directional:
                    {
                        // if the last parameter of GL_POSITION, is zero, the corresponding light source is a Directional one
                        OpenGl_Vec4 anInfDir = -theLight.PackedDirectionRange();

                        // to create a realistic effect,  set the GL_SPECULAR parameter to the same value as the GL_DIFFUSE.
                        theCtx.core11ffp.glLightfv(theLightGlId, LightParameter.Ambient, THE_DEFAULT_AMBIENT);
                        theCtx.core11ffp.glLightfv(theLightGlId, LightParameter.Diffuse, aLightColor.GetData());
                        theCtx.core11ffp.glLightfv(theLightGlId, LightParameter.Specular, aLightColor.GetData());
                        theCtx.core11ffp.glLightfv(theLightGlId, LightParameter.Position, anInfDir.GetData());
                        theCtx.core11ffp.glLightfv(theLightGlId, LightParameter.SpotDirection, THE_DEFAULT_SPOT_DIR);
                        theCtx.core11ffp.glLightf(theLightGlId, LightParameter.SpotExponent, THE_DEFAULT_SPOT_EXPONENT);
                        theCtx.core11ffp.glLightf(theLightGlId, LightParameter.SpotCutoff, THE_DEFAULT_SPOT_CUTOFF);
                        break;
                    }
                case Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Positional:
                    {
                        // to create a realistic effect, set the GL_SPECULAR parameter to the same value as the GL_DIFFUSE
                        OpenGl_Vec4 aPosition = new NCollection_Vec4<float>((float)(theLight.Position().X()),
                            (float)(theLight.Position().Y()),
                            (float)(theLight.Position().Z()), 1.0f);

                        theCtx.core11ffp.glLightfv(theLightGlId, LightParameter.Ambient, THE_DEFAULT_AMBIENT);
                        theCtx.core11ffp.glLightfv(theLightGlId, LightParameter.Diffuse, aLightColor.GetData());
                        theCtx.core11ffp.glLightfv(theLightGlId, LightParameter.Specular, aLightColor.GetData());
                        theCtx.core11ffp.glLightfv(theLightGlId, LightParameter.Position, aPosition.GetData());
                        theCtx.core11ffp.glLightfv(theLightGlId, LightParameter.SpotDirection, THE_DEFAULT_SPOT_DIR);
                        theCtx.core11ffp.glLightf(theLightGlId, LightParameter.SpotExponent, THE_DEFAULT_SPOT_EXPONENT);
                        theCtx.core11ffp.glLightf(theLightGlId, LightParameter.SpotCutoff, THE_DEFAULT_SPOT_CUTOFF);
                        theCtx.core11ffp.glLightf(theLightGlId, LightParameter.ConstantAttenuation, theLight.ConstAttenuation());
                        theCtx.core11ffp.glLightf(theLightGlId, LightParameter.LinearAttenuation, theLight.LinearAttenuation());
                        theCtx.core11ffp.glLightf(theLightGlId, LightParameter.QuadraticAttenuation, 0.0f);
                        break;
                    }
                case Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Spot:
                    {
                        OpenGl_Vec4 aPosition = new NCollection_Vec4<float>((float)(theLight.Position().X()),
                            (float)(theLight.Position().Y()), (float)(theLight.Position().Z()), 1.0f);

                        theCtx.core11ffp.glLightfv(theLightGlId, LightParameter.Ambient, THE_DEFAULT_AMBIENT);
                        theCtx.core11ffp.glLightfv(theLightGlId, LightParameter.Diffuse, aLightColor.GetData());
                        theCtx.core11ffp.glLightfv(theLightGlId, LightParameter.Specular, aLightColor.GetData());
                        theCtx.core11ffp.glLightfv(theLightGlId, LightParameter.Position, aPosition.GetData());
                        theCtx.core11ffp.glLightfv(theLightGlId, LightParameter.SpotDirection, theLight.PackedDirectionRange().GetData());
                        theCtx.core11ffp.glLightf(theLightGlId, LightParameter.SpotExponent, theLight.Concentration() * 128.0f);
                        theCtx.core11ffp.glLightf(theLightGlId, LightParameter.SpotCutoff, (theLight.Angle() * 180.0f) / (float)(Math.PI));
                        theCtx.core11ffp.glLightf(theLightGlId, LightParameter.ConstantAttenuation, theLight.ConstAttenuation());
                        theCtx.core11ffp.glLightf(theLightGlId, LightParameter.LinearAttenuation, theLight.LinearAttenuation());
                        theCtx.core11ffp.glLightf(theLightGlId, LightParameter.QuadraticAttenuation, 0.0f);
                        break;
                    }
            }

            // restore matrix in case of headlight
            if (theLight.IsHeadlight())
            {
                theCtx.core11ffp.glLoadMatrixf(theModelView.GetData());
            }

            theCtx.core11fwd.glEnable((int)theLightGlId);
        }

        protected NCollection_Array1<OpenGl_ShaderLightParameters> myLightParamsArray = new NCollection_Array1<OpenGl_ShaderLightParameters>();

        void pushLightSourceState(OpenGl_ShaderProgram theProgram)
        {
            theProgram.UpdateState(OpenGl_UniformStateType.OpenGl_LIGHT_SOURCES_STATE, myLightSourceState.Index());
            if (theProgram == myFfpProgram)
            {
                if (myContext.core11ffp == null)
                {
                    return;
                }

                int aLightGlId = (int)All.Light0;
                OpenGl_Mat4 aModelView = myWorldViewState.WorldViewMatrix() * myModelWorldState.ModelWorldMatrix();
                for (Graphic3d_LightSet.Iterator aLightIt = new Graphic3d_LightSet.Iterator(myLightSourceState.LightSources(), IterationFilter.IterationFilter_ExcludeDisabledAndAmbient);
                     aLightIt.More(); aLightIt.Next())
                {
                    if (aLightGlId > (int)All.Light7) // only 8 lights in FFP...
                    {
                        //myContext.PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_PORTABILITY, 0, GL_DEBUG_SEVERITY_MEDIUM,
                        //           "Warning: light sources limit (8) has been exceeded within Fixed-function pipeline.");
                        continue;
                    }

                    bindLight(aLightIt.Value(), (uint)aLightGlId, aModelView, myContext);
                    ++aLightGlId;
                }

                // apply accumulated ambient color
                Graphic3d_Vec4 anAmbient1 = myLightSourceState.LightSources() != null
                                                ? myLightSourceState.LightSources().AmbientColor()
                                                : new Graphic3d_Vec4(0.0f, 0.0f, 0.0f, 1.0f);
                myContext.core11ffp.glLightModelfv(All.LightModelAmbient, anAmbient1.GetData());

                // GL_LIGHTING is managed by drawers to switch between shaded / no lighting output,
                // therefore managing the state here does not have any effect - do it just for consistency.
                if (aLightGlId != (int)All.Light0)
                {
                    myContext.core11fwd.glEnable(EnableCap.Lighting);
                }
                else
                {
                    myContext.core11fwd.glDisable(EnableCap.Lighting);
                }
                // switch off unused lights
                for (; aLightGlId <= (int)All.Light7; ++aLightGlId)
                {
                    myContext.core11fwd.glDisable(aLightGlId);
                }
                return;
            }

            int aNbLightsMax = theProgram.NbLightsMax();
            GLint anAmbientLoc = theProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCC_LIGHT_AMBIENT);
            if (aNbLightsMax == 0
             && anAmbientLoc == OpenGl_ShaderProgram.INVALID_LOCATION)
            {
                return;
            }

            if (myLightTypeArray.Size() < aNbLightsMax)
            {
                myLightTypeArray.Resize(0, aNbLightsMax - 1, false);
                myLightParamsArray.Resize(0, aNbLightsMax - 1, false);
            }
            for (int aLightIt = 0; aLightIt < aNbLightsMax; ++aLightIt)
            {
                myLightTypeArray.SetValue(aLightIt, -1);
            }

            if (myLightSourceState.LightSources() == null
             || myLightSourceState.LightSources().IsEmpty())
            {
                theProgram.SetUniform(myContext,
                                        theProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCC_LIGHT_SOURCE_COUNT),
                                        0);
                theProgram.SetUniform(myContext,
                                        anAmbientLoc,
                                        new OpenGl_Vec4(0.0f, 0.0f, 0.0f, 0.0f));
                /*theProgram.SetUniform(myContext,
                                        theProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCC_LIGHT_SOURCE_TYPES),
                                        aNbLightsMax,
                                        myLightTypeArray.list);*/
                return;
            }

            int aLightsNb = 0;
            for (Graphic3d_LightSet.Iterator anIter = new Graphic3d_LightSet.Iterator(myLightSourceState.LightSources(), IterationFilter.IterationFilter_ExcludeDisabledAndAmbient);
                 anIter.More(); anIter.Next())
            {
                Graphic3d_CLight aLight = anIter.Value();
                if (aLightsNb >= aNbLightsMax)
                {
                    if (aNbLightsMax != 0)
                    {
                        //myContext.PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_PORTABILITY, 0, GL_DEBUG_SEVERITY_MEDIUM,
                        //   TCollection_AsciiString("Warning: light sources limit (") + aNbLightsMax + ") has been exceeded.");
                    }
                    continue;
                }

                int aLightType = myLightTypeArray.ChangeValue(aLightsNb);
                OpenGl_ShaderLightParameters aLightParams = myLightParamsArray.ChangeValue(aLightsNb);
                if (!aLight.IsEnabled()) // has no affect with Graphic3d_LightSet::IterationFilter_ExcludeDisabled - here just for consistency
                {
                    // if it is desired to keep disabled light in the same order - we can replace it with a black light so that it will have no influence on result
                    aLightType = -1; // Graphic3d_TypeOfLightSource_Ambient can be used instead
                    aLightParams.Color = new OpenGl_Vec4(0.0f, 0.0f, 0.0f, 0.0f);
                    ++aLightsNb;
                    continue;
                }

                // ignoring OpenGl_Context::ToRenderSRGB() for light colors,
                // as non-absolute colors for lights are rare and require tuning anyway
                aLightType = (int)aLight.Type();
                aLightParams.Color = aLight.PackedColor();
                aLightParams.Color.a(aLight.Intensity()); // used by PBR and ignored by old shading model
                aLightParams.Parameters = aLight.PackedParams();
                switch (aLight.Type())
                {
                    case Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Ambient:
                        {
                            break;
                        }
                    case Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Directional:
                        {
                            if (aLight.IsHeadlight())
                            {
                                Graphic3d_Mat4 anOrientInv = myWorldViewState.WorldViewMatrixInverse();
                                aLightParams.Position = anOrientInv * new Graphic3d_Vec4(-aLight.PackedDirection(), 0.0f);
                                aLightParams.Position.SetValues(aLightParams.Position.xyz().Normalized(), 0.0f);
                            }
                            else
                            {
                                aLightParams.Position = new Graphic3d_Vec4(-aLight.PackedDirection(), 0.0f);
                            }
                            break;
                        }
                    case Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Spot:
                        {
                            if (aLight.IsHeadlight())
                            {
                                Graphic3d_Mat4 anOrientInv = myWorldViewState.WorldViewMatrixInverse();
                                aLightParams.Direction = anOrientInv * new Graphic3d_Vec4(aLight.PackedDirection(), 0.0f);
                                aLightParams.Direction.SetValues(aLightParams.Direction.xyz().Normalized(), 0.0f);
                            }
                            else
                            {
                                aLightParams.Direction = new Graphic3d_Vec4(aLight.PackedDirection(), 0.0f);
                            }
                        }
                        break;
                    case Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Positional:
                        {
                            if (aLight.IsHeadlight())
                            {
                                aLightParams.Position.x((float)(aLight.Position().X()));
                                aLightParams.Position.y((float)(aLight.Position().Y()));
                                aLightParams.Position.z((float)(aLight.Position().Z()));
                                Graphic3d_Mat4 anOrientInv = myWorldViewState.WorldViewMatrixInverse();
                                aLightParams.Position = anOrientInv * new Graphic3d_Vec4(aLightParams.Position.xyz(), 1.0f);
                            }
                            else
                            {
                                aLightParams.Position.x((float)(aLight.Position().X() - myLocalOrigin.X()));
                                aLightParams.Position.y((float)(aLight.Position().Y() - myLocalOrigin.Y()));
                                aLightParams.Position.z((float)(aLight.Position().Z() - myLocalOrigin.Z()));
                                aLightParams.Position.w(0.0f);
                            }
                            aLightParams.Direction.w(aLight.Range());
                            break;
                        }
                }
                ++aLightsNb;
            }

            Graphic3d_Vec4 anAmbient = myLightSourceState.LightSources().AmbientColor();
            theProgram.SetUniform(myContext,
                                    theProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCC_LIGHT_SOURCE_COUNT),
                                    aLightsNb);
            theProgram.SetUniform(myContext,
                                    anAmbientLoc,
                                    anAmbient);
            /*theProgram.SetUniform(myContext,
                                    theProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCC_LIGHT_SOURCE_TYPES),
                                    aNbLightsMax,
                                    myLightTypeArray.list);*/
            if (aLightsNb > 0)
            {
                /*theProgram.SetUniform(myContext,
                                        theProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCC_LIGHT_SOURCE_PARAMS),
                                        aLightsNb * OpenGl_ShaderLightParameters.NbOfVec4(),
                                        myLightParamsArray.First().Packed());*/
            }
            OpenGl_ShaderUniformLocation aLocation = theProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCCT_NB_SPEC_IBL_LEVELS);
            if (aLocation)
            {
                theProgram.SetUniform(myContext, aLocation, myLightSourceState.SpecIBLMapLevels());
            }

            OpenGl_ShaderUniformLocation aShadowMatLoc = theProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCC_LIGHT_SHADOWMAP_MATRICES);
            // update shadow map variables
            if (aShadowMatLoc)
            {
                if (myShadowMatArray.Size() < theProgram.NbShadowMaps())
                {
                    myShadowMatArray.Resize(0, theProgram.NbShadowMaps() - 1, false);
                }

                Graphic3d_Vec2 aSizeBias = new NCollection_Vec2<float>();
                if (myLightSourceState.HasShadowMaps())
                {
                    aSizeBias.SetValues(1.0f / (float)myLightSourceState.ShadowMaps().First().Texture().SizeX(),
                                         myLightSourceState.ShadowMaps().First().ShadowMapBias());
                    int aNbShadows = Math.Min(theProgram.NbShadowMaps(), myLightSourceState.ShadowMaps().Size());
                    for (int aShadowIter = 0; aShadowIter < aNbShadows; ++aShadowIter)
                    {
                        OpenGl_ShadowMap aShadow = myLightSourceState.ShadowMaps().Value(aShadowIter);
                        myShadowMatArray[aShadowIter] = aShadow.LightSourceMatrix();
                    }
                }

                theProgram.SetUniform(myContext, aShadowMatLoc, theProgram.NbShadowMaps(), myShadowMatArray.First());
                theProgram.SetUniform(myContext, theProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCC_LIGHT_SHADOWMAP_SIZE_BIAS), aSizeBias);
            }
        }

        NCollection_Array1<Graphic3d_Mat4> myShadowMatArray = new NCollection_Array1<NCollection_Mat4<float>>();

        //! Pushes current state of OCCT clipping planes to specified program (only on state change).
        void PushClippingState(OpenGl_ShaderProgram theProgram)
        {
            if (myClippingState.Index() != theProgram.ActiveState(OpenGl_UniformStateType.OpenGl_CLIP_PLANES_STATE))
            {
                pushClippingState(theProgram);
            }
        }
        NCollection_Array1<OpenGl_Vec4d> myClipPlaneArrayFfp = new NCollection_Array1<OpenGl_Vec4d>();

        void pushClippingState(OpenGl_ShaderProgram theProgram)
        {
            theProgram.UpdateState(OpenGl_UniformStateType.OpenGl_CLIP_PLANES_STATE, myClippingState.Index());
            if (theProgram == myFfpProgram)
            {
                if (myContext.core11ffp == null)
                {
                    return;
                }

                int aNbMaxPlanes = myContext.MaxClipPlanes();
                if (myClipPlaneArrayFfp.Size() < aNbMaxPlanes)
                {
                    myClipPlaneArrayFfp.Resize(0, aNbMaxPlanes - 1, false);
                }

                int aPlaneId1 = 0;
                bool toRestoreModelView = false;
                Graphic3d_ClipPlane aCappedChain1 = myContext.Clipping().CappedChain();
                for (OpenGl_ClippingIterator aPlaneIter = new OpenGl_ClippingIterator(myContext.Clipping()); aPlaneIter.More(); aPlaneIter.Next())
                {
                    Graphic3d_ClipPlane aPlane = aPlaneIter.Value();
                    if (aPlaneIter.IsDisabled()
                     || aPlane.IsChain()
                     || (aPlane == aCappedChain1
                      && myContext.Clipping().IsCappingEnableAllExcept()))
                    {
                        continue;
                    }
                    else if (aPlaneId1 >= aNbMaxPlanes)
                    {
                        Message.SendWarning("OpenGl_ShaderManager, warning: clipping planes limit (" + aNbMaxPlanes + ") has been exceeded");
                        break;
                    }

                    var anEquation = aPlane.GetEquation();
                    OpenGl_Vec4d aPlaneEq = myClipPlaneArrayFfp.ChangeValue(aPlaneId1);
                    aPlaneEq.x(anEquation.x());
                    aPlaneEq.y(anEquation.y());
                    aPlaneEq.z(anEquation.z());
                    aPlaneEq.w(anEquation.w());
                    if (myHasLocalOrigin)
                    {
                        gp_XYZ aPos = aPlane.ToPlane().Position().Location().XYZ() - myLocalOrigin;
                        double aD = -(anEquation.x() * aPos.X() + anEquation.y() * aPos.Y() + anEquation.z() * aPos.Z());
                        aPlaneEq.w(aD);
                    }

                    var anFfpPlaneID = (int)All.ClipPlane0 + aPlaneId1;
                    if (anFfpPlaneID == (int)All.ClipPlane0)
                    {
                        // set either identity or pure view matrix
                        toRestoreModelView = true;
                        myContext.core11ffp.glMatrixMode(All.Modelview);
                        myContext.core11ffp.glLoadMatrixf(myWorldViewState.WorldViewMatrix().GetData());
                    }

                    myContext.core11fwd.glEnable((int)anFfpPlaneID);
                    myContext.core11ffp.glClipPlane(anFfpPlaneID, aPlaneEq);

                    ++aPlaneId1;
                }

                // switch off unused lights
                for (; aPlaneId1 < aNbMaxPlanes; ++aPlaneId1)
                {
                    myContext.core11fwd.glDisable((int)All.ClipPlane0 + aPlaneId1);
                }

                // restore combined model-view matrix
                if (toRestoreModelView)
                {
                    OpenGl_Mat4 aModelView = myWorldViewState.WorldViewMatrix() * myModelWorldState.ModelWorldMatrix();
                    myContext.core11ffp.glLoadMatrixf(aModelView.GetData());
                }
                return;
            }

            GLint aLocEquations = theProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCC_CLIP_PLANE_EQUATIONS);
            if (aLocEquations == OpenGl_ShaderProgram.INVALID_LOCATION)
            {
                return;
            }

            int aNbClipPlanesMax = theProgram.NbClipPlanesMax();
            int aNbPlanes = Math.Min(myContext.Clipping().NbClippingOrCappingOn(), aNbClipPlanesMax);
            if (aNbPlanes < 1)
            {
                theProgram.SetUniform(myContext, theProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCC_CLIP_PLANE_COUNT), 0);
                return;
            }

            if (myClipPlaneArray.Size() < aNbClipPlanesMax)
            {
                myClipPlaneArray.Resize(0, aNbClipPlanesMax - 1, false);
                myClipChainArray.Resize(0, aNbClipPlanesMax - 1, false);
            }

            int aPlaneId = 0;
            Graphic3d_ClipPlane aCappedChain = myContext.Clipping().CappedChain();
            for (OpenGl_ClippingIterator aPlaneIter = new OpenGl_ClippingIterator(myContext.Clipping()); aPlaneIter.More(); aPlaneIter.Next())
            {
                Graphic3d_ClipPlane aPlane = aPlaneIter.Value();
                if (aPlaneIter.IsDisabled())
                {
                    continue;
                }

                if (myContext.Clipping().IsCappingDisableAllExcept())
                {
                    // enable only specific (sub) plane
                    if (aPlane != aCappedChain)
                    {
                        continue;
                    }

                    int aSubPlaneIndex = 1;
                    for (Graphic3d_ClipPlane aSubPlaneIter = aCappedChain; aSubPlaneIter != null; aSubPlaneIter = aSubPlaneIter.ChainNextPlane(), ++aSubPlaneIndex)
                    {
                        if (aSubPlaneIndex == myContext.Clipping().CappedSubPlane())
                        {
                            addClippingPlane(ref aPlaneId, aSubPlaneIter, aSubPlaneIter.GetEquation(), 1);
                            break;
                        }
                    }
                    break;
                }
                else if (aPlane == aCappedChain) // && myContext->Clipping().IsCappingEnableAllExcept()
                {
                    // enable sub-planes within processed Chain as reversed and ORed, excluding filtered plane
                    if (aPlaneId + aPlane.NbChainNextPlanes() - 1 > aNbClipPlanesMax)
                    {
                        //myContext->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_PORTABILITY, 0, GL_DEBUG_SEVERITY_HIGH,
                        //      TCollection_AsciiString("Error: clipping planes limit (") + aNbClipPlanesMax + ") has been exceeded.");
                        break;
                    }

                    int aSubPlaneIndex = 1;
                    for (Graphic3d_ClipPlane aSubPlaneIter = aPlane; aSubPlaneIter != null; aSubPlaneIter = aSubPlaneIter.ChainNextPlane(), ++aSubPlaneIndex)
                    {
                        if (aSubPlaneIndex != -myContext.Clipping().CappedSubPlane())
                        {
                            addClippingPlane(ref aPlaneId, aSubPlaneIter, aSubPlaneIter.ReversedEquation(), 1);
                        }
                    }
                }
                else
                {
                    // normal case
                    if (aPlaneId + aPlane.NbChainNextPlanes() > aNbClipPlanesMax)
                    {
                        //myContext.PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_PORTABILITY, 0, GL_DEBUG_SEVERITY_HIGH,
                        //        ("Error: clipping planes limit (") + aNbClipPlanesMax + ") has been exceeded.");
                        break;
                    }
                    for (Graphic3d_ClipPlane aSubPlaneIter = aPlane; aSubPlaneIter != null; aSubPlaneIter = aSubPlaneIter.ChainNextPlane())
                    {
                        addClippingPlane(ref aPlaneId, aSubPlaneIter, aSubPlaneIter.GetEquation(), aSubPlaneIter.NbChainNextPlanes());
                    }
                }
            }

            theProgram.SetUniform(myContext, theProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCC_CLIP_PLANE_COUNT), aPlaneId);
            theProgram.SetUniform(myContext, aLocEquations, aNbClipPlanesMax, myClipPlaneArray.list);
            //theProgram.SetUniform(myContext, theProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCC_CLIP_PLANE_CHAINS), aNbClipPlanesMax, myClipChainArray.list);
        }

        NCollection_Array1<int> myClipChainArray = new NCollection_Array1<int>();

        //! Append clipping plane definition to temporary buffers.
        void addClippingPlane(ref int thePlaneId,
                          Graphic3d_ClipPlane thePlane,
                          Graphic3d_Vec4d theEq,
                          int theChainFwd)
        {
            myClipChainArray.SetValue(thePlaneId, theChainFwd);
            OpenGl_Vec4 aPlaneEq = myClipPlaneArray.ChangeValue(thePlaneId);
            aPlaneEq.x((float)(theEq.x()));
            aPlaneEq.y((float)(theEq.y()));
            aPlaneEq.z((float)(theEq.z()));
            aPlaneEq.w((float)(theEq.w()));
            if (myHasLocalOrigin)
            {
                aPlaneEq.w((float)(LocalClippingPlaneW(thePlane)));
            }
            ++thePlaneId;
        }

        //! Return clipping plane W equation value moved considering local camera transformation.
        double LocalClippingPlaneW(Graphic3d_ClipPlane thePlane)
        {
            Graphic3d_Vec4d anEq = thePlane.GetEquation();
            if (myHasLocalOrigin)
            {
                gp_XYZ aPos = thePlane.ToPlane().Position().Location().XYZ() - myLocalOrigin;
                return -(anEq.x() * aPos.X() + anEq.y() * aPos.Y() + anEq.z() * aPos.Z());
            }
            return anEq.w();
        }

        NCollection_Array1<OpenGl_Vec4> myClipPlaneArray = new NCollection_Array1<NCollection_Vec4<float>>();

        // =======================================================================
        // function : PushState
        // purpose  : Pushes state of OCCT graphics parameters to the program
        // =======================================================================
        public void PushState(OpenGl_ShaderProgram theProgram,

                                    Graphic3d_TypeOfShadingModel theShadingModel = Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Unlit)
        {
            OpenGl_ShaderProgram aProgram = theProgram != null ? theProgram : myFfpProgram;
            PushClippingState(aProgram);
            PushLightSourceState(aProgram); // should be before PushWorldViewState()
            PushWorldViewState(aProgram);
            PushModelWorldState(aProgram);
            PushProjectionState(aProgram);/*
            PushMaterialState(aProgram);
            PushOitState(aProgram);*/

            if (theProgram != null)
            {
                OpenGl_ShaderUniformLocation aLocViewPort = theProgram.GetStateLocation(OpenGl_StateVariable.OpenGl_OCCT_VIEWPORT);
                if (aLocViewPort)
                {
                    theProgram.SetUniform(myContext, aLocViewPort, new NCollection_Vec4<float>((float)myContext.Viewport()[0], (float)myContext.Viewport()[1],
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

        internal void UpdateMaterialState()
        {
            myMaterialState.Update();
        }

        // =======================================================================
        // function : RevertClippingState
        // purpose  : Reverts state of OCCT clipping planes
        // =======================================================================
        public void RevertClippingState()
        {
            myClippingState.Revert();
        }
    }

    //! Defines generic state of OCCT clipping state.
    public class OpenGl_ClippingState
    {
        public OpenGl_ClippingState()
        {
            myIndex = (0);
            myNextIndex = 1;
        }
        int myIndex;      //!< Current state index
        int myNextIndex;  //!< Next    state index
        NCollection_List<int> myStateStack = new NCollection_List<int>(); //!< Stack of previous states

        public void Revert()
        {
            if (!myStateStack.IsEmpty())
            {
                myIndex = myStateStack.First();
                myStateStack.RemoveFirst();
            }
            else
            {
                myIndex = 0;
            }
        }


        public void Update()
        {
            myStateStack.Prepend(myIndex);
            myIndex = myNextIndex; // use myNextIndex here to handle properly Update() after Revert()
            ++myNextIndex;
        }


        //! Returns current state index.
        public int Index() { return myIndex; }

    }


    //! Packed properties of light source
    public struct OpenGl_ShaderLightParameters
    {
        public OpenGl_ShaderLightParameters() { }
        public OpenGl_Vec4 Color = new NCollection_Vec4<float>();      //!< RGB color + Intensity (in .w)
        public OpenGl_Vec4 Position = new NCollection_Vec4<float>();   //!< XYZ Direction or Position + IsHeadlight (in .w)
        public OpenGl_Vec4 Direction = new NCollection_Vec4<float>();  //!< spot light XYZ direction + Range (in .w)
        public OpenGl_Vec4 Parameters = new NCollection_Vec4<float>(); //!< same as Graphic3d_CLight::PackedParams()

        //! Returns packed (serialized) representation of light source properties
        public float[] Packed()
        {
            return Color.GetData().Concat(Position.GetData()).Concat(Direction.GetData()).Concat(Parameters.GetData()).ToArray();
            // return reinterpret_cast<const OpenGl_Vec4*> (this); 
        }
        public static int NbOfVec4() { return 4; }


    }


    //! The iterator through clipping planes.
    public class OpenGl_ClippingIterator
    {

        //! Return true if plane has been temporarily disabled either by Graphic3d_ClipPlane->IsOn() property or by temporary filter.
        //! Beware that this method does NOT handle a Chain filter for Capping algorithm OpenGl_Clipping::CappedChain()!
        public bool IsDisabled() { return myDisabled.Value(myCurrIndex) || !Value().IsOn(); }

        //! Main constructor.
        public OpenGl_ClippingIterator(OpenGl_Clipping theClipping)
        {
            myDisabled = (theClipping.myDisabledPlanes);
            myCurrIndex = (1);

            myIter1.Init(theClipping.myPlanesGlobal);
            myIter2.Init(theClipping.myPlanesLocal);
        }

        //! Return true if iterator points to the valid clipping plane.
        public bool More() { return myIter1.More() || myIter2.More(); }

        //! Go to the next clipping plane.
        public void Next()
        {
            ++myCurrIndex;
            if (myIter1.More())
            {
                myIter1.Next();
            }
            else
            {
                myIter2.Next();
            }
        }

        internal Graphic3d_ClipPlane Value()
        {
            throw new NotImplementedException();
        }

        Graphic3d_SequenceOfHClipPlane.Iterator myIter1;
        Graphic3d_SequenceOfHClipPlane.Iterator myIter2;
        NCollection_Vector<bool> myDisabled;
        int myCurrIndex;
    }
}