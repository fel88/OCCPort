using System.Reflection.Metadata;
using TKernel;

namespace TKV3d
{
    //! Class holding the list of selected owners.
    public class AIS_Selection
    {
        public AIS_Selection()
        {
            // for maximum performance on medium selections (< 100000 objects)
            //myResultMap.ReSize(THE_MaxSizeOfResult);
        }
        AIS_NListOfEntityOwner.Iterator myIterator;

        //! Start iteration through selected objects.
        public void Init() { myIterator = new AIS_NListOfEntityOwner.Iterator(myresult); }

        //! Return true if iterator points to selected object.
        public bool More() { return myIterator.More(); }

        //! Return selected object at iterator position.
        public SelectMgr_EntityOwner Value() { return myIterator.Value(); }

        //! Continue iteration through selected objects.
        public void Next() { myIterator.Next(); }

        //! Return the number of selected objects.
        public int Extent() { return myresult.Size(); }
        protected AIS_NListOfEntityOwner myresult = new TKernel.NCollection_List<SelectMgr_EntityOwner>();
        NCollection_DataMap<SelectMgr_EntityOwner, AIS_NListOfEntityOwner.Iterator> myResultMap = new NCollection_DataMap<SelectMgr_EntityOwner, NCollection_List<SelectMgr_EntityOwner>.Iterator>();


    }
}

