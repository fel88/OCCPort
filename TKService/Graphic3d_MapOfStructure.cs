using TKernel;

namespace TKService
{
    public class Graphic3d_MapOfStructure : NCollection_Map<Graphic3d_Structure>
    {
        public Graphic3d_MapOfStructure()
        {

        }

        Prs3d_Presentation key;
        public Prs3d_Presentation Key()
        {
            return key;
        }

        internal int Extent()
        {
            return Count;
        }
    }

}
