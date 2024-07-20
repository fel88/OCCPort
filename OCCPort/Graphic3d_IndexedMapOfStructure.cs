using System;
using System.Collections;
using System.Collections.Generic;

namespace OCCPort
{
	public class Graphic3d_IndexedMapOfStructure : IEnumerable<Graphic3d_CStructure>
	{
		List<Graphic3d_CStructure> list = new List<Graphic3d_CStructure>();
		public IEnumerator<Graphic3d_CStructure> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		internal int Add(Graphic3d_CStructure theStruct)
		{
			list.Add(theStruct);
			return list.Count;
		}

		internal int Size()
		{
			return list.Count;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}
	}
}