namespace TKernel
{
    public interface IInspector
    {
        NCollection_CellFilter_Action Inspect(int theTargetIndex);
    }
}