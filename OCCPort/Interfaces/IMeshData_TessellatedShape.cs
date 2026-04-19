namespace OCCPort.Interfaces
{
    //! Interface class representing shaped model with deflection.

    public interface IMeshData_TessellatedShape : IMeshData_Shape

    {
        //! Gets deflection value for the discrete model.
        double GetDeflection();


        double myDeflection { get; set; }
    }
}