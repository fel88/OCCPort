namespace OCCPort
{
    public class PrsMgr_PresentableObject
    {

        //! Return view affinity mask.
        public Graphic3d_ViewAffinity ViewAffinity() { return myViewAffinity; }


        protected PrsMgr_PresentableObject myParent;                  //!< pointer to the parent object
        protected PrsMgr_Presentations myPresentations;           //!< list of presentations
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
		PrsMgr_ListOfPresentableObjects myChildren;                //!< list of children
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
    }


}