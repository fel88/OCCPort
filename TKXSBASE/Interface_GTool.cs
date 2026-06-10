namespace TKXSBASE
{
    //! GTool - General Tool for a Model
    //! Provides the functions performed by Protocol/GeneralModule for
    //! entities of a Model, and recorded in a GeneralLib
    //! Optimized : once an entity has been queried, the GeneralLib is
    //! not longer queried
    //! Shareable between several users : as a Handle
    public class Interface_GTool
    {


        Interface_Protocol theproto;
        //Interface_SignType thesign;
        //  Interface_GeneralLib thelib;
        Interface_DataMapOfTransientInteger thentnum;
        TColStd_IndexedDataMapOfTransientTransient thentmod;

        public void ClearEntities()
        { thentnum.Clear(); thentmod.Clear(); }

    }
}
