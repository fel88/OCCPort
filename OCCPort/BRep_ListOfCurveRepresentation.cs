using System.Collections.Generic;

namespace OCCPort
{
    public class BRep_ListOfCurveRepresentation : List<BRep_CurveRepresentation>
    {
        
        internal void Append(BRep_CurveRepresentation c3d)
        {
            Add(c3d);
        }

    }
}
