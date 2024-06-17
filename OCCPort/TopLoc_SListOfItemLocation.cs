using System;

namespace OCCPort
{
    //! An SListOfItemLocation is a LISP like list of Items.
    //! An SListOfItemLocation is :
    //! . Empty.
    //! . Or it has a Value and a  Tail  which is an other SListOfItemLocation.
    //!
    //! The Tail of an empty list is an empty list.
    //! SListOfItemLocation are  shared.  It  means   that they  can  be
    //! modified through other lists.
    //! SListOfItemLocation may  be used  as Iterators. They  have Next,
    //! More, and value methods. To iterate on the content
    //! of the list S just do.
    //!
    //! SListOfItemLocation Iterator;
    //! for (Iterator = S; Iterator.More(); Iterator.Next())
    //! X = Iterator.Value();
    public class TopLoc_SListOfItemLocation
    {
        TopLoc_SListNodeOfItemLocation myNode;

        public bool IsEmpty()
        {

            return myNode == null;


        }
        public void Clear()
        {

        }
        public TopLoc_ItemLocation Value()
        {
            //Standard_NoSuchObject_Raise_if(myNode.IsNull(),"TopLoc_SListOfItemLocation::Value");
            return myNode.Value();
        }

        internal void Construct(TopLoc_ItemLocation topLoc_ItemLocation)
        {
            throw new NotImplementedException();
        }


        //! Returns True if the iterator  has a current value.
        //! This is !IsEmpty()
        public bool More()
        {
            return !IsEmpty();
        }


        internal void Next()
        {
            throw new NotImplementedException();
        }

        internal void ToTail()
        {
            throw new NotImplementedException();
        }
    }
}