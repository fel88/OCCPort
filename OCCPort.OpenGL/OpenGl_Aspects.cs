using System;
using System.Reflection.Metadata;
using TKService;

namespace OCCPort.OpenGL
{
    public class OpenGl_Aspects : OpenGl_Element
    {
        //! OpenGl resources
        OpenGl_AspectsProgram myResProgram = new OpenGl_AspectsProgram();
        OpenGl_AspectsTextureSet myResTextureSet = new OpenGl_AspectsTextureSet();
        OpenGl_AspectsSprite myResSprite = new OpenGl_AspectsSprite();
        public OpenGl_Aspects()

        {
            myAspect = (new Graphic3d_Aspects());
            myShadingModel = Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Unlit;
            myAspect.SetInteriorStyle(Aspect_InteriorStyle.Aspect_IS_SOLID);
            /*myAspect.SetInteriorColor(Quantity_NOC_WHITE);
            myAspect.SetEdgeColor(Quantity_NOC_WHITE);
            myAspect.SetFrontMaterial(THE_DEFAULT_MATERIAL);
            myAspect.SetBackMaterial(THE_DEFAULT_MATERIAL);*/
            myAspect.SetShadingModel(myShadingModel);/*
            myAspect.SetHatchStyle(Handle(Graphic3d_HatchStyle)());*/
        }
        //! Returns textures map.
        public OpenGl_TextureSet TextureSet(OpenGl_Context theCtx,
                                               bool theToHighlight = false)
        {
            OpenGl_PointSprite aSprite = myResSprite.Sprite(theCtx, myAspect, false);
            OpenGl_PointSprite aSpriteA = myResSprite.Sprite(theCtx, myAspect, true);
            return myResTextureSet.TextureSet(theCtx, myAspect, aSprite, aSpriteA, theToHighlight);
        }

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
                    && (aMat.ReflectionMode(Graphic3d_TypeOfReflection.Graphic3d_TOR_AMBIENT)
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


        //! Returns shading model; Graphic3d_TypeOfShadingModel_DEFAULT by default.
        //! Graphic3d_TOSM_DEFAULT means that Shading Model set as default for entire Viewer will be used.
        internal Graphic3d_TypeOfShadingModel ShadingModel()
        {
            return myShadingModel;
        }


        //! Init and return OpenGl shader program resource.
        //! @return shader program resource.
        internal OpenGl_ShaderProgram ShaderProgramRes(OpenGl_Context theCtx)
        {
            return myResProgram.ShaderProgram(theCtx, myAspect.ShaderProgram());
        }
    }
}