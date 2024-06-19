using System;
using System.Collections.Generic;

namespace OCCPort
{
	internal class PrsMgr_ListOfPresentableObjectsIter
	{
		public PrsMgr_ListOfPresentableObjectsIter(PrsMgr_ListOfPresentableObjects v)
		{
			list = v;
		}
		PrsMgr_ListOfPresentableObjects list;


		int index = 0;
		internal bool More()
		{
			return index < list.list.Count - 1;
		}

		internal void Next()
		{
			index++; 
		}

		internal PrsMgr_PresentableObject Value()
		{
			return list.list[index];
		}
	}
}