using OCCPort.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Reflection.Metadata;

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

        Vector2i mySize;        //!< window width x height in pixels

        int mySwapInterval;//!< last assigned swap interval (VSync) for this window

        public int Width() { return mySize.X; }
        public int Height() { return mySize.Y; }
        // =======================================================================
        // function : Resize
        // purpose  : call_subr_resize
        // =======================================================================
        public void Resize()
        {
            Vector2i aWinSize;
            int xx, yy;
            mySizeWindow.Size(out xx, out yy);
            aWinSize = new Vector2i(xx, yy);
            if (mySize.X == aWinSize.X && mySize.Y == aWinSize.Y)
            {
                // if the size is not changed - do nothing
                return;
            }

            mySize = aWinSize;

            init();
        }

        // =======================================================================
        // function : Activate
        // purpose  :
        // =======================================================================
        public bool Activate()
        {
            return myGlContext.MakeCurrent();
        }

        public void Init(OpenGl_GraphicDriver theDriver,
                          Aspect_Window thePlatformWindow,
                          Aspect_Window theSizeWindow,
                          Aspect_RenderingContext theGContext,
                          OpenGl_Caps theCaps,
                          OpenGl_Context theShareCtx)
        {
            myGlContext = new OpenGl_Context(theCaps);
            myOwnGContext = (theGContext == null);
            myPlatformWindow = thePlatformWindow;
            mySizeWindow = theSizeWindow;
            if (theCaps != null)
                mySwapInterval = theCaps.swapInterval;

            mySize = new Vector2i();
            mySizeWindow.Size(out mySize.X, out mySize.Y);

            bool isCoreProfile = false;
        }
        // =======================================================================
        // function : init
        // purpose  :
        // =======================================================================
        void init()
        {
            if (!Activate())
            {
                return;
            }



            //myGlContext->core11fwd->glDisable(GL_DITHER);
            //  myGlContext->core11fwd->glDisable(GL_SCISSOR_TEST);
            int[] aViewport = new[] { 0, 0, mySize.X, mySize.Y };
            /*   myGlContext->ResizeViewport(aViewport);
               myGlContext->SetDrawBuffer(GL_BACK);
               if (myGlContext->core11ffp != NULL)
               {
                   myGlContext->core11ffp->glMatrixMode(GL_MODELVIEW);
               }*/
        }

        internal OpenGl_Context GetGlContext()
        {
            return myGlContext;
        }



        //! Return platform window.
        public Aspect_Window PlatformWindow() { return myPlatformWindow; }
    }

}