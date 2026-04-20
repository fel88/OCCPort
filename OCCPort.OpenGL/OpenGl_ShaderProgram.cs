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

        int myProgramID;     //!< Handle of OpenGL shader program

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
    }

}