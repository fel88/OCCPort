using System;
using System.Collections.Generic;

namespace OCCPort
{
    internal class TColStd_SequenceOfReal : List<double>
    {
        internal void Append(double keptT3d)
        {
            Add(keptT3d);
        }

        internal void ChangeValue(int i, double theParam)
        {
            this[i] = theParam;
        }

        internal void InsertBefore(int i, double theParam)
        {
            Insert(i, theParam);
        }
    }
}