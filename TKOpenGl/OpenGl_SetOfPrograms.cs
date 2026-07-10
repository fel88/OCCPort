using System;
using TKOpenGl;
using TKService;

namespace OCCPort.OpenGL
{
    internal class OpenGl_SetOfPrograms
    {
        internal OpenGl_ShaderProgram ChangeValue(int theProgramBits)
        {
            return myPrograms[theProgramBits];
        }
        internal void ChangeValue(int theProgramBits, OpenGl_ShaderProgram sp)//not original code
        {
            myPrograms[theProgramBits] = sp;
        }

        public OpenGl_ShaderProgram[] myPrograms = new OpenGl_ShaderProgram[(int)Graphic3d_ShaderFlags.Graphic3d_ShaderFlags_NB]; //!< programs array

    }
}