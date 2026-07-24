using OCCPort.Common;
using TKernel;

namespace TKMesh
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
        public void RemoveElement(int theIndex)
        {
            BRepMesh_Triangle aElement = (BRepMesh_Triangle)GetElement(theIndex);
            if (aElement.Movability() == BRepMesh_DegreeOfFreedom.BRepMesh_Deleted)
                return;

            cleanElement(theIndex, aElement);
            aElement.SetMovability(BRepMesh_DegreeOfFreedom.BRepMesh_Deleted);
            myElementsOfDomain.Remove(theIndex);
        }

        void cleanElement(int theIndex, BRepMesh_Triangle theElement)
        {
            if (theElement.Movability() != BRepMesh_DegreeOfFreedom.BRepMesh_Free)
                return;

            int[] e = theElement.myEdges;
            for (int i = 0; i < 3; ++i)
                removeElementIndex(theIndex, myLinks[e[i]]);
        }

        void removeElementIndex(int theIndex, BRepMesh_PairOfIndex thePair)
        {
            for (int i = 1, n = thePair.Extent(); i <= n; ++i)
            {
                if (thePair.Index(i) == theIndex)
                {
                    thePair.RemoveIndex(i);
                    return;
                }
            }
        }
        //! Returns map of indices of links registered in mesh.
        public MapOfInteger LinksOfDomain()
        {
            return myLinksOfDomain;
        }
        //! Returns indices of elements connected to the link with the given index.
        //! @param theLinkIndex index of link whose data should be retrieved.
        //! @return indices of elements connected to the link.
        public BRepMesh_PairOfIndex ElementsConnectedTo(int theLinkIndex)
        {
            return myLinks.FindFromIndex(theLinkIndex);
        }

        //! Get list of links attached to the node with the given index.
        //! @param theIndex index of node whose links should be retrieved.
        //! @return list of links attached to the node.
        public ListOfInteger LinksConnectedTo(int theIndex)
        {
            return linksConnectedTo(theIndex);
        }


        //! Returns number of links.
        public int NbLinks()
        {
            return myLinks.Extent();
        }

        //! Removes link from the mesh in case if it has no connected elements 
        //! and its type is Free.
        //! @param theIndex index of link to be removed.
        //! @param isForce if TRUE link will be removed even if movability
        //! is not Free.
        public void RemoveLink(int theIndex, bool isForce = false)
        {
            BRepMesh_Edge aLink = (BRepMesh_Edge)GetLink(theIndex);
            if (aLink.Movability() == BRepMesh_DegreeOfFreedom.BRepMesh_Deleted ||
                (!isForce && aLink.Movability() != BRepMesh_DegreeOfFreedom.BRepMesh_Free) ||
                ElementsConnectedTo(theIndex).Extent() != 0)
            {
                return;
            }

            cleanLink(theIndex, aLink);
            aLink.SetMovability(BRepMesh_DegreeOfFreedom.BRepMesh_Deleted);

            myLinksOfDomain.Remove(theIndex);
            myDelLinks.Append(theIndex);
        }

        public void cleanLink(
     int theIndex,
     BRepMesh_Edge theLink)
        {
            for (int i = 0; i < 2; ++i)
            {
                int aNodeId = (i == 0) ?
                 theLink.FirstNode() : theLink.LastNode();

                ListOfInteger aLinkList = linksConnectedTo(aNodeId);
                if (aLinkList.Contains(theIndex))
                {
                    aLinkList.Remove(theIndex);
                }
                /*foreach (var aLinkIt in aLinkList)
                {                                
                    if (aLinkIt == theIndex)
                    {
                        aLinkList.Remove(aLinkIt);
                        break;
                    }
                }*/
            }
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
        ListOfInteger linksConnectedTo(int theIndex)
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


        //! Removes node from the mesh in case if it has no connected links 
        //! and its type is Free.
        //! @param theIndex index of node to be removed.
        //! @param isForce if TRUE node will be removed even if movability
        //! is not Free.
        internal void RemoveNode(int theIndex, bool isForce = false)
        {
            if (isForce || myNodes.FindKey(theIndex).Movability() == BRepMesh_DegreeOfFreedom.BRepMesh_Free)
            {
                if (LinksConnectedTo(theIndex).Extent() == 0)
                    myNodes.DeleteVertex(theIndex);
            }
        }

        //! Gives the data structure for initialization of cell size and tolerance.
        public BRepMesh_VertexTool Data()
        {
            return myNodes;
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
    public class VectorOfElements : List<BRepMesh_Triangle>
    {
        public VectorOfElements(int capacity, NCollection_IncAllocator myAllocator) : base(capacity)
        {
        }

        internal void Append(BRepMesh_Triangle theElement)
        {
            Add(theElement);
        }

        internal int Size()
        {
            return Count;
        }
    }

    internal class DMapOfIntegerListOfInteger : NCollection_DataMap<int, ListOfInteger>    
    {
        

        public DMapOfIntegerListOfInteger(int v, NCollection_IncAllocator myAllocator)
        {
        }

        
    }
    internal class IDMapOfLink : NCollection_IndexedDataMap<BRepMesh_Edge, BRepMesh_PairOfIndex, NCollection_DefaultHasher<BRepMesh_Edge>>
    {
        public IDMapOfLink(int v, NCollection_IncAllocator myAllocator)
        {
        }
    }
}

