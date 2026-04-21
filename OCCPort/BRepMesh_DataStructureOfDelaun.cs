using System;

namespace OCCPort
{
    //! Describes the data structure necessary for the mesh algorithms in 
    //! two dimensions plane or on surface by meshing in UV space.
    public class BRepMesh_DataStructureOfDelaun
    {
        BRepMesh_VertexTool myNodes;

        //! Returns map of indices of elements registered in mesh.
        public MapOfInteger ElementsOfDomain()
        {
            return myElementsOfDomain;
        }

        MapOfInteger myElementsOfDomain = new MapOfInteger();
        public int NbNodes()
        {
            return myNodes.Extent();
        }
        //! Adds node to the mesh if it is not already in the mesh.
        //! @param theNode node to be added to the mesh.
        //! @param isForceAdd adds the given node to structure without 
        //! checking on coincidence with other nodes.
        //! @return index of the node in the structure.
        public int AddNode(
     BRepMesh_Vertex theNode,
     bool isForceAdd = false)
        {
            int aNodeId = myNodes.Add(theNode, isForceAdd);
            if (!myNodeLinks.IsBound(aNodeId))
                myNodeLinks.Bind(aNodeId, new ListOfInteger(myAllocator));

            return aNodeId;
        }
        NCollection_IncAllocator myAllocator;

        //typedef NCollection_Shared<NCollection_DataMap<Standard_Integer, ListOfInteger> >                             DMapOfIntegerListOfInteger;
        DMapOfIntegerListOfInteger myNodeLinks;

        //=======================================================================
        //function : AddElement
        //purpose  : 
        //=======================================================================
        public int AddElement(BRepMesh_Triangle theElement)
        {
            myElements.Append(theElement);
            int aElementIndex = myElements.Size();
            myElementsOfDomain.Add(aElementIndex);
            //
            //  int (&e)[3] = theElement.myEdges;
            //  for (int i = 0; i < 3; ++i)
            //     myLinks(e[i]).Append(aElementIndex);

            return aElementIndex;
        }

        internal int AddLink(BRepMesh_Edge aLink)
        {
            throw new NotImplementedException();
        }

        VectorOfElements myElements;

        public BRepMesh_DataStructureOfDelaun(NCollection_IncAllocator myAllocator)
        {
            myNodes = (new BRepMesh_VertexTool(myAllocator));
        }
    }
}