using OCCPort.Common;
using System.Data.Common;
using TKernel;

namespace TKMath
{
    //! This class implements the Gauss LU decomposition (Crout algorithm)
    //! with partial pivoting (rows interchange) of a square matrix and
    //! the different possible derived calculation :
    //! - solution of a set of linear equations.
    //! - inverse of a matrix.
    //! - determinant of a matrix.
    public class math_Gauss
    {

        //! Given an input n X n matrix A this constructor performs its LU
        //! decomposition with partial pivoting (interchange of rows).
        //! This LU decomposition is stored internally and may be used to
        //! do subsequent calculation.
        //! If the largest pivot found is less than MinPivot the matrix A is
        //! considered as singular.
        //! Exception NotSquare is raised if A is not a square matrix.
        public math_Gauss(math_Matrix A,
                              double MinPivot = 1.0e-20,
                              Message_ProgressRange theProgress = null)
        {
            if (theProgress == null)
            {
                theProgress = new Message_ProgressRange();
            }
            LU = new(1, A.RowNumber(), 1, A.ColNumber());
            Index = new(1, A.RowNumber());
            D = (0.0);
            Done = false;

            Exceptions.math_NotSquare_Raise_if(A.RowNumber() != A.ColNumber(), " ");
            LU = A;
            int Error = math_Recipes.LU_Decompose(LU,
                                        Index,
                                        D,
                                        MinPivot,
                                        theProgress);
            if (Error != 0)
            {
                Done = true;
            }
            else
            {
                Done = false;
            }
        }
        double D;

        math_Matrix LU;
        math_IntegerVector Index;

        public void Invert(math_Matrix Inv)
        {
            Exceptions.StdFail_NotDone_Raise_if(!Done, " ");
            Exceptions.Standard_DimensionError_Raise_if((Inv.RowNumber() != LU.RowNumber()) ||
                        (Inv.ColNumber() != LU.ColNumber()),
                        " ");

            int LowerRow = Inv.LowerRow();
            int LowerCol = Inv.LowerCol();
            math_Vector Column = new(1, LU.UpperRow());

            int I, J;
            for (J = 1; J <= LU.UpperRow(); J++)
            {
                for (I = 1; I <= LU.UpperRow(); I++)
                {
                    Column[I] = 0.0;
                }
                Column[J] = 1.0;
                math_Recipes.LU_Solve(LU, Index, Column);
                for (I = 1; I <= LU.RowNumber(); I++)
                {
                    Inv[I + LowerRow - 1, J + LowerCol - 1] = Column[I];
                }
            }

        }
        //! Returns true if the computations are successful, otherwise returns false
        public bool IsDone() { return Done; }

        bool Done;

    }
}
