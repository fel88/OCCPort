namespace OCCPort
{
    public static class Std
    {
        internal static void make_heap(VectorOfInteger theVertices, ComparatorOfIndexedVertexOfDelaun aCmp)
        {
            /*Heap<int> heap = new Heap<int>();
            foreach (var item in theVertices)
            {
                heap.Add(item);
            }
            var arr = heap.Array();
            theVertices.Clear();
            foreach (var item in arr)
            {
                theVertices.Add(item);
            }*/
        }

        internal static void sort_heap(VectorOfInteger theVertices, ComparatorOfIndexedVertexOfDelaun aCmp)
        {
            Heap<int> heap = new Heap<int>(aCmp);
            foreach (var item in theVertices)
            {
                heap.Add(item);
            }
            theVertices.Clear();
            while (heap.Count > 0)
            {
                theVertices.Add(heap.PopTop());
            }
        }
    }
}