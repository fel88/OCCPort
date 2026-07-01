global using OpenGl_Mat4 = TKernel.NCollection_Mat4<float>;

namespace OCCPort.OpenGL
{
    //! Defines state of OCCT world-view transformation.
    public class OpenGl_WorldViewState : OpenGl_StateInterface
    {
        // =======================================================================
        // function : Set
        // purpose  : Sets new world-view matrix
        // =======================================================================
        public void Set(OpenGl_Mat4 theWorldViewMatrix)
        {
            myWorldViewMatrix = theWorldViewMatrix;
            myInverseNeedUpdate = true;
        }
        //! Returns current world-view matrix.
        public OpenGl_Mat4 WorldViewMatrix() { return myWorldViewMatrix; }
        // =======================================================================
        // function : WorldViewMatrixInverse
        // purpose  : Returns inverse of current world-view matrix
        // =======================================================================
        public OpenGl_Mat4 WorldViewMatrixInverse()
        {
            if (myInverseNeedUpdate)
            {
                myInverseNeedUpdate = false;
                myWorldViewMatrix.Inverted(out myWorldViewMatrixInverse);
            }
            return myWorldViewMatrixInverse;
        }

        OpenGl_Mat4 myWorldViewMatrix= new TKernel.NCollection_Mat4<float>();        //!< OCCT world-view matrix
        OpenGl_Mat4 myWorldViewMatrixInverse; //!< Inverse of OCCT world-view matrix
        bool myInverseNeedUpdate;      //!< Is inversed matrix outdated?
    }



}