using System;

namespace OCCPort.OpenGL
{
    internal class OpenGl_Aspects : OpenGl_Element
    {
        //! OpenGl resources
        OpenGl_AspectsProgram myResProgram;
        OpenGl_AspectsTextureSet myResTextureSet;
        OpenGl_AspectsSprite myResSprite;

        public OpenGl_Aspects(Graphic3d_Aspects theAspect)
        {
            myShadingModel = Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_DEFAULT;
            SetAspect(theAspect);
        }
        //==============================================================
        // function : SetAspect
        // purpose  :
        // =======================================================================
        public void SetAspect(Graphic3d_Aspects theAspect)
        {
            myAspect = theAspect;

            Graphic3d_MaterialAspect aMat = theAspect.FrontMaterial();
            myShadingModel = theAspect.ShadingModel() != Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Unlit
                    && (aMat.ReflectionMode(Graphic3d_TypeOfReflection. Graphic3d_TOR_AMBIENT)
                     || aMat.ReflectionMode(Graphic3d_TypeOfReflection.Graphic3d_TOR_DIFFUSE)
                     || aMat.ReflectionMode(Graphic3d_TypeOfReflection.Graphic3d_TOR_SPECULAR)
                     || aMat.ReflectionMode(Graphic3d_TypeOfReflection.Graphic3d_TOR_EMISSION))
                     ? theAspect.ShadingModel()
                     : Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Unlit;

            // invalidate resources
            myResTextureSet.UpdateRediness(myAspect);
            myResSprite.UpdateRediness(myAspect);
            myResProgram.UpdateRediness(myAspect);
            if (!myResSprite.IsReady())
            {
                myResTextureSet.Invalidate();
            }
        }

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

        internal Graphic3d_TypeOfShadingModel ShadingModel()
        {
            throw new NotImplementedException();
        }
    }
}