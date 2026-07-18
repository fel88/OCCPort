using OCCPort;
using TKBRep;
using TKernel;

namespace TKBO
{
    //!
    //! The class represents the Intersection phase of the
    //! Boolean Operations algorithm.<br>
    //! It performs the pairwise intersection of the sub-shapes of
    //! the arguments in the following order:<br>
    //! 1. Vertex/Vertex;<br>
    //! 2. Vertex/Edge;<br>
    //! 3. Edge/Edge;<br>
    //! 4. Vertex/Face;<br>
    //! 5. Edge/Face;<br>
    //! 6. Face/Face.<br>
    //!
    //! The results of intersection are stored into the Data Structure
    //! of the algorithm.<br>
    //!
    //! Additionally to the options provided by the parent class,
    //! the algorithm has the following options:<br>
    //! - *Section attributes* - allows to customize the intersection of the faces
    //!                          (avoid approximation or building 2d curves);<br>
    //! - *Safe processing mode* - allows to avoid modification of the input
    //!                            shapes during the operation (by default it is off);<br>
    //! - *Gluing options* - allows to speed up the calculation on the special
    //!                      cases, in which some sub-shapes are coincide.<br>
    //!
    //! The algorithm returns the following Warning statuses:
    //! - *BOPAlgo_AlertSelfInterferingShape* - in case some of the argument shapes are self-interfering shapes;
    //! - *BOPAlgo_AlertTooSmallEdge* - in case some edges of the input shapes have no valid range;
    //! - *BOPAlgo_AlertNotSplittableEdge* - in case some edges of the input shapes has such a small
    //!                                      valid range so it cannot be split;
    //! - *BOPAlgo_AlertBadPositioning* - in case the positioning of the input shapes leads to creation
    //!                                   of small edges;
    //! - *BOPAlgo_AlertIntersectionOfPairOfShapesFailed* - in case intersection of some of the
    //!                                                     sub-shapes has failed;
    //! - *BOPAlgo_AlertAcquiredSelfIntersection* - in case some sub-shapes of the argument become connected
    //!                                             through other shapes;
    //! - *BOPAlgo_AlertBuildingPCurveFailed* - in case building 2D curve for some of the edges
    //!                                         on the faces has failed.
    //!
    //! The algorithm returns the following Error alerts:
    //! - *BOPAlgo_AlertTooFewArguments* - in case there are no enough arguments to
    //!                      perform the operation;<br>
    //! - *BOPAlgo_AlertIntersectionFailed* - in case some unexpected error occurred;<br>
    //! - *BOPAlgo_AlertNullInputShapes* - in case some of the arguments are null shapes.<br>
    //!
    public class BOPAlgo_PaveFiller : BOPAlgo_Algo
    {
        void Init(Message_ProgressRange theRange)
        {
            //if (!myArguments.Extent())
            //{
            //    AddError(new BOPAlgo_AlertTooFewArguments);
            //    return;
            //}
            ////
            //Message_ProgressScope aPS(theRange, "Initialization of Intersection algorithm", 1);
            //TopTools_ListIteratorOfListOfShape aIt = new TopTools_ListIteratorOfListOfShape(myArguments);
            //for (; aIt.More(); aIt.Next())
            //{
            //    if (aIt.Value().IsNull())
            //    {
            //        AddError(new BOPAlgo_AlertNullInputShapes);
            //        return;
            //    }
            //}
            ////
            //// 0 Clear
            //Clear();
            ////
            //// 1.myDS 
            //myDS = new BOPDS_DS(myAllocator);
            //myDS.SetArguments(myArguments);
            //myDS.Init(myFuzzyValue);
            ////
            //// 2 myContext
            //myContext = new IntTools_Context;
            ////
            //// 3.myIterator 
            //myIterator = new BOPDS_Iterator(myAllocator);
            //myIterator->SetRunParallel(myRunParallel);
            //myIterator->SetDS(myDS);
            //myIterator->Prepare(myContext, myUseOBB, myFuzzyValue);
            ////
            // 4 NonDestructive flag
            SetNonDestructive();
        }

