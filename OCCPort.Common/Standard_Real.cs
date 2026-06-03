namespace OCCPort.Common
{
    public static class Standard_Real
    {


        public static double Epsilon(double Value)
        {
            double aEpsilon;

            if (Value >= 0.0)
            {
                aEpsilon = NextAfter(Value, RealLast()) - Value;
            }
            else
            {
                aEpsilon = Value - NextAfter(Value, RealFirst());
            }
            return aEpsilon;
        }

        private static double NextAfter(double value, double v)
        {
            return Math.BitIncrement(value);
        }


        //-------------------------------------------------------------------
        // RealLast : Returns the maximum value of a real
        //-------------------------------------------------------------------
        public static double RealLast()
        { return double.MaxValue; }

        //-------------------------------------------------------------------
        public static double IntegerPart(double Value)
        { return ((Value > 0) ? Math.Floor(Value) : Math.Ceiling(Value)); }

        public static float RealToShortReal(double theVal)
        {
            var FLT_MAX = float.MaxValue;
            var FLT_MIN = float.MinValue;

            return theVal < -FLT_MAX ? -FLT_MAX
    : theVal > FLT_MAX ? FLT_MAX
    : (float)theVal;

        }
        public static double RealEpsilon()
        { return double.Epsilon; }

        internal static double RealSmall()
        {
            return double.MinValue;
        }
        static double DBL_MAX = 1.7976931348623158e+308;// max value

        public static double RealFirst()
        {
            return -DBL_MAX;
        }

        internal static bool IsEqual(double Value1, double Value2)
        {
            return Math.Abs((Value1 - Value2)) < RealSmall();
        }
    }
}
