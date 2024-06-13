namespace OCCPort
{
    public class Aspect_Window
    {
        public int Width = 800;
        public int Height = 600;
        public void Size(out int x, out int y)
        {
            x = Width;
            y = Height;
        }
    }
}
