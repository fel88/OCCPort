namespace TKG3d
{
    //! This package gives resources for Topology oriented
    //! applications such as : Topological Data Structure,
    //! Topological Algorithms.
    //!
    //! It contains :
    //!
    //! * The ShapeEnum   enumeration  to  describe  the
    //! different topological shapes.
    //!
    //! * The  Orientation  enumeration to  describe the
    //! orientation of a topological shape.
    //!
    //! * The  State    enumeration  to  describes  the
    //! position of a point relative to a Shape.
    //!
    //! * Methods to manage the enumerations.

    public class TopAbs
    {

        public static TopAbs_Orientation Reverse(TopAbs_Orientation Ori)
        {
            TopAbs_Orientation[] TopAbs_Table_Reverse =
           {
    TopAbs_Orientation.TopAbs_REVERSED, TopAbs_Orientation.TopAbs_FORWARD, TopAbs_Orientation.TopAbs_INTERNAL, TopAbs_Orientation.TopAbs_EXTERNAL
  };
            return TopAbs_Table_Reverse[(int)Ori];
        }

        public static TopAbs_Orientation Compose(TopAbs_Orientation O1, TopAbs_Orientation O2)
        {
            // see the composition table in the file TopAbs.cdl
            TopAbs_Orientation[,] TopAbs_Table_Compose = new TopAbs_Orientation[4, 4]
{
                { TopAbs_Orientation. TopAbs_FORWARD,  TopAbs_Orientation. TopAbs_REVERSED,  TopAbs_Orientation.TopAbs_INTERNAL, TopAbs_Orientation. TopAbs_EXTERNAL },
    { TopAbs_Orientation. TopAbs_REVERSED, TopAbs_Orientation. TopAbs_FORWARD,  TopAbs_Orientation. TopAbs_INTERNAL, TopAbs_Orientation. TopAbs_EXTERNAL },
    { TopAbs_Orientation. TopAbs_INTERNAL, TopAbs_Orientation. TopAbs_INTERNAL, TopAbs_Orientation. TopAbs_INTERNAL, TopAbs_Orientation. TopAbs_INTERNAL },
    { TopAbs_Orientation. TopAbs_EXTERNAL, TopAbs_Orientation. TopAbs_EXTERNAL, TopAbs_Orientation. TopAbs_EXTERNAL, TopAbs_Orientation. TopAbs_EXTERNAL }
          };
            return TopAbs_Table_Compose[(int)O2, (int)O1];
        }
    }
}
