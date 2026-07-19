
using OCCPort.Common;
using System;
using System.Reflection.Metadata;
using TKernel;

namespace TKMath
{
    //! The geometric processor package, called gp, provides an
    //! implementation of entities used  :
    //! . for algebraic calculation such as "XYZ" coordinates, "Mat"
    //! matrix
    //! . for basis analytic geometry such as Transformations, point,
    //! vector, line, plane, axis placement, conics, and elementary
    //! surfaces.
    //! These entities are defined in 2d and 3d space.
    //! All the classes of this package are non-persistent.
    public static class gp
    {

        public static double Resolution()
        {
            //2.2250738585072014e-308
            return double.Epsilon;
        }
        public static gp_Dir2d DY2d()
        {
            gp_Dir2d gp_DY2d = new gp_Dir2d(0, 1);
            return gp_DY2d;
        }
        public static gp_Dir2d DX2d()
        {
            gp_Dir2d gp_DY2d = new gp_Dir2d(1, 0);
            return gp_DY2d;
        }


        public static gp_Dir DX()
        {
            gp_Dir gp_DX = new gp_Dir(1, 0, 0);
            return gp_DX;

        }

        public static gp_Dir DY()
        {
            gp_Dir gp_DY = new gp_Dir(0, 1, 0);
            return gp_DY;

        }

        public static gp_Dir DZ()
        {
            gp_Dir gp_DZ = new gp_Dir(0, 0, 1);
            return gp_DZ;

        }

        public static gp_Pnt Origin()
        {
            gp_Pnt gp_Origin = new gp_Pnt(0, 0, 0);
            return gp_Origin;

        }

        public static gp_Ax2 XOY()
        {
            gp_Ax2 gp_XOY = new gp_Ax2(new gp_Pnt(0, 0, 0), new gp_Dir(0, 0, 1), new gp_Dir(1, 0, 0));
            return gp_XOY;

        }
    }

    //! This class implements the Jacobi method to find the eigenvalues and
    //! the eigenvectors of a real symmetric square matrix.
    //! A sort of eigenvalues is done.
    public class math_Jacobi
    {

        public math_Jacobi(math_Matrix A)
        {

            AA = new math_Matrix(1, A.RowNumber(), 1, A.RowNumber());
            EigenValues = new math_Vector(1, A.RowNumber());
            EigenVectors = new math_Matrix(1, A.RowNumber(), 1, A.RowNumber());
            Exceptions.math_NotSquare_Raise_if(A.RowNumber() != A.ColNumber(), " ");

            AA = A;
            int Error = Jacobi(AA, EigenValues, EigenVectors, NbRotations);
            if (Error == 0)
            {
                Done = true;
            }
            else
            {
                Done = false;
            }
        }
        //! Returns the eigenvector V of number Num.
        //! Eigenvectors are in the range (1..n).
        //! Exception NotDone is raised if calculation is not done successfully.
        public void Vector(int Num, ref math_Vector V)
        {

            Exceptions.StdFail_NotDone_Raise_if(!Done, " ");
            V = EigenVectors.Col(Num);
        }

        //! returns the eigenvalue number Num.
        //! Eigenvalues are in the range (1..n).
        //! Exception NotDone is raised if calculation is not done successfully.
        public double Value(int Num)
        {
            Exceptions.StdFail_NotDone_Raise_if(!Done, " ");
            return EigenValues[Num];
        }

        public static void EigenSort(math_Vector d, math_Matrix v)
        { // descending order

            int k, i, j;
            double p;
            int n = d.Length();

            for (i = 1; i < n; i++)
            {
                p = d[k = i];
                for (j = i + 1; j <= n; j++)
                    if (d[j] >= p) p = d[k = j];
                if (k != i)
                {
                    d[k] = d[i];
                    d[i] = p;
                    for (j = 1; j <= n; j++)
                    {
                        p = v[j, i];
                        v[j, i] = v[j, k];
                        v[j, k] = p;
                    }
                }
            }
        }

