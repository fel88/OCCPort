using System;
using System.Numerics;

namespace OCCPort
{
    //! Tool class for calculate component-wise vector minimum
    //! and maximum (optimized version).
    //! \tparam T Numeric data type
    //! \tparam N Vector dimension
    public interface IBoxMinMax<BVH_VecNt>  
    {

        BVH_VecNt CwiseMin(BVH_VecNt theVec1, BVH_VecNt theVec2);
        void CwiseMin(ref BVH_VecNt theVec1, BVH_VecNt theVec2);
        //{

        //    theVec1.X = Math.Min(theVec1.X, theVec2.X);
        //    theVec1.Y = Math.Min(theVec1.Y, theVec2.Y);
        //    theVec1.Z = Math.Min(theVec1.Z, theVec2.Z);
        //}

        BVH_VecNt CwiseMax(BVH_VecNt theVec1, BVH_VecNt theVec2);
        void CwiseMax(ref BVH_VecNt theVec1, BVH_VecNt theVec2);
        //{
        //    theVec1.X = Math.Max(theVec1.X, theVec2.X);
        //    theVec1.Y = Math.Max(theVec1.Y, theVec2.Y);
        //    theVec1.Z = Math.Max(theVec1.Z, theVec2.Z);
        //}
    }
}
