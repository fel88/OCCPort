using System;

namespace OCCPort
{
    public abstract class IMeshTools_ModelBuilder : Message_Algorithm
    {
        //! Creates discrete model for the given shape.
        //! Returns nullptr in case of failure.
        public abstract IMeshData_Model performInternal(

    TopoDS_Shape theShape,
    IMeshTools_Parameters theParameters);

        //! Exceptions protected method to create discrete model for the given shape.
        //! Returns nullptr in case of failure.
        public IMeshData_Model Perform(

    TopoDS_Shape theShape,
    IMeshTools_Parameters theParameters)
        {
            ClearStatus();

            try
            {
                //OCC_CATCH_SIGNALS
                return performInternal(theShape, theParameters);
            }
            catch (Exception ex)
            {
                SetStatus(Message_Status.Message_Fail2);
                return null;
            }
        }
    }
}