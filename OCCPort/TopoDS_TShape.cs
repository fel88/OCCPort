using System;

namespace OCCPort
{
    public class TopoDS_TShape
    {
        internal TopoDS_ListOfShape myShapes;

        internal void Checked(bool v)
        {
            throw new NotImplementedException();
        }

        //! Sets the closedness flag.
        public void Closed(bool theIsClosed) { setFlag(TopoDS_TShape_Flags.TopoDS_TShape_Flags_Closed, theIsClosed); }
        //! Set bit flag.
        void setFlag(TopoDS_TShape_Flags theFlag,
                      bool theIsOn)
        {
            if (theIsOn) myFlags |= (int)theFlag;
            else myFlags &= ~(int)theFlag;
        }

        


        //! Returns the closedness flag.
        public bool Closed() { return ((myFlags & (int)TopoDS_TShape_Flags.TopoDS_TShape_Flags_Closed) != 0); }

        // Defined mask values
        enum TopoDS_TShape_Flags
        {
            TopoDS_TShape_Flags_Free = 0x001,
            TopoDS_TShape_Flags_Modified = 0x002,
            TopoDS_TShape_Flags_Checked = 0x004,
            TopoDS_TShape_Flags_Orientable = 0x008,
            TopoDS_TShape_Flags_Closed = 0x010,
            TopoDS_TShape_Flags_Infinite = 0x020,
            TopoDS_TShape_Flags_Convex = 0x040,
            TopoDS_TShape_Flags_Locked = 0x080
        };

        int myFlags;

        internal bool Free()
        {
            throw new NotImplementedException();
        }

        internal void Free(bool theIsFree)
        {
            throw new NotImplementedException();
        }

        internal void Modified(bool v)
        {
            throw new NotImplementedException();
        }
    }
}