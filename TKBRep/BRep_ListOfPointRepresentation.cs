using System;
using System.Collections.Generic;

namespace OCCPort
{
    public class BRep_ListOfPointRepresentation : List<BRep_PointRepresentation>
    {
        internal void Append(BRep_PointRepresentation pOCS)
        {
            Add(pOCS);
        }
    }
}