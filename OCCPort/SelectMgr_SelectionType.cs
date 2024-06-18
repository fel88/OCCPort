using System;

namespace OCCPort
{

	public enum SelectMgr_SelectionType
	{

		//	Possible selection types.
		//Enumerator
		SelectMgr_SelectionType_Unknown,

		//undefined selection type
		SelectMgr_SelectionType_Point,

		//selection by point(frustum with some tolerance or axis)
		SelectMgr_SelectionType_Box,

		//rectangle selection
		SelectMgr_SelectionType_Polyline,

		//polygonal selection
	}
}