using OpenTK.Mathematics;

namespace TKService
{
    public abstract class Aspect_Window
    {
        public int Width = 800;
        public int Height = 600;
        public void Size(out int x, out int y)
        {
            x = Width;
            y = Height;
        }

        //! Returns native Window handle (HWND on Windows, Window with Xlib, and so on)
        public abstract Aspect_Drawable NativeHandle();

        //! Returns window dimensions.
        public Graphic3d_Vec2i Dimensions()
        {
            Vector2i aSize = new Vector2i();
            Size(out aSize.X, out aSize.Y);
            return aSize.ToGraphic3d_Vec2i();

        }
        //! Returns True if the window <me> is opened
        //! and False if the window is closed.
        public abstract bool IsMapped();

    }
}
