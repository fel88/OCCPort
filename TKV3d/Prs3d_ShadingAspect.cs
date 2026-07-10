using TKernel;
using TKService;

namespace TKV3d
{
    //! A framework to define the display of shading.
    //! The attributes which make up this definition include:
    //! -   fill aspect
    //! -   color, and
    //! -   material
    public class Prs3d_ShadingAspect
    {
        //! Constructs an empty framework to display shading.
        public Prs3d_ShadingAspect()
        {
            Graphic3d_MaterialAspect aMat = new Graphic3d_MaterialAspect(Graphic3d_NameOfMaterial.Graphic3d_NameOfMaterial_Brass);
            Quantity_Color aColor = aMat.AmbientColor();
            myAspect = new Graphic3d_AspectFillArea3d(Aspect_InteriorStyle.Aspect_IS_SOLID,
                                   aColor,
                                   aColor,
                                   Aspect_TypeOfLine.Aspect_TOL_SOLID,
                                   1.0,
                                   aMat,
                                   aMat);
        }

        //! Returns the polygons aspect properties.
        public Graphic3d_AspectFillArea3d Aspect() { return myAspect; }
        Graphic3d_AspectFillArea3d myAspect = new Graphic3d_AspectFillArea3d();
        public void SetAspect(Graphic3d_AspectFillArea3d theAspect) { myAspect = theAspect; }

        internal void SetMaterial(Graphic3d_MaterialAspect theMaterial, Aspect_TypeOfFacingModel theModel = Aspect_TypeOfFacingModel.Aspect_TOFM_BOTH_SIDE)
        {
            if (theModel != Aspect_TypeOfFacingModel.Aspect_TOFM_BOTH_SIDE)
            {
                myAspect.SetDistinguishOn();
            }
            if (theModel == Aspect_TypeOfFacingModel.Aspect_TOFM_FRONT_SIDE
             || theModel == Aspect_TypeOfFacingModel.Aspect_TOFM_BOTH_SIDE)
            {
                myAspect.SetFrontMaterial(theMaterial);
            }

            if (theModel == Aspect_TypeOfFacingModel.Aspect_TOFM_BACK_SIDE
             || theModel == Aspect_TypeOfFacingModel.Aspect_TOFM_BOTH_SIDE)
            {
                myAspect.SetBackMaterial(theMaterial);
            }
        }
    }
}

