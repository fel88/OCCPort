using System;
using System.Linq;

namespace OCCPort
{
    internal class NCollection_Vec3
    {
        protected double[] v;


        public NCollection_Vec3()
        {
            v = new double[3];
        }
        //! Computes the square of vector modulus (magnitude, length).
        //! This method may be used for performance tricks.
        public double SquareModulus()
        {
            return x() * x() + y() * y() + z() * z();
        }

        public NCollection_Vec3(double value1, double value2, double value3)
        {
            v = new double[3];

            v[0] = value1;
            v[1] = value2;
            v[2] = value3;
        }

        public NCollection_Vec3(float[] myRgb)
        {
            v = myRgb.Cast<double>().ToArray();
        }

        public static NCollection_Vec3 operator -(NCollection_Vec3 temp)
        {
            return new NCollection_Vec3(-temp.x(), -temp.y(), -temp.z());
        }

        public static NCollection_Vec3 operator -(NCollection_Vec3 temp, NCollection_Vec3 temp2)
        {
            return new NCollection_Vec3(temp.x() - temp2.x(), temp.y() - temp2.y(), temp.z() - temp2.z());
        }




        internal static NCollection_Vec3 Cross(NCollection_Vec3 theVec1, NCollection_Vec3 theVec2)
        {
            return new NCollection_Vec3(theVec1.y() * theVec2.z() - theVec1.z() * theVec2.y(),
            theVec1.z() * theVec2.x() - theVec1.x() * theVec2.z(),
            theVec1.x() * theVec2.y() - theVec1.y() * theVec2.x());

        }

        public void Normalize()
        {
            double aModulus = Modulus();
            if (aModulus != (0.0)) // just avoid divide by zero
            {
                v[0] = x() / aModulus;
                v[1] = y() / aModulus;
                v[2] = z() / aModulus;
            }
        }


        //! Computes the vector modulus (magnitude, length).
        double Modulus()
        {
            return Math.Sqrt(x() * x() + y() * y() + z() * z());
        }


        internal double x()
        {
            return v[0];
        }
        internal double y()
        {
            return v[1];
        }
        internal double z()
        {
            return v[2];
        }
    }


}
