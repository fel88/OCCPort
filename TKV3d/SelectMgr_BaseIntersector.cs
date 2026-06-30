using OCCPort.Common;
using TKMath;
using TKService;

namespace TKV3d
{
    //! This class is an interface for different types of selecting intersector,
    //! defining different selection types, like point, box or polyline
    //! selection. It contains signatures of functions for detection of
    //! overlap by sensitive entity and initializes some data for building
    //! the selecting intersector
    public class SelectMgr_BaseIntersector
    {
        //! Return camera definition.
        public Graphic3d_Camera Camera() { return myCamera; }

        //! Returns current mouse coordinates.
        //! This method returns infinite point for the base class.
        public virtual gp_Pnt2d GetMousePosition()
        {
            gp_Pnt2d aPnt = new gp_Pnt2d(RealLast(), RealLast());
            return aPnt;
        }

        //! Calculates the point on a view ray that was detected during the run of selection algo by given depth.
        //! It makes sense only for intersectors built on a single point.
        //! This method returns infinite point for the base class.
        public gp_Pnt DetectedPoint(double theDepth)
        {
            return new gp_Pnt(RealLast(), RealLast(), RealLast());
        }

        private double RealLast()
        {
            return Standard_Real.RealLast();
        }

        protected Graphic3d_Camera myCamera;        //!< camera definition (if builder isn't NULL it is the same as its camera)
        protected SelectMgr_SelectionType mySelectionType; //!< type of selection

    }
}


