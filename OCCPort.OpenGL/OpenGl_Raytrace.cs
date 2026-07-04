using OpenTK.Graphics.ES11;
using System;

namespace OCCPort.OpenGL
{
    internal class OpenGl_Raytrace
    {
        internal static bool IsRaytracedElement(OpenGl_ElementNode theNode)
        {
            OpenGl_PrimitiveArray anArray = theNode.elem as OpenGl_PrimitiveArray;
            return anArray != null
                && anArray.DrawMode() >= GLConstants.GL_TRIANGLES;
        }

        // purpose  : Checks to see if the element contains ray-trace geometry
        internal static bool IsRaytracedElement(OpenGl_Element theElement)
        {
            OpenGl_PrimitiveArray anArray = theElement as OpenGl_PrimitiveArray;
            return anArray != null
                && anArray.DrawMode() >= (int)All.Triangles;
        }
        internal static bool OpenGl_Group(OpenGl_Element theGroup)
        {
            throw new NotImplementedException();
        }
    }
}