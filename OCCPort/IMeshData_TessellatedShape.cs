namespace OCCPort
{
    //! Interface class representing shaped model with deflection.
    public abstract class IMeshData_TessellatedShape : IMeshData_Shape
    {

        //! Constructor.
        public IMeshData_TessellatedShape(TopoDS_Shape theShape)
     : base(theShape)

        {
            myDeflection = Standard_Real.RealLast();
        }
        double myDeflection;
    }
}