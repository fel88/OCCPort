using OCCPort.Common;

namespace TKernel
{
    public class NCollection_BaseVector<T> : List<T>
    {   //! Locate the memory holding the desired value
        public T findV(int theIndex)
        {
            Exceptions.Standard_OutOfRange_Raise_if(theIndex < 0 || theIndex >= Count,
                                          "NCollection_BaseVector::findV");
            ;
            return this[theIndex];
        }

    }
    public class VectorOfInteger : NCollection_Vector<int>
    {
        public VectorOfInteger() { }
        public VectorOfInteger(int v)
        {
        }
    }

    public static class Std
    {
        public static void make_heap(VectorOfInteger theVertices, IComparer<int> aCmp)
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

        public static void sort_heap(VectorOfInteger theVertices, IComparer<int> aCmp)
        {
            Heap<int> heap = new Heap<int>(aCmp);
            foreach (var item in theVertices)
            {
                heap.Add(item);
            }
            theVertices.Clear();
            List<int> temp = new List<int>();
            while (heap.Count > 0)
            {
                temp.Add(heap.PopTop());
            }
            temp.Reverse();
            foreach (var item in temp)
            {
                theVertices.Add(item);
            }

        }
    }
}