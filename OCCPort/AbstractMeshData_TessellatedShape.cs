using OCCPort.Interfaces;

namespace OCCPort
{
    public abstract class AbstractMeshData_TessellatedShape : AbstractMeshData_Shape, IMeshData_Shape, IMeshData_TessellatedShape, IMeshData_StatusOwner
    {
        //! Constructor.
        public AbstractMeshData_TessellatedShape(TopoDS_Shape theShape)
     : base(theShape)

        {
            myDeflection = Standard_Real.RealLast();
        }

        //! Adds status to status flags of a face.
        public void SetStatus(IMeshData_Status theValue)
        {
            myStatus |= theValue;
        }

        //! Gets deflection value for the discrete model.
        public double GetDeflection()
        {
            return myDeflection;
        }
        //! Sets deflection value for the discrete model.
        public void SetDeflection(double theValue)
        {
            myDeflection = theValue;
        }

        IMeshData_Status myStatus;

        public bool IsSet(IMeshData_Status theValue)
        {
            return (myStatus & theValue) != 0;
        }

        public IMeshData_Status GetStatusMask()
        {
            return myStatus;
        }

        public double myDeflection { get; set; }
    }
}