using TKernel;

namespace TKV3d
{
    //typedef NCollection_DataMap<Handle(AIS_InteractiveObject),Handle(AIS_GlobalStatus),TColStd_MapTransientHasher> AIS_DataMapOfIOStatus;
    public class AIS_DataMapOfIOStatus : NCollection_DataMap<AIS_InteractiveObject, AIS_GlobalStatus, NCollection_DefaultHasher<AIS_InteractiveObject>>
    {
        internal AIS_GlobalStatus Seek(AIS_InteractiveObject theObj)
        {
            throw new NotImplementedException();
        }
    }
}

