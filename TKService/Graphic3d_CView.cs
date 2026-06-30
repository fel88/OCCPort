using System.Drawing;
using System.Xml.Linq;
using TKernel;
using TKMath;

namespace TKService
{

    //! Base class of a graphical view that carries out rendering process for a concrete
    //! implementation of graphical driver. Provides virtual interfaces for redrawing its
    //! contents, management of displayed structures and render settings. The source code 
    //! of the class itself implements functionality related to management of
    //! computed (HLR or "view-dependent") structures.
    public abstract class Graphic3d_CView : Graphic3d_DataStructureManager
    {
        public void ChangeZLayer(Graphic3d_Structure theStructure,
                                    Graphic3d_ZLayerId theLayerId)
        {
            if (!IsActive()
            || !IsDisplayed(theStructure))
            {
                return;
            }

            if (!myIsInComputedMode)
            {
                changeZLayer(theStructure.CStructure(), theLayerId);
                return;
            }

            int anIndex = IsComputed(theStructure);
            Graphic3d_CStructure aCStruct = anIndex != 0
                                           ? myStructsComputed.Value(anIndex).CStructure()
                                           : theStructure.CStructure();

            changeZLayer(aCStruct, theLayerId);
        }

        //! Change Z layer of a structure already presented in view.
        public abstract void changeZLayer(Graphic3d_CStructure theCStructure,
                             Graphic3d_ZLayerId theNewLayerId);

        public Graphic3d_CView(Graphic3d_StructureManager theMgr)
        {
            myCamera = new Graphic3d_Camera();
            myStructureManager = theMgr;
            myId = myStructureManager.Identification(this);

        }

        //! Return subview top-left position relative to parent view in pixels.
        public Graphic3d_Vec2i SubviewTopLeft() { return mySubviewTopLeft; }

        //! Sets gradient background fill colors.
        public abstract void SetGradientBackground(Aspect_GradientBackground theBackground);

        //! Transforms the structure in the view.
        public void SetTransform(Graphic3d_Structure theStructure,
                                     TopLoc_Datum3D theTrsf)
        {
            int anIndex = IsComputed(theStructure);
            if (anIndex != 0)
            {
                // Test is somewhat light !
                // trsf is transferred only if it is :
                // a translation
                // a scale
                if (theTrsf != null
                  && (theTrsf.Form() == gp_TrsfForm.gp_Translation
                   || theTrsf.Form() == gp_TrsfForm.gp_Scale
                   || theTrsf.Form() == gp_TrsfForm.gp_CompoundTrsf))
                {
                    ReCompute(theStructure);
                }
                else
                {
                    Graphic3d_Structure aCompStruct = myStructsComputed.ChangeValue(anIndex);
                    aCompStruct.GraphicTransform(theTrsf);
                }
            }

            theStructure.CalculateBoundBox();
            if (!theStructure.IsMutable()
             && !theStructure.CStructure().IsForHighlight
             && !theStructure.CStructure().IsInfinite)
            {
                Graphic3d_ZLayerId aLayerId = theStructure.GetZLayer();
                InvalidateBVHData(aLayerId);
            }
        }
        //! Marks BVH tree and the set of BVH primitives of correspondent priority list with id theLayerId as outdated.
        public abstract void InvalidateBVHData(Graphic3d_ZLayerId theLayerId);


