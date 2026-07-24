namespace TKernel
{
    public class IndexedDataMapNode<TKey, TValue> : NCollection_TListNode<TValue>
    {

        public TKey myKey1;
        public int myIndex;

                

        //! Constructor with 'Next'
        public IndexedDataMapNode(TKey theKey1,
                            int theIndex,
                            TValue theItem,
                            NCollection_ListNode theNext1
                           ) : base(theItem, theNext1)
        {
            
            myKey1 = (theKey1);
            myIndex = (theIndex);

        }

        public int Index()
        {
            return myIndex;
        }

        public TKey Key1()
        {
            return myKey1;
        }

        
    }
}