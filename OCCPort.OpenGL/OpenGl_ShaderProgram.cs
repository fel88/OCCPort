//! List of shader objects.
global using Graphic3d_ShaderAttributeList = TKernel.NCollection_Sequence<TKService.Graphic3d_ShaderAttribute>;
global using Graphic3d_ShaderObjectList = TKernel.NCollection_Sequence<TKService.Graphic3d_ShaderObject>;
//! List of custom uniform shader variables.
global using Graphic3d_ShaderVariableList = TKernel.NCollection_Sequence<TKService.Graphic3d_ShaderVariable>;
//! List of shader variable setters.
global using OpenGl_SetterList = TKernel.NCollection_DataMap<int, OCCPort.OpenGL.OpenGl_SetterInterface>;
global using OpenGl_ShaderList = TKernel.NCollection_Sequence<OCCPort.OpenGL.OpenGl_ShaderObject>;
//! List of custom vertex shader attributes



using OCCPort.Common;
using OCCPort.Enums;
using OCCPort.OpenGL;
using OpenTK.Graphics.ES11;
using OpenTK.Mathematics;
using System;
using System;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using System.Reflection.Metadata;
using System.Threading;
using TKService;





namespace OCCPort.OpenGL
{
    public class OpenGl_ShaderProgram : OpenGl_NamedResource
    { //! Creates uninitialized shader program.
      //!
      //! WARNING! This constructor is not intended to be called anywhere but from OpenGl_ShaderManager::Create().
      //! Manager has been designed to synchronize camera position, lights definition and other aspects of the program implicitly,
      //! as well as sharing same program across rendering groups.
      //!
      //! Program created outside the manager will be left detached from these routines,
      //! and them should be performed manually by caller.
      //!
      //! This constructor has been made public to provide more flexibility to re-use OCCT OpenGL classes without OCCT Viewer itself.
      //! If this is not the case - create the program using shared OpenGl_ShaderManager instance instead.
        public OpenGl_ShaderProgram(Graphic3d_ShaderProgram theProxy = null, string theId = "") : base(theProxy != null ? theProxy.GetId() : theId)
        {

            myProgramID = (NO_PROGRAM);
            myProxy = theProxy;
            myShareCount = 1;
            myNbLightsMax = 0;
            myNbShadowMaps = 0;
            myNbClipPlanesMax = 0;
            myNbFragOutputs = 1;
            myTextureSetBits = (int)Graphic3d_TextureSetBits.Graphic3d_TextureSetBits_NONE;
            myOitOutput = Graphic3d_RenderTransparentMethod.Graphic3d_RTM_BLEND_UNORDERED;
            myHasAlphaTest = false;
            myHasTessShader = false;

            //memset(myCurrentState, 0, sizeof(myCurrentState));
        }
        public OpenGl_ShaderProgram()
        {
            for (int i = 0; i < myStateLocations.Length; i++)
            {
                myStateLocations[i] = new OpenGl_ShaderUniformLocation();
            }
        }

        //! Return texture units declared within the program, @sa Graphic3d_TextureSetBits.
        public int TextureSetBits() { return myTextureSetBits; }

        //! Specifies the value of the float uniform 4x4 matrix.
        //! Wrapper for glUniformMatrix4fv()
        public bool SetUniform(OpenGl_Context theCtx,
                                               GLint theLocation,
                                                OpenGl_Mat4 theValue,
                                               bool theTranspose = false)
        {

            if (myProgramID == NO_PROGRAM || theLocation == INVALID_LOCATION)
            {
                return false;
            }

            theCtx.core20fwd.glUniformMatrix4fv(theLocation, 1, false, theTranspose ? theValue.Transposed().GetData() : theValue.GetData());
            return true;
        }
        //! Decrements counter of users.
        //! Used by OpenGl_ShaderManager.
        //! @return true when there are no more users of this program has been left
        public bool UnShare()
        {
            return --myShareCount == 0;
        }
        //! Specifies the value of the sampler uniform variable.
        public bool SetSampler(OpenGl_Context theCtx,
                   string theName,
                     Graphic3d_TextureUnit theTextureUnit)
        {
            return SetSampler(theCtx, GetUniformLocation(theCtx, theName), theTextureUnit);
        }

        //! Returns location of the specific uniform variable.

        public OpenGl_ShaderUniformLocation GetUniformLocation(OpenGl_Context theCtx,
                                                                        string theName)
        {
            return new OpenGl_ShaderUniformLocation(myProgramID != NO_PROGRAM
                                               ? theCtx.core20fwd.glGetUniformLocation(myProgramID, theName)
                                               : INVALID_LOCATION);
        }

        public bool SetSampler(OpenGl_Context theCtx,
                                                   int theLocation,
                                                    Graphic3d_TextureUnit theTextureUnit)
        {
            if (myProgramID == NO_PROGRAM || theLocation == INVALID_LOCATION)
            {
                return false;
            }

            theCtx.core20fwd.glUniform1i(theLocation, theTextureUnit);
            return true;
        }



