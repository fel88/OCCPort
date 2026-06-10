using TKMath;
using TKService;

namespace TKV3d
{
    //! A framework to supply the Graphic3d structure of the object to be presented.
    //! On the first display request, this structure is created by calling the appropriate algorithm and retaining this framework for further display.
    //! This abstract framework is inherited in Application Interactive Services (AIS), notably by AIS_InteractiveObject.
    //! Consequently, 3D presentation should be handled by the relevant daughter classes and their member functions in AIS.
    //! This is particularly true in the creation of new interactive objects.
    //!
    //! Key interface methods to be implemented by every Selectable Object:
    //! - AcceptDisplayMode() accepting display modes implemented by this object;
    //! - Compute() computing presentation for the given display mode index.
    //!
    //! Warning! Methods managing standard attributes (SetColor(), SetWidth(), SetMaterial()) have different meaning for objects of different type (or no meaning at all).
    //! Sub-classes might override these methods to modify Prs3d_Drawer or class properties providing a convenient short-cut depending on application needs.
    //! For more sophisticated configuring, Prs3d_Drawer should be modified directly, while short-cuts might be left unimplemented.
    public abstract class PrsMgr_PresentableObject
    {
        public virtual void ResetTransformation()
        {
            setLocalTransformation(null);
        }
        //! Sets local transformation to theTransformation.
        //! Note that the local transformation of the object having Transformation Persistence
        //! is applied within Local Coordinate system defined by this Persistence.
        public void SetLocalTransformation(gp_Trsf theTrsf) { setLocalTransformation(new TopLoc_Datum3D(theTrsf)); }


        public void setLocalTransformation(TopLoc_Datum3D theTransformation)
        {
            myLocalTransformation = theTransformation;
            UpdateTransformation();
        }

        //! Returns parent of current object in scene hierarchy.
        public PrsMgr_PresentableObject Parent() { return myParent; }

        public PrsMgr_PresentableObject()
        {
            myChildren = new PrsMgr_ListOfPresentableObjects();
        }
        //! Get ID of Z layer for main presentation.
        public Graphic3d_ZLayerId ZLayer() { return myDrawer.ZLayer(); }
        //! Return view affinity mask.
        public Graphic3d_ViewAffinity ViewAffinity() { return myViewAffinity; }
        //! Return presentations.
        public PrsMgr_Presentations Presentations() { return myPresentations; }

        //! Returns true if object has a transformation that is different from the identity.
        public bool HasTransformation() { return myTransformation != null && myTransformation.Form() != gp_TrsfForm.gp_Identity; }

        //! Returns the display mode setting of the Interactive Object.
        //! The range of supported display mode indexes should be specified within object definition and filtered by AccepDisplayMode().
        //! @sa AcceptDisplayMode()
        public int DisplayMode() { return myDrawer.DisplayMode(); }
        //! Return the local transformation.
        //! Note that the local transformation of the object having Transformation Persistence
        //! is applied within Local Coordinate system defined by this Persistence.
        public TopLoc_Datum3D LocalTransformationGeom() { return myLocalTransformation; }

        //! Return the transformation taking into account transformation of parent object(s).
        //! Note that the local transformation of the object having Transformation Persistence
        //! is applied within Local Coordinate system defined by this Persistence.
        public gp_Trsf Transformation()
        {
            return myTransformation != null
                                                      ? myTransformation.Trsf()
                                                      : getIdentityTrsf();
        }

        gp_Trsf getIdentityTrsf()
        {
            return new gp_Trsf();
        }

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
        public void SetMaterial(Graphic3d_MaterialAspect theMaterial)
        {
            myDrawer.SetupOwnShadingAspect();
            myDrawer.ShadingAspect().SetMaterial(theMaterial);
            hasOwnMaterial = true;
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
        //! Return presentation display status; PrsMgr_DisplayStatus_None by default.
        public PrsMgr_DisplayStatus DisplayStatus() { return myDisplayStatus; }

        //! Returns highlight display mode.
        //! This is obsolete method for backward compatibility - use ::HilightAttributes() and ::DynamicHilightAttributes() instead.
        //! @sa HilightAttributes()
        public int HilightMode() { return myHilightDrawer != null ? myHilightDrawer.DisplayMode() : -1; }

        protected Prs3d_Drawer myDynHilightDrawer;        //!< (optional) custom presentation attributes for highlighting detected object
        protected Graphic3d_TransformPers myTransformPersistence;    //!< transformation persistence
        protected TopLoc_Datum3D myLocalTransformation;     //!< local transformation relative to parent object
        protected TopLoc_Datum3D myTransformation;          //!< absolute transformation of this object (combined parents + local transformations)
        protected TopLoc_Datum3D myCombinedParentTransform; //!< transformation of parent object (combined for all parents)
        protected PrsMgr_ListOfPresentableObjects myChildren = new PrsMgr_ListOfPresentableObjects();                //!< list of children
        protected gp_GTrsf myInvTransformation;       //!< inversion of absolute transformation (combined parents + local transformations)
        protected PrsMgr_TypeOfPresentation3d myTypeOfPresentation3d;    //!< presentation type
        protected PrsMgr_DisplayStatus myDisplayStatus;           //!< presentation display status

        protected Aspect_TypeOfFacingModel myCurrentFacingModel;      //!< current facing model
        protected float myOwnWidth;                //!< custom width value
        protected bool hasOwnColor;               //!< own color flag
        protected bool hasOwnMaterial;            //!< own material flag

        protected bool myInfiniteState;           //!< infinite flag
        protected bool myIsMutable;               //!< mutable flag
        protected bool myHasOwnPresentations;     //!< flag indicating if object should have own presentations

        protected bool myToPropagateVisualState;  //!< flag indicating if visual state (display/erase/color) should be propagated to all children


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


        //! Return the local transformation.
        //! Note that the local transformation of the object having Transformation Persistence
        //! is applied within Local Coordinate system defined by this Persistence.
        public gp_Trsf LocalTransformation()
        {
            return myLocalTransformation != null
                                                           ? myLocalTransformation.Trsf()
                                                           : getIdentityTrsf();
        }

        public abstract void Compute(PrsMgr_PresentationManager myPresentationManager,
            Prs3d_Presentation prsMgr_Presentation, int aDispMode)
        ;

        //! Fills the given 3D view presentation for specified display mode using Compute() method.
        //! In addition, configures other properties of presentation (transformation, clipping planes).
        //! @param thePrsMgr presentation manager where presentation has been created
        //! @param thePrs    presentation to fill
        //! @param theMode   display mode to compute; can be any number accepted by AcceptDisplayMode() method
        internal void Fill(PrsMgr_PresentationManager thePrsMgr, PrsMgr_Presentation thePrs, int theMode)
        {
            Prs3d_Presentation aStruct3d = thePrs;
            Compute(thePrsMgr, aStruct3d, theMode);
            aStruct3d.SetTransformation(myTransformation);
            aStruct3d.SetClipPlanes(myClipPlanes);
            aStruct3d.SetTransformPersistence(TransformPersistence());
        }

        //! Returns Transformation Persistence defining a special Local Coordinate system where this presentable object is located or NULL handle if not defined.
        //! Position of the object having Transformation Persistence is mutable and depends on camera position.
        //! The same applies to a bounding box of the object.
        //! @sa Graphic3d_TransformPers class description
        public Graphic3d_TransformPers TransformPersistence() { return myTransformPersistence; }

    }
}

