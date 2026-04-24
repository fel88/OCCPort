using System;
using static OCCPort.TopTools_IndexedDataMapOfShapeListOfShape;
using static OpenTK.Graphics.OpenGL.GL;

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

        public TopLoc_SListOfItemLocation(TopLoc_ItemLocation anItem, TopLoc_SListOfItemLocation aTail)
        {
            myNode = (new TopLoc_SListNodeOfItemLocation(anItem, aTail));

            if (!myNode.Tail().IsEmpty())
            {
                gp_Trsf aT = myNode.Tail().Value().myTrsf;
                myNode.Value().myTrsf.PreMultiply(aT);
            }

        }

        public TopLoc_SListOfItemLocation()
        {
        }

        public bool IsEmpty()
        {

            return myNode == null;


        }
        public void Clear()
        {
            myNode = null;

        }
        public TopLoc_ItemLocation Value()
        {
            Standard_NoSuchObject_Raise_if(myNode == null, "TopLoc_SListOfItemLocation::Value");
            return myNode.Value();
        }

        private void Standard_NoSuchObject_Raise_if(bool v1, string v2)
        {
            if (v1)
                throw new Exception(v2);
        }

        internal void Construct(TopLoc_ItemLocation anItem)
        {
            Assign(new TopLoc_SListOfItemLocation(anItem, this));
        }

        public TopLoc_SListOfItemLocation Assign(TopLoc_SListOfItemLocation Other)
        {
            if (this == Other) return this;
            Clear();
            myNode = Other.myNode;

            return this;
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