using OCCPort.Tester;
using System;
using System.Collections.Generic;
using static OCCPort.Tester.Prs3d_Presentation;

namespace OCCPort
{
    public class Graphic3d_StructureManager
    {

		Graphic3d_MapOfStructure myDisplayedStructure;
        public void RegisterObject(object theObject,
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


        protected Graphic3d_MapOfObject myRegisteredObjects;
    }
}