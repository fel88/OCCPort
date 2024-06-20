using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Policy;

namespace OCCPort
{
    internal class SelectMgr_SelectionManager
    {


        public SelectMgr_SelectionManager(SelectMgr_ViewerSelector theSelector)
        {
            mySelector = (theSelector);
        }

        internal void Activate(AIS_InteractiveObject theIObj, int theSelectionMode)
        {
            
        }

        internal bool Contains(SelectMgr_SelectableObject theObject)
        {
            return myGlobal.Contains(theObject);
        }
        List<SelectMgr_SelectableObject> myGlobal = new List<SelectMgr_SelectableObject>();

        internal bool IsActivated(AIS_InteractiveObject theIObj, int theSelectionMode)
        {
            throw new NotImplementedException();
        }

        internal void Load(SelectMgr_SelectableObject theObject,
                                        int theMode = -1)
        {

            if (myGlobal.Contains(theObject))
                return;

            for (PrsMgr_ListOfPresentableObjectsIter anChildrenIter = new PrsMgr_ListOfPresentableObjectsIter(theObject.Children()); anChildrenIter.More(); anChildrenIter.Next())
            {
                Load((SelectMgr_SelectableObject)anChildrenIter.Value(), theMode);
            }


            if (!theObject.HasOwnPresentations())
                return;

            myGlobal.Add(theObject);
            if (!mySelector.Contains(theObject) && theObject.HasOwnPresentations())
            {
                mySelector.AddSelectableObject(theObject);
            }
            if (theMode != -1)
                loadMode(theObject, theMode);

        }

        private void loadMode(SelectMgr_SelectableObject theObject, int theMode)
        {
            throw new NotImplementedException();
        }

        internal StdSelect_ViewerSelector3d Selector()
        {
            throw new NotImplementedException();
        }

        SelectMgr_ViewerSelector mySelector;
    }
}