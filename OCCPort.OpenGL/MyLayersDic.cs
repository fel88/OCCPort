using System;
using System.Collections.Generic;

namespace OCCPort
{
	internal class MyLayersDic : Dictionary<Graphic3d_ZLayerId, Graphic3d_Layer>
	{
		internal Graphic3d_Layer Find(Graphic3d_ZLayerId zz)
		{
			return this[zz];

		}

		internal void Find(Graphic3d_ZLayerId theLayerId, out Graphic3d_Layer aLayer)
		{
			aLayer = this[theLayerId];
		}

		internal Graphic3d_Layer Seek(Graphic3d_ZLayerId theLayerId)
		{
			return this[theLayerId];
		}
	}
}