using System;

namespace OCCPort.OpenGL
{
    internal class OpenGl_Aspects : OpenGl_Element
    {
        //! OpenGl resources
        OpenGl_AspectsProgram myResProgram;
        OpenGl_AspectsTextureSet myResTextureSet;
        OpenGl_AspectsSprite myResSprite;

        Graphic3d_Aspects myAspect;
        Graphic3d_TypeOfShadingModel myShadingModel;

        //! Return aspect.
        public Graphic3d_Aspects Aspect() { return myAspect; }

        public override void Render(OpenGl_Workspace theWorkspace)
        {
            theWorkspace.SetAspects(this);
        }

        internal bool IsDisplayListSprite(OpenGl_Context aCtx)
        {
            throw new NotImplementedException();
        }
    }
}