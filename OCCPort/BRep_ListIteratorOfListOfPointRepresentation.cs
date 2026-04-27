using System;
using System.Collections.Generic;

namespace OCCPort
{
    public class BRep_ListIteratorOfListOfPointRepresentation : List<BRep_PointRepresentation>
    {
        public BRep_ListIteratorOfListOfPointRepresentation(BRep_ListOfPointRepresentation lpr)
        {
        }

        internal bool More()
        {
            throw new NotImplementedException();
        }

        internal void Next()
        {
            throw new NotImplementedException();
        }

        internal BRep_PointRepresentation Value()
        {
            throw new NotImplementedException();
        }
    }
}

