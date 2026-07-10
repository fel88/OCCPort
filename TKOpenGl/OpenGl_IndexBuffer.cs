
using OpenTK.Graphics.OpenGL;
using System;

namespace OCCPort.OpenGL
{
    //! Index buffer is just a VBO with special target (GL_ELEMENT_ARRAY_BUFFER).
    internal class OpenGl_IndexBuffer : OpenGl_Buffer
    {
        public override BufferTarget GetTarget()
        {
            return BufferTarget.ElementArrayBuffer;
            //const uint GL_ELEMENT_ARRAY_BUFFER = 0x8893;            

        }

        
    }
}