        public void ReCompute(Graphic3d_Structure theStruct)
        {
            theStruct.CalculateBoundBox();
            if (!theStruct.IsMutable()
             && !theStruct.CStructure().IsForHighlight
             && !theStruct.CStructure().IsInfinite)
            {
                Graphic3d_ZLayerId aLayerId = theStruct.GetZLayer();
                //InvalidateBVHData(aLayerId);
            }

            if (!ComputedMode()
             || !IsActive()
             || !theStruct.IsDisplayed())
            {
                return;
            }

            Graphic3d_TypeOfAnswer anAnswer = acceptDisplay(theStruct.Visual());
            if (anAnswer != Graphic3d_TypeOfAnswer.Graphic3d_TOA_COMPUTE)
            {
                return;
            }

            int anIndex = IsComputed(theStruct);
            if (anIndex == 0)
            {
                return;
            }

            // compute + validation
            Graphic3d_Structure aCompStructOld = myStructsComputed.ChangeValue(anIndex);
            Graphic3d_Structure aCompStruct = aCompStructOld;
            //aCompStruct.SetTransformation(null);
            //  theStruct.computeHLR(myCamera, aCompStruct);
            if (aCompStruct == null)
            {
                return;
            }

            aCompStruct.SetHLRValidation(true);
            aCompStruct.CalculateBoundBox();

            // of which type will be the computed?
            bool toComputeWireframe = myVisualization == Graphic3d_TypeOfVisualization.Graphic3d_TOV_WIREFRAME
                                                     && theStruct.ComputeVisual() != Graphic3d_TypeOfStructure.Graphic3d_TOS_SHADING;
            bool toComputeShading = myVisualization == Graphic3d_TypeOfVisualization.Graphic3d_TOV_SHADING
                                                     && theStruct.ComputeVisual() != Graphic3d_TypeOfStructure.Graphic3d_TOS_WIREFRAME;
            if (toComputeWireframe)
            {
                aCompStruct.SetVisual(Graphic3d_TypeOfStructure.Graphic3d_TOS_WIREFRAME);
            }
            else if (toComputeShading)
            {
                aCompStruct.SetVisual(Graphic3d_TypeOfStructure.Graphic3d_TOS_SHADING);
            }

            if (theStruct.IsHighlighted())
            {
                //   aCompStruct.Highlight(theStruct.HighlightStyle(), false);
            }

            // The previous calculation is removed and the new one is displayed
            eraseStructure(aCompStructOld.CStructure());
            displayStructure(aCompStruct.CStructure(), theStruct.DisplayPriority());

            // why not just replace existing items?
            //myStructsToCompute.ChangeValue (anIndex) = theStruct;
            //myStructsComputed .ChangeValue (anIndex) = aCompStruct;

            // hlhsr and the new associated compute are added
            myStructsToCompute.Append(theStruct);
            myStructsComputed.Append(aCompStruct);

            // hlhsr and the new associated compute are removed
            myStructsToCompute.Remove(anIndex);
            myStructsComputed.Remove(anIndex);
        }
        public void Erase(Graphic3d_Structure theStructure)
        {
            if (!IsDisplayed(theStructure))
            {
                return;
            }

            Graphic3d_TypeOfAnswer anAnswer = myIsInComputedMode ? acceptDisplay(theStructure.Visual()) : Graphic3d_TypeOfAnswer.Graphic3d_TOA_YES;
            if (anAnswer != Graphic3d_TypeOfAnswer.Graphic3d_TOA_COMPUTE)
            {
                eraseStructure(theStructure.CStructure());
            }

            int anIndex = !myStructsToCompute.IsEmpty() ? IsComputed(theStructure) : 0;
            if (anIndex != 0)
            {
                if (anAnswer == Graphic3d_TypeOfAnswer.Graphic3d_TOA_COMPUTE
                 && myIsInComputedMode)
                {
                    Graphic3d_Structure aCompStruct = myStructsComputed.ChangeValue(anIndex);
                    eraseStructure(aCompStruct.CStructure());
                }
                myStructsComputed.Remove(anIndex);
                myStructsToCompute.Remove(anIndex);
            }

            myStructsDisplayed.Remove(theStructure);
            Update(theStructure.GetZLayer());
        }



        //! Returns anchor camera definition (without tracked head orientation).
        public Graphic3d_Camera BaseXRCamera() { return myBaseXRCamera; }
        Graphic3d_Camera myBaseXRCamera;       //!< neutral camera orientation defining coordinate system in which head tracking is defined

        public void Connect(Graphic3d_Structure theMother,
                               Graphic3d_Structure theDaughter)
        {
            int anIndexM = IsComputed(theMother);
            int anIndexD = IsComputed(theDaughter);
            if (anIndexM != 0
             && anIndexD != 0)
            {
                Graphic3d_Structure aStructM = myStructsComputed.Value(anIndexM);
                Graphic3d_Structure aStructD = myStructsComputed.Value(anIndexD);
                aStructM.GraphicConnect(aStructD);
            }
        }

        public double ConsiderZoomPersistenceObjects()
        {
            if (!IsDefined())
            {
                return 1.0;
            }

            Graphic3d_Camera aCamera = Camera();
            Graphic3d_Vec2i aWinSize = new NCollection_Vec2<int>();
            int x = 0;
            int y = 0;
            Window().Size(out x, out y);
            aWinSize.X = x;
            aWinSize.Y = y;

            double aMaxCoef = 1.0;
            foreach (var aLayer in Layers())
            {
                aMaxCoef = Math.Max(aMaxCoef, aLayer.considerZoomPersistenceObjects(Identification(), aCamera, aWinSize.X, aWinSize.Y));
            }

            return aMaxCoef;
        }
        //! Returns number of displayed structures in the view.
        public virtual int NumberOfDisplayedStructures() { return myStructsDisplayed.Extent(); }
        //! Returns the identification number of the view.
        public int Identification() { return myId; }

        //! Returns layer with given ID or NULL if undefined.
        public abstract Graphic3d_Layer Layer(Graphic3d_ZLayerId theLayerId);
        //! Returns the structure manager handle which manage structures associated with this view.
        public Graphic3d_StructureManager StructureManager() { return myStructureManager; }
        public void Update(Graphic3d_ZLayerId theLayerId = Graphic3d_ZLayerId.Graphic3d_ZLayerId_UNKNOWN)
        {
            InvalidateZLayerBoundingBox(theLayerId);
        }

