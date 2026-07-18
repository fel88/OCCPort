using TKBRep;
using TKernel;

namespace TKBO
{
    //! The root API class for performing Boolean Operations on arbitrary shapes.
    //!
    //! The arguments of the operation are divided in two groups - *Objects* and *Tools*.
    //! Each group can contain any number of shapes, but each shape should be valid
    //! in terms of *BRepCheck_Analyzer* and *BOPAlgo_ArgumentAnalyzer*.
    //! The algorithm builds the splits of the given arguments using the intersection
    //! results and combines the result of Boolean Operation of given type:
    //! - *FUSE* - union of two groups of objects;
    //! - *COMMON* - intersection of two groups of objects;
    //! - *CUT* - subtraction of one group from the other;
    //! - *SECTION* - section edges and vertices of all arguments;
    //!
    //! The rules for the arguments and type of the operation are the following:
    //! - For Boolean operation *FUSE* all arguments should have equal dimensions;
    //! - For Boolean operation *CUT* the minimal dimension of *Tools* should not be
    //!   less than the maximal dimension of *Objects*;
    //! - For Boolean operation *COMMON* the arguments can have any dimension.
    //! - For Boolean operation *SECTION* the arguments can be of any type.
    //!
    //! Additionally to the errors of the base class the algorithm returns
    //! the following Errors:<br>
    //! - *BOPAlgo_AlertBOPNotSet* - in case the type of Boolean Operation is not set.<br>
    public class BRepAlgoAPI_BooleanOperation : BRepAlgoAPI_BuilderAlgo
    {
        public BRepAlgoAPI_BooleanOperation
  (TopoDS_Shape theS1,
    TopoDS_Shape theS2,
    BOPAlgo_Operation theOp)
        {
            myOperation = (theOp);
            myArguments.Append(theS1);
            myTools.Append(theS2);
        }

        public override void Build(Message_ProgressRange theRange = null)
        {
            // Set Not Done status by default
            NotDone();
            // Clear from previous runs
            Clear();
            // Check for availability of arguments and tools
            // Both should be present
            //if (myArguments.IsEmpty() || myTools.IsEmpty())
            //{
            //    AddError(new BOPAlgo_AlertTooFewArguments);
            //    return;
            //}
            //// Check if the operation is set
            //if (myOperation ==BOPAlgo_Operation. BOPAlgo_UNKNOWN)
            //{
            //    AddError(new BOPAlgo_AlertBOPNotSet);
            //    return;
            //}

            //// DEBUG option for dumping shapes and scripts
            //BRepAlgoAPI_DumpOper aDumpOper;
            //{
            //    if (aDumpOper.IsDump())
            //    {
            //        BRepAlgoAPI_Check aChekArgs(myArguments.First(), myTools.First(), myOperation);
            //        aDumpOper.SetIsDumpArgs(!aChekArgs.IsValid());
            //    }
            //}

            //string aPSName = string.Empty;
            //switch (myOperation)
            //{
            //    case BOPAlgo_Operation.BOPAlgo_COMMON:
            //        aPSName = "Performing COMMON operation";
            //        break;
            //    case BOPAlgo_Operation.BOPAlgo_FUSE:
            //        aPSName = "Performing FUSE operation";
            //        break;
            //    case BOPAlgo_Operation.BOPAlgo_CUT:
            //    case BOPAlgo_Operation.BOPAlgo_CUT21:
            //        aPSName = "Performing CUT operation";
            //        break;
            //    case BOPAlgo_Operation.BOPAlgo_SECTION:
            //        aPSName = "Performing SECTION operation";
            //        break;
            //    default:
            //        return;
            //}

            //Message_ProgressScope aPS(theRange, aPSName, myIsIntersectionNeeded? 100 : 30);
            //// If necessary perform intersection of the argument shapes
            //if (myIsIntersectionNeeded)
            //{
            //    // Combine Objects and Tools into a single list for intersection
            //    TopTools_ListOfShape aLArgs = myArguments;
            //    for (TopTools_ListOfShape::Iterator it(myTools); it.More(); it.Next())
            //        aLArgs.Append(it.Value());

            //    // Perform intersection
            //    IntersectShapes(aLArgs, aPS.Next(70));
            //    if (HasErrors())
            //    {
            //        if (aDumpOper.IsDump())
            //        {
            //            aDumpOper.SetIsDumpRes(Standard_False);
            //            aDumpOper.Dump(myArguments.First(), myTools.First(), TopoDS_Shape(), myOperation);
            //        }
            //        return;
            //    }
            //}

            //// Builder Initialization
            //if (myOperation == BOPAlgo_Operation.BOPAlgo_SECTION)
            //{
            //    myBuilder = new BOPAlgo_Section(myAllocator);
            //    myBuilder->SetArguments(myDSFiller->Arguments());
            //}
            //else
            //{
            //    myBuilder = new BOPAlgo_BOP(myAllocator);
            //    myBuilder->SetArguments(myArguments);
            //    ((BOPAlgo_BOP*)myBuilder)->SetTools(myTools);
            //    ((BOPAlgo_BOP*)myBuilder)->SetOperation(myOperation);
            //}

            //// Build the result
            //BuildResult(aPS.Next(30));
            //if (HasErrors())
            //{
            //    return;
            //}

            //if (aDumpOper.IsDump())
            //{
            //    bool isDumpRes = myShape.IsNull() ||
            //                                 !BRepAlgoAPI_Check(myShape).IsValid();
            //    aDumpOper.SetIsDumpRes(isDumpRes);
            //    aDumpOper.Dump(myArguments.First(), myTools.First(), myShape, myOperation);
            //}
        }

        protected TopTools_ListOfShape myTools=new TopTools_ListOfShape ();  //!< Tool arguments of operation
        protected BOPAlgo_Operation myOperation; //!< Type of Boolean Operation
    }
}