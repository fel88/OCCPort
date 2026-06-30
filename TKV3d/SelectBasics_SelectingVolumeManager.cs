using TKMath;

namespace TKV3d
{
    //! This class provides an interface for selecting volume manager,
    //! which is responsible for all overlap detection methods and
    //! calculation of minimum depth, distance to center of geometry
    //! and detected closest point on entity.
    public abstract class SelectBasics_SelectingVolumeManager
    {  
        //! Returns mouse coordinates for Point selection mode.
       //! @return infinite point in case of unsupport of mouse position for this active selection volume.
        public abstract gp_Pnt2d GetMousePosition() ;

    }
}


