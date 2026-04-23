using System;

namespace OCCPort
{
	public class TColStd_Array1OfInteger : NCollection_Array1<int>
	{
        public TColStd_Array1OfInteger(int theLower, int theUpper) : base(theLower, theUpper)
        {
        }

        public int Length()
		{
			return list.Count;
		}
	}
}