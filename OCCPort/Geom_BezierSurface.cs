using System;

namespace OCCPort
{
    //! Describes a rational or non-rational Bezier surface.
    //! - A non-rational Bezier surface is defined by a table
    //! of poles (also known as control points).
    //! - A rational Bezier surface is defined by a table of
    //! poles with varying associated weights.
    //! This data is manipulated using two associative 2D arrays:
    //! - the poles table, which is a 2D array of gp_Pnt, and
    //! - the weights table, which is a 2D array of reals.
    //! The bounds of these arrays are:
    //! - 1 and NbUPoles for the row bounds, where
    //! NbUPoles is the number of poles of the surface
    //! in the u parametric direction, and
    //! - 1 and NbVPoles for the column bounds, where
    //! NbVPoles is the number of poles of the surface
    //! in the v parametric direction.
    //! The poles of the surface, the "control points", are the
    //! points used to shape and reshape the surface. They
    //! comprise a rectangular network of points:
    //! - The points (1, 1), (NbUPoles, 1), (1,
    //! NbVPoles) and (NbUPoles, NbVPoles)
    //! are the four parametric "corners" of the surface.
    //! - The first column of poles and the last column of
    //! poles define two Bezier curves which delimit the
    //! surface in the v parametric direction. These are
    //! the v isoparametric curves corresponding to
    //! values 0 and 1 of the v parameter.
    //! - The first row of poles and the last row of poles
    //! define two Bezier curves which delimit the surface
    //! in the u parametric direction. These are the u
    //! isoparametric curves corresponding to values 0
    //! and 1 of the u parameter.
    //! It is more difficult to define a geometrical significance
    //! for the weights. However they are useful for
    //! representing a quadric surface precisely. Moreover, if
    //! the weights of all the poles are equal, the surface has
    //! a polynomial equation, and hence is a "non-rational surface".
    //! The non-rational surface is a special, but frequently
    //! used, case, where all poles have identical weights.
    //! The weights are defined and used only in the case of
    //! a rational surface. This rational characteristic is
    //! defined in each parametric direction. Hence, a
    //! surface can be rational in the u parametric direction,
    //! and non-rational in the v parametric direction.
    //! Likewise, the degree of a surface is defined in each
    //! parametric direction. The degree of a Bezier surface
    //! in a given parametric direction is equal to the number
    //! of poles of the surface in that parametric direction,
    //! minus 1. This must be greater than or equal to 1.
    //! However, the degree for a Geom_BezierSurface is
    //! limited to a value of (25) which is defined and
    //! controlled by the system. This value is returned by the
    //! function MaxDegree.
    //! The parameter range for a Bezier surface is [ 0, 1 ]
    //! in the two parametric directions.
    //! A Bezier surface can also be closed, or open, in each
    //! parametric direction. If the first row of poles is
    //! identical to the last row of poles, the surface is closed
    //! in the u parametric direction. If the first column of
    //! poles is identical to the last column of poles, the
    //! surface is closed in the v parametric direction.
    //! The continuity of a Bezier surface is infinite in the u
    //! parametric direction and the in v parametric direction.
    //! Note: It is not possible to build a Bezier surface with
    //! negative weights. Any weight value that is less than,
    //! or equal to, gp::Resolution() is considered
    //! to be zero. Two weight values, W1 and W2, are
    //! considered equal if: |W2-W1| <= gp::Resolution()
    public class Geom_BezierSurface : Geom_BoundedSurface
    {
        public override Geom_Geometry Copy()
        {
            throw new NotImplementedException();
        }

        public override void D0(double U, double V, ref gp_Pnt P)
        {
            throw new NotImplementedException();
        }

        public override void D1(double U, double V, out gp_Pnt P, out gp_Vec D1U, out gp_Vec D1V)
        {
            throw new NotImplementedException();
        }

        public override void D2(double U, double V, out gp_Pnt P, out gp_Vec D1U, out gp_Vec D1V, out gp_Vec D2U, out gp_Vec D2V, out gp_Vec D2UV)
        {
            throw new NotImplementedException();
        }

        public override bool IsUPeriodic()
        {
            throw new NotImplementedException();
        }

        public override bool IsVPeriodic()
        {
            throw new NotImplementedException();
        }

        public override void Transform(gp_Trsf t)
        {
            throw new NotImplementedException();
        }
    }
}