using System;

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
    }
}