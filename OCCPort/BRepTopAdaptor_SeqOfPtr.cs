namespace OCCPort
{
    internal class BRepTopAdaptor_SeqOfPtr: NCollection_Sequence<object>
    {
        public int Length()
        {
            return base.Count;
        }
    }
}