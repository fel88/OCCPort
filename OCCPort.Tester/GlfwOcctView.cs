using OCCPort.Common;
using OCCPort.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Windows.Forms;
using TKernel;
using TKOpenGl;
using TKService;
using TKV3d;

namespace OCCPort.Tester
{
    //! Sample class creating 3D Viewer within GLFW window.
    public class GlfwOcctView : AIS_ViewController

    {
        public GlfwOcctView()
        {
            Instance = this;
        }

        public V3d_View myView;
        public V3d_Viewer myViewer;
        public AIS_InteractiveContext myContext;
        bool myToWaitEvents = true;
        GlfwOcctWindow myOcctWindow;
        static GlfwOcctView Instance = null;
        void onResize(int theWidth, int theHeight)
        {
            if (theWidth != 0
                && theHeight != 0
                && myView!=null)
            {                
                myView.Window().DoResize();
                myView.MustBeResized();
                myView.Invalidate();
                FlushViewEvents(myContext, myView, true);
                //renderGui();
            }
        }
        public unsafe void initWindow(int theWidth, int theHeight, nint wnd, string theTitle)
        {
            //GLFW.SetErrorCallback()
            //glfwSetErrorCallback(GlfwOcctView::errorCallback);
            //  glfwInit();
            GLFW.Init();

            bool toAskCoreProfile = true;
            if (toAskCoreProfile)
            {
                GLFW.WindowHint(WindowHintInt.ContextVersionMajor, 3);
                GLFW.WindowHint(WindowHintInt.ContextVersionMinor, 3);
                GLFW.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);

                // glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
                //  glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
                //
                //glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
            }

            // Get an unmanaged pointer to the current instance


            myOcctWindow = new GlfwOcctWindow(theWidth, theHeight, wnd, theTitle);
            //GLFW.SetWindowUserPointer(myOcctWindow.getGlfwWindow(), this);

            // window callback
            GLFW.SetWindowSizeCallback(myOcctWindow.getGlfwWindow(), GlfwOcctView.onResizeCallback);
            GLFW.SetFramebufferSizeCallback(myOcctWindow.getGlfwWindow(), GlfwOcctView.onFBResizeCallback);

            // mouse callback
            //   GLFW.SetScrollCallback(myOcctWindow.getGlfwWindow(), GlfwOcctView::onMouseScrollCallback);
            //   GLFW.SetMouseButtonCallback(myOcctWindow.getGlfwWindow(), GlfwOcctView::onMouseButtonCallback);
            GLFW.SetCursorPosCallback(myOcctWindow.getGlfwWindow(), GlfwOcctView.onMouseMoveCallback);
        }

        private static unsafe void onResizeCallback(Window* window, int theWidth, int theHeight)
        {
            Instance.onResize(theWidth, theHeight);

        }

        private static unsafe void onMouseMoveCallback(Window* window, double thePosX, double thePosY)
        {
            Instance.onMouseMove((int)thePosX, (int)thePosY);

        }

        private void onMouseMove(int thePosX, int thePosY)
        {
            if (myView == null)
            {
                return;
            }

            /*ImGuiIO & aIO = ImGui::GetIO();
            aIO.AddMousePosEvent((float)thePosX, (float)thePosY);

            if (aIO.WantCaptureMouse)*/
            //{
            myView.Redraw();
            /*    }
                else
                {*/
            NCollection_Vec2<int> aNewPos = new(thePosX, thePosY);
            //UpdateMousePosition(aNewPos, PressedMouseButtons(), LastMouseFlags(), Standard_False);
            //   }
        }

        /*static unsafe  GlfwOcctView toView(Window* theWin)
 {
     return *(GlfwOcctView*)(GLFW.GetWindowUserPointer(theWin));
 }*/

        //! Frame-buffer resize callback.
        private static unsafe void onFBResizeCallback(Window* theWin, int theWidth, int theHeight)
        {

            Instance.onResize(theWidth, theHeight);

        }

        internal unsafe void initViewer(nint glctx)
        {

            Aspect_DisplayConnection aDisplayConnection = new Aspect_DisplayConnection();
            OpenGl_GraphicDriver aGraphicDriver
                = new OpenGl_GraphicDriver(aDisplayConnection);

            aGraphicDriver.SetBuffersNoSwap(true);

            V3d_Viewer aViewer = new V3d_Viewer(aGraphicDriver);
            myViewer = aViewer;
            aViewer.SetDefaultLights();
            aViewer.SetLightOn();
            //aViewer->SetDefaultTypeOfView(V3d_PERSPECTIVE);
            aViewer.ActivateGrid(Aspect_GridType.Aspect_GT_Rectangular, Aspect_GridDrawMode.Aspect_GDM_Lines);
            myView = aViewer.CreateView();
            //myView->SetImmediateUpd/ate(Standard_False);

            myView.SetWindow(myOcctWindow, glctx);
            //myView->ChangeRenderingParams().ToShowStats = Standard_True;

            myContext = new AIS_InteractiveContext(aViewer);

            //AIS_ViewCube aCube = new AIS_ViewCube();
            //aCube.SetSize(55);
            //aCube.SetFontHeight(12);
            //aCube.SetAxesLabels("X", "Y", "Z");
            //aCube.SetTransformPersistence(new Graphic3d_TransformPers(Graphic3d_TMF_TriedronPers, Aspect_TOTP_LEFT_LOWER, Graphic3d_Vec2i(100, 100)));
            //aCube.SetViewAnimation(this.ViewAnimation());
            //aCube.SetFixedAnimationLoop(false);



            //myContext.Display(aCube, false);
        }

        internal void iterate()
        {
            // glfwPollEvents() for continuous rendering (immediate return if there are no new events)
            // and glfwWaitEvents() for rendering on demand (something actually happened in the viewer)
            if (myToWaitEvents)
            {
                //glfwWaitEvents();
            }
            else
            {
                //	glfwPollEvents();
            }
            if (myView != null)
            {
                myView.InvalidateImmediate(); // redraw view even if it wasn't modified
                FlushViewEvents(myContext, myView, true);

                //renderGui();
            }
        }
    }
}