        int myProgramID;     //!< Handle of OpenGL shader program
        //OpenGl_ShaderList myShaderObjects; //!< List of attached shader objects
        protected Graphic3d_ShaderProgram myProxy;         //!< Proxy shader program (from application layer)
        int myShareCount;    //!< program users count, initialized with 1 (already shared by one user)
        int myNbLightsMax;   //!< length of array of light sources (THE_MAX_LIGHTS)
        int myNbShadowMaps;  //!< length of array of shadow maps (THE_NB_SHADOWMAPS)
        int myNbClipPlanesMax; //!< length of array of clipping planes (THE_MAX_CLIP_PLANES)
        int myNbFragOutputs; //!< length of array of Fragment Shader outputs (THE_NB_FRAG_OUTPUTS)
        int myTextureSetBits;//!< texture units declared within the program, @sa Graphic3d_TextureSetBits
        Graphic3d_RenderTransparentMethod myOitOutput;   //!< flag indicating that Fragment Shader includes OIT outputs
        bool myHasAlphaTest;  //!< flag indicating that Fragment Shader should perform alpha-test
        bool myHasTessShader; //!< flag indicating that program defines tessellation stage

        // Declare OCCT-specific OpenGL/GLSL shader variables
        string[] PredefinedKeywords =
        {
            "occModelWorldMatrix",                 // OpenGl_OCC_MODEL_WORLD_MATRIX
  "occWorldViewMatrix",                  // OpenGl_OCC_WORLD_VIEW_MATRIX
  "occProjectionMatrix",                 // OpenGl_OCC_PROJECTION_MATRIX
  "occModelWorldMatrixInverse",          // OpenGl_OCC_MODEL_WORLD_MATRIX_INVERSE
  "occWorldViewMatrixInverse",           // OpenGl_OCC_WORLD_VIEW_MATRIX_INVERSE
  "occProjectionMatrixInverse",          // OpenGl_OCC_PROJECTION_MATRIX_INVERSE
  "occModelWorldMatrixTranspose",        // OpenGl_OCC_MODEL_WORLD_MATRIX_TRANSPOSE
  "occWorldViewMatrixTranspose",         // OpenGl_OCC_WORLD_VIEW_MATRIX_TRANSPOSE
  "occProjectionMatrixTranspose",        // OpenGl_OCC_PROJECTION_MATRIX_TRANSPOSE
  "occModelWorldMatrixInverseTranspose", // OpenGl_OCC_MODEL_WORLD_MATRIX_INVERSE_TRANSPOSE
  "occWorldViewMatrixInverseTranspose",  // OpenGl_OCC_WORLD_VIEW_MATRIX_INVERSE_TRANSPOSE
  "occProjectionMatrixInverseTranspose", // OpenGl_OCC_PROJECTION_MATRIX_INVERSE_TRANSPOSE

  "occClipPlaneEquations",  // OpenGl_OCC_CLIP_PLANE_EQUATIONS
  "occClipPlaneChains",     // OpenGl_OCC_CLIP_PLANE_CHAINS
  "occClipPlaneCount",      // OpenGl_OCC_CLIP_PLANE_COUNT

  "occLightSourcesCount",   // OpenGl_OCC_LIGHT_SOURCE_COUNT
  "occLightSourcesTypes",   // OpenGl_OCC_LIGHT_SOURCE_TYPES
  "occLightSources",        // OpenGl_OCC_LIGHT_SOURCE_PARAMS
  "occLightAmbient",        // OpenGl_OCC_LIGHT_AMBIENT
  "occShadowMapSizeBias",   // OpenGl_OCC_LIGHT_SHADOWMAP_SIZE_BIAS
  "occShadowMapSamplers",   // OpenGl_OCC_LIGHT_SHADOWMAP_SAMPLERS,
  "occShadowMapMatrices",   // OpenGl_OCC_LIGHT_SHADOWMAP_MATRICES,

  "occTextureEnable",       // OpenGl_OCCT_TEXTURE_ENABLE
  "occDistinguishingMode",  // OpenGl_OCCT_DISTINGUISH_MODE
  "occPbrMaterial",         // OpenGl_OCCT_PBR_MATERIAL
  "occCommonMaterial",      // OpenGl_OCCT_COMMON_MATERIAL
  "occAlphaCutoff",         // OpenGl_OCCT_ALPHA_CUTOFF
  "occColor",               // OpenGl_OCCT_COLOR

  "occOitOutput",           // OpenGl_OCCT_OIT_OUTPUT
  "occOitDepthFactor",      // OpenGl_OCCT_OIT_DEPTH_FACTOR

  "occTexTrsf2d",           // OpenGl_OCCT_TEXTURE_TRSF2D
  "occPointSize",           // OpenGl_OCCT_POINT_SIZE

  "occViewport",            // OpenGl_OCCT_VIEWPORT
  "occLineWidth",           // OpenGl_OCCT_LINE_WIDTH
  "occLineFeather",         // OpenGl_OCCT_LINE_FEATHER
  "occStipplePattern",      // OpenGl_OCCT_LINE_STIPPLE_PATTERN
  "occStippleFactor",       // OpenGl_OCCT_LINE_STIPPLE_FACTOR
  "occWireframeColor",      // OpenGl_OCCT_WIREFRAME_COLOR
  "occIsQuadMode",          // OpenGl_OCCT_QUAD_MODE_STATE

  "occOrthoScale",          // OpenGl_OCCT_ORTHO_SCALE
  "occSilhouetteThickness", // OpenGl_OCCT_SILHOUETTE_THICKNESS

  "occNbSpecIBLLevels"      // OpenGl_OCCT_NB_SPEC_IBL_LEVELS
};

