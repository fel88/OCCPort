using System;
using System.Collections.Generic;

namespace OCCPort
{
	public class MyLayersDic
	{
		Dictionary<Graphic3d_ZLayerId, Graphic3d_Layer> dic = new Dictionary<Graphic3d_ZLayerId, Graphic3d_Layer>();
		public void Bind(Graphic3d_ZLayerId theNewLayerId, Graphic3d_Layer aNewLayer)
		{
			dic.Add(theNewLayerId, aNewLayer);
		}

		public bool Find(Graphic3d_ZLayerId graphic3d_ZLayerId_Default, out Graphic3d_Layer anOtherLayer)
		{
			var ret = dic.ContainsKey(graphic3d_ZLayerId_Default);
			anOtherLayer = null;
			if (ret)
				anOtherLayer = dic[graphic3d_ZLayerId_Default];
			return ret;
		}

		public Graphic3d_Layer Find(Graphic3d_ZLayerId graphic3d_ZLayerId_Default)
		{
			return dic[graphic3d_ZLayerId_Default];
		}

		public bool IsBound(Graphic3d_ZLayerId theNewLayerId)
		{
			return dic.ContainsKey(theNewLayerId);
		}

		public Graphic3d_Layer Seek(Graphic3d_ZLayerId theLayerId)
		{
			if (!dic.ContainsKey(theLayerId))
			{
				return null;
			}
			return dic[(theLayerId)];
		}


	}
}