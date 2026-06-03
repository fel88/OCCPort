using OpenTK.Mathematics;
using System;

namespace OCCPort
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
            Vector2i aSize;
            Size(out aSize.X, out aSize.Y);
            return aSize.ToGraphic3d_Vec2i();

        }
        //! Returns True if the window <me> is opened
        //! and False if the window is closed.
        public abstract bool IsMapped() ;

    }

    public static class Vector2iExtensions
    {
        public static Graphic3d_Vec2i ToGraphic3d_Vec2i(this Vector2i v)
        {
            return new TKernel.NCollection_Vec2<int>(v.X, v.Y);
        }
    }
}
