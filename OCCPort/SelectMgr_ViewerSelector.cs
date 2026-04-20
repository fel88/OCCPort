using System;
using System.Reflection.Metadata;

namespace OCCPort
{
    //! A framework to define finding, sorting the sensitive
    //! primitives in a view. Services are also provided to
    //! define the return of the owners of those primitives
    //! selected. The primitives are sorted by criteria such
    //! as priority of the primitive or its depth in the view
    //! relative to that of other primitives.
    //! Note that in 3D, the inheriting framework
    //! StdSelect_ViewerSelector3d   is only to be used
    //! if you do not want to use the services provided by
    //! AIS.
    //! Two tools are available to find and select objects
    //! found at a given position in the view. If you want to
    //! select the owners of all the objects detected at
    //! point x,y,z you use the Init - More - Next - Picked
    //! loop. If, on the other hand, you want to select only
    //! one object detected at that point, you use the Init -
    //! More - OnePicked loop. In this iteration, More is
    //! used to see if an object was picked and
    //! OnePicked, to get the object closest to the pick position.
    //! Viewer selectors are driven by
    //! SelectMgr_SelectionManager, and manipulate
    //! the SelectMgr_Selection objects given to them by
    //! the selection manager.
    //!
    //! Tolerances are applied to the entities in the following way:
    //! 1. tolerance value stored in mytolerance will be used to calculate initial
    //!    selecting frustum, which will be applied for intersection testing during
    //!    BVH traverse;
    //! 2. if tolerance of sensitive entity is less than mytolerance, the frustum for
    //!    intersection detection will be resized according to its sensitivity.
    public class SelectMgr_ViewerSelector//typedef SelectMgr_ViewerSelector StdSelect_ViewerSelector3d;
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
        //! Returns the 3D point (intersection of picking axis with the object nearest to eye)
        //! for the object picked at specified position.
        //! @param theRank rank of detected object within range 1...NbPicked()
        public gp_Pnt PickedPoint(int theRank) { return PickedData(theRank).Point; }
        //! Returns instance of selecting volume manager of the viewer selector
        public SelectMgr_SelectingVolumeManager GetManager() { return mySelectingVolumeMgr; }

        //! Returns instance of selecting volume manager of the viewer selector
        //public SelectMgr_SelectingVolumeManager GetManager() { return mySelectingVolumeMgr; }

        SelectMgr_SelectingVolumeManager mySelectingVolumeMgr;

        //! Returns the Entity for the object picked at specified position.
        //! @param theRank rank of detected object within range 1...NbPicked()
        public SelectMgr_SortCriterion PickedData(int theRank)
        {

            Standard_OutOfRange_Raise_if(theRank < 1 || theRank > NbPicked(), "SelectMgr_ViewerSelector::PickedData() out of range index");
            if (!myIsSorted)
            {
                SortResult();
            }

            int anOwnerIdx = myIndexes.Value(theRank);
            return mystored.FindFromIndex(anOwnerIdx);
        }

        private void Standard_OutOfRange_Raise_if(bool v1, string v2)
        {
            if (v1)
                throw new Standard_OutOfRange(v2);
        }

        //=======================================================================
        // Function: Pick
        // Purpose :
        //=======================================================================
        public void Pick(int theXPMin,
                                       int theYPMin,
                                       int theXPMax,
                                       int theYPMax,
                                       V3d_View theView)
        {
            updateZLayers(theView);

            gp_Pnt2d aMinMousePos = new gp_Pnt2d((theXPMin), (theYPMin));
            gp_Pnt2d aMaxMousePos = new gp_Pnt2d((theXPMax), (theYPMax));
            mySelectingVolumeMgr.InitBoxSelectingVolume(aMinMousePos,
                                                         aMaxMousePos);

            mySelectingVolumeMgr.SetCamera(theView.Camera());
            int aWidth = 0, aHeight = 0;
            theView.Window().Size(out aWidth, out aHeight);
            mySelectingVolumeMgr.SetWindowSize(aWidth, aHeight);

          //  mySelectingVolumeMgr.BuildSelectingVolume();
          //  mySelectingVolumeMgr.SetViewClipping(theView.ClipPlanes(), Handle(Graphic3d_SequenceOfHClipPlane)(), NULL);
            //TraverseSensitives(theView.View().Identification());
        }

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

        //! Returns the number of detected owners.
        public int NbPicked()
        {
            return mystored.Extent();

        }

        SelectMgr_SelectableObjectSet mySelectableObjects = new SelectMgr_SelectableObjectSet();

        internal void RebuildObjectsTree(bool theIsForce = false)
        {

            mySelectableObjects.MarkDirty();

            if (theIsForce)
            {

                int xx = 0;
                int yy = 0;

                mySelectingVolumeMgr.WindowSize(ref xx, ref yy);
                Graphic3d_Vec2i aWinSize = new Graphic3d_Vec2i(xx, yy);
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