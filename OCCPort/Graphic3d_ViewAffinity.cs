using System;

namespace OCCPort
{
    public class Graphic3d_ViewAffinity
    {
        //! Empty constructor.
        public Graphic3d_ViewAffinity()
        {
            SetVisible(true);
        }

        //! Return visibility flag.
        public bool IsVisible(int theViewId)
        {
            int aBit = 1 << theViewId;
            return (myMask & aBit) != 0;
        }

        internal void SetVisible(bool theIsVisible)
        {

            myMask = (uint)(theIsVisible ? 0xFF : 0x00);

        }
        uint myMask; //!< affinity mask

    }
}