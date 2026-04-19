using OCCPort.Enums;
using OpenTK.Compute.OpenCL;
using System;
using System.Reflection.Metadata;

namespace OCCPort.OpenGL
{
    internal class OpenGl_ShaderManager : Graphic3d_ShaderManager

    {
        //! Overwrites context
        public void SetContext(OpenGl_Context theCtx)
        {
            myContext = theCtx;
        }
        //OpenGl_ShaderProgramFFP    myFfpProgram;

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
        OpenGl_MaterialState myMaterialState;      //!< State of Front and Back materials
        //OpenGl_OitState myOitState;           //!< State of OIT uniforms

        gp_XYZ myLocalOrigin;        //!< local camera transformation
        bool myHasLocalOrigin;     //!< flag indicating that local camera transformation has been set

        public OpenGl_ShaderManager(OpenGl_Context theContext) : base(theContext.GraphicsLibrary())

        {
            //myFfpProgram(new OpenGl_ShaderProgramFFP()),
            //myShadingModel(Graphic3d_TypeOfShadingModel_Gouraud),
            //myUnlitPrograms(new OpenGl_SetOfPrograms()),
            myContext = theContext;
            //myHasLocalOrigin(Standard_False)
            //mySRgbState = theContext->ToRenderSRGB();
        }

        //! Bind program for filled primitives rendering
        internal bool BindFaceProgram(OpenGl_TextureSet theTextures, Graphic3d_TypeOfShadingModel aShadingModel,
            object value1, object value2,
            object hasVertColor, object toEnableEnvMap, bool v, object value3)
        {
            throw new NotImplementedException();
        }

        // =======================================================================
        // function : prepareStdProgramUnlit
        // purpose  :
        // =======================================================================
        bool prepareStdProgramUnlit(OpenGl_ShaderProgram theProgram,
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
                    prepareStdProgramUnlit(aProgram, theBits, false);
                }
                return aProgram;
            }
            {
                OpenGl_ShaderProgram aProgram = myLightPrograms.ChangeValue(theShadingModel, theBits);
                if (aProgram == null)
                {
                    prepareStdProgramLight(aProgram, theShadingModel, theBits);
                }
                return aProgram;
            }
        }

        //! Prepare standard GLSL program with lighting.
        bool prepareStdProgramLight(OpenGl_ShaderProgram theProgram,
                                                 Graphic3d_TypeOfShadingModel theShadingModel,
                                                 int theBits)
        {
            switch (theShadingModel)
            {
                case Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Unlit: return prepareStdProgramUnlit(theProgram, theBits, false);
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

        //! Returns list of registered shader programs.
        public override OpenGl_ShaderProgramList ShaderPrograms() { return myProgramList; }

        //! Returns true if no program objects are registered in the manager.
        public bool IsEmpty() { return myProgramList.IsEmpty(); }


    }
}