using System;
using System.Collections.Generic;

namespace OCCPort
{
	internal class SelectMgr_MapOfObjectSensitives
	{
		internal void Bind(SelectMgr_SelectableObject theObject, SelectMgr_SensitiveEntitySet anEntitySet)
		{
			map.Add(theObject, anEntitySet);
		}

		Dictionary<SelectMgr_SelectableObject, SelectMgr_SensitiveEntitySet> map = new Dictionary<SelectMgr_SelectableObject, SelectMgr_SensitiveEntitySet>();
		internal bool IsBound(SelectMgr_SelectableObject theObject)
		{
			return map.ContainsKey(theObject);
		}
	}
}