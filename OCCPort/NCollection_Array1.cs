using System.Collections.Generic;

namespace OCCPort
{
	public class NCollection_Array1
	{
		protected List<int> list = new List<int>();
		//! Empty constructor; should be used with caution.
		//! @sa methods Resize() and Move().
		public NCollection_Array1()
		{
			myLowerBound = (1);
			myUpperBound = (0);
			//myDeletable(Standard_False),
			//  myData(NULL)

			//
		}

		public int this[int key]
		{
			get => list[key];
			set => list[key] = value;
		}

		int myLowerBound;
		int myUpperBound;
		public void Init(int val)
		{
			for (int i = myLowerBound; i <= myUpperBound; i++)
			{
				list[i] = val;
			}
		}

		public int Lower()
		{
			return myLowerBound;
		}

		public void SetValue(int v, int aDrawBuffer)
		{
			list[v] = aDrawBuffer;
		}

		public int Upper()
		{
			return myUpperBound;
		}
		public int Value(int index)
		{//Standard_OutOfRange_Raise_if(theIndex<myLowerBound || theIndex> myUpperBound, "NCollection_Array1::Value");
			return list[index];
		}
	}
}