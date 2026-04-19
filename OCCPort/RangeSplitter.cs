namespace OCCPort
{
    public abstract class AbstractRangeSplitter// originally was template parameter
    {
        public abstract (double, double) GetRangeU();

        //! Returns V range.
        public abstract (double, double) GetRangeV();

        //! Returns delta.
        public abstract (double, double) GetDelta();
    }
}