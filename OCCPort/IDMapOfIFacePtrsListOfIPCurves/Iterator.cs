using OCCPort;
using OCCPort.Interfaces;
using System;

namespace OCCPort;

public class IDMapOfIFacePtrsListOfIPCurves : NCollection_IndexedDataMap<IFacePtr, ListOfIPCurves, WeakEqual<IMeshData_Face>>
{
   
}
