using System;

namespace OCCPort
{
    internal class SelectMgr_SelectionManager
    {
        

        public SelectMgr_SelectionManager(StdSelect_ViewerSelector3d stdSelect_ViewerSelector3d)
        {
            
        }

        internal void Activate(AIS_InteractiveObject theIObj, int theSelectionMode)
        {
            throw new NotImplementedException();
        }

        internal bool Contains(SelectMgr_SelectableObject anObj)
        {
            throw new NotImplementedException();
        }

        internal bool IsActivated(AIS_InteractiveObject theIObj, int theSelectionMode)
        {
            throw new NotImplementedException();
        }

        internal void Load(AIS_InteractiveObject theIObj)
        {
            throw new NotImplementedException();
        }

        internal StdSelect_ViewerSelector3d Selector()
        {
            throw new NotImplementedException();
        }
    }
}