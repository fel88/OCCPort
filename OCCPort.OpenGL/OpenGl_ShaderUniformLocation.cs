using OCCPort.Tester;
using TKBRep;

namespace OCCPort.OpenGL
{
    //! Simple class represents GLSL program variable location.

    public class OpenGl_ShaderUniformLocation
    {
        public override string ToString()
        {
            return $"OpenGl_ShaderUniformLocation : {myLocation}";
        }
        int myLocation;

        //! Return TRUE for non-invalid location.
        public static implicit operator bool(OpenGl_ShaderUniformLocation f)
        {
            return f.myLocation != INVALID_LOCATION;
        }
        public int ToInt()
        {
            return myLocation;
        }
        //! Invalid location of uniform/attribute variable.
        public static int INVALID_LOCATION = -1;
        //! Construct an invalid location.
        public OpenGl_ShaderUniformLocation()
        {
            myLocation = (INVALID_LOCATION);
        }

        //! Convert operators help silently put object to GL functions like glUniform*.
        public static implicit operator int(OpenGl_ShaderUniformLocation f)
        {
            return f.myLocation;
        }

        //! Constructor with initialization.
        public OpenGl_ShaderUniformLocation(int theLocation)
        {
            myLocation = (theLocation);
        }

        //! Note you may safely put invalid location in functions like glUniform* - the data passed in will be silently ignored.
        //! @return true if location is not equal to -1.
        public bool IsValid() { return myLocation != INVALID_LOCATION; }


    }
}