using System;

namespace OCCPort
{
    public static class Standard_Real
    {//-------------------------------------------------------------------
     // RealLast : Returns the maximum value of a real
     //-------------------------------------------------------------------
        public static double RealLast()
        { return double.MaxValue; }

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
     static    double DBL_MAX = 1.7976931348623158e+308;// max value

        internal static object RealFirst()
        {
            return -DBL_MAX;
        }
    }

}