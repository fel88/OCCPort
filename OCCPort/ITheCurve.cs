namespace OCCPort
{
    public interface ITheCurve
    {
        int NbIntervals(GeomAbs_Shape geomAbs_CN);
        GeomAbs_CurveType _GetType();
    }
}