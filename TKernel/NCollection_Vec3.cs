using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TKernel
{
    public struct NCollection_Vec3<T> where T : struct, INumber<T>, IMultiplyOperators<T, T, T>
    {
        //! Per-component constructor.
        public NCollection_Vec3(T theX,
                              T theY,
                              T theZ)
        {
            v[0] = theX;
            v[1] = theY;
            v[2] = theZ;
        }
        public NCollection_Vec3(T[] vals)
        {
            v[0] = vals[0];
            v[1] = vals[1];
            v[2] = vals[2];
        }

        public NCollection_Vec3(NCollection_Vec3<T> vec)
        {
            v[0] = vec.X;
            v[1] = vec.Y;
            v[2] = vec.Z;
        }

        //! Compute maximum component of the vector.
        public T maxComp()
        {
            return v[0] > v[1] ? v[0] > v[2] ? v[0] : v[2]
                               : v[1] > v[2] ? v[1] : v[2];
        }

        public static NCollection_Vec3<T> operator -(NCollection_Vec3<T> temp)
        {
            return new NCollection_Vec3<T>(-temp.x(), -temp.y(), -temp.z());
        }

        //! Compute per-component subtraction.
        public static NCollection_Vec3<T> operator -(NCollection_Vec3<T> temp, NCollection_Vec3<T> temp2)
        {
            return new NCollection_Vec3<T>(temp.x() - temp2.x(), temp.y() - temp2.y(), temp.z() - temp2.z());
        }

        public static NCollection_Vec3<T> operator +(NCollection_Vec3<T> temp, NCollection_Vec3<T> temp2)
        {
            return new NCollection_Vec3<T>(temp.x() + temp2.x(), temp.y() + temp2.y(), temp.z() + temp2.z());
        }

        public static NCollection_Vec3<T> operator *(NCollection_Vec3<T> temp, T temp2)
        {
            return new NCollection_Vec3<T>(temp.x() * temp2, temp.y() * temp2, temp.z() * temp2);
        }

        public static NCollection_Vec3<T> operator /(NCollection_Vec3<T> temp, T temp2)
        {
            return new NCollection_Vec3<T>(temp.x() / temp2, temp.y() / temp2, temp.z() / temp2);
        }

        public T X { get => v[0]; set => v[0] = value; }
        public T Y { get => v[1]; set => v[1] = value; }
        public T Z { get => v[2]; set => v[2] = value; }

        //! Computes the vector modulus (magnitude, length).

        //}
        //! Assign new values to the vector.
        public void SetValues(T theX,
                   T theY,
                   T theZ)
        {
            v[0] = theX;
            v[1] = theY;
            v[2] = theZ;
        }

        //! Initialize ALL components of vector within specified value.
        public NCollection_Vec3(T theValue)
        {
            v[0] = v[1] = v[2] = theValue;
        }

        public NCollection_Vec3()
        {

        }


        //! Computes the square of vector modulus(magnitude, length).
        //! This method may be used for performance tricks.
        public T SquareModulus()
        {
            return x() * x() + y() * y() + z() * z();
        }


        public T this[int key]
        {
            get => v[key];
            set => v[key] = value;
        }

        //! Alias to 1st component as X coordinate in XY.
        public T x() { return v[0]; }

        //! Alias to 2nd component as Y coordinate in XY.
        public T y() { return v[1]; }
        public T z() { return v[2]; }

        //! Alias to 1st component as RED channel in RGB.
        public T r() { return v[0]; }

        //! Alias to 2nd component as GREEN channel in RGB.
        public T g() { return v[1]; }

        //! Alias to 3rd component as BLUE channel in RGB.
        public T b() { return v[2]; }

        public void Set(T[] temp)
        {
            for (int i = 0; i < 3; i++)
            {
                v[i] = temp[i];
            }
        }

        public T[] v = new T[3];
        //public class NCollection_Vec3_double
        //{
        //	protected double[] v;
        //	//! Assign new values to the vector.
        //	public void SetValues(double theX,
        //			  double theY,
        //			  double theZ)
        //	{

        //		v[0] = theX;
        //		v[1] = theY;
        //		v[2] = theZ;
        //	}


        //	//! Compute maximum component of the vector.
        //	public double maxComp()
        //	{
        //		return v[0] > v[1] ? (v[0] > v[2] ? v[0] : v[2])
        //						   : (v[1] > v[2] ? v[1] : v[2]);
        //	}

        //	public NCollection_Vec3_double()
        //	{
        //		v = new double[3];
        //	}
        //	//! Computes the square of vector modulus (magnitude, length).
        //	//! This method may be used for performance tricks.
        //	public double SquareModulus()
        //	{
        //		return x() * x() + y() * y() + z() * z();
        //	}

        //	public NCollection_Vec3_double(double value1, double value2, double value3)
        //	{
        //		v = new double[3];

        //		v[0] = value1;
        //		v[1] = value2;
        //		v[2] = value3;
        //	}

        //	public NCollection_Vec3_double(float[] myRgb)
        //	{
        //		v = myRgb.Cast<double>().ToArray();
        //	}

        //	public static NCollection_Vec3_double operator -(NCollection_Vec3_double temp)
        //	{
        //		return new NCollection_Vec3_double(-temp.x(), -temp.y(), -temp.z());
        //	}

        //	//! Compute per-component subtraction.
        //	public static NCollection_Vec3_double operator -(NCollection_Vec3_double temp, NCollection_Vec3_double temp2)
        //	{
        //		return new NCollection_Vec3_double(temp.x() - temp2.x(), temp.y() - temp2.y(), temp.z() - temp2.z());
        //	}




        public static NCollection_Vec3<T> Cross(NCollection_Vec3<T> theVec1, NCollection_Vec3<T> theVec2)
        {
            return new NCollection_Vec3<T>(theVec1.y() * theVec2.z() - theVec1.z() * theVec2.y(),
            theVec1.z() * theVec2.x() - theVec1.x() * theVec2.z(),
            theVec1.x() * theVec2.y() - theVec1.y() * theVec2.x());

        }

        //public void Normalize()
        //{
        //    T aModulus = Modulus();
        //    if (aModulus != (T.Zero)) // just avoid divide by zero
        //    {
        //        v[0] = x() / aModulus;
        //        v[1] = y() / aModulus;
        //        v[2] = z() / aModulus;
        //    }
        //}


        ////! Computes the vector modulus (magnitude, length).
        //T Modulus()
        //{            
        //    return T.Sqrt(x() * x() + y() * y() + z() * z());
        //}


        //	internal double x()
        //	{
        //		return v[0];
        //	}
        //	internal double y()
        //	{
        //		return v[1];
        //	}
        //	internal double z()
        //	{
        //		return v[2];
        //	}
        //}
    }
}