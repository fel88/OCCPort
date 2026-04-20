using OpenTK.Mathematics;

namespace OCCPort.OpenGL
{
    internal class OpenGl_ProjectionState : OpenGl_StateInterface
    {

        Matrix4 myProjectionMatrix;        //!< OCCT projection matrix
        Matrix4 myProjectionMatrixInverse; //!< Inverse of OCCT projection matrix
        bool myInverseNeedUpdate;       //!< Is inversed matrix outdated?

        //! Returns current projection matrix.
        public Matrix4 ProjectionMatrix() { return myProjectionMatrix; }
    }
}