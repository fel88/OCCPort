namespace OCCPort.OpenGL
{
    //! Interface for OpenGl resource with following meaning:
    //!  - object can be constructed at any time;
    //!  - should be explicitly Initialized within active OpenGL context;
    //!  - should be explicitly Released    within active OpenGL context (virtual Release() method);
    //!  - can be destroyed at any time.
    //! Destruction of object with unreleased GPU resources will cause leaks
    //! which will be ignored in release mode and will immediately stop program execution in debug mode using assert.
    public class OpenGl_Resource
    {
    }
}