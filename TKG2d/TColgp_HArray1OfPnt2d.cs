using TKMath;

namespace TKG2d
{
    public class TColgp_HArray1OfPnt2d : List<gp_Pnt2d>
    {
        public int Length()
        {
            return this.Count;
        }
    }
}
