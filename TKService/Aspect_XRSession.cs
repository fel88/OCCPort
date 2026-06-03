using TKernel;

namespace TKService
{
    //! Extended Reality (XR) Session interface.
    public abstract class Aspect_XRSession
    {
        //! Return TRUE if session is opened.    {
        public abstract bool IsOpen();
        //! Return recommended viewport Width x Height for rendering into VR.
        public abstract NCollection_Vec2<int> RecommendedViewport();



    }

}
