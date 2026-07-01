//! List of shader objects.
//! List of custom uniform shader variables.
//! List of shader variable setters.
//! List of custom vertex shader attributes



using TKService;





namespace OCCPort.OpenGL
{
    //! Interface for generic setter of user-defined uniform variables.
    public abstract class OpenGl_SetterInterface
    {
        //! Sets user-defined uniform variable to specified program.
        public abstract void Set(OpenGl_Context theCtx,
                         Graphic3d_ShaderVariable theVariable,
                         OpenGl_ShaderProgram theProgram);

        ////! Destructor
        //virtual ~OpenGl_SetterInterface() { }
    };
}