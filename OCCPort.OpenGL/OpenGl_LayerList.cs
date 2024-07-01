using OCCPort;
using OCCPort.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OCCPort
{
	//! Class defining the list of layers.
	internal class OpenGl_LayerList
	{
		LayersCollection myLayers = new LayersCollection();
		internal Graphic3d_Layer[] Layers()
		{
			return myLayers.ToArray();
		}
		//! Returns the map of Z-layer IDs to indexes.
		public MyLayersDic LayerIDs() { return myLayerIds; }


		//! Method returns the number of available priorities
		public int NbPriorities() { return Constants.Graphic3d_DisplayPriority_NB; }

		//! Number of displayed structures
		public int NbStructures() { return myNbStructures; }

		public void AddStructure(OpenGl_Structure theStruct,
									  Graphic3d_ZLayerId theLayerId,
									  Graphic3d_DisplayPriority thePriority,
									 bool isForChangePriority = false)
		{
			// add structure to associated layer,
			// if layer doesn't exists, display structure in default layer
			Graphic3d_Layer aLayerPtr = myLayerIds.Seek(theLayerId);
			Graphic3d_Layer aLayer = aLayerPtr != null ? aLayerPtr : myLayerIds.Find(Graphic3d_ZLayerId.Graphic3d_ZLayerId_Default);
			aLayer.Add(theStruct, thePriority, isForChangePriority);
			++myNbStructures;
			if (aLayer.IsImmediate())
			{
				++myImmediateNbStructures;
			}

			// Note: In ray-tracing mode we don't modify modification
			// state here. It is redundant, because the possible changes
			// will be handled in the loop for structures
		}
		public void InsertLayerBefore(Graphic3d_ZLayerId theNewLayerId,
										  Graphic3d_ZLayerSettings theSettings,
										  Graphic3d_ZLayerId theLayerAfter)
		{
			if (myLayerIds.IsBound(theNewLayerId))
			{
				return;
			}

			Graphic3d_Layer aNewLayer = new Graphic3d_Layer(theNewLayerId, myBVHBuilder);
			aNewLayer.SetLayerSettings(theSettings);

			Graphic3d_Layer anOtherLayer;
			if (theLayerAfter != Graphic3d_ZLayerId.Graphic3d_ZLayerId_UNKNOWN
			 && myLayerIds.Find(theLayerAfter, out anOtherLayer))
			{
				foreach (var aLayerIter in myLayers)
				
				//	for (NCollection_List < Handle(Graphic3d_Layer) >::Iterator aLayerIter(myLayers); aLayerIter.More(); aLayerIter.Next())
				{
					//if (aLayerIter.Value() == anOtherLayer)
					{
						myLayers.InsertBefore(aNewLayer, aLayerIter);
						break;
					}
				}
			}
			else
			{
				myLayers.Prepend(aNewLayer);
			}

			myLayerIds.Bind(theNewLayerId, aNewLayer);
			//myTransparentToProcess.Allocate(myLayers.Size());
		}

		Select3D_BVHBuilder3d myBVHBuilder;      //!< BVH tree builder for frustum culling

		int myNbStructures;

		//=======================================================================
		//function : InsertLayerAfter
		//purpose  :
		//=======================================================================
		public void InsertLayerAfter(Graphic3d_ZLayerId theNewLayerId,

										 Graphic3d_ZLayerSettings theSettings,
										 Graphic3d_ZLayerId theLayerBefore)
		{
			if (myLayerIds.IsBound(theNewLayerId))
			{
				return;
			}

			Graphic3d_Layer aNewLayer = new Graphic3d_Layer(theNewLayerId, myBVHBuilder);
			aNewLayer.SetLayerSettings(theSettings);

			Graphic3d_Layer anOtherLayer;
			if (theLayerBefore != Graphic3d_ZLayerId.Graphic3d_ZLayerId_UNKNOWN
			 && myLayerIds.Find(theLayerBefore, out anOtherLayer))
			{
				foreach (var aLayerIter in myLayers)

				//for (NCollection_List < Handle(Graphic3d_Layer) >::Iterator aLayerIter(myLayers); aLayerIter.More(); aLayerIter.Next())
				{
					if (aLayerIter == anOtherLayer)
					{
						myLayers.InsertAfter(aNewLayer, aLayerIter);
						break;
					}
				}
			}
			else
			{
				myLayers.Append(aNewLayer);
			}

			myLayerIds.Bind(theNewLayerId, aNewLayer);
			//myTransparentToProcess.Allocate(myLayers.Size());
		}



		//! Return number of structures within immediate layers
		public int NbImmediateStructures() { return myImmediateNbStructures; }

		internal void UpdateCulling(OpenGl_Workspace myWorkspace, bool theToDrawImmediate)
		{
			//throw new NotImplementedException();
		}
		void renderLayer(OpenGl_Workspace theWorkspace,
									 OpenGl_GlobalLayerSettings theDefaultSettings,
									 Graphic3d_Layer theLayer)
		{
			// render priority list
			int aViewId = theWorkspace.View().Identification();
			for (int aPriorityIter = (int)Graphic3d_DisplayPriority.Graphic3d_DisplayPriority_Bottom; aPriorityIter <= (int)Graphic3d_DisplayPriority.Graphic3d_DisplayPriority_Topmost; ++aPriorityIter)
			{
				Graphic3d_IndexedMapOfStructure aStructures = theLayer.Structures((Graphic3d_DisplayPriority)aPriorityIter);
				for (OpenGl_Structure.StructIterator aStructIter = new OpenGl_Structure.StructIterator(aStructures); aStructIter.More(); aStructIter.Next())
				{
					OpenGl_Structure aStruct = aStructIter.Value();
					if (aStruct.IsCulled()
					|| !aStruct.IsVisible(aViewId))
					{
						continue;
					}

					aStruct.Render(theWorkspace);
				}
			}
		}
		internal struct OpenGl_GlobalLayerSettings
		{
			public int DepthFunc;
			public bool DepthMask;
		}
		internal void Render(OpenGl_Workspace theWorkspace,
			bool theToDrawImmediate, OpenGl_LayerFilter theLayersToProcess,
			OpenGl_FrameBuffer theReadDrawFbo,
			OpenGl_FrameBuffer theOitAccumFbo)
		{
			// Remember global settings for glDepth function and write mask.
			OpenGl_GlobalLayerSettings aPrevSettings = new OpenGl_GlobalLayerSettings();

			OpenGl_Context aCtx = theWorkspace.GetGlContext();
			//aCtx.core11fwd.glGetIntegerv(GL_DEPTH_FUNC, &aPrevSettings.DepthFunc);
			//aCtx.core11fwd.glGetBooleanv(GL_DEPTH_WRITEMASK, &aPrevSettings.DepthMask);
			OpenGl_GlobalLayerSettings aDefaultSettings = aPrevSettings;
			/*
			const bool isShadowMapPass = theReadDrawFbo != NULL
									 && !theReadDrawFbo->HasColor();*/
			for (OpenGl_FilteredIndexedLayerIterator aLayerIterStart = new OpenGl_FilteredIndexedLayerIterator(myLayers, theToDrawImmediate, theLayersToProcess); aLayerIterStart.More();)
			{

				OpenGl_FilteredIndexedLayerIterator aLayerIter = new OpenGl_FilteredIndexedLayerIterator(aLayerIterStart);
				for (; aLayerIter.More(); aLayerIter.Next())
				{
					OpenGl_Layer aLayer = aLayerIter.Value();


					renderLayer(theWorkspace, aDefaultSettings, aLayer);
				}
			}
		}

		internal void RemoveStructure(OpenGl_Structure theStructure)
		{
			Graphic3d_ZLayerId aLayerId = theStructure.ZLayer();
			Graphic3d_Layer aLayerPtr = myLayerIds.Seek(aLayerId);
			Graphic3d_Layer aLayer = aLayerPtr != null ? aLayerPtr : myLayerIds.Find(Graphic3d_ZLayerId.Graphic3d_ZLayerId_Default);

			Graphic3d_DisplayPriority aPriority = Graphic3d_DisplayPriority.Graphic3d_DisplayPriority_INVALID;

			// remove structure from associated list
			// if the structure is not found there,
			// scan through layers and remove it
			if (aLayer.Remove(theStructure, ref aPriority))
			{
				--myNbStructures;
				if (aLayer.IsImmediate())
				{
					--myImmediateNbStructures;
				}

				if (aLayer.LayerSettings().IsRaytracable()
				 && theStructure.IsRaytracable())
				{
					++myModifStateOfRaytraceable;
				}

				return;

			}


			// scan through layers and remove it
			foreach (var aLayerEx in myLayers)
			//for (NCollection_List < Handle(Graphic3d_Layer) >::Iterator aLayerIter(myLayers); aLayerIter.More(); aLayerIter.Next())
			{
				//Graphic3d_Layer aLayerEx = aLayerIter.ChangeValue();
				if (aLayerEx == aLayer)
				{
					continue;
				}

				if (aLayerEx.Remove(theStructure, ref aPriority))
				{
					--myNbStructures;
					if (aLayerEx.IsImmediate())
					{
						--myImmediateNbStructures;
					}

					if (aLayerEx.LayerSettings().IsRaytracable()
					 && theStructure.IsRaytracable())
					{
						++myModifStateOfRaytraceable;
					}
					return;
				}
			}
		}

		MyLayersDic myLayerIds = new MyLayersDic();
		
		int myImmediateNbStructures; //!< number of structures within immediate layers

		int myModifStateOfRaytraceable;

		//! Collection of references to layers with transparency gathered during rendering pass.
		OpenGl_LayerStack myTransparentToProcess;
	}
}