namespace OCCPort
{
    public class NCollection_Vec2i
    {
        //! Per-component constructor.
        public NCollection_Vec2i(int theX,

                              int theY)
        {

            v[0] = theX;
            v[1] = theY;
        }

        public NCollection_Vec2i()
        {

        }
        //! Alias to 1st component as X coordinate in XY.
        public int x() { return v[0]; }

        //! Alias to 2nd component as Y coordinate in XY.
        public int y() { return v[1]; }

        int[] v = new int[2];
    }
}