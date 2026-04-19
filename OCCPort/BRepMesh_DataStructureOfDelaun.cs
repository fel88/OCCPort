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
    }
}