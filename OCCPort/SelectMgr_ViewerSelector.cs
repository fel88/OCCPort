using System;

namespace OCCPort
{
    internal class SelectMgr_ViewerSelector
    {

        //=======================================================================
        //function : AllowOverlapDetection
        //purpose  : Sets the detection type: if theIsToAllow is false,
        //           only fully included sensitives will be detected, otherwise
        //           the algorithm will mark both included and overlapped entities
        //           as matched
        //=======================================================================
        public void AllowOverlapDetection(bool theIsToAllow)
        {
            mySelectingVolumeMgr.AllowOverlapDetection(theIsToAllow);
        }


        //! Returns instance of selecting volume manager of the viewer selector
        //public SelectMgr_SelectingVolumeManager GetManager() { return mySelectingVolumeMgr; }

        SelectMgr_SelectingVolumeManager mySelectingVolumeMgr;


        //=======================================================================
        // Function: Pick
        // Purpose :
        //=======================================================================
        public void Pick(int theXPix,
                                           int theYPix,
                                           V3d_View theView)
        {
            updateZLayers(theView);

            gp_Pnt2d aMousePos = new gp_Pnt2d((theXPix),
                                 (theYPix));
            mySelectingVolumeMgr.InitPointSelectingVolume(aMousePos);

            mySelectingVolumeMgr.SetPixelTolerance(myTolerances.Tolerance());
            mySelectingVolumeMgr.SetCamera(theView.Camera());
            int aWidth = 0, aHeight = 0;
            theView.Window().Size(out aWidth, out aHeight);
            mySelectingVolumeMgr.SetWindowSize(aWidth, aHeight);
        }

        SelectMgr_ToleranceMap myTolerances;
        private void updateZLayers(V3d_View theView)
        {
            throw new NotImplementedException();
        }

        //=======================================================================
        public SelectMgr_EntityOwner Picked(int theRank)
        {
            if (theRank < 1 || theRank > NbPicked())
            {
                return new SelectMgr_EntityOwner();
            }

            if (!myIsSorted)
            {
                SortResult();
            }

            int anOwnerIdx = myIndexes.Value(theRank);
            var aStoredOwner = mystored.FindKey(anOwnerIdx);
            return aStoredOwner;
        }

        SelectMgr_IndexedDataMapOfOwnerCriterion mystored;

        bool myIsSorted;
        TColStd_Array1OfInteger myIndexes;
        private void SortResult()
        {
            throw new NotImplementedException();
        }

        private int NbPicked()
        {
            throw new NotImplementedException();
        }

		SelectMgr_SelectableObjectSet mySelectableObjects = new SelectMgr_SelectableObjectSet();

		internal void RebuildObjectsTree(bool theIsForce = false)
		{

			mySelectableObjects.MarkDirty();

			if (theIsForce)
			{
                
                int xx = 0;
                int  yy = 0;
                
                mySelectingVolumeMgr.WindowSize(ref xx, ref yy);
                Graphic3d_Vec2i aWinSize = new Graphic3d_Vec2i(xx,yy);
                mySelectableObjects.UpdateBVH(mySelectingVolumeMgr.Camera(), aWinSize);
			}

		}



		public SelectMgr_SelectableObjectSet SelectableObjects() { return mySelectableObjects; }

        internal bool Contains(SelectMgr_SelectableObject theObject)
        {
			return mySelectableObjects.Contains(theObject);
        }
		SelectMgr_MapOfObjectSensitives myMapOfObjectSensitives = new SelectMgr_MapOfObjectSensitives();
		Select3D_BVHBuilder3d myEntitySetBuilder;
        internal void AddSelectableObject(SelectMgr_SelectableObject theObject)
        {
			if (!myMapOfObjectSensitives.IsBound(theObject))
			{
				mySelectableObjects.Append(theObject);
				SelectMgr_SensitiveEntitySet anEntitySet = new SelectMgr_SensitiveEntitySet(myEntitySetBuilder);
				myMapOfObjectSensitives.Bind(theObject, anEntitySet);
			}

        }
    }
}