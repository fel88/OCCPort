using System;
using System.Collections;
using System.Collections.Generic;

namespace OCCPort
{
	internal class Poly_ListOfTriangulation: List<Poly_Triangulation>
	{
		

        public void Clear()
        {
            this.Clear();
        }
        public void Append(Poly_Triangulation p)
        {
            this.Add(p);
        }
        
        public void Set(int index,Poly_Triangulation p)
        {
            this[index] = p; 
        }

        public bool IsEmpty()
		{
			return this.Count == 0;
		}

		internal Poly_Triangulation First()
		{
			return this[0];
		}

        
    }
}