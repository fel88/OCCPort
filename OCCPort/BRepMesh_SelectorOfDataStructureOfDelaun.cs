using System;

namespace OCCPort
{
    internal class BRepMesh_SelectorOfDataStructureOfDelaun
    {
        public BRepMesh_SelectorOfDataStructureOfDelaun(BRepMesh_DataStructureOfDelaun myMeshData)
        {
            myMesh = myMeshData;
        }
        //! Returns number of links.
        public int NbElements()
        {
            return myElements.Size();
        }

        //! Returns selected elements.
        public MapOfInteger Elements()
        {
            return myElements;
        }

        BRepMesh_DataStructureOfDelaun myMesh;
        MapOfInteger myElements = new MapOfInteger();

        internal void NeighboursOfNode(int theNodeIndex)
        {
            foreach (var aLinkIt in myMesh.LinksConnectedTo(theNodeIndex))
            {
                elementsOfLink(aLinkIt);
            }
        }

        public void elementsOfLink(int theIndex)
        {
            BRepMesh_PairOfIndex aPair = myMesh.ElementsConnectedTo(theIndex);
            for (int j = 1, jn = aPair.Extent(); j <= jn; ++j)
                myElements.Add(aPair.Index(j));
        }

    }
}