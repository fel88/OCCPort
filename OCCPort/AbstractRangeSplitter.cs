using OCCPort.Interfaces;

namespace OCCPort
{
    public abstract class AbstractRangeSplitter// originally was template parameter
    {
        public abstract (double, double) GetRangeU();
        //! Returns True if computed range is valid.
        public abstract bool IsValid();

        //! Returns V range.
        public abstract (double, double) GetRangeV();
        public abstract gp_Pnt Point(gp_Pnt2d thePoint2d);
        public abstract void AdjustRange();
        public abstract (double, double) GetToleranceUV();
        public abstract void AddPoint(gp_Pnt2d thePoint);

        //! Returns delta.
        public abstract (double, double) GetDelta();
        public abstract void Reset(IMeshData_Face theDFace,
                                    IMeshTools_Parameters theParameters);
        public abstract ListOfPnt2d GenerateSurfaceNodes(IMeshTools_Parameters theParameters);
    }
}