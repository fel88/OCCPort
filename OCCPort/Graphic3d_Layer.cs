using OCCPort;
using System;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace OCCPort
{
    public class Graphic3d_Layer
    {

        public double considerZoomPersistenceObjects(int theViewId,
                                                                Graphic3d_Camera theCamera,
                                                               int theWindowWidth,
                                                               int theWindowHeight)
        {
            if (NbOfTransformPersistenceObjects() == 0)
            {
                return 1.0;
            }

            var aProjectionMat = theCamera.ProjectionMatrix();
            Graphic3d_Mat4d aWorldViewMat = theCamera.OrientationMatrix();
            double aMaxCoef = -double.MaxValue;

            for (int aPriorIter = (int)Graphic3d_DisplayPriority.Graphic3d_DisplayPriority_Bottom; aPriorIter <= (int)Graphic3d_DisplayPriority.Graphic3d_DisplayPriority_Topmost; ++aPriorIter)
            {
                Graphic3d_IndexedMapOfStructure aStructures = myArray[aPriorIter];
                foreach (var aStructure in aStructures)
                {
                    if (!aStructure.IsVisible(theViewId)
                     || aStructure.TransformPersistence() == null
                     || !aStructure.TransformPersistence().IsZoomOrRotate())
                    {
                        continue;
                    }

                    BVH_Box aBox = aStructure.BoundingBox();
                    if (!aBox.IsValid())
                    {
                        continue;
                    }

                    aStructure.TransformPersistence().Apply(theCamera, aProjectionMat, aWorldViewMat, theWindowWidth, theWindowHeight, ref aBox);

                    var aCornerMin = aBox.CornerMin();
                    var aCornerMax = aBox.CornerMax();
                    int aNbOfPoints = 8;
                    gp_Pnt[] aPoints = new gp_Pnt[] {new  gp_Pnt (aCornerMin.x(), aCornerMin.y(), aCornerMin.z()),
                                                     new gp_Pnt (aCornerMin.x(), aCornerMin.y(), aCornerMax.z()),
                                                     new gp_Pnt (aCornerMin.x(), aCornerMax.y(), aCornerMin.z()),
                                                     new gp_Pnt (aCornerMin.x(), aCornerMax.y(), aCornerMax.z()),
                                                     new gp_Pnt (aCornerMax.x(), aCornerMin.y(), aCornerMin.z()),
                                                     new gp_Pnt (aCornerMax.x(), aCornerMin.y(), aCornerMax.z()),
                                                     new gp_Pnt (aCornerMax.x(), aCornerMax.y(), aCornerMin.z()),
                                                     new gp_Pnt (aCornerMax.x(), aCornerMax.y(), aCornerMax.z()) };
                    gp_Pnt[] aConvertedPoints = new gp_Pnt[aNbOfPoints];
                    double aConvertedMinX = double.MaxValue;
                    double aConvertedMaxX = -double.MaxValue;
                    double aConvertedMinY = double.MaxValue;
                    double aConvertedMaxY = -double.MaxValue;
                    for (int anIdx = 0; anIdx < aNbOfPoints; ++anIdx)
                    {
                        aConvertedPoints[anIdx] = theCamera.Project(aPoints[anIdx]);

                        aConvertedMinX = Math.Min(aConvertedMinX, aConvertedPoints[anIdx].X());
                        aConvertedMaxX = Math.Max(aConvertedMaxX, aConvertedPoints[anIdx].X());

                        aConvertedMinY = Math.Min(aConvertedMinY, aConvertedPoints[anIdx].Y());
                        aConvertedMaxY = Math.Max(aConvertedMaxY, aConvertedPoints[anIdx].Y());
                    }

                    bool isBigObject = (Math.Abs(aConvertedMaxX - aConvertedMinX) > 2.0)  // width  of zoom pers. object greater than width  of window
                                                       || (Math.Abs(aConvertedMaxY - aConvertedMinY) > 2.0); // height of zoom pers. object greater than height of window
                    bool isAlreadyInScreen = (aConvertedMinX > -1.0 && aConvertedMinX < 1.0)
                                                            && (aConvertedMaxX > -1.0 && aConvertedMaxX < 1.0)
                                                            && (aConvertedMinY > -1.0 && aConvertedMinY < 1.0)
                                                            && (aConvertedMaxY > -1.0 && aConvertedMaxY < 1.0);
                    if (isBigObject || isAlreadyInScreen)
                    {
                        continue;
                    }

                    gp_Pnt aTPPoint = aStructure.TransformPersistence().AnchorPoint();
                    gp_Pnt aConvertedTPPoint = theCamera.Project(aTPPoint);
                    aConvertedTPPoint.SetZ(0.0);

                    if (aConvertedTPPoint.Coord().Modulus() < Precision.Confusion())
                    {
                        continue;
                    }

                    double aShiftX = 0.0;
                    if (aConvertedMinX < -1.0)
                    {
                        aShiftX = ((aConvertedMaxX < -1.0) ? (-(1.0 + aConvertedMaxX) + (aConvertedMaxX - aConvertedMinX)) : -(1.0 + aConvertedMinX));
                    }
                    else if (aConvertedMaxX > 1.0)
                    {
                        aShiftX = ((aConvertedMinX > 1.0) ? ((aConvertedMinX - 1.0) + (aConvertedMaxX - aConvertedMinX)) : (aConvertedMaxX - 1.0));
                    }

                    double aShiftY = 0.0;
                    if (aConvertedMinY < -1.0)
                    {
                        aShiftY = ((aConvertedMaxY < -1.0) ? (-(1.0 + aConvertedMaxY) + (aConvertedMaxY - aConvertedMinY)) : -(1.0 + aConvertedMinY));
                    }
                    else if (aConvertedMaxY > 1.0)
                    {
                        aShiftY = ((aConvertedMinY > 1.0) ? ((aConvertedMinY - 1.0) + (aConvertedMaxY - aConvertedMinY)) : (aConvertedMaxY - 1.0));
                    }

                    double aDifX = Math.Abs(aConvertedTPPoint.X()) - aShiftX;
                    double aDifY = Math.Abs(aConvertedTPPoint.Y()) - aShiftY;
                    if (aDifX > Precision.Confusion())
                    {
                        aMaxCoef = Math.Max(aMaxCoef, Math.Abs(aConvertedTPPoint.X()) / aDifX);
                    }
                    if (aDifY > Precision.Confusion())
                    {
                        aMaxCoef = Math.Max(aMaxCoef, Math.Abs(aConvertedTPPoint.Y()) / aDifY);
                    }
                }
            }

            return (aMaxCoef > 0.0) ? aMaxCoef : 1.0;
        }

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
