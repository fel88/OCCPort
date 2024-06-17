using System;

namespace OCCPort
{
    internal class Precision
    {
        public static double Confusion() { return 1e-7; }

        //! Returns a  big number that  can  be  considered as
        //! infinite. Use -Infinite() for a negative big number.
        public static double Infinite() { return 2e+100; }


        //! Returns True if R may be considered as an infinite
        //! number. Currently Abs(R) > 1e100
        public static bool IsInfinite(double R) { return Math.Abs(R) >= (0.5 * Precision.Infinite()); }

    }
}