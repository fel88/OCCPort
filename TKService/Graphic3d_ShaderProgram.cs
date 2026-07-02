//! List of shader objects.
global using Graphic3d_ShaderAttributeList = TKernel.NCollection_Sequence<TKService.Graphic3d_ShaderAttribute>;
global using Graphic3d_ShaderObjectList = TKernel.NCollection_Sequence<TKService.Graphic3d_ShaderObject>;
//! List of custom uniform shader variables.
global using Graphic3d_ShaderVariableList = TKernel.NCollection_Sequence<TKService.Graphic3d_ShaderVariable>;

using OCCPort.Common;
using System.Linq;

namespace TKService
{
    public class Graphic3d_ShaderProgram
    {

        //! The list of currently pushed but not applied custom uniform variables.
        //! This list is automatically cleared after applying to GLSL program.
        public Graphic3d_ShaderVariableList Variables() { return myVariables; }

        //! Return the list of custom vertex attributes.
        public Graphic3d_ShaderAttributeList VertexAttributes() { return myAttributes; }


        // =======================================================================
        // function : ClearVariables
        // purpose  : Removes all custom uniform variables from the program
        // =======================================================================
        public void ClearVariables()
        {
            myVariables.Clear();
        }

        //! Pushes vec3 uniform.
        public bool PushVariableVec3(string theName, Graphic3d_Vec3 theValue) { return PushVariable(theName, theValue); }

        // =======================================================================
        // function : AttachShader
        // purpose  : Attaches shader object to the program object
        // =======================================================================
        public bool AttachShader(Graphic3d_ShaderObject theShader)
        {
            if (theShader == null)
            {
                return false;
            }

            for (Graphic3d_ShaderObjectList.Iterator anIt = new TKernel.NCollection_Sequence<Graphic3d_ShaderObject>.Iterator(myShaderObjects); anIt.More(); anIt.Next())
            {
                if (anIt.Value() == theShader)
                    return false;
            }

            myShaderObjects.Append(theShader);
            return true;
        }

        //! Default value of THE_MAX_LIGHTS macros within GLSL program (see Declarations.glsl).
        public const int THE_MAX_LIGHTS_DEFAULT = 8;

        //! Default value of THE_MAX_CLIP_PLANES macros within GLSL program (see Declarations.glsl).
        public const int THE_MAX_CLIP_PLANES_DEFAULT = 8;

        //! Default value of THE_NB_FRAG_OUTPUTS macros within GLSL program (see Declarations.glsl).
        public const int THE_NB_FRAG_OUTPUTS = 1;

        //! Returns GLSL header (version code and extensions).
        public string Header() { return myHeader; }

        //! Setup GLSL header containing language version code and used extensions.
        //! Will be prepended to the very beginning of the source code.
        //! Example:
        //! @code
        //!   #version 300 es
        //!   #extension GL_ARB_bindless_texture : require
        //! @endcode
        public void SetHeader(string theHeader) { myHeader = theHeader; }
        //! Return texture units declared within the program, @sa Graphic3d_TextureSetBits.
        public int TextureSetBits() { return myTextureSetBits; }

        //! Return TRUE if standard program header should define functions and variables used in PBR pipeline.
        //! FALSE by default.
        public bool IsPBR() { return myIsPBR; }


        //! Sets unique ID used to manage resource in graphic driver.
        //! WARNING! Graphic3d_ShaderProgram constructor generates a unique id for proper resource management;
        //! however if application overrides it, it is responsibility of application to avoid name collisions.
        public void SetId(string theId) { myID = theId; }

        string myID;
        //! Returns unique ID used to manage resource in graphic driver.
        public string GetId() { return myID; }



        //! Specify the length of array of light sources (THE_MAX_LIGHTS).
        public void SetNbLightsMax(int theNbLights)
        {
            myNbLightsMax = theNbLights;
        }

        //! Specify the length of array of shadow maps (THE_NB_SHADOWMAPS).
        public void SetNbShadowMaps(int theNbMaps)
        {
            myNbShadowMaps = theNbMaps;
        }

