using System;

namespace OCCPort
{
	internal class SelectMgr_SelectableObjectSet
	{
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
		bool []myIsDirty=new bool[(int)BVHSubset.BVHSubsetNb]; //!< Dirty flag for each subset

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
	}
}