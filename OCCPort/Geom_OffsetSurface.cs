using System;

namespace OCCPort
{
    //! Describes an offset surface in 3D space.
    //! An offset surface is defined by:
    //! - the basis surface to which it is parallel, and
    //! - the distance between the offset surface and its basis surface.
    //! A point on the offset surface is built by measuring the
    //! offset value along the normal vector at a point on the
    //! basis surface. This normal vector is given by the cross
    //! product D1u^D1v, where D1u and D1v are the
    //! vectors tangential to the basis surface in the u and v
    //! parametric directions at this point. The side of the
    //! basis surface on which the offset is measured
    //! depends on the sign of the offset value.
    //! A Geom_OffsetSurface surface can be
    //! self-intersecting, even if the basis surface does not
    //! self-intersect. The self-intersecting portions are not
    //! deleted at the time of construction.
    //! Warning
    //! There must be only one normal vector defined at any
    //! point on the basis surface. This must be verified by the
    //! user as no check is made at the time of construction
    //! to detect points with multiple possible normal
    //! directions (for example, the top of a conical surface).
    public class Geom_OffsetSurface : Geom_Surface
    {
        public override void Bounds(out double U1, out double U2, out double V1, out double V2)
        {
            throw new NotImplementedException();
        }

        public override Geom_Geometry Copy()
        {
            throw new NotImplementedException();
        }

        public override bool IsUPeriodic()
        {
            return basisSurf.IsUPeriodic();

        }

        Geom_Surface basisSurf;

        public override bool IsVPeriodic()
        {
            return basisSurf.IsVPeriodic();
        }

        public override void Transform(gp_Trsf t)
        {
            throw new NotImplementedException();
        }
    }
}