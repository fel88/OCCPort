using System.Collections.Generic;
using TKernel;

namespace OCCPort
{
    public class TColStd_HArray1OfReal : NCollection_Array1<double>, IParametersCollection
    {
        public TColStd_HArray1OfReal(int theLower, int theUpper) : base(theLower, theUpper)
        {
        }
    }
}