        private void InvalidateZLayerBoundingBox(Graphic3d_ZLayerId theLayerId)
        {
            Graphic3d_Layer _aLayer = Layer(theLayerId);
            if (_aLayer != null)
            {
                _aLayer.InvalidateBoundingBox();
                return;
            }

            //for (NCollection_List < Handle(Graphic3d_Layer) >::Iterator aLayerIter(Layers()); aLayerIter.More(); aLayerIter.Next())

            foreach (var aLayer in Layers())
            {
                if (aLayer.NbOfTransformPersistenceObjects() > 0)
                {
                    aLayer.InvalidateBoundingBox();
                }
            }
        }

        //! Creates and maps rendering window to the view.
        //! @param[in] theParentVIew parent view or NULL
        //! @param[in] theWindow the window
        //! @param[in] theContext the rendering context; if NULL the context will be created internally
        public abstract void SetWindow(Graphic3d_CView theParentVIew,
                          Aspect_Window theWindow,
                          Aspect_RenderingContext theContext);


        protected Graphic3d_TextureMap myBackgroundImage;
        protected bool myIsSubviewComposer;        //!< flag to skip rendering of viewer contents
        protected Graphic3d_CubeMap myCubeMapBackground;  //!< Cubemap displayed at background
        protected Graphic3d_StructureManager myStructureManager;
        protected Graphic3d_Camera myCamera;
        protected Graphic3d_SequenceOfStructure myStructsToCompute = new Graphic3d_SequenceOfStructure();
        protected Graphic3d_SequenceOfStructure myStructsComputed = new Graphic3d_SequenceOfStructure();
        protected Graphic3d_MapOfStructure myStructsDisplayed;
        protected bool myIsInComputedMode;
        protected bool myIsActive;
        protected bool myIsRemoved;
        protected Graphic3d_TypeOfBackfacingModel myBackfacing;
        protected Graphic3d_TypeOfVisualization myVisualization;


        protected Graphic3d_TextureEnv myTextureEnvData;
        protected Graphic3d_GraduatedTrihedron myGTrihedronData;
        protected Graphic3d_TypeOfBackground myBackgroundType;     //!< Current type of background
        protected Aspect_SkydomeBackground mySkydomeAspect;
        protected bool myToUpdateSkydome;


        //! Redraw content of the view.
        public abstract void Redraw();

        //! Redraw immediate content of the view.
        public abstract void RedrawImmediate();

        //! Invalidates content of the view but does not redraw it.
        public abstract void Invalidate();

        public List<Graphic3d_MapOfStructure> Items = new List<Graphic3d_MapOfStructure>();

        public void DisplayedStructures(out Graphic3d_MapOfStructure[] aSetOfStructures)
        {
            aSetOfStructures = Items.ToArray();
        }
        public abstract Graphic3d_Layer[] Layers();


        public abstract Aspect_Window Window();
        //! Returns current rendering parameters and effect settings.
        public Graphic3d_RenderingParams RenderingParams() { return myRenderParams; }

        //! Returns reference to current rendering parameters and effect settings.
        public Graphic3d_RenderingParams ChangeRenderingParams() { return myRenderParams; }

        protected int myId;
        protected Graphic3d_RenderingParams myRenderParams = new Graphic3d_RenderingParams();

        //! Returns True if the window associated to the view is defined.
        public abstract bool IsDefined();

        public Bnd_Box MinMaxValues()
        {
            return new Bnd_Box();
        }

        //! Returns camera object of the view.
        public Graphic3d_Camera Camera() { return myCamera; }

        public Bnd_Box MinMaxValues(bool theToIncludeAuxiliary)
        {
            if (!IsDefined())
            {
                return new Bnd_Box();
            }

            Graphic3d_Camera aCamera = Camera();
            Graphic3d_Vec2i aWinSize;
            int xx, yy;
            Window().Size(out xx, out yy);
            aWinSize = new Graphic3d_Vec2i(xx, yy);

            Bnd_Box aResult = new Bnd_Box();
            foreach (var aLayer in Layers())
            {
                Bnd_Box aBox = aLayer.BoundingBox(Identification(),
                                                    aCamera,
                                                    aWinSize.x(), aWinSize.y(),
                                                    theToIncludeAuxiliary);
                aResult.Add(aBox);
            }
            return aResult;
        }

        //! Returns the activity flag of the view.
        public bool IsActive() { return myIsActive; }



        //! Adds the structure to display lists of the view.
        public abstract void displayStructure(Graphic3d_CStructure theStructure,
                                      Graphic3d_DisplayPriority thePriority);


