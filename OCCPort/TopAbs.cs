using System;

namespace OCCPort
{
    internal class TopAbs
    {

        public static TopAbs_Orientation Reverse(TopAbs_Orientation Ori)
        {
            TopAbs_Orientation[] TopAbs_Table_Reverse =
           {
    TopAbs_Orientation.TopAbs_REVERSED, TopAbs_Orientation.TopAbs_FORWARD, TopAbs_Orientation.TopAbs_INTERNAL, TopAbs_Orientation.TopAbs_EXTERNAL
  };
            return TopAbs_Table_Reverse[(int)Ori];
        }

        internal static TopAbs_Orientation Compose(TopAbs_Orientation O1, TopAbs_Orientation O2)
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