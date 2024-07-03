using System;
using System.Collections.Generic;

namespace OCCPort
{
	internal class Graphic3d_IndexedMapOfView : List<Graphic3d_CView>
	{
		//public List<Graphic3d_CView> list = new List<Graphic3d_CView>();

		internal class Iterator
		{
			Graphic3d_IndexedMapOfView list;
			public Iterator(Graphic3d_IndexedMapOfView myDefinedViews)
			{
				list = myDefinedViews;
			}

			internal bool More()
			{
				throw new NotImplementedException();
			}

			internal void Next()
			{
				throw new NotImplementedException();
			}

			internal Graphic3d_CView Value()
			{
				throw new NotImplementedException();
			}
		}
	}
}