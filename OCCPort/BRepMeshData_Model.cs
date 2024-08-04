using System;

namespace OCCPort
{
    //! Default implementation of model entity.
    public class BRepMeshData_Model : IMeshData_Model
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
            return myDFaces.Get(theIndex);
        }


        VectorOfIEdgeHandles myDEdges = new VectorOfIEdgeHandles();

        public override IMeshData_Face AddFace(TopoDS_Face theFace)
        {
            var aFace = new BRepMeshData_Face(theFace);
            myDFaces.Append(aFace);
            return myDFaces.Get(FacesNb() - 1);
        }

        public override IMeshData_Edge AddEdge(TopoDS_Edge theEdge)
        {
            IMeshData_Edge aEdge = new BRepMeshData_Edge(theEdge);//(new(myAllocator) BRepMeshData_Edge(theEdge, myAllocator));
            myDEdges.Append(aEdge);
            return myDEdges.Get(EdgesNb() - 1);
        }

        //=======================================================================
        // Function: FacesNb
        // Purpose : 
        //=======================================================================
        public override int FacesNb()
        {
            return myDFaces.Size();
        }

        VectorOfIFaceHandles myDFaces = new VectorOfIFaceHandles();

        //! Sets maximum size of shape's bounding box.
        public void SetMaxSize(double theValue)
        {

            myMaxSize = theValue;
        }

        double myMaxSize;
        //Handle(NCollection_IncAllocator) myAllocator;
        //IMeshData::VectorOfIFaceHandles myDFaces;
        //IMeshData::VectorOfIEdgeHandles myDEdges;
    }

    //! Default implementation of face data model entity.
    public class BRepMeshData_Face : IMeshData_Face
    {
        public BRepMeshData_Face(TopoDS_Face theFace) : base(theFace)
        {
            //myDWires(256, myAllocator)
        }

        VectorOfIWireHandles myDWires = new VectorOfIWireHandles();
        public int WiresNb()
        {
            return myDWires.Size();
        }
    }

}