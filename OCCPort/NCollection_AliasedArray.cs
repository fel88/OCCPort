using OpenTK.Graphics.ES11;
using System;
using TKMath;

namespace OCCPort
{
    public class NCollection_AliasedArray
    {
        protected byte[] myData;      //!< data pointer
        protected int myStride;    //!< element size
        protected int mySize;      //!< number of elements
        protected byte myDeletable; //!< flag showing who allocated the array

        internal T Value<T>(int theIndex)
        {
            if (typeof(T) == typeof(gp_Pnt))
            {
                var offset = 3 * sizeof(double);
                var ret = new gp_Pnt(BitConverter.ToDouble(myData, offset),
                    BitConverter.ToDouble(myData, offset + sizeof(double)),
                    BitConverter.ToDouble(myData, offset + sizeof(double) * 2)
                    );
                dynamic d = ret;
                return (T)d;
            }
            else
            if (typeof(T) == typeof(gp_Vec3f))
            {
                var offset = 3 * sizeof(float);
                var ret = new gp_Vec3f(BitConverter.ToSingle(myData, offset),
                    BitConverter.ToSingle(myData, offset + sizeof(float)),
                    BitConverter.ToSingle(myData, offset + sizeof(float) * 2)
                    );
                dynamic d = ret;
                return (T)d;
            }
            throw new NotImplementedException();
        }
    }
}