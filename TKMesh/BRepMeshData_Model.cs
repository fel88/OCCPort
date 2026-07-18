//typedef NCollection_Shared<NCollection_Vector<IEdgeHandle> >          VectorOfIEdgeHandles;
global using VectorOfIEdgeHandles = TKernel.NCollection_Vector<TKMesh.IMeshData_Edge>;
using OCCPort;
using TKBRep;

namespace TKMesh
{
    //! Default implementation of model entity.
    public class BRepMeshData_Model : AbstractMeshData_Model, IMeshData_Model
    {
        public BRepMeshData_Model(TopoDS_Shape theShape) : base(theShape)
        {
            myMaxSize = 0.0;
            //myAllocator(new NCollection_IncAllocator(IMeshData::MEMORY_BLOCK_SIZE_HUGE)),
            //myDFaces(256, myAllocator),
            //myDEdges(256, myAllocator)

            //myAllocator->SetThreadSafe();
        }
        //=======================================================================
        // Function: EdgesNb
        // Purpose : 
        //=======================================================================
        public override int EdgesNb()
        {
            return myDEdges.Size();
        }

        public override IMeshData_Face GetFace(int theIndex)
        {
            return myDFaces[(theIndex)];
        }


        VectorOfIEdgeHandles myDEdges = new VectorOfIEdgeHandles();

        public override IMeshData_Face AddFace(TopoDS_Face theFace)
        {
            var aFace = new BRepMeshData_Face(theFace);
            myDFaces.Append(aFace);
            return myDFaces[(FacesNb() - 1)];
        }

        //! Adds new edge to shape model.
        public override IMeshData_Edge AddEdge(TopoDS_Edge theEdge)
        {
            IMeshData_Edge aEdge = new BRepMeshData_Edge(theEdge);//(new(myAllocator) BRepMeshData_Edge(theEdge, myAllocator));
            myDEdges.Append(aEdge);
            return myDEdges[(EdgesNb() - 1)];
        }


        //! Sets maximum size of shape's bounding box.
        public void SetMaxSize(double theValue)
        {

            myMaxSize = theValue;
        }

        public override IMeshData_Edge GetEdge(int v)
        {
            return myDEdges[v];
        }

        //! Returns maximum size of shape's bounding box.
        public override double GetMaxSize()
        {
            return myMaxSize;
        }

        double myMaxSize;
        //Handle(NCollection_IncAllocator) myAllocator;
        //IMeshData::VectorOfIFaceHandles myDFaces;
        //IMeshData::VectorOfIEdgeHandles myDEdges;
    }

    
}


