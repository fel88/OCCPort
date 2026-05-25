using System;
using System.Diagnostics.SymbolStore;

namespace OCCPort.OpenGL
{
    internal class OpenGl_AspectsSprite
    {

        //! Return TRUE if resource is up-to-date.
        public bool IsReady() { return myIsSpriteReady; }


        internal void UpdateRediness(Graphic3d_Aspects myAspect)
        {

        }

        internal OpenGl_PointSprite Sprite(OpenGl_Context theCtx, Graphic3d_Aspects theAspects, bool theIsAlphaSprite)
        {
            if (!myIsSpriteReady)
            {
               // build(theCtx, theAspects.MarkerImage(), theAspects.MarkerType(), theAspects.MarkerScale(), theAspects.ColorRGBA(), myMarkerSize);
                myIsSpriteReady = true;
            }
            return theIsAlphaSprite && mySpriteA != null //&& mySpriteA.IsValid()
                 ? mySpriteA
                 : mySprite;
        }

        OpenGl_PointSprite mySprite;
        OpenGl_PointSprite mySpriteA;
        float myMarkerSize;
        bool myIsSpriteReady;
    }
}