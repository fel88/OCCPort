namespace OCCPort.OpenGL
{
    //! OpenGL material definition
    public class OpenGl_MaterialCommon
    {   
        //! Set material color.
        public void SetColor(OpenGl_Vec3 theColor)
        {
            // apply the same formula as in Graphic3d_MaterialAspect::SetColor()
            Ambient.SetValues(theColor * 0.25f, Ambient.a());
            Diffuse.SetValues(theColor, Diffuse.a());
        }
        public float Shine() { return SpecularShininess.a(); }
        public OpenGl_Vec4[] ToArray()
        {
            return [Diffuse, Emission, SpecularShininess, Ambient];
        }
        public OpenGl_Vec4 Diffuse = new TKernel.NCollection_Vec4<float>();           //!< diffuse RGB coefficients + alpha
        public OpenGl_Vec4 Emission = new TKernel.NCollection_Vec4<float>();          //!< material RGB emission
        public OpenGl_Vec4 SpecularShininess = new TKernel.NCollection_Vec4<float>(); //!< glossy  RGB coefficients + shininess
        public OpenGl_Vec4 Ambient = new TKernel.NCollection_Vec4<float>();           //!< ambient RGB coefficients
    }

}