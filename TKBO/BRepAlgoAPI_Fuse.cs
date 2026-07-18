using TKBRep;
using TKernel;

namespace TKBO
{

    //! The class provides Boolean fusion operation
    //! between arguments and tools  (Boolean Union).
    public class BRepAlgoAPI_Fuse : BRepAlgoAPI_BooleanOperation
    {
        //! Constructor with two shapes
        //! <S1>  -argument
        //! <S2>  -tool
        //! <anOperation> - the type of the operation
        //! Obsolete
        public BRepAlgoAPI_Fuse(TopoDS_Shape S1, TopoDS_Shape S2,
                                    Message_ProgressRange theRange = null) : base(S1, S2, BOPAlgo_Operation.BOPAlgo_FUSE)
        {
            if (theRange == null)
            {
                theRange = new Message_ProgressRange();
            }
            Build(theRange);

        }

    }
}