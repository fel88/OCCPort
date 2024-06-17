using System.Collections.Generic;

namespace OCCPort
{
    internal class BRep_ListIteratorOfListOfCurveRepresentation
    {

        List<BRep_CurveRepresentation> list = new List<BRep_CurveRepresentation>();
        public BRep_ListIteratorOfListOfCurveRepresentation(BRep_ListOfCurveRepresentation lcr)
        {
        }

        internal bool More()
        {
            return index < list.Count - 1;
        }

        internal void Next()
        {
            index++;
        }
        int index = 0;
        internal BRep_CurveRepresentation Value()
        {
            return list[index];
        }
    }
}
