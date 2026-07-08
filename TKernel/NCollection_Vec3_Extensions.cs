namespace TKernel
{
    public static class NCollection_Vec3_Extensions
    {
        public static NCollection_Vec3<float> ToFloat(this NCollection_Vec3<double> v)
        {
            return new NCollection_Vec3<float>((float)v.x(), (float)v.y(), (float)v.z());
        }

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
        public static void Normalize(this NCollection_Vec3<float> v)
        {
            var aModulus = Math.Sqrt(v.SquareModulus());
            if (aModulus != (0.0)) // just avoid divide by zero
            {
                v[0] = (float)(v.x() / aModulus);
                v[1] = (float)(v.y() / aModulus);
                v[2] = (float)(v.z() / aModulus);
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


        public static NCollection_Vec3<float> Normalized(this NCollection_Vec3<float> v)
        {
            NCollection_Vec3<float> ret = new NCollection_Vec3<float>();
            var aModulus = v.Modulus();
            if (aModulus != 0) // just avoid divide by zero
            {
                ret = new NCollection_Vec3<float>(v.x() / aModulus,
                    v.y() / aModulus,
                    v.z() / aModulus
                    );
            }
            return ret;
        }

    }

}