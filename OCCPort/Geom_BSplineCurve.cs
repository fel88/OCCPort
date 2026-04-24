using System;

namespace OCCPort
{
    //! Definition of the B_spline curve.
    //! A B-spline curve can be
    //! Uniform  or non-uniform
    //! Rational or non-rational
    //! Periodic or non-periodic
    //!
    //! a b-spline curve is defined by :
    //! its degree; the degree for a
    //! Geom_BSplineCurve is limited to a value (25)
    //! which is defined and controlled by the system.
    //! This value is returned by the function MaxDegree;
    //! - its periodic or non-periodic nature;
    //! - a table of poles (also called control points), with
    //! their associated weights if the BSpline curve is
    //! rational. The poles of the curve are "control
    //! points" used to deform the curve. If the curve is
    //! non-periodic, the first pole is the start point of
    //! the curve, and the last pole is the end point of
    //! the curve. The segment which joins the first pole
    //! to the second pole is the tangent to the curve at
    //! its start point, and the segment which joins the
    //! last pole to the second-from-last pole is the
    //! tangent to the curve at its end point. If the curve
    //! is periodic, these geometric properties are not
    //! verified. It is more difficult to give a geometric
    //! signification to the weights but are useful for
    //! providing exact representations of the arcs of a
    //! circle or ellipse. Moreover, if the weights of all the
    //! poles are equal, the curve has a polynomial
    //! equation; it is therefore a non-rational curve.
    //! - a table of knots with their multiplicities. For a
    //! Geom_BSplineCurve, the table of knots is an
    //! increasing sequence of reals without repetition;
    //! the multiplicities define the repetition of the knots.
    //! A BSpline curve is a piecewise polynomial or
    //! rational curve. The knots are the parameters of
    //! junction points between two pieces. The
    //! multiplicity Mult(i) of the knot Knot(i) of
    //! the BSpline curve is related to the degree of
    //! continuity of the curve at the knot Knot(i),
    //! which is equal to Degree - Mult(i)
    //! where Degree is the degree of the BSpline curve.
    //! If the knots are regularly spaced (i.e. the difference
    //! between two consecutive knots is a constant), three
    //! specific and frequently used cases of knot
    //! distribution can be identified:
    //! - "uniform" if all multiplicities are equal to 1,
    //! - "quasi-uniform" if all multiplicities are equal to 1,
    //! except the first and the last knot which have a
    //! multiplicity of Degree + 1, where Degree is
    //! the degree of the BSpline curve,
    //! - "Piecewise Bezier" if all multiplicities are equal to
    //! Degree except the first and last knot which
    //! have a multiplicity of Degree + 1, where
    //! Degree is the degree of the BSpline curve. A
    //! curve of this type is a concatenation of arcs of Bezier curves.
    //! If the BSpline curve is not periodic:
    //! - the bounds of the Poles and Weights tables are 1
    //! and NbPoles, where NbPoles is the number
    //! of poles of the BSpline curve,
    //! - the bounds of the Knots and Multiplicities tables
    //! are 1 and NbKnots, where NbKnots is the
    //! number of knots of the BSpline curve.
    //! If the BSpline curve is periodic, and if there are k
    //! periodic knots and p periodic poles, the period is:
    //! period = Knot(k + 1) - Knot(1)
    //! and the poles and knots tables can be considered
    //! as infinite tables, verifying:
    //! - Knot(i+k) = Knot(i) + period
    //! - Pole(i+p) = Pole(i)
    //! Note: data structures of a periodic BSpline curve
    //! are more complex than those of a non-periodic one.
    //! Warning
    //! In this class, weight value is considered to be zero if
    //! the weight is less than or equal to gp::Resolution().
    //!
    //! References :
    //! . A survey of curve and surface methods in CADG Wolfgang BOHM
    //! CAGD 1 (1984)
    //! . On de Boor-like algorithms and blossoming Wolfgang BOEHM
    //! cagd 5 (1988)
    //! . Blossoming and knot insertion algorithms for B-spline curves
    //! Ronald N. GOLDMAN
    //! . Modelisation des surfaces en CAO, Henri GIAUME Peugeot SA
    //! . Curves and Surfaces for Computer Aided Geometric Design,
    //! a practical guide Gerald Farin
    public class Geom_BSplineCurve : Geom_BoundedCurve
    {
        //==========================================================
        //function : FirstParameter
        //purpose  : 
        //=======================================================================

        public override double LastParameter()
        {
            return flatknots.Value(flatknots.Upper() - deg);
        }

        public override double FirstParameter()
        {
            return flatknots.Value(deg + 1);
        }

        public override void D0(double U, ref gp_Pnt P)
        {
            throw new System.NotImplementedException();
        }

        public override void Transform(gp_Trsf t)
        {
            throw new System.NotImplementedException();
        }

        public override Geom_Geometry Copy()
        {
            throw new System.NotImplementedException();
        }

        public override bool IsPeriodic()
        {
            throw new System.NotImplementedException();
        }

        public override void Reverse()
        {
            throw new System.NotImplementedException();
        }

        public override double ReversedParameter(double U)
        {
            throw new System.NotImplementedException();
        }
        TColgp_Array1OfPnt poles;

        internal int NbPoles()
        {

            return poles.Length();

        }

        TColStd_HArray1OfReal flatknots;
        int deg;

    }
}