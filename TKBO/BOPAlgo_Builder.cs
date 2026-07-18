namespace TKBO
{
    //!
    //! The class is a General Fuse algorithm - base algorithm for the
    //! algorithms in the Boolean Component. Its main purpose is to build
    //! the split parts of the argument shapes from which the result of
    //! the operations is combined.<br>
    //! The result of the General Fuse algorithm itself is a compound
    //! containing all split parts of the arguments. <br>
    //!
    //! Additionally to the options of the base classes, the algorithm has
    //! the following options:<br>
    //! - *Safe processing mode* - allows to avoid modification of the input
    //!                            shapes during the operation (by default it is off);<br>
    //! - *Gluing options* - allows to speed up the calculation of the intersections
    //!                      on the special cases, in which some sub-shapes are coinciding.<br>
    //! - *Disabling the check for inverted solids* - Disables/Enables the check of the input solids
    //!                          for inverted status (holes in the space). The default value is TRUE,
    //!                          i.e. the check is performed. Setting this flag to FALSE for inverted solids,
    //!                          most likely will lead to incorrect results.
    //!
    //! The algorithm returns the following warnings:
    //! - *BOPAlgo_AlertUnableToOrientTheShape* - in case the check on the orientation of the split shape
    //!                                           to match the orientation of the original shape has failed.
    //!
    //! The algorithm returns the following Error statuses:
    //! - *BOPAlgo_AlertTooFewArguments* - in case there are no enough arguments to perform the operation;
    //! - *BOPAlgo_AlertNoFiller* - in case the intersection tool has not been created;
    //! - *BOPAlgo_AlertIntersectionFailed* - in case the intersection of the arguments has failed;
    //! - *BOPAlgo_AlertBuilderFailed* - in case building splits of arguments has failed with some unexpected error.
    //!
    public class BOPAlgo_Builder : BOPAlgo_BuilderShape
    {
    }
}