        //! Return TRUE if program defines tessellation stage.
        public bool HasTessellationStage() { return myHasTessShader; }

        //! Updates index of last modification of variables of specified state type.
        public void UpdateState(OpenGl_UniformStateType theType,
                    int theIndex)
        {
            if (theType < OpenGl_UniformStateType.OpenGl_UniformStateType_NB)
            {
                myCurrentState[(int)theType] = theIndex;
            }
        }

        int[] myCurrentState = new int[(int)OpenGl_UniformStateType.OpenGl_UniformStateType_NB]; //!< defines last modification for variables of each state type

        //! Returns index of last modification of variables of specified state type.
        public int ActiveState(OpenGl_UniformStateType theType)
        {
            return theType < OpenGl_UniformStateType.OpenGl_UniformStateType_NB
                 ? myCurrentState[(int)theType]
                 : 0;
        }
        //! Non-valid shader name.
        public const int NO_PROGRAM = 0;

        //! Invalid location of uniform/attribute variable.
        public const int INVALID_LOCATION = -1;
        // =======================================================================
        // function : SetAttributeName
        // purpose  :
        // =======================================================================
        bool SetAttributeName(OpenGl_Context theCtx,
                                                         int theIndex,
                                                          string theName)
        {
            theCtx.core20fwd.glBindAttribLocation(myProgramID, theIndex, theName);
            return true;
        }


        // =======================================================================
        // function : FetchInfoLog
        // purpose  : Fetches information log of the last link operation
        // =======================================================================
        public bool FetchInfoLog(OpenGl_Context theCtx,
                                 ref string theOutput)
        {
            if (myProgramID == NO_PROGRAM)
            {
                return false;
            }

            GLint aLength = 0;
            theCtx.core20fwd.glGetProgramiv(myProgramID, OpenTK.Graphics.OpenGL.GetProgramParameterName.InfoLogLength, ref aLength);
            if (aLength > 0)
            {
                //GLchar* aLog = (GLchar*)alloca(aLength);
                //memset(aLog, 0, aLength);

                theCtx.core20fwd.glGetProgramInfoLog(myProgramID, aLength, out int len, out string aLog);
                theOutput = aLog;
            }
            return true;
        }
        //! Links the program object.
        //! @param theCtx bound OpenGL context
        //! @param theIsVerbose flag to print log on error
        public bool Link(OpenGl_Context theCtx,
                                         bool theIsVerbose = true)
        {
            if (!theIsVerbose)
            {
                return link(theCtx);
            }

            if (!link(theCtx))
            {
                string aLog = "";
                FetchInfoLog(theCtx, ref aLog);
                if (aLog.IsEmpty())
                {
                    aLog = "Linker log is empty.";
                }
                theCtx.PushMessage(GLConstants.GL_DEBUG_SOURCE_APPLICATION, GLConstants.GL_DEBUG_TYPE_ERROR, 0, GLConstants.GL_DEBUG_SEVERITY_HIGH,
                         ("Failed to link program object [") + myResourceId + "]! Linker log:\n" + aLog);
                return false;
            }
               else if (theCtx.caps.glslWarnings)
            {
                // TCollection_AsciiString aLog;
                //     FetchInfoLog(theCtx, aLog);
                // if (!aLog.IsEmpty()
                //   && !aLog.IsEqual("No errors.\n"))
                {
                    //     theCtx->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_PORTABILITY, 0, GL_DEBUG_SEVERITY_LOW,
                    //                        TCollection_AsciiString("GLSL linker log [") + myResourceId + "]:\n" + aLog);
                }
            }
            return true;
        }

        bool link(OpenGl_Context theCtx)
        {
            if (myProgramID == NO_PROGRAM)
            {
                return false;
            }

            GLint aStatus = 0;
            theCtx.core20fwd.glLinkProgram(myProgramID);
            theCtx.core20fwd.glGetProgramiv(myProgramID, OpenTK.Graphics.OpenGL.GetProgramParameterName.LinkStatus, ref aStatus);
            if (aStatus == 0)
            {
                return false;
            }

            //memset(myCurrentState, 0, sizeof(myCurrentState));
            Array.Fill<int>(myCurrentState, 0);
            for (int aVar = 0; aVar < (int)OpenGl_StateVariable.OpenGl_OCCT_NUMBER_OF_STATE_VARIABLES; ++aVar)
            {
                myStateLocations[aVar] = GetUniformLocation(theCtx, PredefinedKeywords[aVar]);
            }
            return true;
        }


        //! Convert Graphic3d_TypeOfShaderObject enumeration into OpenGL enumeration.
        static GLenum shaderTypeToGl(Graphic3d_TypeOfShaderObject theType)
        {
            switch (theType)
            {
                case Graphic3d_TypeOfShaderObject.Graphic3d_TOS_VERTEX: return (int)All.VertexShader;
                case Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT: return (int)All.FragmentShader;
                case Graphic3d_TypeOfShaderObject.Graphic3d_TOS_GEOMETRY: return (int)All.GeometryShader;
                case Graphic3d_TypeOfShaderObject.Graphic3d_TOS_TESS_CONTROL: return (int)All.TessControlShader;
                case Graphic3d_TypeOfShaderObject.Graphic3d_TOS_TESS_EVALUATION: return (int)All.TessEvaluationShader;
                case Graphic3d_TypeOfShaderObject.Graphic3d_TOS_COMPUTE: return (int)All.ComputeShader;
            }
            return 0;
        }

