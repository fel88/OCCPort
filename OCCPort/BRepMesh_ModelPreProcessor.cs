using OCCPort.Interfaces;

namespace OCCPort
{
    //! Class implements functionality of model pre-processing tool.
    //! Nullifies existing polygonal data in case if model elements
    //! have IMeshData_Outdated status.
    public class BRepMesh_ModelPreProcessor : IMeshTools_ModelAlgo
    {
        public override bool performInternal(IMeshData_Model theModel, IMeshTools_Parameters theParameters, Message_ProgressRange theRange)
        {
            return true;
        }
    }
}