using OCCPort.Common;

namespace TKMath
{
    //! Defines an array of values of configurable size.
    //! For instance, this class allows defining an array of 32-bit or 64-bit integer values with bitness determined in runtime.
    //! The element size in bytes (stride) should be specified at construction time.
    //! Indexation starts from 0 index.
    //! As actual type of element varies at runtime, element accessors are defined as templates.
    //! Memory for array is allocated with the given alignment (template parameter).
    public class NCollection_AliasedArray
    {  //! Empty constructor.
        public NCollection_AliasedArray(int theStride)
        {
            myData = (null);
            myStride = (theStride);
            mySize = (0);
            //myDeletable = (false);
            myDeletable = (0);

            if (theStride <= 0)
            {
                throw new ArgumentOutOfRangeException("NCollection_AliasedArray, stride should be positive");
            }
        }
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
