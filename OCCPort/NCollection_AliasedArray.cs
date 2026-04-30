using OpenTK.Graphics.ES11;

namespace OCCPort
{
    public class NCollection_AliasedArray
    {
      protected  byte[] myData;      //!< data pointer
        protected int myStride;    //!< element size
        protected int mySize;      //!< number of elements
        protected byte myDeletable; //!< flag showing who allocated the array
    }
}