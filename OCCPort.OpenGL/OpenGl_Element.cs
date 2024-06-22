using OCCPort.OpenGL;
using System;

namespace OCCPort
{
	public abstract class OpenGl_Element
    {
		public abstract void Render(OpenGl_Workspace theWorkspace);

		internal bool IsFillDrawMode()
		{
			throw new NotImplementedException();
		}
	}
}