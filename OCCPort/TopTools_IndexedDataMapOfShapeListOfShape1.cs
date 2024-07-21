using OCCPort.Tester;
using System;
using System.Collections.Generic;

namespace OCCPort
{
    internal partial class TopTools_IndexedDataMapOfShapeListOfShape
    {
        //typedef NCollection_IndexedDataMap<TopoDS_Shape,TopTools_ListOfShape,TopTools_ShapeMapHasher> TopTools_IndexedDataMapOfShapeListOfShape;
        public class Item
        {
            public TopoDS_Shape shape;
            public TopTools_ListOfShape list;
        }
        public List<Item> items = new List<Item>();
        internal int Add(TopoDS_Shape topoDS_Shape, TopTools_ListOfShape empty)
        {
            items.Add(new Item() { list = empty, shape = topoDS_Shape });
            return items.Count;
        }

        int mySize;
        //! IsEmpty
        bool IsEmpty()
        { return items.Count == 0; }
        internal int FindIndex(TopoDS_Shape topoDS_Shape)
        {
            if (IsEmpty()) return 0;
            for (int i = 0; i < items.Count; i++)
            {
                Item item = items[i];
                if (TopTools_ShapeMapHasher.IsEqual(item.shape, topoDS_Shape))
                {
                    return i + 1;
                }
            }
            /*
            IndexedDataMapNode* pNode1 = (IndexedDataMapNode*)myData1[Hasher::HashCode(theKey1, NbBuckets())];
            while (pNode1)
            {
                if (Hasher::IsEqual(pNode1->Key1(), theKey1))
                {
                    return pNode1->Index();
                }
                pNode1 = (IndexedDataMapNode*)pNode1->Next();
            }*/
            return 0;
        }

        internal TopTools_ListOfShape Get(int index)
        {
            return items[index - 1].list;
        }
    }
}