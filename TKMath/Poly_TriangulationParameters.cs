namespace TKMath
{
    //! Represents initial set of parameters triangulation is built for.
    public class Poly_TriangulationParameters
    {

        //! Constructor.
        //! Initializes object with the given parameters.
        //! @param theDeflection linear deflection
        //! @param theAngle angular deflection
        //! @param theMinSize minimum size
        public Poly_TriangulationParameters(double theDeflection = -1.0,
                                 double theAngle = -1.0,
                                 double theMinSize = -1.0)
        {
            myDeflection = theDeflection;
            myAngle = theAngle;
            myMinSize = theMinSize;

        }
        double myDeflection;
        double myAngle;
        double myMinSize;
    }
}