using System;
using System.Collections.Generic;

namespace OCCPort
{
	internal class BRep_ListOfCurveRepresentation
	{
		public List<BRep_CurveRepresentation> list = new List<BRep_CurveRepresentation>();
		internal void Append(BRep_Curve3D c3d)
		{
			list.Add(c3d);
		}
		
	}
}