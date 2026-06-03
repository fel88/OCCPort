using System.Collections.Generic;
using TKMath;

namespace OCCPort
{
    internal class TColgp_SequenceOfPnt2d : List<gp_Pnt2d>
    {
        public void Append(gp_Pnt2d item)
        {
            Add(item);
        }
    }
}