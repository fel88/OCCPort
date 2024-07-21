using OpenTK.Platform.MacOS;
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
    }
}