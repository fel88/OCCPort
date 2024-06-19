using OCCPort.Tester;
using System;
using System.Collections.Generic;


namespace OCCPort
{
    public class Graphic3d_StructureManager
    {

		Graphic3d_MapOfStructure myDisplayedStructure;
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
			List<Prs3d_Presentation> aStructNetwork = new List<Prs3d_Presentation>();
			foreach (var anIter in myDisplayedStructure)
			{
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

		public void RecomputeStructures(List<Prs3d_Presentation> theStructures)
		{
			foreach (var item in theStructures)
			{
				Prs3d_Presentation aStruct = item;
				aStruct.Clear();
				aStruct.Compute();
			}
		}
		Graphic3d_IndexedMapOfView myDefinedViews;
		internal void Display(Graphic3d_Structure theStructure)
		{

			myDisplayedStructure.Add(theStructure);


			for (Graphic3d_IndexedMapOfView.Iterator aViewIt = new Graphic3d_IndexedMapOfView.Iterator(myDefinedViews); aViewIt.More(); aViewIt.Next())
			{
				aViewIt.Value().Display(theStructure);
			}

		}

		protected Graphic3d_MapOfObject myRegisteredObjects = new Graphic3d_MapOfObject();

		public Graphic3d_StructureManager(Graphic3d_GraphicDriver theDriver)
		{
		}
    }
}