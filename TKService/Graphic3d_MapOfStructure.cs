global using Prs3d_Presentation = TKService.Graphic3d_Structure;
using TKernel;

namespace TKService
{
    public class Graphic3d_MapOfStructure : NCollection_Map<Graphic3d_Structure>
    {
        public Graphic3d_MapOfStructure()
        {

        }

        public class Iterator
        {
            public bool More()
            {
                throw new NotImplementedException();
            }

            public object Next()
            {
                throw new NotImplementedException();
            }
        }
    }

}
