namespace OCCPort
{
    //! Interface class providing API for algorithms intended to create mesh for discrete face.
    public interface IMeshTools_MeshAlgo
    {

        //! Performs processing of the given face.
        void Perform(
    IMeshData_Face theDFace,
    IMeshTools_Parameters theParameters,
    Message_ProgressRange theRange);

    }
}