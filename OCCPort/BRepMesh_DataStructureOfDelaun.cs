using OCCPort;
using System;

namespace OCCPort
{
    //! Describes the data structure necessary for the mesh algorithms in 
    //! two dimensions plane or on surface by meshing in UV space.
    public class BRepMesh_DataStructureOfDelaun
    {
        BRepMesh_VertexTool myNodes;
        //! Get element by the index.
        //! @param theIndex index of an element.
        //! @return element with the given index.
        public BRepMesh_Triangle GetElement(int theIndex)
        {
            return myElements[theIndex - 1];
        }

        //! Returns map of indices of elements registered in mesh.
        public MapOfInteger ElementsOfDomain()
        {
            return myElementsOfDomain;
        }
        public void ElementNodes(
      BRepMesh_Triangle theElement,
      int[] theNodes)
        {
            int[] e = theElement.myEdges;
            bool[] o = theElement.myOrientations;

            BRepMesh_Edge aLink1 = GetLink(e[0]);
            if (o[0])
            {
                theNodes[0] = aLink1.FirstNode();
                theNodes[1] = aLink1.LastNode();
            }
            else
            {
                theNodes[1] = aLink1.FirstNode();
                theNodes[0] = aLink1.LastNode();
            }

            BRepMesh_Edge aLink2 = GetLink(e[2]);
            if (o[2])
                theNodes[2] = aLink2.FirstNode();
            else
                theNodes[2] = aLink2.LastNode();
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


        //! Adds element to the mesh if it is not already in the mesh.
        //! @param theElement element to be added to the mesh.
        //! @return index of the element in the structure.
        public int AddElement(BRepMesh_Triangle theElement)
        {
            myElements.Append(theElement);
            int aElementIndex = myElements.Size();
            myElementsOfDomain.Add(aElementIndex);

            var e = theElement.myEdges;
            for (int i = 0; i < 3; ++i)
                myLinks[e[i]].Append(aElementIndex);

            return aElementIndex;
        }


        //! Get link by the index.
        //! @param theIndex index of a link.
        //! @return link with the given index.
        public BRepMesh_Edge GetLink(int theIndex)
        {
            return myLinks.FindKey(theIndex);
        }

        //! Adds link to the mesh if it is not already in the mesh.
        //! @param theLink link to be added to the mesh.
        //! @return index of the link in the structure.
        internal int AddLink(BRepMesh_Edge theLink)
        {
            int aLinkIndex = IndexOf(theLink);
            if (aLinkIndex > 0)
            {
                return theLink.IsSameOrientation(GetLink(aLinkIndex)) ?
                   aLinkIndex : -aLinkIndex;
            }

            BRepMesh_PairOfIndex aPair = new BRepMesh_PairOfIndex();
            if (!myDelLinks.IsEmpty())
            {
                aLinkIndex = myDelLinks.First();
                myLinks.Substitute(aLinkIndex, theLink, aPair);
                myDelLinks.RemoveFirst();
            }
            else
                aLinkIndex = myLinks.Add(theLink, aPair);

            int aLinkId = Math.Abs(aLinkIndex);
            linksConnectedTo(theLink.FirstNode()).Append(aLinkId);
            linksConnectedTo(theLink.LastNode()).Append(aLinkId);
            myLinksOfDomain.Add(aLinkIndex);

            return aLinkIndex;
        }

        MapOfInteger myLinksOfDomain = new MapOfInteger();

        //! Get list of links attached to the node with the given index.
        //! @param theIndex index of node whose links should be retrieved.
        //! @return list of links attached to the node.
        ListOfInteger linksConnectedTo(
    int theIndex)
        {
            return (ListOfInteger)myNodeLinks.Find(theIndex);
        }

        //! Finds the index of the given link.
        //! @param theLink link to find.
        //! @return index of the given element of zero if link is not in the mesh.
        public int IndexOf(BRepMesh_Edge theLink)
        {
            return myLinks.FindIndex(theLink);
        }

        public BRepMesh_Vertex GetNode(int theIndex)
        {
            return myNodes.FindKey(theIndex);
        }

        VectorOfElements myElements;
        IDMapOfLink myLinks;
        ListOfInteger myDelLinks;

        public BRepMesh_DataStructureOfDelaun(NCollection_IncAllocator myAllocator, int theReservedNodeSize = 100)
        {
            myNodes = (new BRepMesh_VertexTool(myAllocator));
            myNodeLinks = new DMapOfIntegerListOfInteger(theReservedNodeSize * 3, myAllocator);
            myLinks = new IDMapOfLink(theReservedNodeSize * 3, myAllocator);
            myDelLinks = new ListOfInteger(myAllocator);
            myElements = new VectorOfElements(theReservedNodeSize * 2, myAllocator);

        }
    }
}