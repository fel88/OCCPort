using OpenTK.Graphics.ES11;
using System;
using System.Collections.Generic;

namespace OCCPort.OpenGL
{
    //! Alias to 2D programs array of predefined length
    internal class OpenGl_SetOfShaderPrograms
    {
        internal OpenGl_ShaderProgram ChangeValue(Graphic3d_TypeOfShadingModel theShadingModel, int theProgramBits)
        {
            OpenGl_SetOfPrograms aSet = myPrograms[(int)(theShadingModel - 1)];
            if (aSet == null)
            {
                aSet = new OpenGl_SetOfPrograms();
                myPrograms[(int)(theShadingModel - 1)] = aSet;
            }
            return aSet.ChangeValue(theProgramBits);
        }

        internal void ChangeValue(Graphic3d_TypeOfShadingModel theShadingModel, int theProgramBits, OpenGl_ShaderProgram sp)//not original code
        {
            OpenGl_SetOfPrograms aSet = myPrograms[(int)(theShadingModel - 1)];
            aSet.ChangeValue(theProgramBits, sp);
        }

        OpenGl_SetOfPrograms[] myPrograms = new OpenGl_SetOfPrograms[Graphic3d_TypeOfShadingModel_NB - 1]; //!< programs array, excluding Graphic3d_TypeOfShadingModel_Unlit

        //! Auxiliary value defining the overall number of values in enumeration Graphic3d_TypeOfShadingModel
        const int Graphic3d_TypeOfShadingModel_NB = (int)Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_PbrFacet + 1;
    }
}