using System;
using System.Collections.Generic;

namespace OCCPort
{
    internal class AppParCurves_SequenceOfMultiCurve : List<AppParCurves_MultiCurve>
    {
        internal void Append(AppParCurves_MultiCurve keptMultiCurve)
        {
            Add(keptMultiCurve);
        }
    }
}