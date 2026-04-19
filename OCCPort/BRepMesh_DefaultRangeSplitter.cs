using OCCPort;

namespace OCCPort
{
    //! Default tool to define range of discrete face model and 
    //! obtain grid points distributed within this range.
    public class BRepMesh_DefaultRangeSplitter : AbstractRangeSplitter
    {
        (double, double) myRangeU;
        (double, double) myRangeV;
        (double, double) myDelta;
        (double, double) myTolerance;
        bool myIsValid;

        //! Returns U range.
        public  override (double, double) GetRangeU()
        {
            return myRangeU;
        }

        //! Returns V range.
        public override (double, double) GetRangeV()
        {
            return myRangeV;
        }

        //! Returns delta.
        public override  (double, double) GetDelta()
        {
            return myDelta;
        }
    }
}