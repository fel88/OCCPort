using System;

namespace OCCPort
{
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
        public void Vector(int Num, math_Vector V)
        {

            Exceptions.StdFail_NotDone_Raise_if(!Done, " ");
            V = EigenVectors.Col(Num);
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
                                ROTATE(a, j, ip, j, iq, tau, ref h, ref g, s);
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

        public const int math_Status_UserAborted = -1;
        public const int math_Status_OK = 0;
        public const int math_Status_SingularMatrix = 1;
        public const int math_Status_ArgumentError = 2;
        public const int math_Status_NoConvergence = 3;
    }
}
