using System;
using System.Collections.Generic;
using TKernel;

namespace OCCPort
{
    public class TColgp_SequenceOfPnt : NCollection_Sequence<gp_Pnt>
    {
        //public new void Clear()
        //{
        //	list.Clear();
        //}

        //public int Length()
        //{
        //	return list.Length();
        //}



        //public gp_Pnt Value(int index)
        //{
        //	return list[index-1];
        //}
        //      public gp_Pnt this[int key]
        //      {
        //          get => list[key - 1];
        //          set => list[key - 1] = value;
        //      }
        //      internal void Append(gp_Pnt thePnt)
        //      {
        //	list.Add(thePnt);
        //      }

        //      internal void InsertBefore(int i, gp_Pnt thePnt)
        //      {
        //	list.Insert(i-1, thePnt);
        //      }



        //      internal int Lower()
        //      {

        //      }
        internal TColgp_SequenceOfPnt ChangeSequence()
        {
            return this;
        }
    }
}