        int myNbLightsMax;   //!< length of array of light sources (THE_MAX_LIGHTS)
        int myNbShadowMaps;  //!< length of array of shadow maps (THE_NB_SHADOWMAPS)
        int myNbClipPlanesMax; //!< length of array of clipping planes (THE_MAX_CLIP_PLANES)
        int myNbFragOutputs; //!< length of array of Fragment Shader outputs (THE_NB_FRAG_OUTPUTS)
        int myTextureSetBits;//!< texture units declared within the program, @sa Graphic3d_TextureSetBits
        //! Set if standard program header should define default texture sampler occSampler0.
        public void SetDefaultSampler(bool theHasDefSampler)
        {
            myHasDefSampler = theHasDefSampler;
        }

        bool myHasDefSampler; //!< flag indicating that program defines default texture sampler occSampler0
        bool myHasAlphaTest;       //!< flag indicating that Fragment Shader performs alpha test
        bool myIsPBR;         //!< flag indicating that program defines functions and variables used in PBR pipeline


        //! Set if Fragment Shader should perform alpha test.
        //! Note that this flag is designed for usage with - custom shader program may discard fragment regardless this flag.
        public void SetAlphaTest(bool theAlphaTest)
        {
            myHasAlphaTest = theAlphaTest;
        }

        //! Returns list of attached shader objects.
        public Graphic3d_ShaderObjectList ShaderObjects()
        {
            return myShaderObjects;
        }

        //! Pushes int uniform.
        public bool PushVariableInt(string theName, int theValue)
        {
            return PushVariable<int>(theName, theValue);
        }



        Graphic3d_ShaderObjectList myShaderObjects = new Graphic3d_ShaderObjectList(); //!< the list of attached shader objects
        Graphic3d_ShaderVariableList myVariables = new Graphic3d_ShaderVariableList();     //!< the list of custom uniform variables
        Graphic3d_ShaderAttributeList myAttributes = new Graphic3d_ShaderAttributeList();    //!< the list of custom vertex attributes
        string myHeader;        //!< GLSL header with version code and used extensions

        bool PushVariable<T>(string theName,
                                                         T theValue)
        {
            Graphic3d_ShaderVariable aVariable = Graphic3d_ShaderVariable.Create(theName, theValue);
            if (aVariable == null || !aVariable.IsDone())
            {
                return false;
            }

            myVariables.Append(aVariable);
            return true;
        }

        public void SetTextureSetBits(int aTextureBits)
        {
            throw new NotImplementedException();
        }

        public void SetNbFragmentOutputs(int v)
        {
            throw new NotImplementedException();
        }

        public void SetPBR(bool theIsPBR)
        {
            throw new NotImplementedException();
        }

  //! Specify the length of array of clipping planes (THE_MAX_CLIP_PLANES).
        public void SetNbClipPlanesMax(int theNbPlanes)
        {
            myNbClipPlanesMax = theNbPlanes;
        }

        public void SetOitOutput(Graphic3d_RenderTransparentMethod graphic3d_RTM_DEPTH_PEELING_OIT)
        {
            throw new NotImplementedException();
        }


        //! Return the length of array of light sources (THE_MAX_LIGHTS),
        //! to be used for initialization occLightSources.
        //! Default value is THE_MAX_LIGHTS_DEFAULT.
        public int NbLightsMax() { return myNbLightsMax; }

        //! Return the length of array of shadow maps (THE_NB_SHADOWMAPS); 0 by default.
        public int NbShadowMaps() { return myNbShadowMaps; }

        //! Return the length of array of clipping planes (THE_MAX_CLIP_PLANES),
        //! to be used for initialization occClipPlaneEquations.
        //! Default value is THE_MAX_CLIP_PLANES_DEFAULT.
        public int NbClipPlanesMax() { return myNbClipPlanesMax; }


    }
    //! Enumerates transparency rendering methods supported by rasterization mode.
    public enum Graphic3d_RenderTransparentMethod
    {
        Graphic3d_RTM_BLEND_UNORDERED,  //!< Basic blend transparency with non-commuting blend operator without sorting
        Graphic3d_RTM_BLEND_OIT,        //!< Weighted Blended Order-Independent Transparency with depth weight factor
        Graphic3d_RTM_DEPTH_PEELING_OIT //!< Depth Peeling with specified number of depth layers
    };


    /*internal class Graphic3d_ShaderVariableList : List<Graphic3d_ShaderVariable>
    {
        internal void Append(Graphic3d_ShaderVariable aVariable)
        {
            Add(aVariable);
        }
    }*/
}
