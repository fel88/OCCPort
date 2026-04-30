namespace OCCPort
{
    //! if N is the normal
    //!
    //! InfinityOfSolutions : ||DN/du||>Resolution, ||DN/dv||>Resolution
    //!
    //! D1NuIsNull          : ||DN/du|| <= Resolution
    //!
    //! D1NvIsNull          : ||DN/dv|| <= Resolution
    //!
    //! D1NIsNull           : ||DN/du||<=Resolution, ||DN/dv||<=Resolution
    //!
    //! D1NuNvRatioIsNull   : ||D1Nu|| / ||D1Nv|| <= RealEpsilon
    //!
    //! D1NvNuRatioIsNull   : ||D1Nu|| / ||D1Nv|| <= RealEpsilon
    //!
    //! D1NuIsParallelD1Nv  : The angle between D1Nu and D1Nv is Null.
   public  enum CSLib_NormalStatus
    {
        CSLib_Singular,
        CSLib_Defined,
        CSLib_InfinityOfSolutions,
        CSLib_D1NuIsNull,
        CSLib_D1NvIsNull,
        CSLib_D1NIsNull,
        CSLib_D1NuNvRatioIsNull,
        CSLib_D1NvNuRatioIsNull,
        CSLib_D1NuIsParallelD1Nv
    };
}