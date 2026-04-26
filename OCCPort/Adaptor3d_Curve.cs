using System;

namespace OCCPort
{
    //! Root class for 3D curves on which geometric
    //! algorithms work.
    //! An adapted curve is an interface between the
    //! services provided by a curve and those required of
    //! the curve by algorithms which use it.
    //! Two derived concrete classes are provided:
    //! - GeomAdaptor_Curve for a curve from the Geom package
    //! - Adaptor3d_CurveOnSurface for a curve lying on
    //! a surface from the Geom package.
    //!
    //! Polynomial coefficients of BSpline curves used for their evaluation are
    //! cached for better performance. Therefore these evaluations are not
    //! thread-safe and parallel evaluations need to be prevented.
    public abstract class Adaptor3d_Curve:ITheCurve
    {

        //! Returns  the number  of  intervals for  continuity
        //! <S>. May be one if Continuity(me) >= <S>
        public abstract int NbIntervals(GeomAbs_Shape S);


        //=======================================================================
        //function : GetType
        //purpose  : 
        //=======================================================================
        public abstract int Degree();
        public abstract int NbKnots();

        public abstract Geom_BSplineCurve BSpline();

        //! Returns the parametric  resolution corresponding
        //! to the real space resolution <R3d>.
        public abstract double Resolution(double R3d);
        public abstract bool IsPeriodic();

        public abstract GeomAbs_CurveType _GetType();
        //=======================================================================
        //function : Line
        //purpose  : 
        //=======================================================================

        public abstract gp_Lin Line();

        public abstract gp_Pnt Value(double d);

        public virtual double FirstParameter()
        {
            throw new Standard_NotImplemented("Adaptor3d_Curve::FirstParameter");
        }
        public virtual double LastParameter()
        {
            throw new Standard_NotImplemented("Adaptor3d_Curve::LastParameter");
        }

        //void Adaptor3d_Curve::D0(const Standard_Real U, gp_Pnt& P) const 
        public abstract void D0(double d, ref gp_Pnt p);
    }
}