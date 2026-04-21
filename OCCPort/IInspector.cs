namespace OCCPort
{
    public interface IInspector
    {
        NCollection_CellFilter_Action Inspect(int theTargetIndex);
    }
}