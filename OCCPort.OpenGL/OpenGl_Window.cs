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
            return myGlContext;
		}

		internal void Init(OpenGl_GraphicDriver theDriver, 
            Aspect_Window thePlatformWindow,
            Aspect_Window theSizeWindow, 
            Aspect_RenderingContext theGContext,
            OpenGl_Caps theCaps, 
            OpenGl_Context theShareCtx)
		{
			myGlContext = new OpenGl_Context(theCaps);
			//myOwnGContext = (theGContext == 0);
			myPlatformWindow = thePlatformWindow;
			mySizeWindow = theSizeWindow;
			//mySwapInterval = theCaps->swapInterval;


		}
	}

}