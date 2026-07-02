using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using TKService;

namespace OCCPort.Tester
{
    //! GLFWwindow wrapper implementing Aspect_Window interface.
    public unsafe class GlfwOcctWindow : Aspect_Window
    {

        Window* myGlfwWindow;
        int myXLeft;
        int myYTop;
        int myXRight;
        int myYBottom;
        public unsafe GlfwOcctWindow(int theWidth, int theHeight, IntPtr wnd, string theTitle)
        {
            myGlfwWindow = ((Window*)wnd);
            myXLeft = (0);
            myYTop = (0);
            myXRight = (0);
            myYBottom = 0;

            if (myGlfwWindow != null)
            {
                int aWidth = 0, aHeight = 0;


                // Assuming 'window' is your Window instance
                GLFW.GetWindowPos(myGlfwWindow, out int myXLeft, out int myYTop);
                GLFW.GetWindowSize(myGlfwWindow, out aWidth, out aHeight);

                //glfwGetWindowPos(myGlfwWindow, &myXLeft, &myYTop);
                //  glfwGetWindowSize(myGlfwWindow, &aWidth, &aHeight);
                myXRight = myXLeft + aWidth;
                myYBottom = myYTop + aHeight;

            }
        }

        public override Aspect_TypeOfResize DoResize()
        {
            if (GLFW.GetWindowAttrib(myGlfwWindow, WindowAttributeGetBool.Visible) )
            {
                int anXPos = 0, anYPos = 0, aWidth = 0, aHeight = 0;
                GLFW.GetWindowPos(myGlfwWindow, out anXPos, out anYPos);
                GLFW.GetWindowSize(myGlfwWindow, out aWidth, out aHeight);
                myXLeft = anXPos;
                myXRight = anXPos + aWidth;
                myYTop = anYPos;
                myYBottom = anYPos + aHeight;
            }
            return Aspect_TypeOfResize. Aspect_TOR_UNKNOWN;
        }

        public override bool IsMapped()
        {
            return GLFW.GetWindowAttrib(myGlfwWindow, WindowAttributeGetBool.Visible);

        }

        public override nint NativeHandle()
        {
            return GLFW.GetWin32Window(myGlfwWindow);

        }

        //! Return GLFW window.
        internal Window* getGlfwWindow()
        {

            return myGlfwWindow;

        }
    }
}