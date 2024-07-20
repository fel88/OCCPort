using System;

namespace OCCPort
{
    public class Graphic3d_MaterialAspect
    {
        public float Alpha()
        {
            throw new NotImplementedException();
        }
        //! Returns TRUE if the reflection mode is active, FALSE otherwise.
        public bool ReflectionMode(Graphic3d_TypeOfReflection theType)
        {
            return !myColors[(int)theType].IsEqual(Quantity_NameOfColor.Quantity_NOC_BLACK);
        }
        const int Graphic3d_TypeOfReflection_NB = 4;
        Quantity_Color[] myColors = new Quantity_Color[Graphic3d_TypeOfReflection_NB];

    }//! Nature of the reflection of a material.
}