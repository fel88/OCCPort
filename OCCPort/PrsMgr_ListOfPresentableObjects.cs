using System;
using System.Collections.Generic;

namespace OCCPort
{
	public class PrsMgr_ListOfPresentableObjects
	{
		public List<PrsMgr_PresentableObject> list = new List<PrsMgr_PresentableObject>();

		internal void Append(PrsMgr_PresentableObject theObject)
		{
			list.Add(theObject);
		}

		internal void Remove(PrsMgr_ListOfPresentableObjectsIter anIter)
		{
			list.Remove(anIter.Value());
		}
	}
}