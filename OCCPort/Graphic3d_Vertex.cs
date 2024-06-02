using System;

namespace OCCPort
{
    internal class Graphic3d_Vertex
    {


        float[] xyz = new float[3];


        //! Returns the X coordinates.
        public float X() { return xyz[0]; }

        //! Returns the Y coordinate.
        public float Y() { return xyz[1]; }

        //! Returns the Z coordinate.
        public float Z() { return xyz[2]; }


        internal void SetCoord(float x, float y, float z)
        {
            xyz[0] = x;
            xyz[1] = y;
            xyz[2] = z;
        }
        internal void SetCoord(double x, double y, double z)
        {
            xyz[0] = (float)x;
            xyz[1] = (float)y;
            xyz[2] = (float)z;
        }
    }
}