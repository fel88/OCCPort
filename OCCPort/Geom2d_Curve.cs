using System;

namespace OCCPort
{
    public abstract class Geom2d_Curve : Geom2d_Geometry
    {

        public gp_Pnt2d Value(double U)
        {
            gp_Pnt2d P = new gp_Pnt2d();
            D0(U, ref P);
            return P;
        }
        //! curve becomes the StartPoint of the reversed curve.
        public abstract void Reverse();
        //! Computes the parameter on the reversed curve for
        //! the point of parameter U on this curve.
        //! Note: The point of parameter U on this curve is
        //! identical to the point of parameter
        //! ReversedParameter(U) on the reversed curve.
        public abstract double ReversedParameter(double U);

        //! Returns the value of the first parameter.
        //! Warnings :
        //! It can be RealFirst or RealLast from package Standard
        //! if the curve is infinite
        public abstract double FirstParameter();

        //! Value of the last parameter.
        //! Warnings :
        //! It can be RealFirst or RealLast from package Standard
        //! if the curve is infinite

        public abstract double LastParameter();

        //! Returns in P the point of parameter U.
        //! If the curve is periodic  then the returned point is P(U) with
        //! U = Ustart + (U - Uend)  where Ustart and Uend are the
        //! parametric bounds of the curve.
        //!
        //! Raised only for the "OffsetCurve" if it is not possible to
        //! compute the current point. For example when the first
        //! derivative on the basis curve and the offset direction
        //! are parallel.
        public abstract void D0(double U, ref gp_Pnt2d P);

    }
}