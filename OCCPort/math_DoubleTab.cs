using System;

namespace OCCPort
{
    class math_DoubleTab
    {

        // macro to get size of C array
        int CARRAY_LENGTH(Array arr) => arr.Length;

        double[] Addr;
        double[] Buf = new double[16];

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