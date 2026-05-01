namespace OCCPort
{
    //! Extends base meshing algo in order to enable possibility 
    //! of addition of free vertices into the mesh.

    public class BRepMesh_NodeInsertionMeshAlgo<RangeSplitter> : BRepMesh_DelabellaBaseMeshAlgo  where RangeSplitter : AbstractRangeSplitter //: BaseAlgo
    {
        //! Returns range splitter.
        public RangeSplitter getRangeSplitter()
        {
            return myRangeSplitter;
        }

        RangeSplitter myRangeSplitter;
    }
}