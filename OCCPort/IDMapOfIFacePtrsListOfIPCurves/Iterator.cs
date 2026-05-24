using OCCPort;
using OCCPort.Interfaces;
using System;

namespace OCCPort;

public class IDMapOfIFacePtrsListOfIPCurves : NCollection_IndexedDataMap<IFacePtr, ListOfIPCurves, WeakEqual<IMeshData_Face>>
{

    //public bool Contains(IMeshData_Face obj)
    //{

    //    return false;
    //}
    public class Iterator
    {
        public Iterator(IDMapOfIFacePtrsListOfIPCurves aMapOfPCurves)
        {
        }

        internal IMeshData_Face Key()
        {
            throw new NotImplementedException();
        }

        internal bool More()
        {
            throw new NotImplementedException();
        }

        internal object Next()
        {
            throw new NotImplementedException();
        }

        internal ListOfIPCurves Value()
        {
            throw new NotImplementedException();
        }
    }
}