        internal void Display(Graphic3d_Structure theStructure)
        {
            if (!IsActive())
            {
                return;
            }

            //// If Display on a structure present in the list of calculated structures while it is not
            //// or more, of calculated type =>
            //// - removes it as well as the associated old computed
            //// THis happens when hlhsr becomes again of type e non computed after SetVisual.
            //int anIndex = IsComputed(theStructure);
            //if (anIndex != 0
            // && theStructure.Visual() != Graphic3d_TOS_COMPUTED)
            //{
            //	myStructsToCompute.Remove(anIndex);
            //	myStructsComputed.Remove(anIndex);
            //	anIndex = 0;
            //}

            //Graphic3d_TypeOfAnswer anAnswer = acceptDisplay(theStructure->Visual());
            //if (anAnswer == Graphic3d_TOA_NO)
            //{
            //	return;
            //}

            //if (!ComputedMode())
            //{
            //	anAnswer = Graphic3d_TOA_YES;
            //}

            //if (anAnswer == Graphic3d_TOA_YES)
            //{
            //	if (!myStructsDisplayed.Add(theStructure))
            //	{
            //		return;
            //	}

            //	theStructure.CalculateBoundBox();
            displayStructure(theStructure.CStructure(), theStructure.DisplayPriority());
            //	Update(theStructure.GetZLayer());
            //	return;
            //}
            //else if (anAnswer != Graphic3d_TOA_COMPUTE)
            //{
            //	return;
            //}

            //if (anIndex != 0)
            //{
            //	// Already computed, is COMPUTED still valid?
            //	 Graphic3d_Structure anOldStruct = myStructsComputed.Value(anIndex);
            //	if (anOldStruct.HLRValidation())
            //	{
            //		// Case COMPUTED valid, to be displayed
            //		if (!myStructsDisplayed.Add(theStructure))
            //		{
            //			return;
            //		}

            //		displayStructure(anOldStruct.CStructure(), theStructure.DisplayPriority());
            //		Update(anOldStruct.GetZLayer());
            //		return;
            //	}
            //	else
            //	{
            //		// Case COMPUTED invalid
            //		// Is there another valid representation?
            //		// Find in the sequence of already calculated structures
            //		// 1/ Structure having the same Owner as <AStructure>
            //		// 2/ That is not <AStructure>
            //		// 3/ The COMPUTED which of is valid
            //		 int aNewIndex = HaveTheSameOwner(theStructure);
            //		if (aNewIndex != 0)
            //		{
            //			// Case of COMPUTED invalid, WITH a valid of replacement; to be displayed
            //			if (!myStructsDisplayed.Add(theStructure))
            //			{
            //				return;
            //			}

            //			 Graphic3d_Structure aNewStruct = myStructsComputed.Value(aNewIndex);
            //			myStructsComputed.SetValue(anIndex, aNewStruct);
            //			displayStructure(aNewStruct.CStructure(), theStructure.DisplayPriority());
            //			Update(aNewStruct.GetZLayer());
            //			return;
            //		}
            //		else
            //		{
            //			// Case COMPUTED invalid, WITHOUT a valid of replacement
            //			// COMPUTED is removed if displayed
            //			if (myStructsDisplayed.Contains(theStructure))
            //			{
            //				eraseStructure(anOldStruct.CStructure());
            //			}
            //		}
            //	}
            //}

            //// Compute + Validation
            //Graphic3d_Structure aStruct;
            //if (anIndex != 0)
            //{
            //	aStruct = myStructsComputed.Value(anIndex);
            //	aStruct.SetTransformation(Handle(TopLoc_Datum3D)());
            //}
            //theStructure.computeHLR(myCamera, aStruct);
            //if (aStruct.IsNull())
            //{
            //	return;
            //}
            //aStruct.SetHLRValidation(true);

            //// TOCOMPUTE and COMPUTED associated to sequences are added
            //myStructsToCompute.Append(theStructure);
            //myStructsComputed.Append(aStruct);

            //// The previous are removed if necessary
            //if (anIndex != 0)
            //{
            //	myStructsToCompute.Remove(anIndex);
            //	myStructsComputed.Remove(anIndex);
            //}

            //// Of which type will be the computed?
            //const Standard_Boolean toComputeWireframe = myVisualization == Graphic3d_TOV_WIREFRAME
            //										 && theStructure->ComputeVisual() != Graphic3d_TOS_SHADING;
            //const Standard_Boolean toComputeShading = myVisualization == Graphic3d_TOV_SHADING
            //										 && theStructure->ComputeVisual() != Graphic3d_TOS_WIREFRAME;
            //if (!toComputeShading && !toComputeWireframe)
            //{
            //	anAnswer = Graphic3d_TOA_NO;
            //}
            //else
            //{
            //	aStruct->SetVisual(toComputeWireframe ? Graphic3d_TOS_WIREFRAME : Graphic3d_TOS_SHADING);
            //	anAnswer = acceptDisplay(aStruct->Visual());
            //}

            //if (theStructure->IsHighlighted())
            //{
            //	aStruct->Highlight(theStructure->HighlightStyle(), Standard_False);
            //}

            //// It is displayed only if the calculated structure
            //// has a proper type corresponding to the one of the view.
            //if (anAnswer == Graphic3d_TOA_NO)
            //{
            //	return;
            //}

            //myStructsDisplayed.Add(theStructure);
            //displayStructure(aStruct->CStructure(), theStructure->DisplayPriority());

            //Update(aStruct->GetZLayer());

        }

