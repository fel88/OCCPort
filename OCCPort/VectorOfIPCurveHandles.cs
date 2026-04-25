using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace OCCPort
{
    internal class VectorOfIPCurveHandles
    {
        List<IMeshData_PCurve> list = new List<IMeshData_PCurve>();
        public VectorOfIPCurveHandles(int capacity) 
        {

        }

        public IMeshData_PCurve this[int key]
        {
            get => list[key ];
            set => list[key ] = value;
        }

        public void Append(IMeshData_PCurve d)
        {
            list.Add(d);
        }
        internal IMeshData_PCurve get(int v)
        {
            return this[v];
        }

        internal int Size()
        {
            return list.Count;
        }
    }
}