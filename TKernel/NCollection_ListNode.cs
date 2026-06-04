namespace TKernel
{
    /**
     * Purpose:     This class is used to  represent a node  in the BaseList and
     *              BaseMap. 
     */
    public class NCollection_ListNode
    {
        object val;
        NCollection_ListNode next;
        internal NCollection_ListNode Next()
        {
            return next;
        }

        internal object Value()
        {
            return val;
        }
    }
}