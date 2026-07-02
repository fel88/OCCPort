using TKernel;
using TKService;

namespace TKV3d
{
    //! Directional light source for a viewer.
    public class V3d_DirectionalLight : V3d_PositionLight
    {
        public V3d_DirectionalLight(V3d_TypeOfOrientation theDirection, Quantity_NameOfColor theColor,  bool theIsHeadlight = false):base(
        Graphic3d_TypeOfLightSource.Graphic3d_TypeOfLightSource_Directional)
        {
            SetColor(new  Quantity_Color( theColor));
            SetHeadlight(theIsHeadlight);
            SetDirection(V3d.GetProjAxis(theDirection));
        }
    }
}


