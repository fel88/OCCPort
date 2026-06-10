using OCCPort.Common;
using TKernel;

namespace TKV3d
{
    //!  Represents the state of a given selection mode for a
    //! Selectable Object. Contains all the sensitive entities available for this mode.
    //! An interactive object can have an indefinite number of
    //! modes of selection, each representing a
    //! "decomposition" into sensitive primitives; each
    //! primitive has an Owner (SelectMgr_EntityOwner)
    //! which allows us to identify the exact entity which has
    //! been detected. Each Selection mode is identified by
    //! an index. The set of sensitive primitives which
    //! correspond to a given mode is stocked in a
    //! SelectMgr_Selection object. By Convention, the
    //! default selection mode which allows us to grasp the
    //! Interactive object in its entirety will be mode 0.
    //! AIS_Trihedron : 4 selection modes
    //! -   mode 0 : selection of a trihedron
    //! -   mode 1 : selection of the origin of the trihedron
    //! -   mode 2 : selection of the axes
    //! -   mode 3 : selection of the planes XOY, YOZ, XOZ
    //! when you activate one of modes 1 2 3 4 , you pick AIS objects of type:
    //! -   AIS_Point
    //! -   AIS_Axis (and information on the type of axis)
    //! -   AIS_Plane (and information on the type of plane).
    //!   AIS_PlaneTrihedron offers 3 selection modes:
    //! -   mode 0 : selection of the whole trihedron
    //! -   mode 1 : selection of the origin of the trihedron
    //! -   mode 2 : selection of the axes - same remarks as for the Trihedron.
    //! AIS_Shape : 7 maximum selection modes, depending
    //! on the complexity of the shape :
    //! -   mode 0 : selection of the AIS_Shape
    //! -   mode 1 : selection of the vertices
    //! -   mode 2 : selection of the edges
    //! -   mode 3 : selection of the wires
    //! -   mode 4 : selection of the faces
    //! -   mode 5 : selection of the shells
    //! -   mode 6 :   selection of the constituent solids.
    public class SelectMgr_Selection
    {

        //! Returns sensitivity of the selection
        public int  Sensitivity() { return mySensFactor; }

        //! Sets status of selection
        public void SetSelectionState(SelectMgr_StateOfSelection theState) { mySelectionState = theState; }

        //! Returns status of selection
        public SelectMgr_StateOfSelection GetSelectionState() { return mySelectionState; }

        public SelectMgr_Selection(int theModeIdx = 0)
        {
            myMode = (theModeIdx);
            mySelectionState = SelectMgr_StateOfSelection.SelectMgr_SOS_Unknown;
            myBVHUpdateStatus = SelectMgr_TypeOfBVHUpdate.SelectMgr_TBU_None;
            mySensFactor = 2;
            myIsCustomSens = false;
        }
        public void Add(Select3D_SensitiveEntity theSensitive)
        {
            // if input is null: in debug mode raise exception
            Standard_NullObject_Raise_if(theSensitive == null, "Null sensitive entity is added to the selection");
            if (theSensitive == null)
            {
                // in release mode do not add
                return;
            }

            SelectMgr_SensitiveEntity anEntity = new SelectMgr_SensitiveEntity(theSensitive);
            myEntities.Append(anEntity);
            if (mySelectionState == SelectMgr_StateOfSelection.SelectMgr_SOS_Activated
            && !anEntity.IsActiveForSelection())
            {
                anEntity.SetActiveForSelection();
            }

            if (myIsCustomSens)
            {
                anEntity.BaseSensitive().SetSensitivityFactor(mySensFactor);
            }
            else
            {
                mySensFactor = Math.Max(mySensFactor, anEntity.BaseSensitive().SensitivityFactor());
            }
        }

        private void Standard_NullObject_Raise_if(bool value, string v)
        {
            if (value)
                throw new Exception(v);
        }

        internal SelectMgr_TypeOfBVHUpdate BVHUpdateStatus()
        {
            return myBVHUpdateStatus;
        }
        SelectMgr_TypeOfUpdate myUpdateStatus;
        NCollection_Vector<SelectMgr_SensitiveEntity> myEntities;

        public void Clear()
        {
            foreach (var item in myEntities)
            {
                item.Clear();
            }

            myEntities.Clear();
        }

        SelectMgr_StateOfSelection mySelectionState;

        int mySensFactor;
        bool myIsCustomSens;

        int myMode;

        //! returns the selection mode represented by this selection
        public int Mode() { return myMode; }

        public void UpdateBVHStatus(SelectMgr_TypeOfBVHUpdate theStatus) { myBVHUpdateStatus = theStatus; }
        SelectMgr_TypeOfBVHUpdate myBVHUpdateStatus;
        //! Return entities.
        public NCollection_Vector<SelectMgr_SensitiveEntity> Entities() { return myEntities; }

        internal SelectMgr_TypeOfUpdate UpdateStatus()
        {
            return myUpdateStatus;
        }
        internal void UpdateStatus(SelectMgr_TypeOfUpdate theStatus)
        {


            myUpdateStatus = theStatus;

        }
    }
}