        public void ROTATE(math_Matrix a, int i, int j, int k, int l, double tau, ref double h, ref double g, double s)
        {
            /*
#define ROTATE(a,i,j,k,l) g=a(i,j);\
                          h=a(k,l);\
                          a(i,j)=g-s*(h+g*tau);\
                          a(k,l)=h+s*(g-h*tau);
*/
            g = a[i, j];
            h = a[k, l];
            a[i, j] = g - s * (h + g * tau);
            a[k, l] = h + s * (g - h * tau);

        }

        public int Jacobi(math_Matrix a, math_Vector d, math_Matrix v, int nrot)
        {

            int n = a.RowNumber();
            int j, iq, ip, i;
            double tresh, theta, tau, t, sm, s, h, g, c;
            math_Vector b = new math_Vector(1, n);
            math_Vector z = new math_Vector(1, n);

            for (ip = 1; ip <= n; ip++)
            {
                for (iq = 1; iq <= n; iq++)
                    v[ip, iq] = 0.0;
                v[ip, ip] = 1.0;
            }
            for (ip = 1; ip <= n; ip++)
            {
                b[ip] = d[ip] = a[ip, ip];
                z[ip] = 0.0;
            }
            nrot = 0;
            for (i = 1; i <= 50; i++)
            {
                sm = 0.0;
                for (ip = 1; ip < n; ip++)
                {
                    for (iq = ip + 1; iq <= n; iq++)
                        sm += Math.Abs(a[ip, iq]);
                }
                if (sm == 0.0)
                {
                    EigenSort(d, v);
                    return math_Recipes.math_Status_OK;
                }
                if (i < 4)
                {
                    tresh = 0.2 * sm / (n * n);
                }
                else
                {
                    tresh = 0.0;
                }
                for (ip = 1; ip < n; ip++)
                {
                    for (iq = ip + 1; iq <= n; iq++)
                    {
                        g = 100.0 * Math.Abs(a[ip, iq]);
                        if (i > 4 &&
                           Math.Abs(d[ip]) + g == Math.Abs(d[ip]) &&
                           Math.Abs(d[iq]) + g == Math.Abs(d[iq])) a[ip, iq] = 0.0;
                        else if (Math.Abs(a[ip, iq]) > tresh)
                        {
                            h = d[iq] - d[ip];
                            if (Math.Abs(h) + g == Math.Abs(h))
                                t = a[ip, iq] / h;
                            else
                            {
                                theta = 0.5 * h / a[ip, iq];
                                t = 1.0 / (Math.Abs(theta) + Math.Sqrt(1.0 + theta * theta));
                                if (theta < 0.0) t = -t;
                            }
                            c = 1.0 / Math.Sqrt(1 + t * t);
                            s = t * c;
                            tau = s / (1.0 + c);
                            h = t * a[ip, iq];
                            z[ip] -= h;
                            z[iq] += h;
                            d[ip] -= h;
                            d[iq] += h;
                            a[ip, iq] = 0.0;
                            for (j = 1; j < ip; j++)
                            {
                                ROTATE(a, j, ip, j, iq, tau, ref h, ref g, s);
                            }
                            for (j = ip + 1; j < iq; j++)
                            {
                                ROTATE(a, j, ip, j, iq, tau, ref h, ref g, s);
                            }
                            for (j = iq + 1; j <= n; j++)
                            {
                                ROTATE(a, j, ip, j, iq, tau, ref h, ref g, s);
                            }
                            for (j = 1; j <= n; j++)
                            {
                                ROTATE(v, j, ip, j, iq, tau, ref h, ref g, s);
                            }
                            nrot++;
                        }
                    }
                }
                for (ip = 1; ip <= n; ip++)
                {
                    b[ip] += z[ip];
                    d[ip] = b[ip];
                    z[ip] = 0.0;
                }
            }
            EigenSort(d, v);
            return math_Recipes.math_Status_NoConvergence;
        }

        public bool IsDone() { return Done; }

        public math_Vector Values()
        {
            Exceptions.StdFail_NotDone_Raise_if(!Done, " ");
            return EigenValues;
        }

