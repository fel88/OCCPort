namespace TKMesh
{
    public class ListOfInteger : List<int>
    {
        public ListOfInteger() { }
        public ListOfInteger(object myAllocator)
        {
        }

        public void Append(int c)
        {
            Add(c);
        }
        internal int First()
        {
            return this[0];
        }
        public bool IsEmpty()
        {
            return Count == 0;
        }

        internal void RemoveFirst()
        {
            RemoveAt(0);
        }

        internal int Last()
        {
            return this[^1];
        }

        internal int Extent()
        {
            return Count;
        }
    }
}

