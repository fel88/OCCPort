using System;

namespace OCCPort.OpenGL
{
	internal class OpenGl_Raytrace
	{
		internal static bool IsRaytracedElement(OpenGl_ElementNode theNode)
		{
			OpenGl_PrimitiveArray anArray = (OpenGl_PrimitiveArray)(theNode.elem);
			return anArray != null
				&& anArray.DrawMode() >= GLConstants.GL_TRIANGLES;
		}
		internal static bool IsRaytracedElement(OpenGl_Element aNode)
		{
			throw new NotImplementedException();
		}
		internal static bool OpenGl_Group(OpenGl_Element theGroup)
		{
			throw new NotImplementedException();
		}
	}
}