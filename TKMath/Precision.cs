namespace TKMath
{
    public class Precision
    {
        public static double Confusion() { return 1e-7; }

        //! Returns a  big number that  can  be  considered as
        //! infinite. Use -Infinite() for a negative big number.
        public static double Infinite() { return 2e+100; }
        //! Returns the recommended precision value
        //! when checking the equality of two angles (given in radians).
        //! Standard_Real Angle1 = ... , Angle2 = ... ;
        //! If ( Abs( Angle2 - Angle1 ) < Precision::Angular() ) ...
        //! The tolerance of angular equality may be used to check the parallelism of two vectors :
        //! gp_Vec V1, V2 ;
        //! V1 = ...
        //! V2 = ...
        //! If ( V1.IsParallel (V2, Precision::Angular() ) ) ...
        //! The tolerance of angular equality is equal to 1.e-12.
        //! Note : The tolerance of angular equality can be used when working with scalar products or
        //! cross products since sines and angles are equivalent for small angles. Therefore, in order to
        //! check whether two unit vectors are perpendicular :
        //! gp_Dir D1, D2 ;
        //! D1 = ...
        //! D2 = ...
        //! you can use :
        //! If ( Abs( D1.D2 ) < Precision::Angular() ) ...
        //! (although the function IsNormal does exist).
        public static double Angular() { return 1e-12; }

        //! Returns True if R may be considered as  a positive
        //! infinite number. Currently R > 1e100
        public static bool IsPositiveInfinite(double R) { return R >= (0.5 * Precision.Infinite()); }
        //! Used for  Approximations  in parametric space on a
        //! default curve.
        //!
        //! This is Precision::Parametric(Precision::Approximation())
        public static double PApproximation() { return Parametric(Approximation()); }

        //! Returns True if R may  be considered as a negative
        //! infinite number. Currently R < -1e100
        public static bool IsNegativeInfinite(double R) { return R <= -(0.5 * Precision.Infinite()); }

        //! Convert a real  space precision  to  a  parametric
        //! space precision.   <T>  is the mean  value  of the
        //! length of the tangent of the curve or the surface.
        //!
        //! Value is P / T
        public static double Parametric(double P, double T) { return P / T; }

        //! Convert a real  space precision  to  a  parametric
        //! space precision on a default curve.
        //!
        //! Value is Parametric(P,1.e+2)
        public static double Parametric(double P)
        {
            return Parametric(P, 100.0);
        }


        //! Used  to test distances  in parametric  space on a
        //! default curve.
        //!
        //! This is Precision::Parametric(Precision::Confusion())
        public static double PConfusion() { return Parametric(Confusion()); }


        //! Returns True if R may be considered as an infinite
        //! number. Currently Abs(R) > 1e100
        public static bool IsInfinite(double R) { return Math.Abs(R) >= (0.5 * Precision.Infinite()); }


        //! Returns square of PConfusion.
        //! Created for speed and convenience.
        public static double SquarePConfusion() { return PConfusion() * PConfusion(); }
        //! Returns square of Confusion.
        //! Created for speed and convenience.
        public static double SquareConfusion() { return Confusion() * Confusion(); }


        //! Returns the precision value in real space, frequently used
        //! by approximation algorithms.
        //! This function provides an acceptable level of precision for
        //! an approximation process to define adjustment limits.
        //! The tolerance of approximation is designed to ensure
        //! an acceptable computation time when performing an
        //! approximation process. That is why the tolerance of
        //! approximation is greater than the tolerance of confusion.
        //! The tolerance of approximation is equal to :
        //! Precision::Confusion() * 10.
        //! (that is, 1.e-6).
        //! You may use a smaller tolerance in an approximation
        //! algorithm, but this option might be costly.
        public static double Approximation() { return Confusion() * 10.0; }
    }
}
