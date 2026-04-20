using System.Collections.Generic;

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