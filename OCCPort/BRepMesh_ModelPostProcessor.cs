using OCCPort.Interfaces;

namespace OCCPort
{
    //! Class implements functionality of model post-processing tool.
    //! Stores polygons on triangulations to TopoDS_Edge.
    public class BRepMesh_ModelPostProcessor : IMeshTools_ModelAlgo
    {
        public override bool performInternal(IMeshData_Model theModel, IMeshTools_Parameters theParameters, Message_ProgressRange theRange)
        {
            return true;
        }
    }
}