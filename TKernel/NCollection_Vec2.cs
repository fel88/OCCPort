using System;
using System.Collections;
using System.Numerics;

namespace TKernel
{
    public struct NCollection_Vec2<T> where T : struct, INumber<T>, IMultiplyOperators<T, T, T>
    {
        //! Per-component constructor.
        public NCollection_Vec2(T theX,

                              T theY)
        {

            v[0] = theX;
            v[1] = theY;
        }
        //! Assign new values to the vector.
        public void SetValues(T theX,
                   T theY)
        {
            v[0] = theX;
            v[1] = theY;
        }

        public NCollection_Vec2(byte[] data, int offset, IElementBinaryExtractor<T> extractor)
        {

        }
        public NCollection_Vec2(T theXY)
        {

            v[0] = theXY;
            v[1] = theXY;
        }

        public NCollection_Vec2()
        {

        }
        public T[] GetData()
        {
            return v;
        }
        public static bool operator ==(NCollection_Vec2<T> temp, NCollection_Vec2<T> temp2)
        {
            return temp.IsEqual(temp2);
        }

        public static bool operator !=(NCollection_Vec2<T> temp, NCollection_Vec2<T> temp2)
        {
            return !temp.IsEqual(temp2);
        }

        //! Check this vector with another vector for equality (without tolerance!).
        public bool IsEqual(NCollection_Vec2<T> theOther)
        {
            return v[0] == theOther.v[0]
                && v[1] == theOther.v[1];
        }
        public static NCollection_Vec2<T> operator -(NCollection_Vec2<T> temp, NCollection_Vec2<T> temp2)
        {
            return new NCollection_Vec2<T>(temp.x() - temp2.x(), temp.y() - temp2.y());
        }
        public static NCollection_Vec2<T> operator /(NCollection_Vec2<T> vv, T theInvFactor)
        {
            return new NCollection_Vec2<T>(vv.v[0] / theInvFactor,
                    vv.v[1] / theInvFactor);
        }
        public static NCollection_Vec2<T> operator /(NCollection_Vec2<T> vv, NCollection_Vec2<T> v2)
        {
            return new NCollection_Vec2<T>(vv.v[0] / v2.v[0],
                    vv.v[1] / v2.v[1]);
        }
        public NCollection_Vec2(NCollection_Vec2<int> aDestSize)
        {
            dynamic x = aDestSize[0];
            dynamic y = aDestSize[1];
            v[0] = (T)x;
            v[1] = (T)y;
        }
        public static NCollection_Vec2<T> operator +(NCollection_Vec2<T> temp, NCollection_Vec2<T> temp2)
        {
            return new NCollection_Vec2<T>(temp.x() + temp2.x(), temp.y() + temp2.y());
        }
        public static NCollection_Vec2<T> operator *(NCollection_Vec2<T> temp, T temp2)
        {
            return new NCollection_Vec2<T>(temp.x() * temp2, temp.y() * temp2);
        }
        public NCollection_Vec2(NCollection_Vec2<double> aDestSize)
        {
            dynamic x = aDestSize[0];
            dynamic y = aDestSize[1];
            v[0] = (T)x;
            v[1] = (T)y;
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
        public void y(T newVal) { v[1] = newVal; }
        public void x(T newVal) { v[0] = newVal; }
        public T X
        {
            get => x();
            set => v[0] = value;
        }
        public T Y
        {
            get => y();
            set => v[1] = value;
        }
        public T[] v = new T[2];
    }


}