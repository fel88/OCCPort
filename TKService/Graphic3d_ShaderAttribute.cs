//! List of shader objects.

namespace TKService
{
    //! Describes custom vertex shader attribute.
    public class Graphic3d_ShaderAttribute
    {


        //! Returns attribute location to be bound on GLSL program linkage stage.
        public int Location()
        {
            return myLocation;
        }
        //! Returns name of shader variable.
        public string Name()
        {
            return myName;
        }


        string myName;     //!< attribute name
        int myLocation; //!< attribute location

    }
}
