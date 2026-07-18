using OCCPort.Common;
using TKTopAlgo;

namespace TKBO
{
    //! Provides the root interface for the API algorithms
    public class BRepAlgoAPI_Algo : BRepBuilderAPI_MakeShape
    //protected BOPAlgo_Options
    {
        [NotOrigin]
        public BOPAlgo_Options Options;
    }
}