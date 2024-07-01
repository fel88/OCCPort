using System;

namespace OCCPort
{
	public class Graphic3d_Layer
	{
		public void Add(Graphic3d_CStructure theStruct, Graphic3d_DisplayPriority thePriority,
			bool isForChangePriority = false)
		{

			int anIndex = Math.Min(Math.Max((int)thePriority, (int)Graphic3d_DisplayPriority.Graphic3d_DisplayPriority_Bottom), (int)Graphic3d_DisplayPriority.Graphic3d_DisplayPriority_Topmost);
			if (theStruct == null)
			{
				return;
			}

			myArray[anIndex].Add(theStruct);
			if (theStruct.IsAlwaysRendered())
			{
				theStruct.MarkAsNotCulled();
				if (!isForChangePriority)
				{
					myAlwaysRenderedMap.Add(theStruct);
				}
			}
			else if (!isForChangePriority)
			{
				if (theStruct.TransformPersistence() == null)
				{
					myBVHPrimitives.Add(theStruct);
				}
				else
				{
					myBVHPrimitivesTrsfPers.Add(theStruct);
				}
			}
			++myNbStructures;
		}
		//! Indexed map of always rendered structures.
		Graphic3d_IndexedMapOfStructure myAlwaysRenderedMap;

		//! Set of Graphic3d_CStructures structures for building BVH tree.
		Graphic3d_BvhCStructureSet myBVHPrimitives = new Graphic3d_BvhCStructureSet();


		//! Return layer id.
		public Graphic3d_ZLayerId LayerId() { return myLayerId; }
		//! Layer id.
		Graphic3d_ZLayerId myLayerId;
		Graphic3d_IndexedMapOfStructure[] myArray = new Graphic3d_IndexedMapOfStructure[Constants.Graphic3d_DisplayPriority_NB];
		//! Return true if layer was marked with immediate flag.
		public bool IsImmediate() { return myLayerSettings.IsImmediate(); }

		public Graphic3d_ZLayerSettings LayerSettings()
		{
			return myLayerSettings;
		}

		public bool Remove(Graphic3d_CStructure theStruct, ref Graphic3d_DisplayPriority thePriority,
							  bool isForChangePriority = false)
		{
			if (theStruct == null)
			{
				thePriority = Graphic3d_DisplayPriority.Graphic3d_DisplayPriority_INVALID;
				return false;
			}
			thePriority = Graphic3d_DisplayPriority.Graphic3d_DisplayPriority_INVALID;
			return false;

		}

		//! Layer setting flags.
		Graphic3d_ZLayerSettings myLayerSettings = new Graphic3d_ZLayerSettings();

		public Graphic3d_Layer(Graphic3d_ZLayerId theId, Select3D_BVHBuilder3d theBuilder)
		{
			myNbStructures = (0);
			myNbStructuresNotCulled = (0);
			myLayerId = (theId);
			myBVHPrimitivesTrsfPers = new Graphic3d_BvhCStructureSetTrsfPers(theBuilder);
			myBVHIsLeftChildQueuedFirst = (true);
			myIsBVHPrimitivesNeedsReset = (false);

			//			myIsBoundingBoxNeedsReset[0] = myIsBoundingBoxNeedsReset[1] = true;

			for (int i = 0; i < myArray.Length; i++)
			{
				myArray[i] = new Graphic3d_IndexedMapOfStructure();
			}

		}

		//! Is needed for implementation of stochastic order of BVH traverse.
		bool myBVHIsLeftChildQueuedFirst;

		//! Defines if the primitive set for BVH is outdated.
		bool myIsBVHPrimitivesNeedsReset;

		//! Overall number of structures rendered in the layer.
		int myNbStructures;

		//! Number of NOT culled structures in the layer.
		int myNbStructuresNotCulled;


		public Graphic3d_IndexedMapOfStructure Structures(Graphic3d_DisplayPriority aPriorityIter)
		{
			throw new NotImplementedException();
		}

		internal Bnd_Box BoundingBox(object v, object aCamera, object value1, object value2, bool theToIncludeAuxiliary)
		{
			throw new NotImplementedException();
		}

		internal void InvalidateBoundingBox()
		{
			throw new NotImplementedException();
		}

		internal int NbOfTransformPersistenceObjects()
		{

			return myBVHPrimitivesTrsfPers.Size();

		}

		//! Set of transform persistent Graphic3d_CStructures for building BVH tree.
		Graphic3d_BvhCStructureSetTrsfPers myBVHPrimitivesTrsfPers;



		public void SetLayerSettings(Graphic3d_ZLayerSettings theSettings)
		{
			bool toUpdateTrsf = !myLayerSettings.Origin().IsEqual(theSettings.Origin(), gp.Resolution());
			myLayerSettings = theSettings;
			if (!toUpdateTrsf)
			{
				return;
			}

			for (int aPriorIter = (int)Graphic3d_DisplayPriority.Graphic3d_DisplayPriority_Bottom; aPriorIter <= (int)Graphic3d_DisplayPriority.Graphic3d_DisplayPriority_Topmost; ++aPriorIter)
			{
				Graphic3d_IndexedMapOfStructure aStructures = myArray[aPriorIter];
				foreach (var aStructIter in aStructures.list)
				{
					Graphic3d_CStructure aStructure = (Graphic3d_CStructure)(aStructIter);
					aStructure.updateLayerTransformation();
				}
				//for (Graphic3d_IndexedMapOfStructure::Iterator aStructIter (aStructures); aStructIter.More(); aStructIter.Next())				
			}
		}
	}
}
