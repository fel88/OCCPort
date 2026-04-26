namespace OCCPort
{
    //! Interface class representing list of parameters on curve.
    public interface IMeshData_ParametersList:IParametersListPtrType
    {  //! Returns parameter with the given index.
        double GetParameter(int theIndex);

        //! Returns number of parameters.
        int ParametersNb();

        //! Clears parameters list.
        void Clear(bool isKeepEndPoints);

    }
}