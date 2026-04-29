using OpenTK.Graphics.OpenGL;
using System;

namespace OCCPort.OpenGL
{
	//! Vertex Buffer Object - is a general storage object for vertex attributes (position, normal, color).
	//! Notice that you should use OpenGl_IndexBuffer specialization for array of indices.	

	public abstract class OpenGl_VertexBuffer : OpenGl_Buffer
	{
		public override BufferTarget GetTarget()
		{
			return BufferTarget.ArrayBuffer;
            //const int GL_ARRAY_BUFFER = 0x8892;			
		}

        internal bool HasColorAttribute()
        {
            throw new NotImplementedException();
        }

        internal bool HasNormalAttribute()
		{
			return false;
		}
        public virtual void BindAllAttributes(OpenGl_Context theGlCtx)
        {

        }

        //! Unbind all vertex attributes. Default implementation does nothing.
        public virtual void UnbindAllAttributes(OpenGl_Context aGlContext)
        {
            
        }
    }
}