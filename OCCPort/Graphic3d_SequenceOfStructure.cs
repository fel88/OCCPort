using System;
using System.Collections.Generic;

namespace OCCPort
{
    public class Graphic3d_SequenceOfStructure: List<Graphic3d_Structure> 
    {
		
		public Graphic3d_Structure Value(int index)
		{
			return this[index];			
		}
    }
}