using OCCPort.Interfaces;
using System;
using TKernel;

namespace OCCPort;

public class IDMapOfIFacePtrsListOfIPCurves : NCollection_IndexedDataMap<IFacePtr, ListOfIPCurves, WeakEqual<IMeshData_Face>>
{
   
}
