namespace TKGeomBase
{
    internal class AppParCurves_SequenceOfMultiCurve : List<AppParCurves_MultiCurve>
    {
        internal void Append(AppParCurves_MultiCurve keptMultiCurve)
        {
            Add(keptMultiCurve);
        }
    }
}
