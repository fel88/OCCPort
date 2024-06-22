using System;
using System.Collections.Generic;

namespace OCCPort
{
	public abstract class Graphic3d_CView : Graphic3d_DataStructureManager
	{

		public Graphic3d_CView(Graphic3d_StructureManager theMgr)
		{
			myCamera = new Graphic3d_Camera();
			myStructureManager = theMgr;
		}

		//! Returns layer with given ID or NULL if undefined.
		public abstract Graphic3d_Layer Layer(Graphic3d_ZLayerId theLayerId);

		void Update(Graphic3d_ZLayerId theLayerId = Graphic3d_ZLayerId.Graphic3d_ZLayerId_UNKNOWN)
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
		protected Graphic3d_SequenceOfStructure myStructsToCompute;
		protected Graphic3d_SequenceOfStructure myStructsComputed;
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

		internal void DisplayedStructures(out Graphic3d_MapOfStructure[] aSetOfStructures)
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

		public virtual bool IsDefined()
		{
			return true;
		}

		internal Bnd_Box MinMaxValues()
		{
			return new Bnd_Box();
		}


		//! Returns the activity flag of the view.
		public bool IsActive() { return myIsActive; }



		//! Adds the structure to display lists of the view.
		public abstract void displayStructure(Graphic3d_CStructure theStructure,
									  Graphic3d_DisplayPriority thePriority);


		internal void Display(Graphic3d_Structure theStructure)
		{
			//if (!IsActive())
			//{
			//	return;
			//}

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

		private int IsComputed(Graphic3d_Structure theStructure)
		{
			int aStructId = theStructure.Identification();
			int aStructIndex = 1;
			foreach (var aStructIter in myStructsToCompute.list)
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
		internal bool IsRemoved()
		{
			return myIsRemoved;
		}

		internal void Activate()
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
	}

}