        public void SetComputedMode(bool theMode)
        {

            if ((theMode && myIsInComputedMode)
             || (!theMode && !myIsInComputedMode))
            {
                return;
            }

            myIsInComputedMode = theMode;
            if (!myIsInComputedMode)
            {
                foreach (var aStruct in myStructsDisplayed)
                {
                    Graphic3d_TypeOfAnswer anAnswer = acceptDisplay(aStruct.Visual());
                    if (anAnswer != Graphic3d_TypeOfAnswer.Graphic3d_TOA_COMPUTE)
                        continue;

                    int anIndex = IsComputed(aStruct);
                    if (anIndex != 0)
                    {
                        Graphic3d_Structure aStructComp = myStructsComputed.Value(anIndex);
                        eraseStructure(aStructComp.CStructure());
                        displayStructure(aStruct.CStructure(), aStruct.DisplayPriority());
                        Update(aStruct.GetZLayer());
                    }
                }
                return;

            }
        }

        public abstract void eraseStructure(Graphic3d_CStructure graphic3d_CStructure);

        internal void Clear(Graphic3d_Structure theStructure, bool theWithDestruction)
        {
            int anIndex = IsComputed(theStructure);
            if (anIndex != 0)
            {
                Graphic3d_Structure aCompStruct = myStructsComputed.Value(anIndex);
                aCompStruct.GraphicClear(theWithDestruction);
                aCompStruct.SetHLRValidation(false);
            }

        }


        // ========================================================================
        // function : IsComputed
        // purpose  :
        // ========================================================================
        public bool IsComputed(int theStructId,
                                                    ref Graphic3d_Structure theComputedStruct)
        {
            theComputedStruct = null;
            if (!ComputedMode())
                return false;

            int aNbStructs = myStructsToCompute.Length();
            for (int aStructIter = 1; aStructIter <= aNbStructs; ++aStructIter)
            {
                if (myStructsToCompute.Value(aStructIter).Identification() == theStructId)
                {
                    theComputedStruct = myStructsComputed.Value(aStructIter);
                    return true;
                }
            }
            return false;
        }
        public int IsComputed(Graphic3d_Structure theStructure)
        {
            int aStructId = theStructure.Identification();
            int aStructIndex = 1;
            foreach (var aStructIter in myStructsToCompute)
            {
                Graphic3d_Structure aStruct = aStructIter;
                if (aStruct.Identification() == aStructId)
                {
                    return aStructIndex;
                }
            }
            return 0;

        }



        //! Returns true if the view was removed.
        public bool IsRemoved()
        {
            return myIsRemoved;
        }

        public void Activate()
        {

            if (!IsActive())
            {
                myIsActive = true;

                // Activation of a new view =>
                // Display structures that can be displayed in this new view.
                // All structures with status
                // Displayed in ViewManager are returned and displayed in
                // the view directly, if the structure is not already
                // displayed and if the view accepts it in its context.
                Graphic3d_MapOfStructure aDisplayedStructs = new Graphic3d_MapOfStructure();
                myStructureManager.DisplayedStructures(ref aDisplayedStructs);
                //for (Graphic3d_MapIteratorOfMapOfStructure aStructIter (aDisplayedStructs); aStructIter.More(); aStructIter.Next())
                foreach (var aStruct in aDisplayedStructs)
                {
                    //Graphic3d_Structure aStruct = aStructIter.Key();
                    if (IsDisplayed(aStruct))
                    {
                        continue;
                    }

                    // If the structure can be displayed in the new context of the view, it is displayed.
                    Graphic3d_TypeOfAnswer anAnswer = acceptDisplay(aStruct.Visual());
                    if (anAnswer == Graphic3d_TypeOfAnswer.Graphic3d_TOA_YES
                     || anAnswer == Graphic3d_TypeOfAnswer.Graphic3d_TOA_COMPUTE)
                    {
                        Display(aStruct);
                    }
                }
            }

            Update();

        }

