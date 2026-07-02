namespace OCCPort.Tester
{
    public interface IOCCTProxyInterface
    {
        void ActivateGrid(bool v);
        void RedrawView();
        void runOpenTk(nint windowPtr, nint value);
        void SetDisplayMode(int v);
        void SetMaterial(int v);
        void UpdateCurrentViewer();
        void UpdateView();
        void iterate();

    }
}
