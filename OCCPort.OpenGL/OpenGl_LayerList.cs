using System.Collections.Generic;

namespace OCCPort
{
    internal class OpenGl_LayerList
	{
		List<Graphic3d_Layer> myLayers = new List<Graphic3d_Layer>();
		internal Graphic3d_Layer[] Layers()
		{
			return myLayers.ToArray();
		}

	}

}