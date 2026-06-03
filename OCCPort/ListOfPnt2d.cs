using System.Collections.Generic;
using TKMath;

namespace OCCPort
{
    public class ListOfPnt2d : List<gp_Pnt2d>
    {
        public bool IsEmpty()
        {
            return Count == 0;
        }
        public int Size()
        {
            return Count ;
        }

    }
}