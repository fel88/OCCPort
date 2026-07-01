
global using OpenGl_Vec4 = TKernel.NCollection_Vec4<float>;
using TKService;

namespace OCCPort.OpenGL
{
    internal class OpenGl_GradientParameters
    {
        public OpenGl_Vec4 color1;
        public OpenGl_Vec4 color2;
      public  Aspect_GradientFillMethod type;
    }
}