        public math_Matrix Vectors()
        {
            Exceptions.StdFail_NotDone_Raise_if(!Done, " ");
            return EigenVectors;
        }


        bool Done;
        math_Matrix AA;
        int NbRotations;
        math_Vector EigenValues;
        math_Matrix EigenVectors;
    }

    public static class math_Recipes
    {


        public static int LU_Decompose(math_Matrix a,
                            math_IntegerVector indx,
                            double d,
                            double TINY,
                     Message_ProgressRange theProgress)
        {

            math_Vector vv = new(1, a.RowNumber());
            return LU_Decompose(a, indx, d, vv, TINY, theProgress);
        }

        public static int LU_Decompose(math_Matrix a,
                     math_IntegerVector indx,
                     double d,
                     math_Vector vv,
                     double TINY,
                      Message_ProgressRange theProgress)
        {

            int i, imax = 0, j, k;
            double big, dum, sum, temp;

            int n = a.RowNumber();
            d = 1.0;

            Message_ProgressScope aPS = new(theProgress, "math_Gauss LU_Decompose", n);

            for (i = 1; i <= n; i++)
            {
                big = 0.0;
                for (j = 1; j <= n; j++)
                    if ((temp = Math.Abs(a[i, j])) > big) big = temp;
                if (big <= TINY)
                {
                    return math_Status_SingularMatrix;
                }
                vv[(i)] = 1.0 / big;
            }

            for (j = 1; j <= n && aPS.More(); j++, aPS.Next())
            {
                for (i = 1; i < j; i++)
                {
                    sum = a[i, j];
                    for (k = 1; k < i; k++)
                        sum -= a[i, k] * a[k, j];
                    a[i, j] = sum;
                }
                big = 0.0;
                for (i = j; i <= n; i++)
                {
                    sum = a[i, j];
                    for (k = 1; k < j; k++)
                        sum -= a[i, k] * a[k, j];
                    a[i, j] = sum;

                    // Note that comparison is made so as to have imax updated even if argument is NAN, Inf or IND, see #25559
                    if ((dum = vv[i] * Math.Abs(sum)) < big)
                    {
                        continue;
                    }
                    big = dum;
                    imax = i;
                }
                if (j != imax)
                {
                    for (k = 1; k <= n; k++)
                    {
                        dum = a[imax, k];
                        a[imax, k] = a[j, k];
                        a[j, k] = dum;
                    }
                    d = -d;
                    vv[imax] = vv[j];
                }

                indx[j] = imax;
                if (Math.Abs(a[j, j]) <= TINY)
                {
                    return math_Status_SingularMatrix;
                }
                if (j != n)
                {
                    dum = 1.0 / (a[j, j]);
                    for (i = j + 1; i <= n; i++)
                        a[i, j] *= dum;
                }
            }

            if (j <= n)
            {
                return math_Status_UserAborted;
            }

            return math_Status_OK;
        }

        public static void LU_Solve(math_Matrix a,
                   math_IntegerVector indx,
                  math_Vector b)
        {

            int i, ii = 0, ip, j;
            double sum;

            int n = a.RowNumber();
            int nblow = b.Lower() - 1;
            for (i = 1; i <= n; i++)
            {
                ip = indx[i];
                sum = b[ip + nblow];
                b[ip + nblow] = b[i + nblow];
                //if (ii)
                if (ii != 0)
                    for (j = ii; j < i; j++)
                        sum -= a[i, j] * b[j + nblow];
                else
                    //if (sum) 
                    if (sum != 0)
                    ii = i;
                b[i + nblow] = sum;
            }
            for (i = n; i >= 1; i--)
            {
                sum = b[i + nblow];
                for (j = i + 1; j <= n; j++)
                    sum -= a[i, j] * b[j + nblow];
                b[i + nblow] = sum / a[i, i];
            }
        }
        public const int math_Status_UserAborted = -1;
        public const int math_Status_OK = 0;
        public const int math_Status_SingularMatrix = 1;
        public const int math_Status_ArgumentError = 2;
        public const int math_Status_NoConvergence = 3;
    }
}
