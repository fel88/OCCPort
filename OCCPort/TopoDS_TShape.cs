using System;

namespace OCCPort
{
    public abstract class TopoDS_TShape
    {
        public TopoDS_TShape()
        {
            myShapes = new TopoDS_ListOfShape();
        }
        internal TopoDS_ListOfShape myShapes;
        //! Returns the type as a term of the ShapeEnum enum :
        //! VERTEX, EDGE, WIRE, FACE, ....
        public abstract TopAbs_ShapeEnum ShapeType();



        //! Returns the checked flag.
        public bool Checked()  { return ((myFlags & (int)TopoDS_TShape_Flags.TopoDS_TShape_Flags_Checked) != 0); }


    //! Sets the checked flag.
    public void Checked(bool theIsChecked) { setFlag(TopoDS_TShape_Flags.TopoDS_TShape_Flags_Checked, theIsChecked); }


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

        //! Returns the free flag.
        public bool Free()  { return ((myFlags & (int)TopoDS_TShape_Flags.TopoDS_TShape_Flags_Free) != 0); }

    //! Sets the free flag.
    public void Free(bool theIsFree) { setFlag(TopoDS_TShape_Flags.TopoDS_TShape_Flags_Free, theIsFree); }




    //! Sets the modification flag.
    public void Modified(bool theIsModified)
        {
            setFlag(TopoDS_TShape_Flags.TopoDS_TShape_Flags_Modified, theIsModified);
            if (theIsModified)
            {
                setFlag(TopoDS_TShape_Flags.TopoDS_TShape_Flags_Checked, false); // when a TShape is modified it is also unchecked
            }
        }


        //! Returns the modification flag.
        public bool Modified() { return ((myFlags &(int) TopoDS_TShape_Flags.TopoDS_TShape_Flags_Modified) != 0); }

        //! Returns the locked flag.
        public bool Locked() { return ((myFlags & (int)TopoDS_TShape_Flags.TopoDS_TShape_Flags_Locked) != 0); }

        //! Sets the locked flag.
        public void Locked(bool theIsLocked) { setFlag(TopoDS_TShape_Flags.TopoDS_TShape_Flags_Locked, theIsLocked); }


    }
}