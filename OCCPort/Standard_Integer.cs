namespace OCCPort
{
    public static class Standard_Integer
    {
        // ------------------------------------------------------------------
        // IntegerLast : Returns the maximum value of an integer
        // ------------------------------------------------------------------
        public static int IntegerLast()
        { return int.MaxValue; }
        // ------------------------------------------------------------------
        // IntegerFirst : Returns the minimum value of an integer
        // ------------------------------------------------------------------
        public static int IntegerFirst()
        { return int.MinValue; }

    }

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

    }

}