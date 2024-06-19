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


        protected PrsMgr_PresentableObject myParent;                  //!< pointer to the parent object
		public PrsMgr_Presentations myPresentations=new PrsMgr_Presentations ();           //!< list of presentations
        protected Graphic3d_ViewAffinity myViewAffinity;            //!< view affinity mask
        protected Graphic3d_SequenceOfHClipPlane myClipPlanes;              //!< sequence of object-specific clipping planes
        protected Prs3d_Drawer myDrawer;                  //!< main presentation attributes
        protected Prs3d_Drawer myHilightDrawer;           //!< (optional) custom presentation attributes for highlighting selected object

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
		public bool  HasOwnPresentations() { return myHasOwnPresentations; }

	public virtual bool AcceptDisplayMode(int theMode)
		{
			return true;
		}

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

        public  abstract void Compute(PrsMgr_PresentationManager myPresentationManager,
            Prs3d_Presentation prsMgr_Presentation, int aDispMode)
        ;
    }


}