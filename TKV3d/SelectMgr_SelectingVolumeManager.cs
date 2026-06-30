using OCCPort.Common;
using TKMath;
using TKService;

namespace TKV3d
{
    //! This class is used to switch between active selecting volumes depending
    //! on selection type chosen by the user.
    //! The sample of correct selection volume initialization procedure:
    //! @code
    //!   aMgr.InitPointSelectingVolume (aMousePos);
    //!   aMgr.SetPixelTolerance (aTolerance);
    //!   aMgr.SetCamera (aCamera);
    //!   aMgr.SetWindowSize (aWidth, aHeight);
    //!   aMgr.BuildSelectingVolume();
    //! @endcode
    public abstract class SelectMgr_SelectingVolumeManager : SelectBasics_SelectingVolumeManager
    {

        //! Returns mouse coordinates for Point selection mode.
        //! @return infinite point in case of unsupport of mouse position for this active selection volume.
        public override gp_Pnt2d GetMousePosition()
        {
            if (myActiveSelectingVolume==null)
            {
                return new gp_Pnt2d(Standard_Real. RealLast(), Standard_Real.RealLast());
            }
            return myActiveSelectingVolume.GetMousePosition();
        }

        SelectMgr_BaseIntersector myActiveSelectingVolume;
        Graphic3d_SequenceOfHClipPlane myViewClipPlanes;                  //!< view clipping planes
        Graphic3d_SequenceOfHClipPlane myObjectClipPlanes;                //!< object clipping planes
        SelectMgr_ViewClipRange myViewClipRange;
        bool myToAllowOverlap;                  //!< Defines if partially overlapped entities will me detected or not

        //=======================================================================
        // function : AllowOverlapDetection
        // purpose  : If theIsToAllow is false, only fully included sensitives will
        //            be detected, otherwise the algorithm will mark both included
        //            and overlapped entities as matched
        internal void AllowOverlapDetection(bool theIsToAllow)
        {
            myToAllowOverlap = theIsToAllow;
        }

        public void InitBoxSelectingVolume(gp_Pnt2d theMinPt,
                                                                gp_Pnt2d theMaxPt)
        {
            SelectMgr_RectangularFrustum aBoxVolume = (SelectMgr_RectangularFrustum)myActiveSelectingVolume;
            if (aBoxVolume == null)
            {
                aBoxVolume = new SelectMgr_RectangularFrustum();
            }
            aBoxVolume.Init(theMinPt, theMaxPt);
            myActiveSelectingVolume = aBoxVolume;
        }

        //! Calculates the point on a view ray that was detected during the run of selection algo by given depth.
        //! Throws exception if active selection type is not Point.
        //public abstract gp_Pnt DetectedPoint(double theDepth);
        // =======================================================================
        // function : DetectedPoint
        // purpose  : Calculates the point on a view ray that was detected during
        //            the run of selection algo by given depth. Is valid for point
        //            selection only
        // =======================================================================
        public gp_Pnt DetectedPoint(double theDepth)
        {
            Standard_ASSERT_RAISE(myActiveSelectingVolume != null,
              "SelectMgr_SelectingVolumeManager::DetectedPoint() should be called after initialization of selection volume");
            return myActiveSelectingVolume.DetectedPoint(theDepth);
        }

        private void Standard_ASSERT_RAISE(bool cond, string v)
        {
            if (!cond)
                throw new Standard_ASSERT_RAISE(v);
        }

        internal Graphic3d_Camera Camera()
        {
            if (myActiveSelectingVolume == null)
            {
                Graphic3d_Camera anEmptyCamera = new Graphic3d_Camera();
                return anEmptyCamera;
            }
            return myActiveSelectingVolume.Camera();

        }

        internal void InitPointSelectingVolume(gp_Pnt2d aMousePos)
        {
            throw new NotImplementedException();
        }

        internal void SetCamera(Graphic3d_Camera graphic3d_Camera)
        {
            throw new NotImplementedException();
        }

        internal void SetPixelTolerance(object value)
        {
            throw new NotImplementedException();
        }

        internal void SetWindowSize(int aWidth, int aHeight)
        {
            throw new NotImplementedException();
        }

        internal void WindowSize(ref int xx, ref int yy)
        {
            throw new NotImplementedException();
        }
    }
}


