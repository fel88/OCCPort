using TKMath;

namespace TKMesh
{
    public abstract class AbstractRangeSplitter// originally was template parameter
    {
        public abstract (double, double) GetRangeU();
        //! Returns True if computed range is valid.
        public abstract bool IsValid();
        //! Scales the given point from real parametric space 
        //! to face basis and otherwise.
        //! @param thePoint point to be scaled.
        //! @param isToFaceBasis if TRUE converts point to face basis,
        //! otherwise performs reverse conversion.
        //! @return scaled point.
        public abstract gp_Pnt2d Scale(gp_Pnt2d thePoint,
                                 bool isToFaceBasis);

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


