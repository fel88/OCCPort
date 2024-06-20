using System;
using System.Collections.Generic;

namespace OCCPort.OpenGL
{
	//! This class defines an OpenGl graphic driver
	public class OpenGl_GraphicDriver : Graphic3d_GraphicDriver
    {

		OpenGl_Caps myCaps;
		List<OpenGl_View> myMapOfView=new List<OpenGl_View> ();
		Dictionary<int, OpenGl_Structure> myMapOfStructure = new Dictionary<int, OpenGl_Structure>();


		OpenGl_StateCounter myStateCounter; //!< State counter for OpenGl structures.

		public override Graphic3d_CStructure CreateStructure(Graphic3d_StructureManager theManager)
		{
			OpenGl_Structure aStructure = new OpenGl_Structure(theManager);
			myMapOfStructure.Add(aStructure.Identification(), aStructure);
			return aStructure;

		}

		public override Graphic3d_CView CreateView(Graphic3d_StructureManager theMgr)
		{

			OpenGl_View aView = new OpenGl_View(theMgr, this, myCaps, myStateCounter);
			myMapOfView.Add(aView);
			/*for (NCollection_List < Handle(Graphic3d_Layer) >::Iterator aLayerIter(myLayers); aLayerIter.More(); aLayerIter.Next())
			{
				Graphic3d_Layer aLayer = aLayerIter.Value();
				aView.InsertLayerAfter(aLayer->LayerId(), aLayer->LayerSettings(), Graphic3d_ZLayerId_UNKNOWN);
			}*/
			return aView;

		}

		internal int GetNextPrimitiveArrayUID()
		{
			throw new NotImplementedException();
		}

		internal OpenGl_Context GetSharedContext()
		{
			throw new NotImplementedException();
		}

        internal void setDeviceLost()
        {
            throw new NotImplementedException();
        }
    }
}