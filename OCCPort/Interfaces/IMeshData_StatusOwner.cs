namespace OCCPort.Interfaces
{
    //! Extension interface class providing status functionality.
   public  interface IMeshData_StatusOwner
    {
        public IMeshData_Status GetStatusMask();

        //! Adds status to status flags of a face.
        public void SetStatus(IMeshData_Status theValue);
  
      bool IsSet(IMeshData_Status meshData_SelfIntersectingWire);
    }
}