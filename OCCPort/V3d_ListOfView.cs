using System.Collections.Generic;

namespace OCCPort
{
	public class V3d_ListOfView : List<V3d_View>
	{

		public void Append(V3d_View v)
		{
			Add(v);
		}
	}
}