using OCCPort.Enums;
using OpenTK.Mathematics;
using System;

namespace OCCPort.OpenGL
{
    public class OpenGl_ShaderProgram : OpenGl_NamedResource
    {
        public OpenGl_ShaderProgram()
        {
        }

        public OpenGl_ShaderProgram(Graphic3d_ShaderProgram theProxy)
        {
        }

        int myProgramID;     //!< Handle of OpenGL shader program
                             // OpenGl_ShaderList myShaderObjects; //!< List of attached shader objects
        Graphic3d_ShaderProgram myProxy;         //!< Proxy shader program (from application layer)
        int myShareCount;    //!< program users count, initialized with 1 (already shared by one user)
        int myNbLightsMax;   //!< length of array of light sources (THE_MAX_LIGHTS)
        int myNbShadowMaps;  //!< length of array of shadow maps (THE_NB_SHADOWMAPS)
        int myNbClipPlanesMax; //!< length of array of clipping planes (THE_MAX_CLIP_PLANES)
        int myNbFragOutputs; //!< length of array of Fragment Shader outputs (THE_NB_FRAG_OUTPUTS)
        int myTextureSetBits;//!< texture units declared within the program, @sa Graphic3d_TextureSetBits
        Graphic3d_RenderTransparentMethod myOitOutput;   //!< flag indicating that Fragment Shader includes OIT outputs
        bool myHasAlphaTest;  //!< flag indicating that Fragment Shader should perform alpha-test
        bool myHasTessShader; //!< flag indicating that program defines tessellation stage


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
        internal bool Initialize(object myContext, object v)
        {
            throw new NotImplementedException();
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

        

        internal void Release(OpenGl_Context myContext)
        {
            throw new NotImplementedException();
        }

        internal bool Share()
        {
            throw new NotImplementedException();
        }

        internal void ApplyVariables(OpenGl_Context myContext)
        {
            throw new NotImplementedException();
        }


        //! Stores locations of OCCT state uniform variables.
        OpenGl_ShaderUniformLocation[] myStateLocations = new OpenGl_ShaderUniformLocation[(int)OpenGl_StateVariable.OpenGl_OCCT_NUMBER_OF_STATE_VARIABLES];
        //! Returns location of the OCCT state uniform variable.
        public OpenGl_ShaderUniformLocation GetStateLocation(OpenGl_StateVariable theVariable)
        {
            return myStateLocations[(int)theVariable];
        }

        internal bool SetUniform(OpenGl_Context theCtx, OpenGl_ShaderUniformLocation theLocation, Vector4 theValue)
        {
            if (myProgramID == NO_PROGRAM || theLocation.ToInt() == INVALID_LOCATION)
            {
                return false;
            }

            theCtx.core20fwd.glUniform4fv(theLocation.ToInt(), 1, theValue);
            return true;
        }
    }

}