using OCCPort.Tester;
using System;
using System.Collections.Generic;


namespace OCCPort
{
    public class Graphic3d_StructureManager
    {
		Graphic3d_GraphicDriver myGraphicDriver;
		Graphic3d_MapOfStructure myDisplayedStructure = new Graphic3d_MapOfStructure();
		public void RegisterObject(AIS_InteractiveObject theObject,
                                                  Graphic3d_ViewAffinity theAffinity)
        {
            Graphic3d_ViewAffinity aResult;
            if (myRegisteredObjects.Find(theObject, out aResult)
             && aResult == theAffinity)
            {
                return;
            }

            myRegisteredObjects.Bind(theObject, theAffinity);
        }

		//! Returns TRUE if Device Lost flag has been set and presentation data should be reuploaded onto graphics driver.
		public bool IsDeviceLost() { return myDeviceLostFlag; }
		bool myDeviceLostFlag = true;

		internal void RecomputeStructures()
		{
			myDeviceLostFlag = false;

			// Go through all unique structures including child (connected) ones and ensure that they are computed.
			List<Graphic3d_Structure> aStructNetwork = new List<Graphic3d_Structure>();
			foreach (var anIter in myDisplayedStructure)
			{
				Graphic3d_Structure.Network(anIter, Graphic3d_TypeOfConnection.Graphic3d_TOC_DESCENDANT, aStructNetwork);
				//aStructNetwork.Add(anIter.net);
				//anIter.Network();
			}
			/*
			for (Graphic3d_MapIteratorOfMapOfStructure anIter(myDisplayedStructure); anIter.More(); anIter.Next())
			{
				Handle(Graphic3d_Structure) aStructure = anIter.Key();
				anIter.Key()->Network(anIter.Key().get(), Graphic3d_TOC_DESCENDANT, aStructNetwork);
			}*/

			RecomputeStructures(aStructNetwork);

		}

		public void RecomputeStructures(List<Graphic3d_Structure> theStructures)
		{
			foreach (var item in theStructures)
			{
				var aStruct = item;
				aStruct.Clear();
				aStruct.Compute();
			}
		}

		Graphic3d_IndexedMapOfView myDefinedViews = new Graphic3d_IndexedMapOfView();

		internal void Display(Graphic3d_Structure theStructure)
		{
			myDisplayedStructure.Add(theStructure);

			foreach (var anIter in myDefinedViews.list)
			{
				anIter.Display(theStructure);

			}
			/*for (Graphic3d_IndexedMapOfView.Iterator aViewIt = new Graphic3d_IndexedMapOfView.Iterator(myDefinedViews); aViewIt.More(); aViewIt.Next())
			{
				aViewIt.Value().Display(theStructure);
			}*/
			}
		public Graphic3d_GraphicDriver GraphicDriver()
		{
			return (myGraphicDriver);
		}

		internal void Clear(Graphic3d_Structure theStructure, bool theWithDestruction)
		{
			foreach (var aViewIt in myDefinedViews.list)
			{
				aViewIt.Clear(theStructure, theWithDestruction);
			}
			

		}

		protected Graphic3d_MapOfObject myRegisteredObjects = new Graphic3d_MapOfObject();

		public Graphic3d_StructureManager(Graphic3d_GraphicDriver theDriver)
		{
			myGraphicDriver = (theDriver);
		}
    }
}