using System;

namespace OCCPort.OpenGL
{
    internal class OpenGl_StateInterface
    {

        //! Returns current state index.
        public int Index() { return myIndex; }

        //! Increment current state.
        public void Update() { ++myIndex; }

        protected int myIndex; //!< current state index
    }
}