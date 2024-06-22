using System.Collections.Generic;

namespace OCCPort.OpenGL
{
	internal class MyMapOfView: List<OpenGl_View>
	{
		public bool IsEmpty()
		{
			return Count == 0;
		}
	}
}