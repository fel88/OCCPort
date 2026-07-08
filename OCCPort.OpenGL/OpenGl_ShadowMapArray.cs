using OCCPort.Common;
using System.Reflection.Metadata;
using TKernel;

namespace OCCPort.OpenGL
{
    public class OpenGl_ShadowMapArray : NCollection_Array1<OpenGl_ShadowMap>
    {
        //! Return TRUE if defined.
        public bool IsValid()
        {
            return !IsEmpty()
                 && First().IsValid();
        }
    }
}