using System;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

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


        public Graphic3d_IndexedMapOfStructure Structures(Graphic3d_DisplayPriority thePriority)
        {
            return myArray[(int)thePriority];

        }
        void updateBVH()
        {
            if (!myIsBVHPrimitivesNeedsReset)
            {
                return;
            }

            myBVHPrimitives.Clear();
            //myBVHPrimitivesTrsfPers.Clear();
            //myAlwaysRenderedMap.Clear();
            //myIsBVHPrimitivesNeedsReset = false;
            //for (Standard_Integer aPriorIter = Graphic3d_DisplayPriority_Bottom; aPriorIter <= Graphic3d_DisplayPriority_Topmost; ++aPriorIter)
            //{
            //    const Graphic3d_IndexedMapOfStructure&aStructures = myArray[aPriorIter];
            //    for (Graphic3d_IndexedMapOfStructure::Iterator aStructIter (aStructures); aStructIter.More(); aStructIter.Next())
            //    {
            //        const Graphic3d_CStructure* aStruct = aStructIter.Value();
            //        if (aStruct->IsAlwaysRendered())
            //        {
            //            aStruct->MarkAsNotCulled();
            //            myAlwaysRenderedMap.Add(aStruct);
            //        }
            //        else if (aStruct->TransformPersistence().IsNull())
            //        {
            //            myBVHPrimitives.Add(aStruct);
            //        }
            //        else
            //        {
            //            myBVHPrimitivesTrsfPers.Add(aStruct);
            //        }
            //    }
            //}
        }
        //! Returns layer bounding box.
        //! @param theViewId             view index to consider View Affinity in structure
        //! @param theCamera             camera definition
        //! @param theWindowWidth        viewport width  (for applying transformation-persistence)
        //! @param theWindowHeight       viewport height (for applying transformation-persistence)
        //! @param theToIncludeAuxiliary consider also auxiliary presentations (with infinite flag or with trihedron transformation persistence)
        //! @return computed bounding box
        public Bnd_Box BoundingBox(int theViewId,
                                       Graphic3d_Camera theCamera,
                                       int theWindowWidth,
                                       int theWindowHeight,
                                       bool theToIncludeAuxiliary)
        {
            updateBVH();
            return new Bnd_Box();
            int aBoxId = !theToIncludeAuxiliary ? 0 : 1;
  //          Graphic3d_Mat4d aProjectionMat = theCamera.ProjectionMatrix();
  //          Graphic3d_Mat4d aWorldViewMat = theCamera.OrientationMatrix();
  //          if (myIsBoundingBoxNeedsReset[aBoxId])
  //          {
  //              // Recompute layer bounding box
  //              myBoundingBox[aBoxId].SetVoid();

  //              for (Standard_Integer aPriorIter = Graphic3d_DisplayPriority_Bottom; aPriorIter <= Graphic3d_DisplayPriority_Topmost; ++aPriorIter)
  //              {
  //                  const Graphic3d_IndexedMapOfStructure&aStructures = myArray[aPriorIter];
  //                  for (Graphic3d_IndexedMapOfStructure::Iterator aStructIter (aStructures); aStructIter.More(); aStructIter.Next())
  //                  {
  //                      const Graphic3d_CStructure* aStructure = aStructIter.Value();
  //                      if (!aStructure->IsVisible(theViewId))
  //                      {
  //                          continue;
  //                      }

  //                      // "FitAll" operation ignores object with transform persistence parameter
  //                      // but adds transform persistence point in a bounding box of layer (only zoom pers. objects).
  //                      if (!aStructure->TransformPersistence().IsNull())
  //                      {
  //                          if (!theToIncludeAuxiliary
  //                            && aStructure->TransformPersistence()->IsZoomOrRotate())
  //                          {
  //                              const gp_Pnt anAnchor = aStructure->TransformPersistence()->AnchorPoint();
  //                              myBoundingBox[aBoxId].Add(anAnchor);
  //                              continue;
  //                          }
  //                          // Panning and 2d persistence apply changes to projection or/and its translation components.
  //                          // It makes them incompatible with z-fitting algorithm. Ignored by now.
  //                          else if (!theToIncludeAuxiliary
  //                                 || aStructure->TransformPersistence()->IsTrihedronOr2d())
  //                          {
  //                              continue;
  //                          }
  //                      }

  //                      if (!theToIncludeAuxiliary
  //                        && aStructure->HasGroupTransformPersistence())
  //                      {
  //                          // add per-group transform-persistence point in a bounding box
  //                          for (Graphic3d_SequenceOfGroup::Iterator aGroupIter (aStructure->Groups()); aGroupIter.More(); aGroupIter.Next())
  //                          {
  //                              const Handle(Graphic3d_Group)&aGroup = aGroupIter.Value();
  //                              if (!aGroup->TransformPersistence().IsNull()
  //                                && aGroup->TransformPersistence()->IsZoomOrRotate())
  //                              {
  //                                  const gp_Pnt anAnchor = aGroup->TransformPersistence()->AnchorPoint();
  //                                  myBoundingBox[aBoxId].Add(anAnchor);
  //                              }
  //                          }
  //                      }

  //                      Graphic3d_BndBox3d aBox = aStructure->BoundingBox();
  //                      if (!aBox.IsValid())
  //                      {
  //                          continue;
  //                      }

  //                      if (aStructure->IsInfinite
  //                      && !theToIncludeAuxiliary)
  //                      {
  //                          // include center of infinite object
  //                          aBox = centerOfinfiniteBndBox(aBox);
  //                      }

  //                      if (!aStructure->TransformPersistence().IsNull())
  //                      {
  //                          aStructure->TransformPersistence()->Apply(theCamera, aProjectionMat, aWorldViewMat, theWindowWidth, theWindowHeight, aBox);
  //                      }
  //                      addBox3dToBndBox(myBoundingBox[aBoxId], aBox);
  //                  }
  //              }

  //              myIsBoundingBoxNeedsReset[aBoxId] = false;
  //          }

  //          Bnd_Box aResBox = myBoundingBox[aBoxId];
  //          if (!theToIncludeAuxiliary
  //            || myAlwaysRenderedMap.IsEmpty())
  //          {
  //              return aResBox;
  //          }

  //          // add transformation-persistent objects which depend on camera position (and thus can not be cached) for operations like Z-fit
  //          for (NCollection_IndexedMap <const Graphic3d_CStructure*>::Iterator aStructIter(myAlwaysRenderedMap); aStructIter.More(); aStructIter.Next())
  //{
  //              const Graphic3d_CStructure* aStructure = aStructIter.Value();
  //              if (!aStructure->IsVisible(theViewId))
  //              {
  //                  continue;
  //              }

  //              // handle per-group transformation persistence specifically
  //              if (aStructure->HasGroupTransformPersistence())
  //              {
  //                  for (Graphic3d_SequenceOfGroup::Iterator aGroupIter (aStructure->Groups()); aGroupIter.More(); aGroupIter.Next())
  //                  {
  //                      const Handle(Graphic3d_Group)&aGroup = aGroupIter.Value();
  //                      const Graphic3d_BndBox4f&aBoxF = aGroup->BoundingBox();
  //                      if (aGroup->TransformPersistence().IsNull()
  //                      || !aBoxF.IsValid())
  //                      {
  //                          continue;
  //                      }

  //                      Graphic3d_BndBox3d aBoxCopy(Graphic3d_Vec3d (aBoxF.CornerMin().xyz()),
  //                                   Graphic3d_Vec3d(aBoxF.CornerMax().xyz()));
  //                  aGroup->TransformPersistence()->Apply(theCamera, aProjectionMat, aWorldViewMat, theWindowWidth, theWindowHeight, aBoxCopy);
  //                  addBox3dToBndBox(aResBox, aBoxCopy);
  //              }
  //          }

  //          const Graphic3d_BndBox3d&aStructBox = aStructure->BoundingBox();
  //          if (!aStructBox.IsValid()
  //           || aStructure->TransformPersistence().IsNull()
  //           || !aStructure->TransformPersistence()->IsTrihedronOr2d())
  //          {
  //              continue;
  //          }

  //          Graphic3d_BndBox3d aBoxCopy = aStructBox;
  //          aStructure->TransformPersistence()->Apply(theCamera, aProjectionMat, aWorldViewMat, theWindowWidth, theWindowHeight, aBoxCopy);
  //          addBox3dToBndBox(aResBox, aBoxCopy);
  //      }

  //return aResBox;
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
                foreach (var aStructIter in aStructures)
                {
                    Graphic3d_CStructure aStructure = (Graphic3d_CStructure)(aStructIter);
                    aStructure.updateLayerTransformation();
                }
                //for (Graphic3d_IndexedMapOfStructure::Iterator aStructIter (aStructures); aStructIter.More(); aStructIter.Next())				
            }
        }
    }
}
