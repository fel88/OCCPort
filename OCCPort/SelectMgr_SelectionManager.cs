using OCCPort;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Security.Policy;

namespace OCCPort
{
    internal class SelectMgr_SelectionManager
    {


        public SelectMgr_SelectionManager(SelectMgr_ViewerSelector theSelector)
        {
            mySelector = (theSelector);
        }

        //=======================================================================
        //function : Update
        //purpose  : Selections are recalculated if they are flagged
        //           "TO RECALCULATE" and activated in one of selectors.
        //           If ForceUpdate = True, and they are "TO RECALCULATE"
        //           This is done without caring for the state of activation.
        //=======================================================================
        public void Update(SelectMgr_SelectableObject theObject,
                                         bool theIsForce)
        {
            /*for (PrsMgr_ListOfPresentableObjectsIter aChildIter (theObject.Children()); aChildIter.More(); aChildIter.Next())
            {
                Update((SelectMgr_SelectableObject)(aChildIter.Value()), theIsForce);
            }*/
            if (!theObject.HasOwnPresentations())
            {
                return;
            }

            foreach (var item in theObject.Selections())
            {
                //SelectMgr_Selection aSelection = aSelIter.Value();
                //if (theIsForce || mySelector.Status(aSelection) ==  SelectMgr_SOS_Activated)
                //{
                //    switch (aSelection.UpdateStatus())
                //    {
                //        case SelectMgr_TypeOfUpdate.SelectMgr_TOU_Full:
                //            {
                //                ClearSelectionStructures(theObject, aSelection.Mode());
                //                theObject.RecomputePrimitives(aSelection.Mode()); // no break on purpose...
                //                RestoreSelectionStructures(theObject, aSelection.Mode());
                //                // pass through SelectMgr_TOU_Partial
                //            }break;//???
                //        //  Standard_FALLTHROUGH
                //        case SelectMgr_TypeOfUpdate.SelectMgr_TOU_Partial:
                //            {
                //                theObject.UpdateTransformations(aSelection);
                //                mySelector.RebuildObjectsTree();
                //                break;
                //            }
                //        default:
                //            break;
                //    }
                ///  aSelection.UpdateStatus(SelectMgr_TypeOfUpdate.SelectMgr_TOU_None);
                //  aSelection.UpdateBVHStatus(SelectMgr_TypeOfBVHUpdate.SelectMgr_TBU_None);
            }
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