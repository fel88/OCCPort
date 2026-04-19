using OCCPort.Interfaces;

namespace OCCPort
{
    public abstract class AbstractMeshData_TessellatedShape : AbstractMeshData_Shape, IMeshData_Shape, IMeshData_TessellatedShape
    {
        //! Constructor.
        public AbstractMeshData_TessellatedShape(TopoDS_Shape theShape)
     : base(theShape)

        {
            myDeflection = Standard_Real.RealLast();
        }
        //! Gets deflection value for the discrete model.
        public double GetDeflection()
        {
            return myDeflection;
        }

        public double myDeflection { get; set; }
    }
}