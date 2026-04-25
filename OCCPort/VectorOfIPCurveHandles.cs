using System;
using System.Collections.Generic;

namespace OCCPort
{
    internal class VectorOfIPCurveHandles : List<IPCurveHandle>
    {
        public VectorOfIPCurveHandles(int capacity) : base(capacity)
        {
        }

        internal IPCurveHandle get(int v)
        {
            return this[v];
        }
    }
}