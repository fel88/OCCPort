namespace OCCPort.OpenGL
{
    //! Simple class represents GLSL program variable location.

    public class OpenGl_ShaderUniformLocation
    {
        int myLocation;
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