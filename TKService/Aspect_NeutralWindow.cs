using OCCPort.Common;

namespace TKService
{
    //! Defines a platform-neutral window.
    //! This class is intended to be used in context when window management (including OpenGL context creation)
    //! is performed on application side (e.g. using external framework).
    //!
    //! Window properties should be managed by application and assigned to this class as properties.
    public class Aspect_NeutralWindow : Aspect_Window
    {
        Aspect_Drawable myHandle;
        Aspect_Drawable myParentHandle;
        // Aspect_FBConfig myFBConfig;
        int myPosX;
        int myPosY;
        int myWidth;
        int myHeight;
        bool myIsMapped;
        //! Return the window size.
        public override void Size(out int theWidth,
                           out int theHeight)
        {
            theWidth = myWidth;
            theHeight = myHeight;
        }
        //! Return native handle of this drawable.
        public override Aspect_Drawable NativeHandle() { return myHandle; }

        public override bool IsMapped()
        {
            throw new NotImplementedException();
        }

        public bool SetSize(int theWidth, int theHeight)
        {
            if (myWidth == theWidth
             && myHeight == theHeight)
            {
                return false;
            }

            myWidth = theWidth;
            myHeight = theHeight;
            return true;
        }

        public override Aspect_TypeOfResize DoResize()
        {
            return Aspect_TypeOfResize.Aspect_TOR_UNKNOWN;
        }
    }

}
