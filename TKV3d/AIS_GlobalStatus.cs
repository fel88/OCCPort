using OCCPort.Common;
using System.Reflection.Metadata;
using TKernel;

namespace TKV3d
{
    //! Stores information about objects in graphic context:
    public class AIS_GlobalStatus
    {
        internal bool AddSelectionMode(int theMode)
        {
            if (!mySelModes.Contains(theMode))
            {
                mySelModes.Append(theMode);
                return true;
            }
            return false;
        }

        //! Sets highlighted state.
        public void SetHilightStatus(bool theStatus) { myIsHilit = theStatus; }

        //! Changes applied highlight style for a particular object
        public void SetHilightStyle(Prs3d_Drawer theStyle) { myHiStyle = theStyle; }

        //! Remove selection mode.
        public bool RemoveSelectionMode(int theMode)
        {
            return mySelModes.Remove(theMode);
        }


        //! Returns active selection modes of the object.
        public TColStd_ListOfInteger SelectionModes() { return mySelModes; }
        TColStd_ListOfInteger mySelModes = new TColStd_ListOfInteger();

        internal int DisplayMode()
        {
            return myDispMode;
        }

        internal Prs3d_Drawer HilightStyle()
        {
            return myHiStyle;
        }

        internal bool IsHilighted()
        {
            return myIsHilit;
        }
        Prs3d_Drawer myHiStyle;


        int myDispMode;
        bool myIsHilit;
        bool mySubInt;
        //! Sets display mode.
        internal void SetDisplayMode(int theMode)
        {
            myDispMode = theMode;
        }

        //! Remove all selection modes.
        public void ClearSelectionModes()
        {
            mySelModes.Clear();
        }

        internal bool IsSubIntensityOn()
        {
            return mySubInt;
        }
    }
}

