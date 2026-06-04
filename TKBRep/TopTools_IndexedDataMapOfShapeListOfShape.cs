using System;
using System.Collections.Generic;
using TKBRep;
using TKernel;

namespace OCCPort
{
    public partial class TopTools_IndexedDataMapOfShapeListOfShape : NCollection_IndexedDataMap<TopoDS_Shape, TopTools_ListOfShape, TopTools_ShapeMapHasher>
    {
        //typedef NCollection_IndexedDataMap<TopoDS_Shape,TopTools_ListOfShape,TopTools_ShapeMapHasher> TopTools_IndexedDataMapOfShapeListOfShape;
        //public class Item
        //{
        //    public TopoDS_Shape shape;
        //    public TopTools_ListOfShape list;
        //}
        ////public List<Item> items = new List<Item>();
        public new int Add(TopoDS_Shape topoDS_Shape, TopTools_ListOfShape empty)
        {
           return base.Add(topoDS_Shape, empty);
        }

        //int mySize;
        ////! IsEmpty
        //bool IsEmpty()
        //{ return items.Count == 0; }
        //internal int FindIndex(TopoDS_Shape topoDS_Shape)
        //{
        //    if (IsEmpty()) return 0;
        //    for (int i = 0; i < items.Count; i++)
        //    {
        //        Item item = items[i];
        //        if (TopTools_ShapeMapHasher.IsEqual(item.shape, topoDS_Shape))
        //        {
        //            return i + 1;
        //        }
        //    }
        //    /*
        //    IndexedDataMapNode* pNode1 = (IndexedDataMapNode*)myData1[Hasher::HashCode(theKey1, NbBuckets())];
        //    while (pNode1)
        //    {
        //        if (Hasher::IsEqual(pNode1->Key1(), theKey1))
        //        {
        //            return pNode1->Index();
        //        }
        //        pNode1 = (IndexedDataMapNode*)pNode1->Next();
        //    }*/
        //    return 0;
        //}

        //internal TopTools_ListOfShape Get(int index)
        //{
        //    return items[index - 1].list;
        //}

        //internal int Extent()
        //{
        //    return items.Count;
        //}
        internal class Iterator
        {
            TopTools_IndexedDataMapOfShapeListOfShape source = null;
            public Iterator(TopTools_IndexedDataMapOfShapeListOfShape anEdgeMap)
            {
                source = anEdgeMap;
            }

            int index=0;
            internal bool More()
            {
                return index < source.Extent();
            }

            internal TopoDS_Shape Key()
            {
                return source.dic[index].Key;
            }

            internal TopTools_ListOfShape Value()
            {
                return source.dic[index].Value;

            }

            internal void Next()
            {
                index++;
            }
        }
    }
}