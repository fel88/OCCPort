using System;
using System.Collections.Generic;

namespace OCCPort
{
	public class LayersCollection: List<Graphic3d_Layer>
	{		
		public void Append(Graphic3d_Layer aLayer)
		{
			Add(aLayer);
		}

		public void InsertAfter(Graphic3d_Layer aNewLayer, Graphic3d_Layer aLayerIter)
		{
			throw new NotImplementedException();
		}

		public void InsertBefore(Graphic3d_Layer aNewLayer, Graphic3d_Layer aLayerIter)
		{
			throw new NotImplementedException();
		}
	}
}