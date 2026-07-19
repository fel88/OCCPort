using OCCPort.Common;
using TKernel;

namespace TKMath
{
    //! This class implements the real IntegerVector abstract data type.
    //! IntegerVectors can have an arbitrary range which must be define at
    //! the declaration and cannot be changed after this declaration.
    //! Example:
    //! @code
    //!    math_IntegerVector V1(-3, 5); // an IntegerVector with range [-3..5]
    //! @endcode
    //!
    //! IntegerVector is copied through assignment:
    //! @code
    //!    math_IntegerVector V2( 1, 9);
    //!    ....
    //!    V2 = V1;
    //!    V1(1) = 2.0; // the IntegerVector V2 will not be modified.
    //! @endcode
    //!
    //! The Exception RangeError is raised when trying to access outside
    //! the range of an IntegerVector :
    //! @code
    //!    V1(11) = 0 // --> will raise RangeError;
    //! @endcode
    //!
    //! The Exception DimensionError is raised when the dimensions of two
    //! IntegerVectors are not compatible :
    //! @code
    //!    math_IntegerVector V3(1, 2);
    //!    V3 = V1;    // --> will raise DimensionError;
    //!    V1.Add(V3)  // --> will raise DimensionError;
    //! @endcode
    public class math_IntegerVector
    {
        //! constructs an IntegerVector in the range [Lower..Upper]
        public  math_IntegerVector( int theFirst,  int theLast)
        {
            myLocArray = new int[(theLast - theFirst + 1)];
            Array=new(myLocArray[0], theFirst, theLast);
        }
        int[] myLocArray=new int[512];


        public int this[int key]
        {
            get => Array[key];
            set => Array[key ] = value;
        }

        NCollection_Array1<int> Array = new NCollection_Array1<int>();

    }
}
