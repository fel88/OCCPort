using OCCPort.Common;
using TKernel;

namespace TKMath
{
    //! Describes a unit vector in 3D space. This unit vector is also called "Direction".
    //! See Also
    //! gce_MakeDir which provides functions for more complex
    //! unit vector constructions
    //! Geom_Direction which provides additional functions for
    //! constructing unit vectors and works, in particular, with the
    //! parametric equations of unit vectors.
    public struct gp_Dir
    {
        private gp_XYZ coord;

        public override string ToString()
        {
            return $"gp_Dir: X:{coord.X()} Y:{coord.Y()} Z:{coord.Z()}";
        }

        //! Creates a direction corresponding to X axis.
        //! Returns true if the angle between this unit vector and the
        //! unit vector theOther is equal to 0 or to Pi.
        //! Note: the tolerance criterion is given by theAngularTolerance.
        public bool IsParallel(gp_Dir theOther, double theAngularTolerance)
        {
            double anAng = Angle(theOther);
            return anAng <= theAngularTolerance || Math.PI - anAng <= theAngularTolerance;
        }

        public double X()
        {
            return coord.X();
        }

        //! Returns for the  unit vector  its three coordinates theXv, theYv, and theZv.
        public void Coord(ref double theXv, ref double theYv, ref double theZv)
        {
            coord.Coord(ref theXv, ref theYv, ref theZv);
        }


        //! Computes the scalar product
        public double Dot(gp_Dir theOther) { return coord.Dot(theOther.coord); }

        //! Assigns the three coordinates of theCoord to this unit vector.
        public void SetXYZ(gp_XYZ theXYZ)
        {
            double aX = theXYZ.X();
            double anY = theXYZ.Y();
            double aZ = theXYZ.Z();
            double aD = Math.Sqrt(aX * aX + anY * anY + aZ * aZ);
            Exceptions.Standard_ConstructionError_Raise_if(aD <= gp.Resolution(), "gp_Dir::SetX() - input vector has zero norm");
            coord.SetX(aX / aD);
            coord.SetY(anY / aD);
            coord.SetZ(aZ / aD);
        }

        public void Reverse() { coord.Reverse(); }
        public double Y()
        {
            return coord.Y();
        }
        public double Z()
        {
            return coord.Z();
        }

        public static gp_Dir operator ^(gp_Dir v, gp_Dir theRight)
        {
            return v.Crossed(theRight);
        }

        public void SetCoord(double theXv,
                             double theYv,
                              double theZv)
        {
            double aD = Math.Sqrt(theXv * theXv + theYv * theYv + theZv * theZv);
            if (aD <= gp.Resolution())
                throw new Exception("gp_Dir::SetCoord() - input vector has zero norm");

            coord.SetX(theXv / aD);
            coord.SetY(theYv / aD);
            coord.SetZ(theZv / aD);
        }




        public static double operator *(gp_Dir v, gp_Dir theOther)
        {
            return v.Dot(theOther);
        }
        public static gp_Vec operator *(double theScalar, gp_Dir theV)
        {
            return theV.Multiplied(theScalar);
        }

        //! Multiplies a vector by a scalar
        public gp_Vec Multiplied(double theScalar)
        {
            this.coord.Multiply(theScalar);
            return new gp_Vec(this);
        }
        public static gp_Dir operator -(gp_Dir temp)
        {
            return temp.Reversed();
        }

        //! Normalizes the vector theV and creates a direction. Raises ConstructionError if theV.Magnitude() <= Resolution.
        public gp_Dir(gp_Vec theV)
        {
            gp_XYZ aXYZ = theV.XYZ();
            double aX = aXYZ.X();
            double aY = aXYZ.Y();
            double aZ = aXYZ.Z();
            double aD = Math.Sqrt(aX * aX + aY * aY + aZ * aZ);
            //Standard_ConstructionError_Raise_if(aD <= gp::Resolution(), "gp_Dir() - input vector has zero norm");
            coord = new gp_XYZ();
            coord.SetX(aX / aD);
            coord.SetY(aY / aD);
            coord.SetZ(aZ / aD);

        }
        //! Normalizes the vector theV and creates a direction. Raises ConstructionError if theV.Magnitude() <= Resolution.
        public gp_Dir(gp_Dir theV)
        {
            coord = new gp_XYZ(theV.coord);
        }
        //! Creates a direction from a triplet of coordinates. Raises ConstructionError if theCoord.Modulus() <= Resolution from gp.
        public gp_Dir(gp_XYZ theXYZ)
        {
            double aX = theXYZ.X();
            double aY = theXYZ.Y();
            double aZ = theXYZ.Z();
            double aD = Math.Sqrt(aX * aX + aY * aY + aZ * aZ);
            //Standard_ConstructionError_Raise_if(aD <= gp::Resolution(), "gp_Dir() - input vector has zero norm");
            coord = new gp_XYZ();
            coord.SetX(aX / aD);
            coord.SetY(aY / aD);
            coord.SetZ(aZ / aD);

        }



