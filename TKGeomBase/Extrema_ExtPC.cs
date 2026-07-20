using OCCPort.Common;
using TKG3d;
using TKMath;

namespace TKGeomBase
{
    public interface IExtrema_ExtPC
    {
        int NbExt();
        bool IsDone();

        double SquareDistance(int i);

        //! Returns the point of the <N>th extremum distance.
         Extrema_POnCurv Point( int N) ;


    }

    
}
