using System;
using System.Diagnostics.SymbolStore;

namespace OCCPort.OpenGL
{
    internal class OpenGl_AspectsSprite
    {

        //! Return TRUE if resource is up-to-date.
        public bool IsReady() { return myIsSpriteReady; }

        float myMarkerSize;
        bool myIsSpriteReady;
        internal void UpdateRediness(Graphic3d_Aspects myAspect)
        {

        }
    }
}