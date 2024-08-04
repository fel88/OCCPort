namespace OCCPort
{
    //! Describes a portion of a surface (a patch) limited
    //! by two values of the u parameter in the u
    //! parametric direction, and two values of the v
    //! parameter in the v parametric direction. The
    //! domain of the trimmed surface must be within the
    //! domain of the surface being trimmed.
    //! The trimmed surface is defined by:
    //! - the basis surface, and
    //! - the values (umin, umax) and (vmin, vmax)
    //! which limit it in the u and v parametric directions.
    //! The trimmed surface is built from a copy of the basis
    //! surface. Therefore, when the basis surface is
    //! modified the trimmed surface is not changed.
    //! Consequently, the trimmed surface does not
    //! necessarily have the same orientation as the basis surface.
    //! Warning:  The  case of surface   being trimmed is  periodic and
    //! parametrics values are outside the domain is possible.
    //! But, domain of the  trimmed surface can be translated
    //! by (n X) the period.
    public class Geom_RectangularTrimmedSurface : Geom_BoundedSurface
    {
        public Geom_Surface BasisSurface()
        {
            return basisSurf;
        }
        Geom_Surface basisSurf;
        double utrim1;
        double vtrim1;
        double utrim2;
        double vtrim2;
        bool isutrimmed;
        bool isvtrimmed;
    }

}