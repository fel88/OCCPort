namespace OCCPort
{
    //! This class implements the real matrix abstract data type.
    //! Matrixes can have an arbitrary range which must be defined
    //! at the declaration and cannot be changed after this declaration
    //! math_Matrix(-3,5,2,4); //a vector with range [-3..5, 2..4]
    //! Matrix values may be initialized and
    //! retrieved using indexes which must lie within the range
    //! of definition of the matrix.
    //! Matrix objects follow "value semantics", that is, they
    //! cannot be shared and are copied through assignment
    //! Matrices are copied through assignment:
    //! @code
    //! math_Matrix M2(1, 9, 1, 3);
    //! ...
    //! M2 = M1;
    //! M1(1) = 2.0;//the matrix M2 will not be modified.
    //! @endcode
    //! The exception RangeError is raised when trying to access
    //! outside the range of a matrix :
    //! @code
    //! M1(11, 1)=0.0// --> will raise RangeError.
    //! @endcode
    //!
    //! The exception DimensionError is raised when the dimensions of
    //! two matrices or vectors are not compatible.
    //! @code
    //! math_Matrix M3(1, 2, 1, 2);
    //! M3 = M1;   // will raise DimensionError
    //! M1.Add(M3) // --> will raise DimensionError.
    //! @endcode
    //! A Matrix can be constructed with a pointer to "c array".
    //! It allows to carry the bounds inside the matrix.
    //! Example :
    //! @code
    //! Standard_Real tab1[10][20];
    //! Standard_Real tab2[200];
    //!
    //! math_Matrix A (tab1[0][0], 1, 10, 1, 20);
    //! math_Matrix B (tab2[0],    1, 10, 1, 20);
    //! @endcode
    public class math_Matrix
    {
        double[,] data = null;
        public double this[int key, int key2]
        {
            get => data[key, key2];
            set => data[key, key2] = value;
        }

        public math_Matrix(int LowerRow,

            int UpperRow,
            int LowerCol,
            int UpperCol,
              double InitialValue)
        {


            LowerRowIndex = LowerRow;
            UpperRowIndex = UpperRow;
            LowerColIndex = LowerCol;
            UpperColIndex = UpperCol;
            Array = new math_DoubleTab(LowerRow, UpperRow,
              LowerCol, UpperCol);


            Exceptions.Standard_RangeError_Raise_if((LowerRow > UpperRow)
                                          || (LowerCol > UpperCol), "math_Matrix() - invalid dimensions");
            Array.Init(InitialValue);
        }

        int LowerRowIndex;
        int UpperRowIndex;
        int LowerColIndex;
        int UpperColIndex;
        math_DoubleTab Array;
        public math_Matrix(double[] Tab,

              int LowerRow,

              int UpperRow,

              int LowerCol,

              int UpperCol)
        {


            LowerRowIndex = (LowerRow);
            UpperRowIndex = (UpperRow);
            LowerColIndex = (LowerCol);
            UpperColIndex = (UpperCol);
            Array = new math_DoubleTab(Tab, LowerRow, UpperRow, LowerCol, UpperCol);


            Exceptions.Standard_RangeError_Raise_if((LowerRow > UpperRow)
                                            || (LowerCol > UpperCol), "math_Matrix() - invalid dimensions");
        }

    }
}