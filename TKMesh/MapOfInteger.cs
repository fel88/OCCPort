namespace TKMesh
{
    public class MapOfInteger : HashSet<int>
    {
        public MapOfInteger() { }
        public MapOfInteger(IEnumerable<int> collection) : base(collection)
        {
            foreach (var item in collection)
            {
                Add(item);
            }
        }


        public bool IsEmpty()
        {
            return Count == 0;
        }

        internal int Extent()
        {
            return Count;
        }

        internal int Size()
        {
            return Count;
        }
    }
}

