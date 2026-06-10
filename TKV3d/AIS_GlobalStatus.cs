using OCCPort.Common;
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
    }
}

