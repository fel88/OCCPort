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

        public NCollection_Vec2i(int theXY)
        {
            v[0] = v[1] = theXY;
        }
        //! Compute per-component division by scale factor.
        public static NCollection_Vec2i operator /(NCollection_Vec2i vv, int theInvFactor)
        {
            return new NCollection_Vec2i(vv.v[0] / theInvFactor,
                    vv.v[1] / theInvFactor);
        }
        public NCollection_Vec2i()
        {

        }
        //! Alias to 1st component as X coordinate in XY.
        public int x() { return v[0]; }

        //! Alias to 2nd component as Y coordinate in XY.
        public int y() { return v[1]; }

        public int[] v = new int[2];
    }
}