using OpenTK.Windowing.Desktop;

namespace OCCPort.OpenGL
{
    //! OpenGL material definition
    public class OpenGl_Material
    {
        public OpenGl_Material()
        {
            for (int i = 0; i < 2; i++)
            {
                Common[i] = new OpenGl_MaterialCommon();
            }
        }
        public OpenGl_MaterialCommon[] Common = new OpenGl_MaterialCommon[2];
        //public OpenGl_MaterialPBR Pbr[2];

        //! Set material color.
        public void SetColor(OpenGl_Vec3 theColor)
        {
            Common[0].SetColor(theColor);
            Common[1].SetColor(theColor);
          //  Pbr[0].SetColor(theColor);
          //  Pbr[1].SetColor(theColor);
        }
    }

    //! OpenGL material definition
    public struct OpenGl_MaterialCommon
    {  //! Set material color.
     public    void SetColor( OpenGl_Vec3 theColor)
  {
    // apply the same formula as in Graphic3d_MaterialAspect::SetColor()
    Ambient.SetValues(theColor* 0.25f, Ambient.a());
    Diffuse.SetValues(theColor, Diffuse.a());
  }
        public OpenGl_Vec4 Diffuse;           //!< diffuse RGB coefficients + alpha
        public OpenGl_Vec4 Emission;          //!< material RGB emission
        public OpenGl_Vec4 SpecularShininess; //!< glossy  RGB coefficients + shininess
        public OpenGl_Vec4 Ambient;           //!< ambient RGB coefficients
    }

}