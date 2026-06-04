using System.Collections.Generic;

namespace OCCPort
{
    public class BRep_ListIteratorOfListOfCurveRepresentation
    {

        BRep_ListOfCurveRepresentation list;
        public BRep_ListIteratorOfListOfCurveRepresentation(BRep_ListOfCurveRepresentation lcr)
        {
            list = lcr;
        }

        public bool More()
        {
            return index < list.Count ;
        }

        public void Next()
        {
            index++;
        }
        int index = 0;
        public BRep_CurveRepresentation Value()
        {
            return list[index];
        }
    }
}

