using OpenTK.Graphics.ES10;

namespace OCCPort.OpenGL
{
    //! Index buffer is just a VBO with special target (GL_ELEMENT_ARRAY_BUFFER).
    internal class OpenGl_IndexBuffer : OpenGl_Buffer
    {
        public override uint GetTarget()
        {
            const uint GL_ELEMENT_ARRAY_BUFFER = 0x8893;
            return GL_ELEMENT_ARRAY_BUFFER;

        }

    }
}