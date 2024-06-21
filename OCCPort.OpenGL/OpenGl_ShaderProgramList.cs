using System.Collections.Generic;

namespace OCCPort.OpenGL
{
    public class OpenGl_ShaderProgramList : List<OpenGl_ShaderProgram>
    {
        public bool IsEmpty()
        {
            return (this.Count == 0);
        }
    }
}