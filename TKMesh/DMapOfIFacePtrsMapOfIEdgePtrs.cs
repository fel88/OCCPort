using System.Reflection.Metadata;
using TKernel;

namespace TKMesh
{
    internal class DMapOfIFacePtrsMapOfIEdgePtrs: NCollection_DataMap<IFacePtr, MapOfIEdgePtr, WeakEqual<IMeshData_Face> > 
    {
    }
}

