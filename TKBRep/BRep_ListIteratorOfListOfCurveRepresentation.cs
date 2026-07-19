using System.Collections.Generic;
using System.Reflection.Metadata;
using TKernel;

namespace OCCPort
{
    public class BRep_ListIteratorOfListOfCurveRepresentation : NCollection_List<BRep_CurveRepresentation>.Iterator
    {
        public BRep_ListIteratorOfListOfCurveRepresentation(NCollection_List<BRep_CurveRepresentation> aDisplayedObjects) : base(aDisplayedObjects)
        {
        }
    }
}

