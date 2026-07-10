using System;

namespace OCCPort.OpenGL
{
    //! Interface for OpenGl resource with following meaning:
    //!  - object can be constructed at any time;
    //!  - should be explicitly Initialized within active OpenGL context;
    //!  - should be explicitly Released    within active OpenGL context (virtual Release() method);
    //!  - can be destroyed at any time.
    //! Destruction of object with unreleased GPU resources will cause leaks
    //! which will be ignored in release mode and will immediately stop program execution in debug mode using assert.
    public abstract class OpenGl_Resource
    {

        //! Release GPU resources.
        //! Notice that implementation should be SAFE for several consecutive calls
        //! (thus should invalidate internal structures / ids to avoid multiple-free errors).
        //! @param theGlCtx - bound GL context, shouldn't be NULL.
        public abstract   void Release(OpenGl_Context theGlCtx);

    }
}