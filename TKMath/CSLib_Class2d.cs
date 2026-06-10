using TKernel;

namespace TKMath
{
    //! *** Class2d    : Low level algorithm for 2d classification
    //! this class was moved from package BRepTopAdaptor
    public class CSLib_Class2d
    {
        public CSLib_Class2d(TColgp_Array1OfPnt2d thePnts2d,
                           double theTolU,
                           double theTolV,
                           double theUMin,
                           double theVMin,
                           double theUMax,
                           double theVMax)
        {
            Init(thePnts2d, theTolU, theTolV, theUMin,
                 theVMin, theUMax, theVMax);
        }
        public void Init(TColgp_Array1OfPnt2d TP2d,
                 double aTolu,
                 double aTolv,
                 double umin,
                 double vmin,
                 double umax,
                 double vmax)
        {
            Umin = umin;
            Vmin = vmin;
            Umax = umax;
            Vmax = vmax;
            //
            if ((umax <= umin) || (vmax <= vmin) || (TP2d.Length() < 3))
            {
                MyPnts2dX = null;
                MyPnts2dY = null;
                N = 0;
            }
            //
            else
            {
                int i, iLower;
                double du, dv, aPrc;
                //
                aPrc = 1e-10;
                N = TP2d.Length();
                Tolu = aTolu;
                Tolv = aTolv;
                MyPnts2dX = new TColStd_Array1OfReal(0, N);
                MyPnts2dY = new TColStd_Array1OfReal(0, N);
                du = umax - umin;
                dv = vmax - vmin;
                //
                iLower = TP2d.Lower();
                for (i = 0; i < N; ++i)
                {
                    gp_Pnt2d aP2D = TP2d[i + iLower];
                    MyPnts2dX[i] = Transform2d(aP2D.X(), umin, du);
                    MyPnts2dY[i] = Transform2d(aP2D.Y(), vmin, dv);
                }
                MyPnts2dX[MyPnts2dX.Upper()] = MyPnts2dX.First();
                MyPnts2dY[MyPnts2dY.Upper()] = MyPnts2dY.First();
                //
                if (du > aPrc)
                {
                    Tolu /= du;
                }
                if (dv > aPrc)
                {
                    Tolv /= dv;
                }
            }
        }
        double Tolu;
        double Tolv;
        int N;
        double Umin;
        double Vmin;
        double Umax;
        double Vmax;
        TColStd_Array1OfReal MyPnts2dX, MyPnts2dY;

        public double Transform2d(double u,

              double umin,

              double umaxmumin)
        {
            if (umaxmumin > 1e-10)
            {
                double U = (u - umin) / umaxmumin;
                return U;
            }
            else
            {
                return u;
            }
        }
        public int SiDans(gp_Pnt2d P)
        {
            if (N == 0)
            {
                return 0;
            }
            //
            double x, y, aTolu, aTolv;
            //
            x = P.X(); y = P.Y();
            aTolu = Tolu * (Umax - Umin);
            aTolv = Tolv * (Vmax - Vmin);
            //
            if (Umin < Umax && Vmin < Vmax)
            {
                if ((x < (Umin - aTolu)) ||
                   (x > (Umax + aTolu)) ||
                   (y < (Vmin - aTolv)) ||
                   (y > (Vmax + aTolv)))
                {
                    return -1;
                }
                x = Transform2d(x, Umin, Umax - Umin);
                y = Transform2d(y, Vmin, Vmax - Vmin);
            }


            int res = InternalSiDansOuOn(x, y);
            if (res == -1)
            {
                return 0;
            }
            if (Tolu != 0 || Tolv != 0)
            {
                if (res != InternalSiDans(x - Tolu, y - Tolv)) return 0;
                if (res != InternalSiDans(x + Tolu, y - Tolv)) return 0;
                if (res != InternalSiDans(x - Tolu, y + Tolv)) return 0;
                if (res != InternalSiDans(x + Tolu, y + Tolv)) return 0;
            }
            //
            return ((res != 0) ? 1 : -1);
        }
        public int InternalSiDans(double Px, double Py)
        {
            int nbc, i, ip1, SH, NH;
            double x, y, nx, ny;
            //
            nbc = 0;
            i = 0;
            ip1 = 1;
            x = (MyPnts2dX.Value(i) - Px);
            y = (MyPnts2dY.Value(i) - Py);
            SH = (y < 0.0) ? -1 : 1;
            //
            for (i = 0; i < N; i++, ip1++)
            {
                nx = MyPnts2dX.Value(ip1) - Px;
                ny = MyPnts2dY.Value(ip1) - Py;

                NH = (ny < 0.0) ? -1 : 1;
                if (NH != SH)
                {
                    if (x > 0.0 && nx > 0.0)
                    {
                        nbc++;
                    }
                    else
                    {
                        if (x > 0.0 || nx > 0.0)
                        {
                            if ((x - y * (nx - x) / (ny - y)) > 0.0)
                            {
                                nbc++;
                            }
                        }
                    }
                    SH = NH;
                }
                x = nx; y = ny;
            }
            return (nbc & 1);
        }


        //=======================================================================
        //function : InternalSiDansOuOn
        //purpose  : same code as above + test on ON (return(-1) in this case
        //=======================================================================
        public int InternalSiDansOuOn(double Px,
                           double Py)
        {
            int nbc, i, ip1, SH, NH, iRet;
            double x, y, nx, ny, aX;
            double aYmin;
            //
            nbc = 0;
            i = 0;
            ip1 = 1;
            x = (MyPnts2dX.Value(i) - Px);
            y = (MyPnts2dY.Value(i) - Py);
            aYmin = y;
            SH = (y < 0.0) ? -1 : 1;
            for (i = 0; i < N; i++, ip1++)
            {

                nx = MyPnts2dX.Value(ip1) - Px;
                ny = MyPnts2dY.Value(ip1) - Py;
                //-- le 14 oct 97 
                if (nx < Tolu && nx > -Tolu && ny < Tolv && ny > -Tolv)
                {
                    iRet = -1;
                    return iRet;
                }
                //find Y coordinate of polyline for current X gka
                //in order to detect possible status ON
                double aDx = (MyPnts2dX.Value(ip1) - MyPnts2dX.Value(ip1 - 1));
                if ((MyPnts2dX.Value(ip1 - 1) - Px) * nx < 0.0)
                {
                    double aCurPY = MyPnts2dY.Value(ip1) - (MyPnts2dY.Value(ip1) - MyPnts2dY.Value(ip1 - 1)) / aDx * nx;
                    double aDeltaY = aCurPY - Py;
                    if (aDeltaY >= -Tolv && aDeltaY <= Tolv)
                    {
                        iRet = -1;
                        return iRet;
                    }
                }
                //

                NH = (ny < 0.0) ? -1 : 1;
                if (NH != SH)
                {
                    if (x > 0.0 && nx > 0.0)
                    {
                        nbc++;
                    }
                    else
                    {
                        if (x > 0.0 || nx > 0.0)
                        {
                            aX = x - y * (nx - x) / (ny - y);
                            if (aX > 0.0)
                            {
                                nbc++;
                            }
                        }
                    }
                    SH = NH;
                }
                else
                {// y has same sign as ny  
                    if (ny < aYmin)
                    {
                        aYmin = ny;
                    }
                }
                x = nx; y = ny;
            }// for(i=0; i<N ; i++,ip1++) { 

            iRet = nbc & 1;
            return iRet;
        }
    }
}

