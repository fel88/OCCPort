using OCCPort.OpenGL;
using System;

namespace OCCPort
{
    public abstract class OpenGl_Element
    {
        public abstract void Render(OpenGl_Workspace theWorkspace);


        //! Return TRUE if primitive type generates shaded triangulation (to be used in filters).
        public virtual bool IsFillDrawMode()
        {
            return false;
        }

    }
}