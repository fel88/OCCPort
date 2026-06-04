namespace TKernel
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

        //! Computes the vector modulus (magnitude, length).
        public static double Modulus(this NCollection_Vec3<double> v)
        {

            return Math.Sqrt(v.x() * v.x() + v.y() * v.y() + v.z() * v.z());
        }

        //! Computes the vector modulus (magnitude, length).
        public static float Modulus(this NCollection_Vec3<float> v)
        {
            return (float)Math.Sqrt(v.x() * v.x() + v.y() * v.y() + v.z() * v.z());
        }

    }

}