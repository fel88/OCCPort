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
    {
        public static double RealEpsilon()
        { return double.Epsilon; }

    }

}