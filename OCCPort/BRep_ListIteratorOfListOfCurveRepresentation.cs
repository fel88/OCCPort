using System.Collections.Generic;

namespace OCCPort
{
    internal class BRep_ListIteratorOfListOfCurveRepresentation
    {

        BRep_ListOfCurveRepresentation list;
        public BRep_ListIteratorOfListOfCurveRepresentation(BRep_ListOfCurveRepresentation lcr)
        {
            list = lcr;
        }

        internal bool More()
        {
            return index < list.Count ;
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
