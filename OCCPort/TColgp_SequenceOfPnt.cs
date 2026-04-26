using System;
using System.Collections.Generic;

namespace OCCPort
{
	public class TColgp_SequenceOfPnt
	{
		public void Clear()
		{
			list.Clear();
		}
		internal int Size()
		{
			return list.Count;
		}

		public List<gp_Pnt> list = new List<gp_Pnt>();
		public gp_Pnt Value(int index)
		{
			return list[index-1];
		}

        internal void Append(gp_Pnt thePnt)
        {
			list.Add(thePnt);
        }

        internal void InsertBefore(int i, gp_Pnt thePnt)
        {
			list.Insert(i-1, thePnt);
        }

        internal void ChangeValue(int i, gp_Pnt thePnt)
        {
			list[i-1] = thePnt;
        }
    }
}