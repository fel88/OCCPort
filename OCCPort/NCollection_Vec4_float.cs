namespace OCCPort
{
    public class NCollection_Vec4_float
    {
        public NCollection_Vec4_float()
        {
            v = new float[4];
        }
        //! Per-component constructor.
        public NCollection_Vec4_float(float theX,
                            float theY,
                            float theZ,
                            float theW)
        {
            v = new float[4];
            v[0] = theX;
            v[1] = theY;
            v[2] = theZ;
            v[3] = theW;
        }


        protected float[] v; //!< define the vector as array to avoid structure alignment issues
                              //! Alias to 1st component as X coordinate in XYZW.
        public float x() { return v[0]; }

        //! Alias to 1st component as RED channel in RGBA.
        public float r() { return v[0]; }

        //! Alias to 2nd component as Y coordinate in XYZW.
        public double y() { return v[1]; }

        //! Alias to 2nd component as GREEN channel in RGBA.
        public float g() { return v[1]; }

        //! Alias to 3rd component as Z coordinate in XYZW.
        public float z() { return v[2]; }

        //! Alias to 3rd component as BLUE channel in RGBA.
        public float b() { return v[2]; }

        //! Alias to 4th component as W coordinate in XYZW.
        public float w() { return v[3]; }

        //! Alias to 4th component as ALPHA channel in RGBA.
        public float a() { return v[3]; }


    }
}
