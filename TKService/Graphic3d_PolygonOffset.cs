using System.Diagnostics.CodeAnalysis;

namespace TKService
{
    //! Polygon offset parameters.
    public struct Graphic3d_PolygonOffset
    {
        public Aspect_PolygonOffsetMode Mode;

        public float Factor;
        public float Units;
        //! Empty constructor.
        public Graphic3d_PolygonOffset()
        {
            Mode = Aspect_PolygonOffsetMode.Aspect_POM_Fill;
            Factor = (1.0f);
            Units = (1.0f);
        }


        //! Equality comparison.

        public static bool operator !=(Graphic3d_PolygonOffset a, Graphic3d_PolygonOffset b)
        {
            return a.Mode != b.Mode
              || a.Factor != b.Factor
              || a.Units != b.Units;
        }

        public static bool operator ==(Graphic3d_PolygonOffset a, Graphic3d_PolygonOffset b)
        {
            return a.Mode == b.Mode
                && a.Factor == b.Factor
                && a.Units == b.Units;
        }

    }
}
