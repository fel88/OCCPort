using System;
using System.Collections.Generic;

namespace OCCPort
{
    internal class VectorOfIPCurveHandles : List<IPCurveHandle>
    {
        internal IPCurveHandle get(int v)
        {
            return this[v];
        }
    }
}