        private Graphic3d_TypeOfAnswer acceptDisplay(Graphic3d_TypeOfStructure theStructType)
        {

            switch (theStructType)
            {
                case Graphic3d_TypeOfStructure.Graphic3d_TOS_ALL:
                    {
                        return Graphic3d_TypeOfAnswer.Graphic3d_TOA_YES; // The structure accepts any type of view
                    }
                case Graphic3d_TypeOfStructure.Graphic3d_TOS_SHADING:
                    {
                        return myVisualization == Graphic3d_TypeOfVisualization.Graphic3d_TOV_SHADING
                             ? Graphic3d_TypeOfAnswer.Graphic3d_TOA_YES
                             : Graphic3d_TypeOfAnswer.Graphic3d_TOA_NO;
                    }
                case Graphic3d_TypeOfStructure.Graphic3d_TOS_WIREFRAME:
                    {
                        return myVisualization == Graphic3d_TypeOfVisualization.Graphic3d_TOV_WIREFRAME
                             ? Graphic3d_TypeOfAnswer.Graphic3d_TOA_YES
                             : Graphic3d_TypeOfAnswer.Graphic3d_TOA_NO;
                    }
                case Graphic3d_TypeOfStructure.Graphic3d_TOS_COMPUTED:
                    {
                        return (myVisualization == Graphic3d_TypeOfVisualization.Graphic3d_TOV_SHADING
                            || myVisualization == Graphic3d_TypeOfVisualization.Graphic3d_TOV_WIREFRAME)
                             ? Graphic3d_TypeOfAnswer.Graphic3d_TOA_COMPUTE
                             : Graphic3d_TypeOfAnswer.Graphic3d_TOA_NO;
                    }
            }
            return Graphic3d_TypeOfAnswer.Graphic3d_TOA_NO;

        }

        private bool IsDisplayed(Graphic3d_Structure aStruct)
        {
            throw new NotImplementedException();
        }
        protected NCollection_Sequence<Graphic3d_CView> mySubviews = new NCollection_Sequence<Graphic3d_CView>(); //!< list of child views

        public abstract bool IsInvalidated();
        protected Aspect_XRSession myXRSession;

        public bool IsActiveXR() => myXRSession != null
                 && myXRSession.IsOpen();
        //! Returns the computed HLR mode state
        public bool ComputedMode()
        {

            return myIsInComputedMode;
        }

        public void Compute()
        {


            // force HLRValidation to False on all structures calculated in the view
            foreach (var aStructIter in myStructsComputed)
            {
                aStructIter.SetHLRValidation(false);
            }

            if (!ComputedMode())
            {
                return;
            }

            // Change of orientation or of projection type =>
            // Remove structures that were calculated for the previous orientation.
            // Recalculation of new structures.
            List<Graphic3d_Structure> aStructsSeq = new List<Graphic3d_Structure>();
            foreach (var aStructIter in myStructsDisplayed)
            {
                Graphic3d_TypeOfAnswer anAnswer = acceptDisplay(aStructIter.Visual());
                if (anAnswer == Graphic3d_TypeOfAnswer.Graphic3d_TOA_COMPUTE)
                {
                    aStructsSeq.Add(aStructIter); // if the structure was calculated, it is recalculated
                }
            }
            foreach (var aStructIter in aStructsSeq)
            {
                //Display(aStructIter.ChangeValue());
                Display(aStructIter);
            }


        }

        public bool RemoveSubview(Graphic3d_CView theView)
        {
            foreach (var aViewIter in mySubviews)
            {
                if (aViewIter == theView)
                {
                    mySubviews.Remove(aViewIter);
                    return true;
                }
            }
            return false;
        }

        Graphic3d_Vec2i mySubviewTopLeft;           //!< subview top-left position relative to parent view
        Graphic3d_Vec2i mySubviewMargins;           //!< subview margins in pixels
        Graphic3d_Vec2d mySubviewSize;              //!< subview size
        Graphic3d_Vec2d mySubviewOffset;            //!< subview corner offset within parent view
        Aspect_TypeOfTriedronPosition mySubviewCorner;            //!< position within parent view

        protected Graphic3d_CView myParentView;               //!< back-pointer to the parent view

        //! Return TRUE if this is a subview of another view.
        public bool IsSubview() { return myParentView != null; }
        public virtual void Resized()
        {
            if (IsSubview())
            {
                Aspect_NeutralWindow aWindow = (Aspect_NeutralWindow)Window();
                SubviewResized(aWindow);
            }
        }

