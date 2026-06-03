namespace TKService
{
    public static class Vector2iExtensions
    {
        public static Graphic3d_Vec2i ToGraphic3d_Vec2i(this Vector2i v)
        {
            return new TKernel.NCollection_Vec2<int>(v.X, v.Y);
        }
    }
}
