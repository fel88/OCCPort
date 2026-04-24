namespace OCCPort
{
    internal class TopLoc_SListNodeOfItemLocation
    {
        TopLoc_SListOfItemLocation myTail;
        TopLoc_ItemLocation myValue;

        public TopLoc_SListNodeOfItemLocation(TopLoc_ItemLocation I, TopLoc_SListOfItemLocation T)
        {
            myTail = (T);myValue = (I);
        }

        public TopLoc_SListOfItemLocation Tail()
        {
            return myTail;
        }

        public TopLoc_ItemLocation Value()
        {
            return myValue;
        }


    }
}