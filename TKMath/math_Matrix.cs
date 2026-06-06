using OCCPort.Common;

namespace TKMath
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

        //! Returns the column of index <Col> of a matrix.
        public math_Vector Col(int Col)
        {
            math_Vector Result = new math_Vector(LowerRowIndex, UpperRowIndex);

            for (int Index = LowerRowIndex; Index <= UpperRowIndex; Index++)
            {
                Result.Array[Index] = Array[Index, Col];
            }
            return Result;
        }

        public double this[int key, int key2]
        {
            get => Array[key, key2];
            set => Array[key, key2] = value;
        }

        public int RowNumber()
        { return UpperRowIndex - LowerRowIndex + 1; }

        public int ColNumber()
        { return UpperColIndex - LowerColIndex + 1; }

        //! Constructs a non-initialized  matrix of range [LowerRow..UpperRow,
        //! LowerCol..UpperCol]
        //! For the constructed matrix:
        //! -   LowerRow and UpperRow are the indexes of the
        //! lower and upper bounds of a row, and
        //! -   LowerCol and UpperCol are the indexes of the
        //! lower and upper bounds of a column.
        public math_Matrix(int LowerRow, int UpperRow, int LowerCol, int UpperCol)
        {
            LowerRowIndex = (LowerRow);
            UpperRowIndex = (UpperRow);
            LowerColIndex = (LowerCol);
            UpperColIndex = (UpperCol);
            Array = new math_DoubleTab(LowerRow, UpperRow,
              LowerCol, UpperCol);

            Exceptions.Standard_RangeError_Raise_if((LowerRow > UpperRow)
                             || (LowerCol > UpperCol), "math_Matrix() - invalid dimensions");
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

    class math_DoubleTab
    {

        // macro to get size of C array
        int CARRAY_LENGTH(Array arr) => arr.Length;

        double[] Addr;
        double[] Buf = new double[16];
        public double this[int key, int key2]
        {
            get => Value(key, key2);
            set => Value(key, key2, value);
        }
        public math_DoubleTab(int LowerRow,

                   int UpperRow,
                   int LowerCol,
                   int UpperCol)
        {

            Addr = (Buf);
            isAllocated = ((UpperRow - LowerRow + 1) * (UpperCol - LowerCol + 1) > CARRAY_LENGTH(Buf));
            LowR = (LowerRow);
            UppR = (UpperRow);
            LowC = (LowerCol);
            UppC = (UpperCol);

            Allocate();
        }

        public double Value(int RowIndex,

                     int ColIndex)
        {
            return Addr[(UppC - LowC + 1) * (RowIndex - LowR) + (ColIndex - LowC)];
        }
        public void Value(int RowIndex,

                    int ColIndex, double val
            )
        {
            Addr[(UppC - LowC + 1) * (RowIndex - LowR) + (ColIndex - LowC)] = val;
        }

        void Allocate()
        {
            int RowNumber = UppR - LowR + 1;
            int ColNumber = UppC - LowC + 1;

            if (isAllocated)
                Addr = new double[RowNumber * ColNumber];
        }

        bool isAllocated;
        int LowR;
        int UppR;
        int LowC;
        int UppC;
        public void Init(double InitValue)
        {
            for (int anIndex = 0; anIndex < (UppR - LowR + 1) * (UppC - LowC + 1); anIndex++)
            {
                Addr[anIndex] = InitValue;
            }
        }
        public math_DoubleTab(double[] Tab, int lowerRow, int upperRow, int lowerCol, int upperCol)
        {
            Addr = (Tab);
            isAllocated = false;
            LowR = (lowerRow);
            UppR = (upperRow);
            LowC = (lowerCol);
            UppC = upperCol;

            Allocate();
        }
    }
}
