using System;

namespace OCCPort
{
    public class NCollection_Vec3<T>
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

        public NCollection_Vec3()
        {

        }

        public T this[int key]
        {
            get => v[key ];
            set => v[key ] = value;
        }

        //! Alias to 1st component as X coordinate in XY.
        public T x() { return v[0]; }

        //! Alias to 2nd component as Y coordinate in XY.
        public T y() { return v[1]; }
        public T z() { return v[2]; }

        internal void Set(T[] temp)
        {
            for (int i = 0; i < 3; i++)
            {
                v[i] = temp[i];
            }
        }

        T[] v = new T[3];
    }
}