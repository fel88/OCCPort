using TKernel;

namespace TKMesh
{
    //typedef NCollection_Shared<NCollection_DataMap<IFacePtr, ListOfInteger, WeakEqual<IMeshData_Face> > >  
    internal class DMapOfIFacePtrsListOfInteger: NCollection_DataMap<IFacePtr, ListOfInteger,WeakEqual<IMeshData_Face>>
    {
        

        public DMapOfIFacePtrsListOfInteger(int v)
        {

        }

        
        
    }
}




