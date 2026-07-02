namespace OCCPort.Common
{
    public static class Standard_ShortReal
    {

        //-------------------------------------------------------------------
        // ShortRealEpsilon : Returns the minimum positive ShortReal such that 
        //               1.0 + x is not equal to 1.0
        //-------------------------------------------------------------------
        public  static  float     ShortRealEpsilon()
        { return FLT_EPSILON; }
        const float FLT_EPSILON = 1.192092896e-07F;       // smallest such that 1.0+FLT_EPSILON != 1.0

    }
}
