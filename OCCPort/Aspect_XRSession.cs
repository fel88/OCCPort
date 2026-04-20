using OpenTK.Mathematics;
using System;

namespace OCCPort
{
    public abstract class Aspect_XRSession
    {
        //! Return TRUE if session is opened.    {
        public abstract bool IsOpen();
        //! Return recommended viewport Width x Height for rendering into VR.
        public abstract Vector2i RecommendedViewport();
        
        

    }
}