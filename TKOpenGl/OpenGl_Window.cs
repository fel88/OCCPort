using OCCPort.OpenGL;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using TKOpenGl;
using TKService;

namespace OCCPort
{
    public class OpenGl_Window
    {
        //! Return window object defining dimensions.
        internal Aspect_Window SizeWindow()
        {
            return mySizeWindow;
        }

        OpenGl_Context myGlContext;
        bool myOwnGContext;    //!< set to TRUE if GL context was not created by this class
        Aspect_Window myPlatformWindow; //!< software platform window wrapper
        public Aspect_Window mySizeWindow;     //!< window object defining dimensions

        Vector2i mySize;        //!< window width x height in pixels

        int mySwapInterval;//!< last assigned swap interval (VSync) for this window
        public void SetSwapInterval(bool theToForceNoSync)
        {
            int aSwapInterval = theToForceNoSync ? 0 : myGlContext.caps.swapInterval;
            if (mySwapInterval != aSwapInterval)
            {
                mySwapInterval = aSwapInterval;
                myGlContext.SetSwapInterval(mySwapInterval);
            }
        }
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

        [DllImport("opengl32.dll", SetLastError = true)]
        public static extern int wglMakeCurrent(IntPtr hdc, IntPtr hglrc);
        [DllImport("opengl32.dll", SetLastError = true)]
        public static extern IntPtr wglCreateContext(IntPtr hdc);

        [DllImport("opengl32.dll", SetLastError = true)]
        public static extern IntPtr wglGetCurrentDC();

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        // Example P/Invoke signature for wglChoosePixelFormatARB
        [DllImport("opengl32.dll", EntryPoint = "wglChoosePixelFormatARB")]
        public static extern bool wglChoosePixelFormatARB(IntPtr hdc, int[] piAttribIList, float[] pfAttribFList, uint nMaxFormats, [Out] int[] piFormats, out uint nNumFormats);

        [StructLayout(LayoutKind.Sequential)]
        public struct PIXELFORMATDESCRIPTOR
        {
            public void Init()
            {
                nSize = (ushort)Marshal.SizeOf(typeof(PIXELFORMATDESCRIPTOR));
                nVersion = 1;
                dwFlags = PFD_FLAGS.PFD_DRAW_TO_WINDOW | PFD_FLAGS.PFD_SUPPORT_OPENGL | PFD_FLAGS.PFD_DOUBLEBUFFER | PFD_FLAGS.PFD_SUPPORT_COMPOSITION;
                iPixelType = PFD_PIXEL_TYPE.PFD_TYPE_RGBA;
                cColorBits = 24;
                cRedBits = cRedShift = cGreenBits = cGreenShift = cBlueBits = cBlueShift = 0;
                cAlphaBits = cAlphaShift = 0;
                cAccumBits = cAccumRedBits = cAccumGreenBits = cAccumBlueBits = cAccumAlphaBits = 0;
                cDepthBits = 32;
                cStencilBits = cAuxBuffers = 0;
                iLayerType = PFD_LAYER_TYPES.PFD_MAIN_PLANE;
                bReserved = 0;
                dwLayerMask = dwVisibleMask = dwDamageMask = 0;

            }
            ushort nSize;
            ushort nVersion;
            PFD_FLAGS dwFlags;
            PFD_PIXEL_TYPE iPixelType;
            byte cColorBits;
            byte cRedBits;
            byte cRedShift;
            byte cGreenBits;
            byte cGreenShift;
            byte cBlueBits;
            byte cBlueShift;
            byte cAlphaBits;
            byte cAlphaShift;
            byte cAccumBits;
            byte cAccumRedBits;
            byte cAccumGreenBits;
            byte cAccumBlueBits;
            byte cAccumAlphaBits;
            byte cDepthBits;
            byte cStencilBits;
            byte cAuxBuffers;
            PFD_LAYER_TYPES iLayerType;
            byte bReserved;
            uint dwLayerMask;
            uint dwVisibleMask;
            uint dwDamageMask;
        }

