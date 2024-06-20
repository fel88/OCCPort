using System;
using System.Collections.Generic;

namespace OCCPort
{
	internal class Poly_ListOfTriangulation
	{
		public List<Poly_Triangulation> list = new List<Poly_Triangulation>();
		public bool IsEmpty()
		{
			return list.Count == 0;
		}

		internal Poly_Triangulation First()
		{
			return list[0];
		}
	}
}