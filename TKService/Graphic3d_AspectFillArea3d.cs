
using OCCPort.Common;
using TKernel;

namespace TKService
{
    //! This class defines graphic attributes for opaque 3d primitives (polygons, triangles, quadrilaterals).
    public class Graphic3d_AspectFillArea3d : Graphic3d_Aspects
    {
        public Graphic3d_AspectFillArea3d()
        {
            myInteriorStyle = Aspect_InteriorStyle.Aspect_IS_EMPTY;
        }


        public Graphic3d_AspectFillArea3d(Aspect_InteriorStyle theInteriorStyle,
                                                        Quantity_Color theInteriorColor,
                                                        Quantity_Color theEdgeColor,
                                                        Aspect_TypeOfLine theEdgeLineType,
                                                        double theEdgeLineWidth,
                                                        Graphic3d_MaterialAspect theFrontMaterial,
                                                        Graphic3d_MaterialAspect theBackMaterial)
        {
            myFrontMaterial = theFrontMaterial;
            myBackMaterial = theBackMaterial;
            myInteriorColor.SetRGB(theInteriorColor);
            myBackInteriorColor.SetRGB(theInteriorColor);
            myEdgeColor.SetRGB(theEdgeColor);
            myInteriorStyle = theInteriorStyle;
            myLineType = theEdgeLineType;
            SetEdgeWidth((float)theEdgeLineWidth);
        }
    }
}