        // =======================================================================
        // function : AttachShader
        // purpose  : Attaches shader object to the program object
        // =======================================================================
        public bool AttachShader(OpenGl_Context theCtx,
                                                      OpenGl_ShaderObject theShader)
        {
            if (myProgramID == NO_PROGRAM || theShader == null)
            {
                return false;
            }

            for (OpenGl_ShaderList.Iterator anIter = new OpenGl_ShaderList.Iterator(myShaderObjects); anIter.More(); anIter.Next())
            {
                if (theShader == anIter.Value())
                {
                    return false;
                }
            }

            myShaderObjects.Append(theShader);
            theCtx.core20fwd.glAttachShader(myProgramID, theShader.myShaderID);
            return true;
        }

        // =======================================================================
        // function : Create
        // purpose  : Creates new empty shader program of specified type
        // =======================================================================
        public bool Create(OpenGl_Context theCtx)
        {
            if (myProgramID == NO_PROGRAM
             && theCtx.core20fwd != null)
            {
                myProgramID = theCtx.core20fwd.glCreateProgram();
            }

            return myProgramID != NO_PROGRAM;
        }
        OpenGl_ShaderList myShaderObjects = new OpenGl_ShaderList(); //!< List of attached shader objects
        //! Initializes program object with the list of shader objects.
        public bool Initialize(OpenGl_Context theCtx, Graphic3d_ShaderObjectList theShaders)
        {
            myHasTessShader = false;
            if (theCtx == null || !Create(theCtx))
            {
                return false;
            }

            string aHeaderVer = myProxy != null ? myProxy.Header() : "";
            int aShaderMask = 0;
            for (Graphic3d_ShaderObjectList.Iterator anIter = new TKernel.NCollection_Sequence<Graphic3d_ShaderObject>.Iterator(theShaders); anIter.More(); anIter.Next())
            {
                aShaderMask |= (int)anIter.Value().Type();
            }
            //myHasTessShader = (aShaderMask & (Graphic3d_TOS_TESS_CONTROL | Graphic3d_TOS_TESS_EVALUATION)) != 0;
            //myNbFragOutputs = !myProxy.IsNull() ? myProxy->NbFragmentOutputs() : 1;
            myTextureSetBits = (int)Graphic3d_TextureSetBits.Graphic3d_TextureSetBits_NONE;
            //myHasAlphaTest = !myProxy.IsNull() && myProxy->HasAlphaTest();
            //myOitOutput = !myProxy.IsNull() ? myProxy->OitOutput() : Graphic3d_RTM_BLEND_UNORDERED;
            if (myOitOutput == Graphic3d_RenderTransparentMethod.Graphic3d_RTM_BLEND_OIT
             && myNbFragOutputs < 2)
            {
                myOitOutput = Graphic3d_RenderTransparentMethod.Graphic3d_RTM_BLEND_UNORDERED;
            }
            else if (myOitOutput == Graphic3d_RenderTransparentMethod.Graphic3d_RTM_DEPTH_PEELING_OIT
                  && myNbFragOutputs < 3)
            {
                myOitOutput = Graphic3d_RenderTransparentMethod.Graphic3d_RTM_BLEND_UNORDERED;
            }

            // detect the minimum GLSL version required for defined Shader Objects
            if (theCtx.GraphicsLibrary() == Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGLES)
            {
                if (myHasTessShader)
                {
                    if (!theCtx.IsGlGreaterEqual(3, 2))
                    {
                        //theCtx.PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH,
                        //            "Error! Tessellation shader requires OpenGL ES 3.2+");
                        return false;
                    }
                    else if (aHeaderVer.IsEmpty())
                    {
                        aHeaderVer = "#version 320 es";
                    }
                }
                else if ((aShaderMask & (int)Graphic3d_TypeOfShaderObject.Graphic3d_TOS_GEOMETRY) != 0)
                {
                    switch (theCtx.hasGeometryStage)
                    {
                        case OpenGl_FeatureFlag.OpenGl_FeatureNotAvailable:
                            {
                                //  theCtx->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH,
                                //                     "Error! Geometry shader requires OpenGL ES 3.2+ or GL_EXT_geometry_shader");
                                return false;
                            }
                        case OpenGl_FeatureFlag.OpenGl_FeatureInExtensions:
                            {
                                if (aHeaderVer.IsEmpty())
                                {
                                    aHeaderVer = "#version 310 es";
                                }
                                break;
                            }
                        case OpenGl_FeatureFlag.OpenGl_FeatureInCore:
                            {
                                if (aHeaderVer.IsEmpty())
                                {
                                    aHeaderVer = "#version 320 es";
                                }
                                break;
                            }
                    }
                }
                else if ((aShaderMask & (int)Graphic3d_TypeOfShaderObject.Graphic3d_TOS_COMPUTE) != 0)
                {
                    if (!theCtx.IsGlGreaterEqual(3, 1))
                    {
                        //theCtx.PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH,
                        //    "Error! Compute shaders require OpenGL ES 3.1+");
                        return false;
                    }
                    else if (aHeaderVer.IsEmpty())
                    {
                        aHeaderVer = "#version 310 es";
                    }
                }
            }
            else
            {
                if ((aShaderMask & (int)Graphic3d_TypeOfShaderObject.Graphic3d_TOS_COMPUTE) != 0)
                {
                    if (!theCtx.IsGlGreaterEqual(4, 3))
                    {
                        //theCtx->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH,
                        //          "Error! Compute shaders require OpenGL 4.3+");
                        return false;
                    }
                    else if (aHeaderVer.IsEmpty())
                    {
                        aHeaderVer = "#version 430";
                    }
                }
                else if (myHasTessShader)
                {
                    if (!theCtx.IsGlGreaterEqual(4, 0))
                    {
                        //theCtx->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH,
                        //              "Error! Tessellation shaders require OpenGL 4.0+");
                        return false;
                    }
                    else if (aHeaderVer.IsEmpty())
                    {
                        aHeaderVer = "#version 400";
                    }
                }
                else if ((aShaderMask & (int)Graphic3d_TypeOfShaderObject.Graphic3d_TOS_GEOMETRY) != 0)
                {
                    if (!theCtx.IsGlGreaterEqual(3, 2))
                    {
                        // theCtx->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH,
                        //                      "Error! Geometry shaders require OpenGL 3.2+");
                        return false;
                    }
                    else if (aHeaderVer.IsEmpty())
                    {
                        aHeaderVer = "#version 150";
                    }
                }
            }

            for (Graphic3d_ShaderObjectList.Iterator anIter = new TKernel.NCollection_Sequence<Graphic3d_ShaderObject>.Iterator(theShaders); anIter.More(); anIter.Next())
            {
                if (!anIter.Value().IsDone())
                {
                    string aMsg = "Error! Failed to get shader source";
                    // theCtx.PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH, aMsg);
                    return false;
                }

                GLenum aShaderType = shaderTypeToGl(anIter.Value().Type());
                if (aShaderType == 0)
                {
                    return false;
                }

                OpenGl_ShaderObject aShader = new OpenGl_ShaderObject(aShaderType);
                if (!aShader.Create(theCtx))
                {
                    //aShader.Release(theCtx.operator->());
                    return false;
                }

                string anExtensions = "// Enable extensions used in OCCT GLSL programs\n";
                if (myNbFragOutputs > 1)
                {
                    if (theCtx.hasDrawBuffers != OpenGl_FeatureFlag.OpenGl_FeatureNotAvailable)
                    {
                        anExtensions += "#define OCC_ENABLE_draw_buffers\n";
                        switch (myOitOutput)
                        {
                            case Graphic3d_RenderTransparentMethod.Graphic3d_RTM_BLEND_UNORDERED:
                                break;
                            case Graphic3d_RenderTransparentMethod.Graphic3d_RTM_BLEND_OIT:
                                anExtensions += "#define OCC_WRITE_WEIGHT_OIT_COVERAGE\n";
                                break;
                            case Graphic3d_RenderTransparentMethod.Graphic3d_RTM_DEPTH_PEELING_OIT:
                                anExtensions += "#define OCC_DEPTH_PEEL_OIT\n";
                                break;
                        }
                    }
                    else
                    {
                        //theCtx->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH,
                        //                   "Error! Multiple draw buffers required by the program, but aren't supported by OpenGL");
                        return false;
                    }

                    if (theCtx.hasDrawBuffers == OpenGl_FeatureFlag.OpenGl_FeatureInExtensions)
                    {
                        if (theCtx.arbDrawBuffers)
                        {
                            anExtensions += "#extension GL_ARB_draw_buffers : enable\n";
                        }
                        else if (theCtx.extDrawBuffers)
                        {
                            anExtensions += "#extension GL_EXT_draw_buffers : enable\n";
                        }
                    }
                }
                if (myHasAlphaTest)
                {
                    anExtensions += "#define OCC_ALPHA_TEST\n";
                }

                if (theCtx.hasSampleVariables == OpenGl_FeatureFlag.OpenGl_FeatureInExtensions)
                {
                    if (theCtx.GraphicsLibrary() == Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGLES
                     && theCtx.oesSampleVariables)
                    {
                        anExtensions += "#extension GL_OES_sample_variables : enable\n";
                    }
                    else if (theCtx.GraphicsLibrary() == Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGL
                          && theCtx.arbSampleShading)
                    {
                        anExtensions += "#extension GL_ARB_sample_shading : enable\n";
                    }
                }

                if (theCtx.GraphicsLibrary() == Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGLES
                 && theCtx.hasGeometryStage == OpenGl_FeatureFlag.OpenGl_FeatureInExtensions)
                {
                    anExtensions += "#extension GL_EXT_geometry_shader : enable\n" +
                                  "#extension GL_EXT_shader_io_blocks : enable\n";
                }

                string aPrecisionHeader = "";
                if (anIter.Value().Type() == Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT
                 && theCtx.GraphicsLibrary() == Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGLES)
                {
                    aPrecisionHeader = theCtx.hasHighp
                                     ? "precision highp float;\n" +
                                       "precision highp int;\n"
                                   : "precision mediump float;\n" +
                                     "precision mediump int;\n";
                }

                string aHeaderType = "";
                switch (anIter.Value().Type())
                {
                    case Graphic3d_TypeOfShaderObject.Graphic3d_TOS_COMPUTE: { aHeaderType = "#define COMPUTE_SHADER\n"; break; }
                    case Graphic3d_TypeOfShaderObject.Graphic3d_TOS_VERTEX: { aHeaderType = "#define VERTEX_SHADER\n"; break; }
                    case Graphic3d_TypeOfShaderObject.Graphic3d_TOS_TESS_CONTROL: { aHeaderType = "#define TESS_CONTROL_SHADER\n"; break; }
                    case Graphic3d_TypeOfShaderObject.Graphic3d_TOS_TESS_EVALUATION: { aHeaderType = "#define TESS_EVALUATION_SHADER\n"; break; }
                    case Graphic3d_TypeOfShaderObject.Graphic3d_TOS_GEOMETRY: { aHeaderType = "#define GEOMETRY_SHADER\n"; break; }
                    case Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT: { aHeaderType = "#define FRAGMENT_SHADER\n"; break; }
                }

                string aHeaderConstants = "";
                myNbLightsMax = myProxy != null ? myProxy.NbLightsMax() : 0;
                myNbShadowMaps = myProxy != null ? myProxy.NbShadowMaps() : 0;
                myNbClipPlanesMax = myProxy != null ? myProxy.NbClipPlanesMax() : 0;
                aHeaderConstants += ("#define THE_MAX_LIGHTS ") + myNbLightsMax + "\n";
                aHeaderConstants += ("#define THE_MAX_CLIP_PLANES ") + myNbClipPlanesMax + "\n";
                aHeaderConstants += ("#define THE_NB_FRAG_OUTPUTS ") + myNbFragOutputs + "\n";
                if (myNbShadowMaps > 0)
                {
                    aHeaderConstants += ("#define THE_NB_SHADOWMAPS ") + myNbShadowMaps + "\n";
                }
                if (theCtx.caps.useZeroToOneDepth
                 && theCtx.arbClipControl)
                {
                    aHeaderConstants += "#define THE_ZERO_TO_ONE_DEPTH\n";
                }
                if (myProxy != null
                  && myProxy.HasDefaultSampler())
                {
                    aHeaderConstants += "#define THE_HAS_DEFAULT_SAMPLER\n";
                }
                if (myProxy != null)
                {
                    if (myProxy.IsPBR())
                    {
                        aHeaderConstants += "#define THE_IS_PBR\n";
                    }
                    if ((myProxy.TextureSetBits() & (int)Graphic3d_TextureSetBits.Graphic3d_TextureSetBits_BaseColor) != 0)
                    {
                        aHeaderConstants += "#define THE_HAS_TEXTURE_COLOR\n";
                    }
                    if ((myProxy.TextureSetBits() & (int)Graphic3d_TextureSetBits.Graphic3d_TextureSetBits_Emissive) != 0)
                    {
                        aHeaderConstants += "#define THE_HAS_TEXTURE_EMISSIVE\n";
                    }
                    if ((myProxy.TextureSetBits() & (int)Graphic3d_TextureSetBits.Graphic3d_TextureSetBits_Normal) != 0)
                    {
                        aHeaderConstants += "#define THE_HAS_TEXTURE_NORMAL\n";
                    }
                    if ((myProxy.TextureSetBits() & (int)Graphic3d_TextureSetBits.Graphic3d_TextureSetBits_Occlusion) != 0)
                    {
                        aHeaderConstants += "#define THE_HAS_TEXTURE_OCCLUSION\n";
                    }
                    if ((myProxy.TextureSetBits() & (int)Graphic3d_TextureSetBits.Graphic3d_TextureSetBits_MetallicRoughness) != 0)
                    {
                        aHeaderConstants += "#define THE_HAS_TEXTURE_METALROUGHNESS\n";
                    }
                }

                string aSource = aHeaderVer                     // #version   - header defining GLSL version, should be first
                                                     + (!aHeaderVer.IsEmpty() ? "\n" : "")
                                                     + anExtensions                   // #extension - list of enabled extensions,   should be second
                                                     + aPrecisionHeader               // precision  - default precision qualifiers, should be before any code
                                                     + aHeaderType                    // auxiliary macros defining a shader stage (type)
                                                     + aHeaderConstants
                                                      + ShadersConstants.Shaders_Declarations_glsl      // common declarations (global constants and Vertex Shader inputs)
                                                      + ShadersConstants.Shaders_DeclarationsImpl_glsl
                                                     + anIter.Value().Source();      // the source code itself (defining main() function)
                ;
                if (!aShader.LoadAndCompile(theCtx, myResourceId, aSource))
                {
                    //aShader.Release(theCtx.operator->());
                    return false;
                }

                //if (theCtx.caps.glslDumpLevel)
                //{
                //    string anOutputSource = aSource;
                //    if (theCtx.caps.glslDumpLevel == OpenGl_ShaderProgramDumpLevel_Short)
                //    {
                //        anOutputSource = aHeaderVer
                //                       + (!aHeaderVer.IsEmpty() ? "\n" : "")
                //                       + anExtensions
                //                       + aPrecisionHeader
                //                       + aHeaderType
                //                       + aHeaderConstants
                //                       + anIter.Value()->Source();
                //    }
                //    aShader.DumpSourceCode(theCtx, myResourceId, anOutputSource);
                //}

                if (!AttachShader(theCtx, aShader))
                {
                    //aShader->Release(theCtx.operator->());
                    return false;
                }
            }

            // bind locations for pre-defined Vertex Attributes
            SetAttributeName(theCtx, (int)Graphic3d_TypeOfAttribute.Graphic3d_TOA_POS, "occVertex");
            SetAttributeName(theCtx, (int)Graphic3d_TypeOfAttribute.Graphic3d_TOA_NORM, "occNormal");
            SetAttributeName(theCtx, (int)Graphic3d_TypeOfAttribute.Graphic3d_TOA_UV, "occTexCoord");
            SetAttributeName(theCtx, (int)Graphic3d_TypeOfAttribute.Graphic3d_TOA_COLOR, "occVertColor");

            // bind custom Vertex Attributes
            if (myProxy != null)
            {
                for (Graphic3d_ShaderAttributeList.Iterator anAttribIter = new Graphic3d_ShaderAttributeList.Iterator(myProxy.VertexAttributes());
                     anAttribIter.More(); anAttribIter.Next())
                {
                    SetAttributeName(theCtx, anAttribIter.Value().Location(), anAttribIter.Value().Name());
                }
            }

            if (!Link(theCtx))
            {
                return false;
            }

            // set uniform defaults
            OpenGl_ShaderProgram anOldProgram = theCtx.ActiveProgram();
            theCtx.core20fwd.glUseProgram(myProgramID);
            OpenGl_ShaderUniformLocation aLocTexEnable = GetStateLocation(OpenGl_StateVariable.OpenGl_OCCT_TEXTURE_ENABLE);
            if (aLocTexEnable)
            {
                SetUniform(theCtx, aLocTexEnable, 0); // Off
            }
            OpenGl_ShaderUniformLocation aLocSampler = GetUniformLocation(theCtx, "occActiveSampler");
            if (aLocSampler)
            {
                SetUniform(theCtx, aLocSampler, (int)Graphic3d_TextureUnit.Graphic3d_TextureUnit_0);
            }
            aLocSampler = GetUniformLocation(theCtx, "occSamplerBaseColor");
            if (aLocSampler)
            {
                myTextureSetBits |= (int)Graphic3d_TextureSetBits.Graphic3d_TextureSetBits_BaseColor;
                SetUniform(theCtx, aLocSampler, (int)(Graphic3d_TextureUnit.Graphic3d_TextureUnit_BaseColor));
            }
            aLocSampler = GetUniformLocation(theCtx, "occSamplerPointSprite");
            if (aLocSampler)
            {
                // Graphic3d_TextureUnit_PointSprite
                //myTextureSetBits |= Graphic3d_TextureSetBits_PointSprite;
                SetUniform(theCtx, aLocSampler, (int)(theCtx.SpriteTextureUnit()));
            }
            //          if (OpenGl_ShaderUniformLocation aLocSampler = GetUniformLocation(theCtx, "occSamplerMetallicRoughness"))
            //{
            //              myTextureSetBits |= Graphic3d_TextureSetBits_MetallicRoughness;
            //              SetUniform(theCtx, aLocSampler, GLint(Graphic3d_TextureUnit_MetallicRoughness));
            //          }
            //          if (OpenGl_ShaderUniformLocation aLocSampler = GetUniformLocation(theCtx, "occSamplerEmissive"))
            //{
            //              myTextureSetBits |= Graphic3d_TextureSetBits_Emissive;
            //              SetUniform(theCtx, aLocSampler, GLint(Graphic3d_TextureUnit_Emissive));
            //          }
            //          if (OpenGl_ShaderUniformLocation aLocSampler = GetUniformLocation(theCtx, "occSamplerOcclusion"))
            //{
            //              myTextureSetBits |= Graphic3d_TextureSetBits_Occlusion;
            //              SetUniform(theCtx, aLocSampler, GLint(Graphic3d_TextureUnit_Occlusion));
            //          }
            //          if (OpenGl_ShaderUniformLocation aLocSampler = GetUniformLocation(theCtx, "occSamplerNormal"))
            //{
            //              myTextureSetBits |= Graphic3d_TextureSetBits_Normal;
            //              SetUniform(theCtx, aLocSampler, GLint(Graphic3d_TextureUnit_Normal));
            //          }
            //          if (OpenGl_ShaderUniformLocation aLocSampler = GetUniformLocation(theCtx, "occDiffIBLMapSHCoeffs"))
            //{
            //              SetUniform(theCtx, aLocSampler, GLint(theCtx->PBRDiffIBLMapSHTexUnit()));
            //          }
            //          if (OpenGl_ShaderUniformLocation aLocSampler = GetUniformLocation(theCtx, "occSpecIBLMap"))
            //{
            //              SetUniform(theCtx, aLocSampler, GLint(theCtx->PBRSpecIBLMapTexUnit()));
            //          }
            //          if (OpenGl_ShaderUniformLocation aLocSampler = GetUniformLocation(theCtx, "occEnvLUT"))
            //{
            //              SetUniform(theCtx, aLocSampler, GLint(theCtx->PBREnvLUTTexUnit()));
            //          }
            //          if (OpenGl_ShaderUniformLocation aLocSampler = GetUniformLocation(theCtx, "occShadowMapSamplers"))
            //{
            //              int[] aShadowSamplers = new int[myNbShadowMaps];
            //              int aSamplFrom = (int)(theCtx.ShadowMapTexUnit()) - myNbShadowMaps + 1;
            //              for (Standard_Integer aSamplerIter = 0; aSamplerIter < myNbShadowMaps; ++aSamplerIter)
            //              {
            //                  aShadowSamplers[aSamplerIter] = aSamplFrom + aSamplerIter;
            //              }
            //              SetUniform(theCtx, aLocSampler, myNbShadowMaps, &aShadowSamplers.front());
            //          }

            //          if (OpenGl_ShaderUniformLocation aLocSampler = GetUniformLocation(theCtx, "occDepthPeelingDepth"))
            //{
            //              SetUniform(theCtx, aLocSampler, (int)(theCtx.DepthPeelingDepthTexUnit()));
            //          }
            //          if (OpenGl_ShaderUniformLocation aLocSampler = GetUniformLocation(theCtx, "occDepthPeelingFrontColor"))
            //{
            //              SetUniform(theCtx, aLocSampler, GLint(theCtx->DepthPeelingFrontColorTexUnit()));
            //          }

            string aSamplerNamePrefix = ("occSampler");
            int aNbUnitsMax = Math.Max(theCtx.MaxCombinedTextureUnits(), (int)Graphic3d_TextureUnit.Graphic3d_TextureUnit_NB);
            for (int aUnitIter = 0; aUnitIter < aNbUnitsMax; ++aUnitIter)
            {
                string aName = aSamplerNamePrefix + aUnitIter;
                aLocSampler = GetUniformLocation(theCtx, aName);
                if (aLocSampler)
                {
                    SetUniform(theCtx, aLocSampler, aUnitIter);
                }
            }

            theCtx.core20fwd.glUseProgram(anOldProgram != null ? anOldProgram.ProgramId() : OpenGl_ShaderProgram.NO_PROGRAM);
            return true;
        }

