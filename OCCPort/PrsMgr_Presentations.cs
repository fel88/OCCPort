using System;
using System.Collections.Generic;

namespace OCCPort
{
    public class PrsMgr_Presentations
    {
		List<PrsMgr_Presentation> list = new List<PrsMgr_Presentation>();

        internal void Append(PrsMgr_Presentation aPrs)
        {
            list.Add(aPrs);
        }

        internal class Iterator
        {
			PrsMgr_Presentations list;
            public Iterator(PrsMgr_Presentations myPresentations)
            {
				list = myPresentations;
            }

			int index = 0;
            internal bool More()
            {
				return index < list.list.Count - 1;
            }

			internal PrsMgr_Presentation Next()
            {
				return list.list[index++];
            }

            internal PrsMgr_Presentation Value()
            {
				return list.list[index];
            }
        }
    }
}