using OCCPort.Enums;
using System;
using System.Diagnostics.Contracts;

namespace OCCPort
{
    public class Graphic3d_ShaderProgram
    {

        //! Default value of THE_MAX_LIGHTS macros within GLSL program (see Declarations.glsl).
        public const int THE_MAX_LIGHTS_DEFAULT = 8;

        //! Default value of THE_MAX_CLIP_PLANES macros within GLSL program (see Declarations.glsl).
        public const int THE_MAX_CLIP_PLANES_DEFAULT = 8;

        //! Default value of THE_NB_FRAG_OUTPUTS macros within GLSL program (see Declarations.glsl).
        public const int THE_NB_FRAG_OUTPUTS = 1;

        //! Setup GLSL header containing language version code and used extensions.
        //! Will be prepended to the very beginning of the source code.
        //! Example:
        //! @code
        //!   #version 300 es
        //!   #extension GL_ARB_bindless_texture : require
        //! @endcode
      public  void SetHeader( string theHeader) { myHeader = theHeader; }


        //! Sets unique ID used to manage resource in graphic driver.
        //! WARNING! Graphic3d_ShaderProgram constructor generates a unique id for proper resource management;
        //! however if application overrides it, it is responsibility of application to avoid name collisions.
      public void SetId( string theId) { myID = theId; }

        string myID;
        //! Returns unique ID used to manage resource in graphic driver.
        public string GetId() { return myID; }
        //! Attaches shader object to the program object.
        public bool AttachShader(Graphic3d_ShaderObject theShader)
        {
            if (theShader == null)
            {
                return false;
            }

            //for (Graphic3d_ShaderObjectList::Iterator anIt (myShaderObjects); anIt.More(); anIt.Next())
            foreach (var anIt in myShaderObjects)
            {
                if (anIt == theShader)
                    return false;
            }

            myShaderObjects.Append(theShader);
            return true;
        }



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
        public object ShaderObjects()
        {
            return myShaderObjects;
        }

        //! Pushes int uniform.
        public bool PushVariableInt(string theName, int theValue)
        {
            return PushVariable<int>(theName, theValue);
        }



        Graphic3d_ShaderObjectList myShaderObjects = new Graphic3d_ShaderObjectList(); //!< the list of attached shader objects
        Graphic3d_ShaderVariableList myVariables;     //!< the list of custom uniform variables
        Graphic3d_ShaderAttributeList myAttributes;    //!< the list of custom vertex attributes
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

        public void SetNbClipPlanesMax(int aNbClipPlanes)
        {
            throw new NotImplementedException();
        }

        public void SetOitOutput(Graphic3d_RenderTransparentMethod graphic3d_RTM_DEPTH_PEELING_OIT)
        {
            throw new NotImplementedException();
        }
    }
}