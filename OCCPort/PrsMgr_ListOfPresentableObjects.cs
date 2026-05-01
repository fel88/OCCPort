using System;
using System.Collections.Generic;

namespace OCCPort
{
	public class PrsMgr_ListOfPresentableObjects: List<PrsMgr_PresentableObject>
    {
		

		internal void Append(PrsMgr_PresentableObject theObject)
		{
			base.Add(theObject);
		}

		internal void Remove(PrsMgr_ListOfPresentableObjectsIter anIter)
		{
			base.Remove(anIter.Value());
		}
	}
}