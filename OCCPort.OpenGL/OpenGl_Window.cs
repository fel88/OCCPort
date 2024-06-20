using OCCPort.OpenGL;
using System;

namespace OCCPort
{
    public class OpenGl_Window
    {
        internal Aspect_Window SizeWindow()
        {
            throw new NotImplementedException();
        }

        OpenGl_Context myGlContext;
        bool myOwnGContext;    //!< set to TRUE if GL context was not created by this class
        Aspect_Window myPlatformWindow; //!< software platform window wrapper
        Aspect_Window mySizeWindow;     //!< window object defining dimensions

        Graphic3d_Vec2i mySize;        //!< window width x height in pixels

        int mySwapInterval;//!< last assigned swap interval (VSync) for this window

        public int Width() { return mySize.x(); }
        public int Height() { return mySize.y(); }

		internal OpenGl_Context GetGlContext()
		{
			throw new NotImplementedException();
		}
    }

}