using System;

namespace OCCPort.OpenGL
{
	//! Vertex Buffer Object - is a general storage object for vertex attributes (position, normal, color).
	//! Notice that you should use OpenGl_IndexBuffer specialization for array of indices.	

	public class OpenGl_VertexBuffer : OpenGl_Buffer
	{
		public override uint GetTarget()
		{
			const int GL_ARRAY_BUFFER = 0x8892;
			return GL_ARRAY_BUFFER;
		}

		internal bool HasNormalAttribute()
		{
			return false;
		}
	}
}