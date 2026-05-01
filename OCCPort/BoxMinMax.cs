using System;

namespace OCCPort
{
    public class BoxMinMax
    {

        public static void CwiseMin(BVH_VecNt theVec1, BVH_VecNt theVec2)
        {
            theVec1.X = Math.Min(theVec1.X, theVec2.X);
            theVec1.Y = Math.Min(theVec1.Y, theVec2.Y);
            theVec1.Z = Math.Min(theVec1.Z, theVec2.Z);
        }

        public static void CwiseMax(BVH_VecNt theVec1, BVH_VecNt theVec2)
        {
            theVec1.X = Math.Max(theVec1.X, theVec2.X);
            theVec1.Y = Math.Max(theVec1.Y, theVec2.Y);
            theVec1.Z = Math.Max(theVec1.Z, theVec2.Z);
        }
    }
}
