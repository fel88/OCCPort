using System;
using System.Collections.Generic;

namespace OCCPort
{
    public class AIS_DataMapOfIOStatus : Dictionary<AIS_InteractiveObject, AIS_GlobalStatus>
    {
        internal AIS_GlobalStatus ChangeSeek(AIS_InteractiveObject theIObj)
        {
            throw new NotImplementedException();
        }

        internal bool IsBound(AIS_InteractiveObject theIObj)
        {
            return ContainsKey(theIObj);
        }
    }
}