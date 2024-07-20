using System.Security.AccessControl;

namespace OCCPort
{
    //! Creates and updates a group of attributes for 3d line primitives.
    //! This group contains the color, the type of line, and its thickness.
    public class Graphic3d_AspectLine3d : Graphic3d_Aspects
    {
        public Graphic3d_AspectLine3d(Quantity_Color theColor,
                                                Aspect_TypeOfLine theType,
                                                double theWidth)
        {
            myShadingModel = Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Unlit;
            myInteriorColor.SetRGB(theColor);
            SetLineType(theType);
            SetLineWidth((float)theWidth);
        }
    }
}