using System;
using TKernel;

namespace OCCPort
{
    public static class NCollection_Vec3_Extensions
    {
        public static void Normalize(this NCollection_Vec3<double> v)
        {
            var aModulus = Math.Sqrt(v.SquareModulus());
            if (aModulus != (0.0)) // just avoid divide by zero
            {
                v[0] = v.x() / aModulus;
                v[1] = v.y() / aModulus;
                v[2] = v.z() / aModulus;
            }

        }
    }
}
