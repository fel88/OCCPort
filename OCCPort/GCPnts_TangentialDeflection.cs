namespace OCCPort
{
    //! Computes a set of  points on a curve from package
    //! Adaptor3d  such  as between  two successive   points
    //! P1(u1)and P2(u2) :
    //! @code
    //! . ||P1P3^P3P2||/||P1P3||*||P3P2||<AngularDeflection
    //! . ||P1P2^P1P3||/||P1P2||<CurvatureDeflection
    //! @endcode
    //! where P3 is the point of abscissa ((u1+u2)/2), with
    //! u1 the abscissa of the point P1 and u2 the abscissa
    //! of the point P2.
    //!
    //! ^ is the cross product of two vectors, and ||P1P2||
    //! the magnitude of the vector P1P2.
    //!
    //! The conditions AngularDeflection > gp::Resolution()
    //! and CurvatureDeflection > gp::Resolution() must be
    //! satisfied at the construction time.
    //!
    //! A minimum number of points can be fixed for a linear or circular element.
    //! Example:
    //! @code
    //! Handle(Geom_BezierCurve) aCurve = new Geom_BezierCurve (thePoles);
    //! GeomAdaptor_Curve aCurveAdaptor (aCurve);
    //! double aCDeflect  = 0.01; // Curvature deflection
    //! double anADeflect = 0.09; // Angular   deflection
    //!
    //! GCPnts_TangentialDeflection aPointsOnCurve;
    //! aPointsOnCurve.Initialize (aCurveAdaptor, anADeflect, aCDeflect);
    //! for (int i = 1; i <= aPointsOnCurve.NbPoints(); ++i)
    //! {
    //!   double aU   = aPointsOnCurve.Parameter (i);
    //!   gp_Pnt aPnt = aPointsOnCurve.Value (i);
    //! }
    //! @endcode
    public class GCPnts_TangentialDeflection
    {
    }
}