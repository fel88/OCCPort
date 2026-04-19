namespace OCCPort
{
    //! Class provides base functionality to build face triangulation using Dealunay approach.
    //! Performs generation of mesh using raw data from model.
    public abstract class BRepMesh_ConstrainedBaseMeshAlgo : BRepMesh_BaseMeshAlgo
    {  //! Returns size of cell to be used by acceleration circles grid structure.
        public virtual (int, int) getCellsCount(int theVerticesNb)
        {
            return (-1, -1);
        }
    }
}