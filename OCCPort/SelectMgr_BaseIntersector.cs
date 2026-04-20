using System;

namespace OCCPort
{
    public class SelectMgr_BaseIntersector
    {
        //! Return camera definition.
        public Graphic3d_Camera Camera() { return myCamera; }

        //! Calculates the point on a view ray that was detected during the run of selection algo by given depth.
        //! It makes sense only for intersectors built on a single point.
        //! This method returns infinite point for the base class.
        public gp_Pnt DetectedPoint(double theDepth)
        {
            return new gp_Pnt(RealLast(), RealLast(), RealLast());
        }

        private double RealLast()
        {
            return double.MaxValue;
        }

        protected Graphic3d_Camera myCamera;        //!< camera definition (if builder isn't NULL it is the same as its camera)
        protected SelectMgr_SelectionType mySelectionType; //!< type of selection

    }
}