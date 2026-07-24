namespace TKernel
{
    /**
     * Purpose:     This class is used to  represent a node  in the BaseList and
     *              BaseMap. 
     */
    public class NCollection_ListNode
    {
        //! The only constructor
        public NCollection_ListNode(NCollection_ListNode theNext)
        {
            myNext = (theNext);
        }
        protected NCollection_ListNode myNext;
        internal NCollection_ListNode Next()
        {
            return myNext;
        }
        internal void Next(NCollection_ListNode next)
        {
            myNext = next;
        }

    }
}