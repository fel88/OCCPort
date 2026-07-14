//typedef NCollection_Shared<NCollection_Map<IEdgePtr, WeakEqual<IMeshData_Edge> > >                            MapOfIEdgePtr;
global using MapOfIEdgePtr = TKernel.NCollection_Map<TKMesh.IMeshData_Edge, TKMesh.WeakEqual<TKMesh.IMeshData_Edge>>;

namespace TKMesh
{
    internal class BRepMesh_FaceChecker
    {
        public BRepMesh_FaceChecker(IFaceHandle theDFace, IMeshTools_Parameters myParameters)
        {
        }

        internal MapOfIEdgePtr GetIntersectingEdges()
        {
            throw new NotImplementedException();
        }

        internal bool Perform()
        {
            return true;
        }
    }
}