        //! Calculate offset in pixels from fraction.
        public static int getSubViewOffset(double theOffset, int theWinSize)
        {
            if (theOffset >= 1.0)
            {
                return (int)(theOffset);
            }
            else
            {
                return (int)(theOffset * theWinSize);
            }
        }
        public void AddSubview(Graphic3d_CView theView)
        {
            mySubviews.Add(theView);
        }
        public void SubviewResized(Aspect_NeutralWindow theWindow)
        {
            if (!IsSubview()
    || theWindow == null)
            {
                return;
            }

            Graphic3d_Vec2i aWinSize = myParentView.Window().Dimensions();
            Graphic3d_Vec2i aViewSize = new Graphic3d_Vec2i((int)(aWinSize.x() * mySubviewSize.x()), (int)(aWinSize.y() * mySubviewSize.y()));
            if (mySubviewSize.x() > 1.0)
            {
                aViewSize.x((int)mySubviewSize.x());
            }
            if (mySubviewSize.y() > 1.0)
            {
                aViewSize.y((int)mySubviewSize.y());
            }

            Graphic3d_Vec2i anOffset = new Graphic3d_Vec2i(getSubViewOffset(mySubviewOffset.x(), aWinSize.x()),
                            getSubViewOffset(mySubviewOffset.y(), aWinSize.y()));
            mySubviewTopLeft = (aWinSize - aViewSize) / 2; // Aspect_TOTP_CENTER
            if ((mySubviewCorner & Aspect_TypeOfTriedronPosition.Aspect_TOTP_LEFT) != 0)
            {
                mySubviewTopLeft.x(anOffset.X);
            }
            else if ((mySubviewCorner & Aspect_TypeOfTriedronPosition.Aspect_TOTP_RIGHT) != 0)
            {
                mySubviewTopLeft.x(Math.Max(aWinSize.X - anOffset.X - aViewSize.X, 0));
            }

            if ((mySubviewCorner & Aspect_TypeOfTriedronPosition.Aspect_TOTP_TOP) != 0)
            {
                mySubviewTopLeft.y(anOffset.Y);
            }
            else if ((mySubviewCorner & Aspect_TypeOfTriedronPosition.Aspect_TOTP_BOTTOM) != 0)
            {
                mySubviewTopLeft.y(Math.Max(aWinSize.Y - anOffset.Y - aViewSize.Y, 0));
            }

            mySubviewTopLeft += mySubviewMargins;
            aViewSize -= mySubviewMargins * 2;

            int aRight = Math.Min(mySubviewTopLeft.x() + aViewSize.X, aWinSize.X);
            aViewSize.X = aRight - mySubviewTopLeft.x();

            int aBot = Math.Min(mySubviewTopLeft.y() + aViewSize.Y, aWinSize.Y);
            aViewSize.Y = aBot - mySubviewTopLeft.y();

            theWindow.SetSize(aViewSize.X, aViewSize.Y);
        }
    }


    //! This is an abstract class for managing texture applyable on polygons.
    public class Graphic3d_TextureMap : Graphic3d_TextureRoot

    {
        Graphic3d_TextureParams myParams;     //!< associated texture parameters
        string myTexId;      //!< unique identifier of this resource (for sharing graphic resource); should never be modified outside constructor
        Image_PixMap myPixMap;     //!< image pixmap - as one of the ways for defining the texture source
                                   //OSD_Path myPath;       //!< image file path - as one of the ways for defining the texture source
                                   //Standard_Size myRevision;   //!< image revision - for signaling changes in the texture source (e.g. file update, pixmap update)
        Graphic3d_TypeOfTexture myType;       //!< texture type
        bool myIsColorMap; //!< flag indicating color nature of values within the texture
        bool myIsTopDown;  //!< Stores rows's memory layout
        bool myHasMipmaps; //!< Indicates whether mipmaps should be generated or not

        //! Returns whether row's memory layout is top-down.
        public bool IsTopDown() { return myIsTopDown; }
    }

    public class Graphic3d_TextureRoot
    {
    }

    public class Graphic3d_TextureParams
    {
        public void SetModulate(bool theToModulate)
        {
            myToModulate = theToModulate;
        }

        public void SetGenMode(Graphic3d_TypeOfTextureMode theMode,
                                            Graphic3d_Vec4 thePlaneS,
                                            Graphic3d_Vec4 thePlaneT)
        {
            myGenMode = theMode;
            myGenPlaneS = thePlaneS;
            myGenPlaneT = thePlaneT;
        }

        Graphic3d_Vec4 myGenPlaneS;       //!< texture coordinates generation plane S
        Graphic3d_Vec4 myGenPlaneT;       //!< texture coordinates generation plane T
        Graphic3d_Vec2 myScale;           //!< texture coordinates scale factor vector; (1,1) by default
        Graphic3d_Vec2 myTranslation;     //!< texture coordinates translation vector;  (0,0) by default
        uint mySamplerRevision; //!< modification counter of parameters related to sampler state
                                //  Graphic3d_TextureUnit myTextureUnit;     //!< default texture unit to bind texture; Graphic3d_TextureUnit_BaseColor by default
                                // Graphic3d_TypeOfTextureFilter myFilter;          //!< texture filter, Graphic3d_TOTF_NEAREST by default
                                //   Graphic3d_LevelOfTextureAnisotropy myAnisoLevel;      //!< level of anisotropy filter, Graphic3d_LOTA_OFF by default
        Graphic3d_TypeOfTextureMode myGenMode;         //!< texture coordinates generation mode, Graphic3d_TOTM_MANUAL by default
        int myBaseLevel;       //!< base texture mipmap level (0 by default)
        int myMaxLevel;        //!< maximum texture mipmap array level (1000 by default)
        float myRotAngle;        //!< texture coordinates rotation angle in degrees, 0 by default
        bool myToModulate;      //!< flag to modulate texture with material color, FALSE by default
        bool myToRepeat;        //!< flag to repeat (true) or wrap (false) texture coordinates out of [0,1] range


    } //! Type of the texture projection.

