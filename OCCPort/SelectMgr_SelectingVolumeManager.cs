using System;

namespace OCCPort
{
    internal class SelectMgr_SelectingVolumeManager
    {
        internal void AllowOverlapDetection(bool theIsToAllow)
        {
            throw new NotImplementedException();
        }

		SelectMgr_BaseIntersector myActiveSelectingVolume;

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