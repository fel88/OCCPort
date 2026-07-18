namespace TKBO
{
    //! The class BOPDS_DS provides the control
    //! of data structure for the algorithms in the
    //! Boolean Component such as General Fuse, Boolean operations,
    //! Section, Maker Volume, Splitter and Cells Builder.<br>
    //!
    //! The data structure has the  following contents:<br>
    //! 1. the arguments of an operation [myArguments];<br>
    //! 2  the information about arguments/new shapes
    //! and their sub-shapes (type of the shape,
    //! bounding box, etc) [myLines];<br>
    //! 3. each argument shape(and its subshapes)
    //! has/have own range of indices (rank);<br>
    //! 4. pave blocks on source edges [myPaveBlocksPool];<br>
    //! 5. the state of source faces  [myFaceInfoPool];<br>
    //! 6  the collection of same domain shapes [myShapesSD];<br>
    //! 7  the collection of interferences  [myInterfTB,
    //! myInterfVV,..myInterfFF]
    public class BOPDS_DS
    {
    }
    }