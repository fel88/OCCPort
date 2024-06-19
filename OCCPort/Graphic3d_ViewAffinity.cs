using System;

namespace OCCPort
{
    public class Graphic3d_ViewAffinity
    {
        internal void SetVisible(bool theIsVisible)
        {

			myMask = (uint)(theIsVisible ? 0xFF : 0x00);

        }
		uint myMask; //!< affinity mask

    }
}