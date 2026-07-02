using TKernel;
using TKService;

namespace TKV3d
{
    //! Creation of an ambient light source in a viewer.
    public class V3d_AmbientLight : Graphic3d_CLight
    {
        //! Constructs an ambient light source in the viewer.
        //! The default Color of this light source is WHITE.
        public V3d_AmbientLight(Quantity_NameOfColor theColor = Quantity_NameOfColor.Quantity_NOC_WHITE)
            :base(Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Ambient)
        {
            SetColor( new Quantity_Color ( theColor));

        }
    }
}


