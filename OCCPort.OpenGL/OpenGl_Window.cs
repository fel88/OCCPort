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
        // =======================================================================
        // function : Resize
        // purpose  : call_subr_resize
        // =======================================================================
        void Resize()
        {
            Graphic3d_Vec2i aWinSize;
            int xx, yy;
            mySizeWindow.Size(out xx, out yy);
            aWinSize = new Graphic3d_Vec2i(xx, yy);
            if (mySize.x() == aWinSize.x() && mySize.y() == aWinSize.y())
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
        bool Activate()
        {
            return myGlContext.MakeCurrent();
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
            int[] aViewport = new[] { 0, 0, mySize.x(), mySize.y() };
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