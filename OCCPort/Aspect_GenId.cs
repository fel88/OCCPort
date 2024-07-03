using System;
using System.Collections.Generic;
using System.Threading;

namespace OCCPort
{
	internal class Aspect_GenId
	{
		int last = 0;
		internal int Next()
		{
			return last++;
			int aNewId = 0;
			//if (!Next(aNewId))
			{
				throw new Aspect_IdentDefinitionError("Aspect_GenId::Next(), Error: Available == 0");
			}
			return aNewId;

		}

		int  myFreeCount;
		int myLength;
		int myLowerBound;
		int myUpperBound;

		public Aspect_GenId()
		{

		}

		public Aspect_GenId(int theLow,
							int theUpper)
		{

			myFreeCount = (theUpper - theLow + 1);
			myLength = (theUpper - theLow + 1);
			myLowerBound = (theLow);
			myUpperBound = (theUpper);
			if (theLow > theUpper)
			{
				throw new Aspect_IdentDefinitionError("GenId Create Error: wrong interval");
			}
		}

		List<int> myFreeIds = new List<int>();
		//bool Next(ref int theId)
		//{
		//	if (!myFreeIds.IsEmpty())
		//	{
		//		theId = myFreeIds.First();
		//		myFreeIds.RemoveFirst();
		//		return true;
		//	}
		//	else if (myFreeCount < 1)
		//	{
		//		return false;
		//	}

		//	--myFreeCount;
		//	theId = myLowerBound + myLength - myFreeCount - 1;
		//	return Standard_True;
		//}

	}
}