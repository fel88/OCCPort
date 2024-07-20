using System;
using System.Collections.Generic;
using System.Linq;

namespace OCCPort
{
    public class TopoDS_ListIteratorOfListOfShape
    {

        //typedef NCollection_List<TopoDS_Shape>::Iterator TopoDS_ListIteratorOfListOfShape;
        protected List<TopoDS_Shape> list = new List<TopoDS_Shape>();
        public TopoDS_Shape this[int i]
        {
            get { return list[i]; }
            set { list[i] = value; }
        }

        internal void Initialize(TopoDS_ListOfShape myShapes)
        {
            list = myShapes.list.ToArray().ToList();
        }

        NCollection_ListNode myCurrent;
        NCollection_ListNode myPrevious;
        int index;
        internal bool More()
        {
            return index < list.Count;
            //return (myCurrent != null);
        }

        internal void Next()
        {
            index++;
            //myPrevious = myCurrent;
            //myCurrent = myCurrent.Next();
        }

        internal TopoDS_Shape Value()
        {
            return list[index];
            //return (TopoDS_Shape)myCurrent.Value(); ;
        }
    }
}