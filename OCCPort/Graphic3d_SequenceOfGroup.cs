using System;
using System.Collections.Generic;

namespace OCCPort
{
	public class Graphic3d_SequenceOfGroup: List<Graphic3d_Group>
    {
		

		public bool IsEmpty()
		{
			return Count == 0;
		}
		public void Append(Graphic3d_Group aGroup)
		{
			Add(aGroup);
		}

        internal Graphic3d_Group Last()
        {
            return this[this.Count - 1];
        }
        //typedef NCollection_Sequence<Handle(Graphic3d_Group)> Graphic3d_SequenceOfGroup;
    }
}