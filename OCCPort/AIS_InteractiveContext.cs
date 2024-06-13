using System;

namespace OCCPort
{
    public class AIS_InteractiveContext
    {
        public AIS_InteractiveContext(V3d_Viewer MainViewer)
        {
            mgrSelector = new SelectMgr_SelectionManager(new StdSelect_ViewerSelector3d());
        }
        SelectMgr_SelectionManager mgrSelector;
        StdSelect_ViewerSelector3d MainSelector()
        {
            return mgrSelector.Selector();
        }

        public AIS_StatusOfPick SelectRectangle(Graphic3d_Vec2i thePntMin,
                                                               Graphic3d_Vec2i thePntMax,
                                                               V3d_View theView,
                                                               AIS_SelectionScheme theSelScheme)
        {
            //if (theView.Viewer() != myMainVwr)
            {
                //throw new Exception("AIS_InteractiveContext::SelectRectangle() - invalid argument");
            }

            //myLastActiveView = theView.get();
            MainSelector().Pick(thePntMin.x(), thePntMin.y(), thePntMax.x(), thePntMax.y(), theView);

            AIS_NArray1OfEntityOwner aPickedOwners = new AIS_NArray1OfEntityOwner();
            if (MainSelector().NbPicked() > 0)
            {
                aPickedOwners.Resize(1, MainSelector().NbPicked(), false);
                for (int aPickIter = 1; aPickIter <= MainSelector().NbPicked(); ++aPickIter)
                {
                    aPickedOwners.SetValue(aPickIter, MainSelector().Picked(aPickIter));
                }
            }

            return Select(aPickedOwners, theSelScheme);
        }

        // ============================================================================
        // function : Deactivate
        // purpose  :
        // ============================================================================
        public void Deactivate()
        {
            AIS_ListOfInteractive aDisplayedObjects;
            DisplayedObjects(out aDisplayedObjects);

            for (AIS_ListOfInteractive.Iterator anIter = new AIS_ListOfInteractive.Iterator(aDisplayedObjects); anIter.More(); anIter.Next())
            {
                Deactivate(anIter.Value());
            }
        }


        // ============================================================================
        public void Deactivate(int theMode)
        {
            AIS_ListOfInteractive aDisplayedObjects;
            DisplayedObjects(out aDisplayedObjects);
            for (AIS_ListOfInteractive.Iterator anIter = new AIS_ListOfInteractive.Iterator(aDisplayedObjects); anIter.More(); anIter.Next())
            {
                Deactivate(anIter.Value(), theMode);
            }
        }

        // ============================================================================
        // function : Activate
        // purpose  :
        // ============================================================================
        public void Activate(int theMode,
                                        bool theIsForce)
        {
            AIS_ListOfInteractive aDisplayedObjects;
            DisplayedObjects(out aDisplayedObjects);
            for (AIS_ListOfInteractive.Iterator anIter = new AIS_ListOfInteractive.Iterator(aDisplayedObjects); anIter.More(); anIter.Next())
            {
                Load(anIter.Value(), -1);
                Activate(anIter.Value(), theMode, theIsForce);
            }
        }

        private void Load(int v1, int v2)
        {
            throw new NotImplementedException();
        }

        private void Activate(int v, int theMode, bool theIsForce)
        {
            throw new NotImplementedException();
        }

        private void Deactivate(int v, int theMode)
        {
            throw new NotImplementedException();
        }

        private void DisplayedObjects(out AIS_ListOfInteractive aDisplayedObjects)
        {
            throw new NotImplementedException();
        }

        private AIS_StatusOfPick Select(AIS_NArray1OfEntityOwner aPickedOwners, AIS_SelectionScheme theSelScheme)
        {
            throw new NotImplementedException();
        }
    }

}
