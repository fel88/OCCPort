namespace OCCPort
{
    //! Interface class providing API for algorithms intended to update or modify discrete model.
    public abstract class IMeshTools_ModelAlgo
    {

        //! Performs processing of the given model.
        public abstract bool performInternal(
    IMeshData_Model theModel,
    IMeshTools_Parameters theParameters,
    Message_ProgressRange theRange);

        //! Exceptions protected processing of the given model.
        public bool Perform(
    IMeshData_Model theModel,
    IMeshTools_Parameters theParameters,
    Message_ProgressRange theRange)
        {
            try
            {
                //OCC_CATCH_SIGNALS

                return performInternal(theModel, theParameters, theRange);
            }
            catch (Standard_Failure ex)
            {
                return false;
            }
        }
    }


}