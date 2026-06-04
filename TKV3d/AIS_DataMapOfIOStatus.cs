namespace TKV3d
{
    //typedef NCollection_DataMap<Handle(AIS_InteractiveObject),Handle(AIS_GlobalStatus),TColStd_MapTransientHasher> AIS_DataMapOfIOStatus;
    public class AIS_DataMapOfIOStatus : Dictionary<AIS_InteractiveObject, AIS_GlobalStatus>
    {
        

        internal void Bind(AIS_InteractiveObject theIObj, AIS_GlobalStatus aStatus)
        {
            Add(theIObj, aStatus);
        }

        internal AIS_GlobalStatus ChangeSeek(AIS_InteractiveObject theIObj)
        {
            throw new NotImplementedException();
        }

        internal bool IsBound(AIS_InteractiveObject theIObj)
        {
            return ContainsKey(theIObj);
        }

        internal void UnBind(AIS_InteractiveObject theIObj)
        {
            Remove(theIObj);
        }
    }
}

