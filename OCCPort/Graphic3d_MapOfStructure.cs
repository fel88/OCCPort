using OCCPort.Tester;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Security;


namespace OCCPort
{
    public class Graphic3d_MapOfStructure:List<Graphic3d_Structure>
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
