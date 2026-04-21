using System;
using System.Collections.Generic;

namespace OCCPort
{
    internal class DMapOfIntegerListOfInteger
    //typedef NCollection_Shared<NCollection_DataMap<Standard_Integer, ListOfInteger> >                             DMapOfIntegerListOfInteger;
    {
        public Dictionary<int, ListOfInteger> map = new Dictionary<int, ListOfInteger>();
        internal void Bind(int aNodeId, ListOfInteger value)
        {
            map.Add(aNodeId, value);
        }

        internal bool IsBound(int aNodeId)
        {
            return map.ContainsKey(aNodeId);
        }
    }
}