        [Flags]
        public enum PFD_FLAGS : uint
        {
            PFD_DOUBLEBUFFER = 0x00000001,
            PFD_STEREO = 0x00000002,
            PFD_DRAW_TO_WINDOW = 0x00000004,
            PFD_DRAW_TO_BITMAP = 0x00000008,
            PFD_SUPPORT_GDI = 0x00000010,
            PFD_SUPPORT_OPENGL = 0x00000020,
            PFD_GENERIC_FORMAT = 0x00000040,
            PFD_NEED_PALETTE = 0x00000080,
            PFD_NEED_SYSTEM_PALETTE = 0x00000100,
            PFD_SWAP_EXCHANGE = 0x00000200,
            PFD_SWAP_COPY = 0x00000400,
            PFD_SWAP_LAYER_BUFFERS = 0x00000800,
            PFD_GENERIC_ACCELERATED = 0x00001000,
            PFD_SUPPORT_DIRECTDRAW = 0x00002000,
            PFD_DIRECT3D_ACCELERATED = 0x00004000,
            PFD_SUPPORT_COMPOSITION = 0x00008000,
            PFD_DEPTH_DONTCARE = 0x20000000,
            PFD_DOUBLEBUFFER_DONTCARE = 0x40000000,
            PFD_STEREO_DONTCARE = 0x80000000
        }

        public enum PFD_LAYER_TYPES : byte
        {
            PFD_MAIN_PLANE = 0,
            PFD_OVERLAY_PLANE = 1,
            PFD_UNDERLAY_PLANE = 255
        }

        public enum PFD_PIXEL_TYPE : byte
        {
            PFD_TYPE_RGBA = 0,
            PFD_TYPE_COLORINDEX = 1
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int ChoosePixelFormat(IntPtr hdc, [In] ref PIXELFORMATDESCRIPTOR ppfd);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern bool SetPixelFormat(IntPtr hdc, int iPixelFormat, ref PIXELFORMATDESCRIPTOR ppfd);

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
            //#elif defined(_WIN32)
            var aWindow = myPlatformWindow.NativeHandle();
            var aWindowDC = GetDC(aWindow);
            var aGContext = theGContext;

            //PIXELFORMATDESCRIPTOR aPixelFrmt;
            //memset(&aPixelFrmt, 0, sizeof(aPixelFrmt));
            //aPixelFrmt.nSize = sizeof(PIXELFORMATDESCRIPTOR);
            //aPixelFrmt.nVersion = 1;
            //aPixelFrmt.dwFlags = PFD_DRAW_TO_WINDOW | PFD_SUPPORT_OPENGL | PFD_DOUBLEBUFFER;
            //aPixelFrmt.iPixelType = PFD_TYPE_RGBA;
            //aPixelFrmt.cColorBits = 24;
            //aPixelFrmt.cDepthBits = 24;
            //aPixelFrmt.cStencilBits = 8;
            //aPixelFrmt.iLayerType = PFD_MAIN_PLANE;
            //if (theCaps->contextStereo)
            //{
            //    aPixelFrmt.dwFlags |= PFD_STEREO;
            //}


            myGlContext.Init((IntPtr)aWindow, (IntPtr)aWindowDC, (Aspect_RenderingContext)aGContext, isCoreProfile);
            myGlContext.Share(theShareCtx);
            myGlContext.SetSwapInterval(mySwapInterval);
            init();

        }
        // =======================================================================
        // function : init
        // purpose  :
        // =======================================================================
        void init()
        {
            if (!Activate())            
                return;            

            myGlContext.core11fwd.glDisable(EnableCap.Dither);
            myGlContext.core11fwd.glDisable(EnableCap.ScissorTest);
            int[] aViewport = new[] { 0, 0, mySize.X, mySize.Y };
            myGlContext.ResizeViewport(aViewport);
            myGlContext.SetDrawBuffer((int)All.Back);
            if (myGlContext.core11ffp != null)
            {
                myGlContext.core11ffp.glMatrixMode(All.Modelview);
            }
        }

        internal OpenGl_Context GetGlContext()
        {
            return myGlContext;
        }



        //! Return platform window.
        public Aspect_Window PlatformWindow() { return myPlatformWindow; }
    }

}