        BOPDS_DS myDS;
        void SetNonDestructive()
        {
            if (!myIsPrimary || myNonDestructive)
            {
                return;
            }
            //
            bool bFlag;
            TopTools_ListIteratorOfListOfShape aItLS = new TopTools_ListIteratorOfListOfShape();
            //
            bFlag = false;
            aItLS.Initialize(myArguments);
            for (; aItLS.More() && (!bFlag); aItLS.Next())
            {
                TopoDS_Shape aS = aItLS.Value();
                bFlag = aS.Locked();
            }
            myNonDestructive = bFlag;
        }
        TopTools_ListOfShape myArguments;
        //   BOPDS_PDS myDS;
        //   BOPDS_PIterator myIterator;
        //   IntTools_Context myContext;
        //BOPAlgo_SectionAttribute mySectionAttribute;
        bool myNonDestructive;
        bool myIsPrimary;
        bool myAvoidBuildPCurve;
        BOPAlgo_GlueEnum myGlue;
        void PerformInternal(Message_ProgressRange theRange)
        {
            Message_ProgressScope aPS=new (theRange, "Performing intersection of shapes", 100);

            //Init(aPS.Next(5));
            //if (HasErrors())
            //{
            //    return;
            //}

            //// Compute steps of the PI
            //BOPAlgo_PISteps aSteps(PIOperation_Last);
            //analyzeProgress(95, aSteps);
            ////
            //Prepare(aPS.Next(aSteps.GetStep(PIOperation_Prepare)));
            //if (HasErrors())
            //{
            //    return;
            //}
            //// 00
            //PerformVV(aPS.Next(aSteps.GetStep(PIOperation_PerformVV)));
            //if (HasErrors())
            //{
            //    return;
            //}
            //// 01
            //PerformVE(aPS.Next(aSteps.GetStep(PIOperation_PerformVE)));
            //if (HasErrors())
            //{
            //    return;
            //}
            ////
            //UpdatePaveBlocksWithSDVertices();
            //// 11
            //PerformEE(aPS.Next(aSteps.GetStep(PIOperation_PerformEE)));
            //if (HasErrors())
            //{
            //    return;
            //}
            //UpdatePaveBlocksWithSDVertices();
            //// 02
            //PerformVF(aPS.Next(aSteps.GetStep(PIOperation_PerformVF)));
            //if (HasErrors())
            //{
            //    return;
            //}
            //UpdatePaveBlocksWithSDVertices();
            //// 12
            //PerformEF(aPS.Next(aSteps.GetStep(PIOperation_PerformEF)));
            //if (HasErrors())
            //{
            //    return;
            //}
            //UpdatePaveBlocksWithSDVertices();
            //UpdateInterfsWithSDVertices();

            //// Repeat Intersection with increased vertices
            //RepeatIntersection(aPS.Next(aSteps.GetStep(PIOperation_RepeatIntersection)));
            //if (HasErrors())
            //    return;
            //// Force intersection of edges after increase
            //// of the tolerance values of their vertices
            //ForceInterfEE(aPS.Next(aSteps.GetStep(PIOperation_ForceInterfEE)));
            //if (HasErrors())
            //{
            //    return;
            //}
            //// Force Edge/Face intersection after increase
            //// of the tolerance values of their vertices
            //ForceInterfEF(aPS.Next(aSteps.GetStep(PIOperation_ForceInterfEF)));
            //if (HasErrors())
            //{
            //    return;
            //}
            ////
            //// 22
            //PerformFF(aPS.Next(aSteps.GetStep(PIOperation_PerformFF)));
            //if (HasErrors())
            //{
            //    return;
            //}
            ////
            //UpdateBlocksWithSharedVertices();
            ////
            //myDS.RefineFaceInfoIn();
            ////
            //MakeSplitEdges(aPS.Next(aSteps.GetStep(PIOperation_MakeSplitEdges)));
            //if (HasErrors())
            //{
            //    return;
            //}
            ////
            //UpdatePaveBlocksWithSDVertices();
            ////
            //MakeBlocks(aPS.Next(aSteps.GetStep(PIOperation_MakeBlocks)));
            //if (HasErrors())
            //{
            //    return;
            //}
            ////
            //CheckSelfInterference();
            ////
            //UpdateInterfsWithSDVertices();
            //myDS->ReleasePaveBlocks();
            //myDS->RefineFaceInfoOn();
            ////
            //RemoveMicroEdges();
            ////
            //MakePCurves(aPS.Next(aSteps.GetStep(PIOperation_MakePCurves)));
            //if (HasErrors())
            //{
            //    return;
            //}
            ////
            //ProcessDE(aPS.Next(aSteps.GetStep(PIOperation_ProcessDE)));
            //if (HasErrors())
            //{
            //    return;
            //}
        }


    }
}