        //! Creates a direction with its 3 cartesian coordinates. Raises ConstructionError if Sqrt(theXv*theXv + theYv*theYv + theZv*theZv) <= Resolution
        //! Modification of the direction's coordinates
        //! If Sqrt (theXv*theXv + theYv*theYv + theZv*theZv) <= Resolution from gp where
        //! theXv, theYv ,theZv are the new coordinates it is not possible to
        //! construct the direction and the method raises the
        //! exception ConstructionError.
        public gp_Dir(double theXv, double theYv, double theZv)
        {
            double aD = Math.Sqrt(theXv * theXv + theYv * theYv + theZv * theZv);
            //Standard_ConstructionError_Raise_if(aD <= gp::Resolution(), "gp_Dir() - input vector has zero norm");
            coord = new gp_XYZ();
            coord.SetX(theXv / aD);
            coord.SetY(theYv / aD);
            coord.SetZ(theZv / aD);

        }

        //! for this unit vector, returns  its three coordinates as a number triplea.
        public gp_XYZ XYZ() { return coord; }
        //! Computes the angular value in radians between <me> and
        //! <theOther>. This value is always positive in 3D space.
        //! Returns the angle in the range [0, PI]
        public double Angle(gp_Dir Other)
        {
            //    Commentaires :
            //    Au dessus de 45 degres l'arccos donne la meilleur precision pour le
            //    calcul de l'angle. Sinon il vaut mieux utiliser l'arcsin.
            //    Les erreurs commises sont loin d'etre negligeables lorsque l'on est
            //    proche de zero ou de 90 degres.
            //    En 3d les valeurs angulaires sont toujours positives et comprises entre
            //    0 et PI
            double Cosinus = coord.Dot(Other.coord);
            if (Cosinus > -0.70710678118655 && Cosinus < 0.70710678118655)
                return Math.Acos(Cosinus);
            else
            {
                double Sinus = (coord.Crossed(Other.coord)).Modulus();
                if (Cosinus < 0.0) return Math.PI - Math.Asin(Sinus);
                else return Math.Asin(Sinus);
            }
        }


        //! Returns True if the angle between the two directions is
        //! lower or equal to theAngularTolerance.
        public bool IsEqual(gp_Dir theOther, double theAngularTolerance)
        {
            return Angle(theOther) <= theAngularTolerance;
        }

        public gp_Dir Crossed(gp_Dir theRight)
        {
            gp_Dir aV = this;
            aV.coord.Cross(theRight.coord);
            double aD = aV.coord.Modulus();
            //Standard_ConstructionError_Raise_if(aD <= gp::Resolution(), "gp_Dir::Crossed() - result vector has zero norm");

            aV.coord.Divide(aD);
            return aV;
        }


        //! Reverses the orientation of a direction
        //! geometric transformations
        //! Performs the symmetrical transformation of a direction
        //! with respect to the direction V which is the center of
        //! the  symmetry.]
        public gp_Dir Reversed()
        {
            gp_Dir aV = this;
            aV.coord.Reverse();
            return aV;
        }

        public void Transform(gp_Trsf T)
        {
            if (T.Form() == gp_TrsfForm.gp_Identity || T.Form() == gp_TrsfForm.gp_Translation) { }
            else if (T.Form() == gp_TrsfForm.gp_PntMirror) { coord.Reverse(); }
            else if (T.Form() == gp_TrsfForm.gp_Scale)
            {
                if (T.ScaleFactor() < 0.0) { coord.Reverse(); }
            }
            else
            {
                coord.Multiply(T.HVectorialPart());
                double D = coord.Modulus();
                coord.Divide(D);
                if (T.ScaleFactor() < 0.0) { coord.Reverse(); }
            }
        }

        public void CrossCross(gp_Dir theV1, gp_Dir theV2)
        {
            coord.CrossCross(theV1.coord, theV2.coord);
            var aD = coord.Modulus();
            //Standard_ConstructionError_Raise_if(aD <= gp::Resolution(), "gp_Dir::CrossCross() - result vector has zero norm");
            coord.Divide(aD);
        }

        public void Cross(gp_Dir theRight)
        {
            coord.Cross(theRight.coord);
            var aD = coord.Modulus();
            //Standard_ConstructionError_Raise_if(aD <= gp::Resolution(), "gp_Dir::Cross() - result vector has zero norm");
            coord.Divide(aD);
        }

        //! Returns True if  the angle between this unit vector and the unit vector theOther is equal to  Pi (opposite).
        public bool IsOpposite(gp_Vec theOther, double theAngularTolerance)
        {
            return Math.PI - Angle(new gp_Dir(theOther)) <= theAngularTolerance;

        }
        public static implicit operator gp_Vec(gp_Dir f)
        {
            return new gp_Vec(f);
        }


    }

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
    public class TColgp_Array1OfPnt2d : NCollection_Array1<gp_Pnt2d>
    {
        public TColgp_Array1OfPnt2d(int theLower, int theUpper) : base(theLower, theUpper)
        {
        }
    }
}