        bool SetUniform(OpenGl_Context theCtx,
                                                   int theLocation,
                                                   int theValue)
        {
            if (myProgramID == NO_PROGRAM || theLocation == INVALID_LOCATION)
            {
                return false;
            }

            theCtx.core20fwd.glUniform1i(theLocation, theValue);
            return true;
        }

        //! @return true if current object was initialized
        public bool IsValid()
        {
            return myProgramID != NO_PROGRAM;
        }

        //! @return program ID
        public int ProgramId()
        {
            return myProgramID;
        }

        public override void Release(OpenGl_Context theCtx)
        {
            if (myProgramID == NO_PROGRAM)
            {
                return;
            }

            Exceptions.Standard_ASSERT_RETURN(theCtx != null,
              "OpenGl_ShaderProgram destroyed without GL context! Possible GPU memory leakage...");

            for (OpenGl_ShaderList.Iterator anIter = new OpenGl_ShaderList.Iterator(myShaderObjects); anIter.More(); anIter.Next())
            {
                if (anIter.Value() != null)
                {
                    //anIter.ChangeValue().Release(theCtx);
                    //anIter.ChangeValue().Nullify();
                }
            }

            if (theCtx.core20fwd != null
             && theCtx.IsValid())
            {
                theCtx.core20fwd.glDeleteProgram(myProgramID);
            }

            myProgramID = NO_PROGRAM;
        }


