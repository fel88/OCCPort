//! List of shader objects.
//! List of custom uniform shader variables.
//! List of shader variable setters.
using TKService;
//! List of custom vertex shader attributes



using OCCPort.Common;
using TKOpenGl;



namespace OCCPort.OpenGL
{
    //! Support tool for setting user-defined uniform variables.
    public class OpenGl_VariableSetterSelector
    {// =======================================================================
     // function : Set
     // purpose  : Sets generic variable to specified shader program
     // =======================================================================
        public void Set(OpenGl_Context theCtx,
                                           Graphic3d_ShaderVariable theVariable,
                                           OpenGl_ShaderProgram theProgram)
        {
            Exceptions.Standard_ASSERT_RETURN(mySetterList.IsBound(theVariable.Value().TypeID()),
              "The type of user-defined uniform variable is not supported..." );

            mySetterList.Find(theVariable.Value().TypeID()).Set(theCtx, theVariable, theProgram);
        }

        //! List of variable setters.
        OpenGl_SetterList mySetterList;
    }
}