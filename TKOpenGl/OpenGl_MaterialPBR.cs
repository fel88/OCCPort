using System;
using System.Collections.Generic;

namespace OCCPort.OpenGL
{
    public class OpenGl_MaterialPBR
    {
        //! Empty constructor.
        public OpenGl_MaterialPBR()
        {
            BaseColor = new TKernel.NCollection_Vec4<float> (1.0f);
            EmissionIOR = new TKernel.NCollection_Vec4<float> (1.0f);
            Params = new TKernel.NCollection_Vec4<float>(1.0f, 1.0f, 1.0f, 1.0f);
        }

        //! Set material color.
        public void SetColor(OpenGl_Vec3 theColor)
        {
            BaseColor.SetValues(theColor, BaseColor.a());
        }

        public void ChangeMetallic(float val) {  Params.b(val); }

        internal IEnumerable<OpenGl_Vec4> ToArray()
        {
            return [BaseColor, EmissionIOR, Params];
        }

        internal void ChangeRoughness(float val)
        {
            Params.g(val);
        }

        public OpenGl_Vec4 BaseColor;   //!< base color of PBR material with alpha component
        public OpenGl_Vec4 EmissionIOR; //!< light intensity which is emitted by PBR material and index of refraction
        public OpenGl_Vec4 Params;      //!< extra packed parameters

    }
}