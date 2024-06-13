using System.Net.Security;

namespace OCCPort
{
    public class Graphic3d_MapOfStructure
    {
		public Graphic3d_MapOfStructure(Graphic3d_Structure ss)
		{
			key = ss;
		}

        Graphic3d_Structure key;
        public Graphic3d_Structure Key()
        {
            return key;
        }
    }
}
