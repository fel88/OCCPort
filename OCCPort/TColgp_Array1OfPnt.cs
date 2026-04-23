using System;
using System.Collections.Generic;

namespace OCCPort
{
    public class TColgp_Array1OfPnt : NCollection_Array1<gp_Pnt>
    {
        public TColgp_Array1OfPnt(int theLower, int theUpper) : base(theLower, theUpper)
        {
        }
    }

}