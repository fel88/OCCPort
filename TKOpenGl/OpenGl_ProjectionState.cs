using OpenTK.Mathematics;

namespace OCCPort.OpenGL
{
    //! Defines state of OCCT projection transformation.
    public class OpenGl_ProjectionState : OpenGl_StateInterface
    {

        OpenGl_Mat4 myProjectionMatrix=new TKernel.NCollection_Mat4<float> ();        //!< OCCT projection matrix
        OpenGl_Mat4 myProjectionMatrixInverse; //!< Inverse of OCCT projection matrix
        bool myInverseNeedUpdate;       //!< Is inversed matrix outdated?
                                        // function : Set
                                        // purpose  : Sets new OCCT projection state
                                        // =======================================================================
        public void Set(OpenGl_Mat4 theProjectionMatrix)
        {
            myProjectionMatrix = theProjectionMatrix;
            myInverseNeedUpdate = true;
        }


        //! Returns current projection matrix.
        public OpenGl_Mat4 ProjectionMatrix() { return myProjectionMatrix; }

        // =======================================================================
        // function : ProjectionMatrixInverse
        // purpose  : Returns inverse of current projection matrix
        // =======================================================================
        public OpenGl_Mat4 ProjectionMatrixInverse()
        {
            if (myInverseNeedUpdate)
            {
                myInverseNeedUpdate = false;
                myProjectionMatrix.Inverted(out myProjectionMatrixInverse);
            }
            return myProjectionMatrixInverse;
        }
    }
}