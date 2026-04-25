using System;

namespace OCCPort
{
    //! The abstract class BoundedCurve describes the
    //! common behavior of bounded curves in 2D space. A
    //! bounded curve is limited by two finite values of the
    //! parameter, termed respectively "first parameter" and
    //! "last parameter". The "first parameter" gives the "start
    //! point" of the bounded curve, and the "last parameter"
    //! gives the "end point" of the bounded curve.
    //! The length of a bounded curve is finite.
    //! The Geom2d package provides three concrete
    //! classes of bounded curves:
    //! - two frequently used mathematical formulations of complex curves:
    //! - Geom2d_BezierCurve,
    //! - Geom2d_BSplineCurve, and
    //! - Geom2d_TrimmedCurve to trim a curve, i.e. to
    //! only take part of the curve limited by two values of
    //! the parameter of the basis curve.
    public abstract class Geom2d_BoundedCurve : Geom2d_Curve
    {
        public override void D0(double U, ref gp_Pnt2d P)
        {
            throw new NotImplementedException();
        }

        public override double FirstParameter()
        {
            throw new NotImplementedException();
        }

        public override bool IsPeriodic()
        {
            throw new NotImplementedException();
        }

        public override double LastParameter()
        {
            throw new NotImplementedException();
        }

        public override void Reverse()
        {
            throw new NotImplementedException();
        }

        public override double ReversedParameter(double U)
        {
            throw new NotImplementedException();
        }
    }
}