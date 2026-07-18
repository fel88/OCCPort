using TKBRep;
using TKernel;

namespace TKBO
{
    //! The class contains API level of the General Fuse algorithm.<br>
    //!
    //! Additionally to the options defined in the base class, the algorithm has
    //! the following options:<br>
    //! - *Safe processing mode* - allows to avoid modification of the input
    //!                            shapes during the operation (by default it is off);
    //! - *Gluing options* - allows to speed up the calculation of the intersections
    //!                      on the special cases, in which some sub-shapes are coinciding.
    //! - *Disabling the check for inverted solids* - Disables/Enables the check of the input solids
    //!                          for inverted status (holes in the space). The default value is TRUE,
    //!                          i.e. the check is performed. Setting this flag to FALSE for inverted solids,
    //!                          most likely will lead to incorrect results.
    //! - *Disabling history collection* - allows disabling the collection of the history
    //!                                    of shapes modifications during the operation.
    //!
    //! It returns the following Error statuses:<br>
    //! - 0 - in case of success;<br>
    //! - *BOPAlgo_AlertTooFewArguments* - in case there are no enough arguments to perform the operation;<br>
    //! - *BOPAlgo_AlertIntersectionFailed* - in case the intersection of the arguments has failed;<br>
    //! - *BOPAlgo_AlertBuilderFailed* - in case building of the result shape has failed.<br>
    //!
    //! Warnings statuses from underlying DS Filler and Builder algorithms
    //! are collected in the report.
    //!
    //! The class provides possibility to simplify the resulting shape by unification
    //! of the tangential edges and faces. It is performed by the method *SimplifyResult*.
    //! See description of this method for more details.
    //!
    public class BRepAlgoAPI_BuilderAlgo : BRepAlgoAPI_Algo
    {
        public void Clear()
        {
            //base.Clear();
            //  if (myDSFiller && myIsIntersectionNeeded)
            {
                //  delete myDSFiller;
                //myDSFiller = null;
            }
            if (myBuilder != null)
            {
                //delete myBuilder;
                myBuilder = null;
            }
            //if (myHistory != null)
            //    myHistory = null;

            if (mySimplifierHistory != null)
                mySimplifierHistory = null;
        }
        //=======================================================================
        //function : IntersectShapes
        //purpose  : Intersects the given shapes with the intersection tool
        //=======================================================================
        public void IntersectShapes(TopTools_ListOfShape theArgs, Message_ProgressRange theRange)
        {
            if (!myIsIntersectionNeeded)
                return;

            //if (myDSFiller)
               // delete myDSFiller;

            // Create new Filler
            myDSFiller = new BOPAlgo_PaveFiller();
            // Set arguments for intersection
            //myDSFiller.SetArguments(theArgs);
            //// Set options for intersection
            //myDSFiller.SetRunParallel(myRunParallel);

            //myDSFiller.SetFuzzyValue(myFuzzyValue);
            //myDSFiller.SetNonDestructive(myNonDestructive);
            //myDSFiller.SetGlue(myGlue);
            //myDSFiller.SetUseOBB(myUseOBB);
            //// Set Face/Face intersection options to the intersection algorithm
            //SetAttributes();
            //// Perform intersection
            //myDSFiller.Perform(theRange);
            //// Check for the errors during intersection
            //GetReport()->Merge(myDSFiller.GetReport());
        }

        // Tools
        protected bool myIsIntersectionNeeded; //!< Flag to control whether the intersection
                                               //! of arguments should be performed or not
        protected BOPAlgo_PaveFiller myDSFiller;          //!< Intersection tool performs intersection of the
                                                          //! argument shapes.
        protected BRepTools_History mySimplifierHistory; //!< History of result shape simplification
        protected BOPAlgo_Builder myBuilder;             //!< Building tool performs construction of the result
                                                         //! basing on the results of intersection
                                                         // Inputs
        protected TopTools_ListOfShape myArguments = new TopTools_ListOfShape(); //!< Arguments of the operation

    }
}