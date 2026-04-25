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
    }
}