namespace OCCPort
{
    //! Defines a portion of a curve limited by two values of
    //! parameters inside the parametric domain of the curve.
    //! The trimmed curve is defined by:
    //! - the basis curve, and
    //! - the two parameter values which limit it.
    //! The trimmed curve can either have the same
    //! orientation as the basis curve or the opposite orientation.
    public class Geom2d_TrimmedCurve : Geom2d_BoundedCurve
    {
        Geom2d_Curve basisCurve;
        double uTrim1;
        double uTrim2;
        public Geom2d_Curve BasisCurve()
        {
            return basisCurve;
        }
    }

}