        //! Increments counter of users.
        //! Used by OpenGl_ShaderManager.
        //! @return true when resource has been restored from delayed release queue
        public bool Share()
        {
            return ++myShareCount == 1;
        }

        static OpenGl_VariableSetterSelector mySetterSelector = new OpenGl_VariableSetterSelector();

        public bool ApplyVariables(OpenGl_Context theCtx)
        {
            if (myProxy == null || myProxy.Variables().IsEmpty())
            {
                return false;
            }

            for (Graphic3d_ShaderVariableList.Iterator anIter = new Graphic3d_ShaderVariableList.Iterator(myProxy.Variables()); anIter.More(); anIter.Next())
            {
                mySetterSelector.Set(theCtx, anIter.Value(), this);
            }

            myProxy.ClearVariables();
            return true;
        }


        //! Stores locations of OCCT state uniform variables.
        OpenGl_ShaderUniformLocation[] myStateLocations = new OpenGl_ShaderUniformLocation[(int)OpenGl_StateVariable.OpenGl_OCCT_NUMBER_OF_STATE_VARIABLES];
        //! Returns location of the OCCT state uniform variable.
        public OpenGl_ShaderUniformLocation GetStateLocation(OpenGl_StateVariable theVariable)
        {
            return myStateLocations[(int)theVariable];
        }

        public bool SetUniform(OpenGl_Context theCtx, OpenGl_ShaderUniformLocation theLocation, Vector4 theValue)
        {
            if (myProgramID == NO_PROGRAM || theLocation.ToInt() == INVALID_LOCATION)
            {
                return false;
            }

            theCtx.core20fwd.glUniform4fv(theLocation.ToInt(), 1, theValue);
            return true;
        }

        public bool SetUniform(OpenGl_Context theCtx, OpenGl_ShaderUniformLocation theLocation, OpenGl_Vec4 theValue)
        {
            if (myProgramID == NO_PROGRAM || theLocation.ToInt() == INVALID_LOCATION)
            {
                return false;
            }

            theCtx.core20fwd.glUniform4fv(theLocation.ToInt(), 1, theValue.v);
            return true;
        }
    }
}