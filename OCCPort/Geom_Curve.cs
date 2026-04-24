using System;

namespace OCCPort
{
    public abstract class Geom_Curve : Geom_Geometry
    {

        //! Is the parametrization of the curve periodic ?
        //! It is possible only if the curve is closed and if the
        //! following relation is satisfied :
        //! for each parametric value U the distance between the point
        //! P(u) and the point P (u + T) is lower or equal to Resolution
        //! from package gp, T is the period and must be a constant.
        //! There are three possibilities :
        //! . the curve is never periodic by definition (SegmentLine)
        //! . the curve is always periodic by definition (Circle)
        //! . the curve can be defined as periodic (BSpline). In this case
        //! a function SetPeriodic allows you to give the shape of the
        //! curve.  The general rule for this case is : if a curve can be
        //! periodic or not the default periodicity set is non periodic
        //! and you have to turn (explicitly) the curve into a periodic
        //! curve  if you want the curve to be periodic.
        public abstract bool IsPeriodic();

        //! Changes the direction of parametrization of <me>.
        //! The "FirstParameter" and the "LastParameter" are not changed
        //! but the orientation  of the curve is modified. If the curve
        //! is bounded the StartPoint of the initial curve becomes the
        //! EndPoint of the reversed curve  and the EndPoint of the initial
        //! curve becomes the StartPoint of the reversed curve.
        public abstract void Reverse();

        //! Returns the  parameter on the  reversed  curve for
        //! the point of parameter U on <me>.
        //!
        //! me->Reversed()->Value(me->ReversedParameter(U))
        //!
        //! is the same point as
        //!
        //! me->Value(U)
        public abstract double ReversedParameter(double U);

        //! Warnings :
        //! It can be RealFirst from package Standard
        //! if the curve is infinite
        public abstract double FirstParameter();

        //! Returns the value of the last parameter.
        //! Warnings :
        //! It can be RealLast from package Standard
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
        public abstract void D0(double U, ref gp_Pnt P);

        public virtual double TransformedParameter(double U,
                           gp_Trsf t)
        {
            return U;
        }

       


    }
}