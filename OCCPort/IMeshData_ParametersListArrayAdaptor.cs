using System;

namespace OCCPort
{
    //! Auxiliary tool representing adaptor interface for child classes of 
    //! IMeshData_ParametersList to be used in tools working on NCollection_Array structure.
    public class IMeshData_ParametersListArrayAdaptor: IParametersCollection, IParametersListPtrType
    {
        //! Constructor. Initializes tool by the given parameters.
        public IMeshData_ParametersListArrayAdaptor(IParametersListPtrType theParameters)

        {
            myParameters = (theParameters);
        }
        IParametersListPtrType myParameters;
        public int Lower()
        {
            return 0;
        }
        public int ParametersNb()
        {
            return myParameters.ParametersNb();
        }

        public double Value(int theIndex)
        {
            return myParameters.GetParameter(theIndex);

        }
        //! Returns upper index in parameters array.
        public int Upper()
        {
            return myParameters.ParametersNb() - 1;
        }

        public double GetParameter(int theIndex)
        {
            return myParameters.GetParameter(theIndex);
        }
    }
}