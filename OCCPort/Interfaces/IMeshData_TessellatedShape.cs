namespace OCCPort.Interfaces
{
    //! Interface class representing shaped model with deflection.

    public interface IMeshData_TessellatedShape : IMeshData_Shape

    {
        //! Gets deflection value for the discrete model.
        double GetDeflection();
        //! Sets deflection value for the discrete model.
        void SetDeflection(double theValue);

        double myDeflection { get; set; }
    }
}