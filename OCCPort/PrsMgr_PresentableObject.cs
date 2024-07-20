using OCCPort.Tester;
using System;

namespace OCCPort
{
    public abstract class PrsMgr_PresentableObject
    {

        public PrsMgr_PresentableObject()
        {
            myChildren = new PrsMgr_ListOfPresentableObjects();
        }

        //! Return view affinity mask.
        public Graphic3d_ViewAffinity ViewAffinity() { return myViewAffinity; }
        //! Return presentations.
        public PrsMgr_Presentations Presentations() { return myPresentations; }

        //! Returns the display mode setting of the Interactive Object.
        //! The range of supported display mode indexes should be specified within object definition and filtered by AccepDisplayMode().
        //! @sa AcceptDisplayMode()
        public int DisplayMode() { return myDrawer.DisplayMode(); }

        //! Sets the display mode for the interactive object.
        //! An object can have its own temporary display mode, which is different from that proposed by the interactive context.
        //! @sa AcceptDisplayMode()
        public void SetDisplayMode(int theMode)
        {
            if (AcceptDisplayMode(theMode))
            {
                myDrawer.SetDisplayMode(theMode);
            }
        }



        protected PrsMgr_PresentableObject myParent;                  //!< pointer to the parent object
        public PrsMgr_Presentations myPresentations = new PrsMgr_Presentations();           //!< list of presentations
        protected Graphic3d_ViewAffinity myViewAffinity;            //!< view affinity mask
        protected Graphic3d_SequenceOfHClipPlane myClipPlanes;              //!< sequence of object-specific clipping planes
        protected Prs3d_Drawer myDrawer;                  //!< main presentation attributes
        protected Prs3d_Drawer myHilightDrawer;           //!< (optional) custom presentation attributes for highlighting selected object

        //! Returns true if the interactive object is infinite; FALSE by default.
        //! This flag affects various operations operating on bounding box of graphic presentations of this object.
        //! For instance, infinite objects are not taken in account for View FitAll.
        //! This does not necessarily means that object is actually infinite,
        //! auxiliary objects might be also marked with this flag to achieve desired behavior.
        public bool IsInfinite() { return myInfiniteState; }

        public PrsMgr_PresentableObject(PrsMgr_TypeOfPresentation3d theType)
        {
            myParent = null;
            myViewAffinity = new Graphic3d_ViewAffinity();
            myDrawer = (new Prs3d_Drawer());
            myTypeOfPresentation3d = (theType);
            myDisplayStatus = (PrsMgr_DisplayStatus.PrsMgr_DisplayStatus_None);
            //
            myCurrentFacingModel = (Aspect_TypeOfFacingModel.Aspect_TOFM_BOTH_SIDE);
            myOwnWidth = (0.0f);
            hasOwnColor = (false);
            hasOwnMaterial = (false);
            //
            myInfiniteState = (false);
            myIsMutable = (false);
            myHasOwnPresentations = (true);
            myToPropagateVisualState = (true);

            myDrawer.SetDisplayMode(-1);
        }


        //! Returns true if the class of objects accepts specified display mode index.
        //! The interactive context can have a default mode of representation for the set of Interactive Objects.
        //! This mode may not be accepted by a given class of objects.
        //! Consequently, this virtual method allowing us to get information about the class in question must be implemented.
        //! At least one display mode index should be accepted by this method.
        //! Although subclass can leave default implementation, it is highly desired defining exact list of supported modes instead,
        //! which is usually an enumeration for one object or objects class sharing similar list of display modes.
        public virtual bool AcceptDisplayMode(int theMode)
        {
            //(void ) theMode;
            return true;
        }