    public enum Graphic3d_TypeOfTextureMode
    {
        Graphic3d_TOTM_OBJECT,
        Graphic3d_TOTM_SPHERE,
        Graphic3d_TOTM_EYE,
        Graphic3d_TOTM_MANUAL,
        Graphic3d_TOTM_SPRITE
    };

    internal class Graphic3d_TypeOfTexture
    {
    }
    public class Image_PixMap
    {
    }

    //! Base class for cubemaps.
    //! It is iterator over cubemap sides.

    public class Graphic3d_CubeMap : Graphic3d_TextureMap
    {
        //! Returns whether Z axis is inverted.
        public bool ZIsInverted()
        {
            return myZIsInverted;
        }

        Graphic3d_CubeMapSide myCurrentSide;  //!< Iterator state
        bool myEndIsReached; //!< Indicates whether end of iteration has been reached or hasn't
        bool myZIsInverted;  //!< Indicates whether Z axis is inverted that allows to synchronize vertical flip of cubemap

    }
    internal class Graphic3d_CubeMapSide
    {
    }
    public class Graphic3d_TextureEnv
    {
    }



    //! Defines the class of a graduated trihedron.
    //! It contains main style parameters for implementation of graduated trihedron
    //! @sa OpenGl_GraduatedTrihedron    
    public class Graphic3d_GraduatedTrihedron
    {
    }
    //! This class allows the definition of a window gradient background.
    public class Aspect_GradientBackground : Aspect_Background
    {
    }

    //! This class allows the definition of
    //! a window background.
    public class Aspect_Background
    {
    }



    //! Vertex attribute definition.
    public struct Graphic3d_Attribute
    {

        public Graphic3d_TypeOfAttribute Id;       //!< attribute identifier in vertex shader, 0 is reserved for vertex position
        public Graphic3d_TypeOfData DataType; //!< vec2,vec3,vec4,vec4ub


        public int Stride() { return Stride(DataType); }

        //! @return size of attribute of specified data type
        public static int Stride(Graphic3d_TypeOfData theType)
        {
            switch (theType)
            {
                case Graphic3d_TypeOfData.Graphic3d_TOD_USHORT: return sizeof(ushort);
                case Graphic3d_TypeOfData.Graphic3d_TOD_UINT: return sizeof(uint);
                case Graphic3d_TypeOfData.Graphic3d_TOD_VEC2: return sizeof(float) * 2;
                case Graphic3d_TypeOfData.Graphic3d_TOD_VEC3: return sizeof(float) * 3;
                case Graphic3d_TypeOfData.Graphic3d_TOD_VEC4: return sizeof(float) * 4;
                //case Graphic3d_TypeOfData.Graphic3d_TOD_VEC4UB: return sizeof(Graphic3d_Vec4ub);
                case Graphic3d_TypeOfData.Graphic3d_TOD_FLOAT: return sizeof(float);
            }
            return 0;
        }


    }


    public enum Graphic3d_TypeOfData
    {

        //Type of the element in Vertex or Index Buffer.
        //Enumerator
        Graphic3d_TOD_USHORT,

        //unsigned 16-bit integer
        Graphic3d_TOD_UINT,

        //unsigned 32-bit integer
        Graphic3d_TOD_VEC2,

        //2-components float vector
        Graphic3d_TOD_VEC3,

        //3-components float vector
        Graphic3d_TOD_VEC4,

        //4-components float vector
        Graphic3d_TOD_VEC4UB,

        //4-components unsigned byte vector
        Graphic3d_TOD_FLOAT,

        //float value
    }

    public enum Graphic3d_TypeOfAttribute
    {
        Graphic3d_TOA_POS = 0, Graphic3d_TOA_NORM, Graphic3d_TOA_UV, Graphic3d_TOA_COLOR,
        Graphic3d_TOA_CUSTOM
    }

    public class Aspect_CircularGrid : Aspect_Grid
    {  //! creates a new grid. By default this grid is not
       //! active.
        public Aspect_CircularGrid(double aRadiusStep, int aDivisionNumber, double anXOrigin = 0, double anYOrigin = 0, double aRotationAngle = 0)
            : base(anXOrigin, anYOrigin, aRotationAngle)
        {
            myRadiusStep = (aRadiusStep);
            myDivisionNumber = aDivisionNumber;
        }

        double myRadiusStep;
        int myDivisionNumber;
        double myAlpha;
        double myA1;
        double myB1;

    }

    public class Graphic3d_Vertex
    {


        float[] xyz = new float[3];


        //! Returns the X coordinates.
        public float X() { return xyz[0]; }

        //! Returns the Y coordinate.
        public float Y() { return xyz[1]; }

        //! Returns the Z coordinate.
        public float Z() { return xyz[2]; }


        public void SetCoord(float x, float y, float z)
        {
            xyz[0] = x;
            xyz[1] = y;
            xyz[2] = z;
        }

        public void SetCoord(double x, double y, double z)
        {
            xyz[0] = (float)x;
            xyz[1] = (float)y;
            xyz[2] = (float)z;
        }
    }
}



