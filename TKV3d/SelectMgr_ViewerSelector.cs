global using TColStd_Array1OfInteger=TKernel.NCollection_Array1<int> ;

using OCCPort.Common;
using System.Reflection.Metadata;
using TKernel;
using TKMath;
using TKService;

namespace TKV3d
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
        public void ClearPicked()
        {
            mystored.Clear();
            myIsSorted = true;
        }

        public void Deactivate(SelectMgr_Selection theSelection)
        {
            for (NCollection_Vector<SelectMgr_SensitiveEntity>.Iterator aSelEntIter = new NCollection_Vector<SelectMgr_SensitiveEntity>.Iterator(theSelection.Entities()); aSelEntIter.More(); aSelEntIter.Next())
            {
                aSelEntIter.Value().ResetSelectionActiveStatus();
            }

            if (theSelection.GetSelectionState() == SelectMgr_StateOfSelection.SelectMgr_SOS_Activated)
            {
                theSelection.SetSelectionState(SelectMgr_StateOfSelection.SelectMgr_SOS_Deactivated);

                myTolerances.Decrement(theSelection.Sensitivity());
            }
        }

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

        SelectMgr_IndexedDataMapOfOwnerCriterion mystored = new SelectMgr_IndexedDataMapOfOwnerCriterion();

        bool myIsSorted;
        TColStd_Array1OfInteger myIndexes = new TColStd_Array1OfInteger();
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
    public enum SelectMgr_SelectionType
    {

        //	Possible selection types.
        //Enumerator
        SelectMgr_SelectionType_Unknown,

        //undefined selection type
        SelectMgr_SelectionType_Point,

        //selection by point(frustum with some tolerance or axis)
        SelectMgr_SelectionType_Box,

        //rectangle selection
        SelectMgr_SelectionType_Polyline,

        //polygonal selection
    }

    //! This is an internal class containing representation of rectangular selecting frustum, created in case
    //! of point and box selection, and algorithms for overlap detection between selecting
    //! frustum and sensitive entities. The principle of frustum calculation:
    //! - for point selection: on a near view frustum plane rectangular neighborhood of
    //!                        user-picked point is created according to the pixel tolerance
    //!                        given and then this rectangle is projected onto far view frustum
    //!                        plane. This rectangles define the parallel bases of selecting frustum;
    //! - for box selection: box points are projected onto near and far view frustum planes.
    //!                      These 2 projected rectangles define parallel bases of selecting frustum.
    //! Overlap detection tests are implemented according to the terms of separating axis
    //! theorem (SAT).
    //! Vertex order:
    //! - for triangular frustum: V0_Near, V1_Near, V2_Near,
    //!                           V0_Far, V1_Far, V2_Far;
    //! - for rectangular frustum: LeftTopNear, LeftTopFar,
    //!                            LeftBottomNear,LeftBottomFar,
    //!                            RightTopNear, RightTopFar,
    //!                            RightBottomNear, RightBottomFar.
    //! Plane order in array:
    //! - for triangular frustum: V0V1, V1V2, V0V2, Near, Far;
    //! - for rectangular frustum: Top, Bottom, Left, Right, Near, Far.
    //! Uncollinear edge directions order:
    //! - for rectangular frustum: Horizontal, Vertical,
    //!                            LeftLower, RightLower,
    //!                            LeftUpper, RightUpper;
    //! - for triangular frustum: V0_Near - V0_Far, V1_Near - V1_Far, V2_Near - V2_Far,
    //!                           V1_Near - V0_Near, V2_Near - V1_Near, V2_Near - V0_Near.
    public class SelectMgr_Frustum : SelectMgr_BaseFrustum
    {
    }

    //! This class is an interface for different types of selecting frustums,
    //! defining different selection types, like point, box or polyline
    //! selection. It contains signatures of functions for detection of
    //! overlap by sensitive entity and initializes some data for building
    //! the selecting frustum
    public class SelectMgr_BaseFrustum : SelectMgr_BaseIntersector
    {
    }
    //! An internal class for calculation of current largest tolerance value which will be applied for creation of selecting frustum by default.
    //! Each time the selection set is deactivated, maximum tolerance value will be recalculated.
    //! If a user enables custom precision using StdSelect_ViewerSelector3d::SetPixelTolerance, it will be applied to all sensitive entities without any checks.
    public class SelectMgr_ToleranceMap
    {
        //! Decrements a counter of the tolerance given, checks if the current tolerance value
        //! should be recalculated
        public void Decrement(int theTolerance)
        {
            int? aFreq = myTolerances.ChangeSeek(theTolerance);
            if (aFreq == null)
            {
                return;
            }

            Exceptions.Standard_ProgramError_Raise_if(aFreq.Value == 0, "SelectMgr_ToleranceMap::Decrement() - internal error");
            //--(*aFreq);// todo: make return ref-pointers to use

            if (theTolerance == myLargestKey
            && aFreq.Value == 0)
            {
                myLargestKey = -1;
                for (NCollection_DataMap<int, int>.Iterator anIter = new(myTolerances); anIter.More(); anIter.Next())
                {
                    if (anIter.Value() != 0)
                    {
                        myLargestKey = Math.Max(myLargestKey, anIter.Key());
                    }
                }
            }
        }
        NCollection_DataMap<int, int> myTolerances = new NCollection_DataMap<int, int>();
        int myLargestKey;
        int myCustomTolerance;
        //! Returns a current tolerance that must be applied
        public int Tolerance()
        {
            if (myLargestKey < 0)
            {
                return 2; // default tolerance value
            }
            return myCustomTolerance < 0
                 ? myLargestKey
                 : myLargestKey + myCustomTolerance;
        }
    }

    //! This class provides data and criterion for sorting candidate
    //! entities in the process of interactive selection by mouse click
    public class SelectMgr_SortCriterion
    {
        public Select3D_SensitiveEntity Entity; //!< detected entity
        public gp_Pnt Point;           //!< 3D point
        public Graphic3d_Vec3 Normal;          //!< surface normal or 0 vector if undefined
        public double Depth;           //!< distance from the view plane to the entity
        public double MinDist;         //!< distance from the clicked point to the entity on the view plane
        public double Tolerance;       //!< tolerance used for selecting candidates
        public int Priority;        //!< selection priority
        public int ZLayerPosition;  //!< ZLayer rendering order index, stronger than a depth
        public int NbOwnerMatches;  //!< overall number of entities collected for the same owner
    }
    public class SelectMgr_ViewClipRange
    {
    }

    public class SelectMgr_SelectableObjectSet
    {
        public SelectMgr_SelectableObjectSet()
        {
            for (int i = 0; i < myObjects.Length; i++)
            {
                myObjects[i] = new List<SelectMgr_SelectableObject>();
            }
        }
        //! is represented directly in eye space coordinates.
        //! This subset uses linear BVH builder with 32 levels of depth and 1 element per leaf.
        enum BVHSubset
        {
            BVHSubset_3d,
            BVHSubset_3dPersistent,
            BVHSubset_2dPersistent,
            BVHSubsetNb
        };

        Graphic3d_Vec2i myLastWinSize;          //!< Last viewport's (window's) width used for construction of BVH
        bool[] myIsDirty = new bool[(int)BVHSubset.BVHSubsetNb]; //!< Dirty flag for each subset

        internal void MarkDirty()
        {

            myIsDirty[(int)BVHSubset.BVHSubset_3d] = true;
            myIsDirty[(int)BVHSubset.BVHSubset_3dPersistent] = true;
            myIsDirty[(int)BVHSubset.BVHSubset_2dPersistent] = true;

        }

        internal void UpdateBVH(Graphic3d_Camera theCam, Graphic3d_Vec2i theWinSize)
        {
            //// -----------------------------------------
            //// check and update 3D BVH tree if necessary
            //// -----------------------------------------
            //if (!IsEmpty(BVHSubset_3d) && myIsDirty[BVHSubset_3d])
            //{
            //	// construct adaptor over private fields to provide direct access for the BVH builder
            //	BVHBuilderAdaptorRegular anAdaptor(myObjects[BVHSubset_3d]);

            //	// update corresponding BVH tree data structure
            //	myBuilder[BVHSubset_3d]->Build(&anAdaptor, myBVH[BVHSubset_3d].get(), anAdaptor.Box());

            //	// release dirty state
            //	myIsDirty[BVHSubset_3d] = Standard_False;
            //}

            //if (!theCam.IsNull())
            //{
            //	 bool isWinSizeChanged = myLastWinSize != theWinSize;
            //	 Graphic3d_Mat4d aProjMat = theCam.ProjectionMatrix();
            //	 Graphic3d_Mat4d aWorldViewMat = theCam.OrientationMatrix();
            //	 Graphic3d_WorldViewProjState aViewState = theCam.WorldViewProjState();

            //	// -----------------------------------------------------
            //	// check and update 3D persistence BVH tree if necessary
            //	// -----------------------------------------------------
            //	if (!IsEmpty(BVHSubset_3dPersistent)
            //	 && (myIsDirty[BVHSubset_3dPersistent]
            //	  || myLastViewState.IsChanged(aViewState)
            //	  || isWinSizeChanged))
            //	{
            //		// construct adaptor over private fields to provide direct access for the BVH builder
            //		BVHBuilderAdaptorPersistent anAdaptor(myObjects[BVHSubset_3dPersistent],
            //											   theCam, aProjMat, aWorldViewMat, theWinSize);

            //		// update corresponding BVH tree data structure
            //		myBuilder[BVHSubset_3dPersistent]->Build(&anAdaptor, myBVH[BVHSubset_3dPersistent].get(), anAdaptor.Box());
            //	}

            //	// -----------------------------------------------------
            //	// check and update 2D persistence BVH tree if necessary
            //	// -----------------------------------------------------
            //	if (!IsEmpty(BVHSubset_2dPersistent)
            //	 && (myIsDirty[BVHSubset_2dPersistent]
            //	  || myLastViewState.IsProjectionChanged(aViewState)
            //	  || isWinSizeChanged))
            //	{
            //		// construct adaptor over private fields to provide direct access for the BVH builder
            //		BVHBuilderAdaptorPersistent anAdaptor(myObjects[BVHSubset_2dPersistent],
            //											   theCam, aProjMat, SelectMgr_SelectableObjectSet_THE_IDENTITY_MAT, theWinSize);

            //		// update corresponding BVH tree data structure
            //		myBuilder[BVHSubset_2dPersistent]->Build(&anAdaptor, myBVH[BVHSubset_2dPersistent].get(), anAdaptor.Box());
            //	}

            //	// release dirty state for every subset
            //	myIsDirty[BVHSubset_3dPersistent] = Standard_False;
            //	myIsDirty[BVHSubset_2dPersistent] = Standard_False;

            //	// keep last view state
            //	myLastViewState = aViewState;
            //}

            //// keep last window state
            //myLastWinSize = theWinSize;

        }

        List<SelectMgr_SelectableObject>[] myObjects = new List<SelectMgr_SelectableObject>[(int)BVHSubset.BVHSubsetNb]; //!< Map of objects for each subset

        internal bool Contains(SelectMgr_SelectableObject theObject)
        {

            return myObjects[(int)BVHSubset.BVHSubset_3d].Contains(theObject)
                || myObjects[(int)BVHSubset.BVHSubset_3dPersistent].Contains(theObject)
                || myObjects[(int)BVHSubset.BVHSubset_2dPersistent].Contains(theObject);

        }

        internal void Append(SelectMgr_SelectableObject theObject)
        {

        }
    }
    internal class SelectMgr_IndexedDataMapOfOwnerCriterion : NCollection_IndexedDataMap<SelectMgr_EntityOwner, SelectMgr_SortCriterion, NCollection_DefaultHasher<SelectMgr_EntityOwner>>
    {

    }
}