        Prs3d_Drawer myDynHilightDrawer;        //!< (optional) custom presentation attributes for highlighting detected object
        Graphic3d_TransformPers myTransformPersistence;    //!< transformation persistence
        TopLoc_Datum3D myLocalTransformation;     //!< local transformation relative to parent object
        TopLoc_Datum3D myTransformation;          //!< absolute transformation of this object (combined parents + local transformations)
        TopLoc_Datum3D myCombinedParentTransform; //!< transformation of parent object (combined for all parents)
        PrsMgr_ListOfPresentableObjects myChildren = new PrsMgr_ListOfPresentableObjects();                //!< list of children
        gp_GTrsf myInvTransformation;       //!< inversion of absolute transformation (combined parents + local transformations)
        PrsMgr_TypeOfPresentation3d myTypeOfPresentation3d;    //!< presentation type
        PrsMgr_DisplayStatus myDisplayStatus;           //!< presentation display status

        Aspect_TypeOfFacingModel myCurrentFacingModel;      //!< current facing model
        float myOwnWidth;                //!< custom width value
        bool hasOwnColor;               //!< own color flag
        bool hasOwnMaterial;            //!< own material flag

        bool myInfiniteState;           //!< infinite flag
        bool myIsMutable;               //!< mutable flag
        bool myHasOwnPresentations;     //!< flag indicating if object should have own presentations

        bool myToPropagateVisualState;  //!< flag indicating if visual state (display/erase/color) should be propagated to all children


        //! Returns true if the Interactive Object has display mode setting overriding global setting (within Interactive Context).
        public bool HasDisplayMode() { return myDrawer.DisplayMode() != -1; }
        //! Returns true if object should have own presentations.
        public bool HasOwnPresentations() { return myHasOwnPresentations; }



        //! Returns true if the Interactive Object is in highlight mode.
        //! @sa HilightAttributes()
        public bool HasHilightMode() { return myHilightDrawer != null && myHilightDrawer.DisplayMode() != -1; }

        //! Returns children of the current object.
        public PrsMgr_ListOfPresentableObjects Children() { return myChildren; }
        public void AddChild(PrsMgr_PresentableObject theObject)
        {
            PrsMgr_PresentableObject aHandleGuard = theObject;
            if (theObject.myParent != null)
            {
                theObject.myParent.RemoveChild(aHandleGuard);
            }

            myChildren.Append(theObject);
            theObject.myParent = this;
            theObject.SetCombinedParentTransform(myTransformation);
        }

        private void RemoveChild(PrsMgr_PresentableObject theObject)
        {
            PrsMgr_ListOfPresentableObjectsIter anIter = new PrsMgr_ListOfPresentableObjectsIter(myChildren);
            for (; anIter.More(); anIter.Next())
            {
                if (anIter.Value() == theObject)
                {
                    theObject.myParent = null;
                    theObject.SetCombinedParentTransform(new TopLoc_Datum3D());
                    myChildren.Remove(anIter);
                    break;
                }
            }

        }

        private void SetCombinedParentTransform(TopLoc_Datum3D theTrsf)
        {
            myCombinedParentTransform = theTrsf;
            UpdateTransformation();

        }

        private void UpdateTransformation()
        {

            myTransformation = null;
            myInvTransformation = new gp_GTrsf(new gp_Trsf());
            if (myCombinedParentTransform != null && myCombinedParentTransform.Form() != gp_TrsfForm.gp_Identity)
            {
                if (myLocalTransformation != null && myLocalTransformation.Form() != gp_TrsfForm.gp_Identity)
                {
                    gp_Trsf aTrsf = myCombinedParentTransform.Trsf() * myLocalTransformation.Trsf();
                    myTransformation = new TopLoc_Datum3D(aTrsf);
                    myInvTransformation = aTrsf.Inverted();
                }
                else
                {
                    myTransformation = myCombinedParentTransform;
                    myInvTransformation = myCombinedParentTransform.Trsf().Inverted();
                }
            }
            else if (myLocalTransformation != null && myLocalTransformation.Form() != gp_TrsfForm.gp_Identity)
            {
                myTransformation = myLocalTransformation;
                myInvTransformation = myLocalTransformation.Trsf().Inverted();
            }


        }

        public abstract void Compute(PrsMgr_PresentationManager myPresentationManager,
            Prs3d_Presentation prsMgr_Presentation, int aDispMode)
        ;
    }


}