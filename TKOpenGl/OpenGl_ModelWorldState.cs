namespace OCCPort.OpenGL
{
    public class OpenGl_ModelWorldState: OpenGl_StateInterface
    {
        //! Returns current model-world matrix.
        public OpenGl_Mat4 ModelWorldMatrix() { return myModelWorldMatrix; }

        OpenGl_Mat4 myModelWorldMatrix=new TKernel.NCollection_Mat4<float> ();        //!< OCCT model-world matrix
        OpenGl_Mat4 myModelWorldMatrixInverse; //!< Inverse of OCCT model-world matrix
        bool myInverseNeedUpdate;       //!< Is inversed matrix outdated?

        // =======================================================================
        // function : Set
        // purpose  : Sets new model-world matrix
        // =======================================================================
     public    void Set( OpenGl_Mat4 theModelWorldMatrix)
{
  myModelWorldMatrix = theModelWorldMatrix;
  myInverseNeedUpdate = true;
}
        // =======================================================================
        // function : ModelWorldMatrixInverse
        // purpose  : Returns inverse of current model-world matrix
        // =======================================================================
       public OpenGl_Mat4 ModelWorldMatrixInverse()
        {
            if (myInverseNeedUpdate)
            {
                myInverseNeedUpdate = false;
                myModelWorldMatrix.Inverted(out myModelWorldMatrixInverse);
            }
            return myModelWorldMatrixInverse;
        }
    }
}