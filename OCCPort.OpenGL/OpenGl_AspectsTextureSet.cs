using OCCPort;
using OCCPort.OpenGL;
using System;
using System.Reflection.Metadata;

namespace OCCPort.OpenGL
{
    //! OpenGl resources for custom textures.
    internal class OpenGl_AspectsTextureSet
    {
        //! Invalidate resource state.
        internal void Invalidate()
        {
            myIsTextureReady = false;
        }
        bool myIsTextureReady;

        //! Update texture resource up-to-date state.
        internal void UpdateRediness(Graphic3d_Aspects myAspect)
        {

        }
        OpenGl_TextureSet[] myTextures = new OpenGl_TextureSet[2];
        void build(OpenGl_Context theCtx,
                                      Graphic3d_Aspects theAspect,
                                      OpenGl_PointSprite theSprite,
                                      OpenGl_PointSprite theSpriteA)
        {
            //Graphic3d_TextureSet aNewTextureSet = theAspect.TextureSet();

            //bool hasSprite = theAspect.IsMarkerSprite();
            //int aNbTexturesOld = !myTextures[0].IsNull() ? myTextures[0]->Size() : 0;
            //int aNbTexturesNew = !aNewTextureSet.IsNull() && theAspect->ToMapTexture()
            //                                ? aNewTextureSet.Size()
            //                                : 0;
            //if (hasSprite)
            //{
            //    ++aNbTexturesNew;
            //}

            //// release old texture resources
            //if (aNbTexturesOld != aNbTexturesNew)
            //{
            //    Release(theCtx.get());
            //    if (aNbTexturesNew > 0)
            //    {
            //        myTextures[0] = new OpenGl_TextureSet(aNbTexturesNew);
            //    }
            //    else
            //    {
            //        myTextures[0].Nullify();
            //        myTextures[1].Nullify();
            //    }
            //}
            //if (myTextures[0].IsNull())
            //{
            //    return;
            //}

            //if (theSprite == theSpriteA)
            //{
            //    myTextures[1].Nullify();
            //}
            //else
            //{
            //    if (myTextures[1].IsNull()
            //     || myTextures[1]->Size() != myTextures[0]->Size())
            //    {
            //        myTextures[1] = new OpenGl_TextureSet(aNbTexturesNew);
            //    }
            //    else
            //    {
            //        myTextures[1]->InitZero();
            //    }
            //}

            //Standard_Integer & aTextureSetBits = myTextures[0]->ChangeTextureSetBits();
            //aTextureSetBits = Graphic3d_TextureSetBits_NONE;
            //Standard_Integer aPrevTextureUnit = -1;
            //if (theAspect->ToMapTexture())
            //{
            //    Graphic3d_TextureSet::Iterator aTextureIter(aNewTextureSet);
            //    OpenGl_TextureSet::Iterator aResIter0(myTextures[0]);
            //    for (; aTextureIter.More(); aResIter0.Next(), aTextureIter.Next())
            //    {
            //        Handle(OpenGl_Texture) & aResource = aResIter0.ChangeValue();
            //        const Handle(Graphic3d_TextureMap)&aTexture = aTextureIter.Value();
            //        if (!aResource.IsNull())
            //        {
            //            if (!aTexture.IsNull()
            //             && aTexture->GetId() == aResource->ResourceId()
            //             && aTexture->Revision() != aResource->Revision())
            //            {
            //                if (aResource->Init(theCtx, aTexture))
            //                {
            //                    aResIter0.ChangeUnit() = aResource->Sampler()->Parameters()->TextureUnit();
            //                    if (aResIter0.Unit() < aPrevTextureUnit)
            //                    {
            //                        throw Standard_ProgramError("Graphic3d_TextureMap defines texture units in non-ascending order");
            //                    }
            //                    aPrevTextureUnit = aResIter0.Unit();
            //                    aResource->Sampler()->SetParameters(aTexture->GetParams());
            //                    aResource->SetRevision(aTexture->Revision());
            //                }
            //            }

            //            if (aResource->ResourceId().IsEmpty())
            //            {
            //                theCtx->DelayedRelease(aResource);
            //                aResource.Nullify();
            //            }
            //            else
            //            {
            //                const TCollection_AsciiString aTextureKey = aResource->ResourceId();
            //                aResource.Nullify(); // we need nullify all handles before ReleaseResource() call
            //                theCtx->ReleaseResource(aTextureKey, Standard_True);
            //            }
            //        }

            //        if (!aTexture.IsNull())
            //        {
            //            const TCollection_AsciiString&aTextureKeyNew = aTexture->GetId();
            //            if (aTextureKeyNew.IsEmpty()
            //            || !theCtx->GetResource < Handle(OpenGl_Texture) > (aTextureKeyNew, aResource))
            //            {
            //                aResource = new OpenGl_Texture(aTextureKeyNew, aTexture->GetParams());

            //                if (aResource->Init(theCtx, aTexture))
            //                {
            //                    aResource->SetRevision(aTexture->Revision());
            //                }
            //                if (!aTextureKeyNew.IsEmpty())
            //                {
            //                    theCtx->ShareResource(aTextureKeyNew, aResource);
            //                }
            //            }
            //            else
            //            {
            //                if (aTexture->Revision() != aResource->Revision())
            //                {
            //                    if (aResource->Init(theCtx, aTexture))
            //                    {
            //                        aResource->SetRevision(aTexture->Revision());
            //                    }
            //                }
            //                aResource->Sampler()->SetParameters(aTexture->GetParams());
            //            }

            //            // update occupation of texture units
            //            const Graphic3d_TextureUnit aTexUnit = aResource->Sampler()->Parameters()->TextureUnit();
            //            aResIter0.ChangeUnit() = aTexUnit;
            //            if (aResIter0.Unit() < aPrevTextureUnit)
            //            {
            //                throw Standard_ProgramError("Graphic3d_TextureMap defines texture units in non-ascending order");
            //            }
            //            aPrevTextureUnit = aResIter0.Unit();
            //            if (aTexUnit >= Graphic3d_TextureUnit_0 && aTexUnit <= Graphic3d_TextureUnit_5)
            //            {
            //                aTextureSetBits |= (1 << int(aTexUnit));
            //            }
            //        }
            //    }
            //}

            //if (hasSprite)
            //{
            //    myTextures[0]->ChangeLast() = theSprite;
            //    myTextures[0]->ChangeLastUnit() = theCtx->SpriteTextureUnit();
            //    // Graphic3d_TextureUnit_PointSprite
            //    if (!theSprite.IsNull())
            //    {
            //        theSprite->Sampler()->Parameters()->SetTextureUnit(theCtx->SpriteTextureUnit());
            //    }
            //    if (!theSpriteA.IsNull())
            //    {
            //        theSpriteA->Sampler()->Parameters()->SetTextureUnit(theCtx->SpriteTextureUnit());
            //    }
            //}
            //if (myTextures[1].IsNull())
            //{
            //    return;
            //}

            //myTextures[1]->ChangeTextureSetBits() = aTextureSetBits;
            //for (OpenGl_TextureSet::Iterator aResIter0 (myTextures[0]), aResIter1(myTextures[1]); aResIter0.More(); aResIter0.Next(), aResIter1.Next())
            //{
            //    aResIter1.ChangeValue() = aResIter0.Value();
            //    aResIter1.ChangeUnit() = aResIter0.Unit();
            //}
            //if (hasSprite)
            //{
            //    myTextures[1]->ChangeLast() = theSpriteA;
            //    myTextures[1]->ChangeLastUnit() = theCtx->SpriteTextureUnit();
            //}
        }
        internal OpenGl_TextureSet TextureSet(OpenGl_Context theCtx,
            Graphic3d_Aspects theAspect,
            OpenGl_PointSprite theSprite,
            OpenGl_PointSprite theSpriteA, bool theToHighlight)
        {
            if (!myIsTextureReady)
            {
                build(theCtx, theAspect, theSprite, theSpriteA);
                myIsTextureReady = true;
            }
            return theToHighlight && myTextures[1] != null
                 ? myTextures[1]
                 : myTextures[0];
        }
    }
}