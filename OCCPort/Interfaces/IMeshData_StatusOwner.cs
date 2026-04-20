namespace OCCPort.Interfaces
{
    //! Extension interface class providing status functionality.
   public  interface IMeshData_StatusOwner
    {

        bool IsSet(IMeshData_Status meshData_SelfIntersectingWire);
    }
}