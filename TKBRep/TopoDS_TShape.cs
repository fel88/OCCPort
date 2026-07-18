using OCCPort.Common;
using System;
using TKBRep;
using TKG3d;

namespace OCCPort
{
    //! A TShape  is a topological  structure describing a
    //! set of points in a 2D or 3D space.
    //!
    //! A topological shape is a structure made from other
    //! shapes.  This is a deferred class  used to support
    //! topological objects.
    //!
    //! TShapes are   defined   by  their  optional domain
    //! (geometry)  and  their  components  (other TShapes
    //! with  Locations and Orientations).  The components
    //! are stored in a List of Shapes.
    //!
    //! A   TShape contains  the   following boolean flags :
    //!
    //! - Free       : Free or Frozen.
    //! - Modified   : Has been modified.
    //! - Checked    : Has been checked.
    //! - Orientable : Can be oriented.
    //! - Closed     : Is closed (note that only Wires and Shells may be closed).
    //! - Infinite   : Is infinite.
    //! - Convex     : Is convex.
    //!
    //! Users have no direct access to the classes derived
    //! from TShape.  They  handle them with   the classes
    //! derived from Shape.
    public abstract class TopoDS_TShape
    {
        public TopoDS_TShape()
        {
            myShapes = new TopoDS_ListOfShape();
            myFlags = (int)(TopoDS_TShape_Flags.TopoDS_TShape_Flags_Free
          | TopoDS_TShape_Flags.TopoDS_TShape_Flags_Modified
          | TopoDS_TShape_Flags.TopoDS_TShape_Flags_Orientable);
        }

        [Aux]
        public TopoDS_Shape[] InernalShapes => myShapes.ToArray();
        internal TopoDS_ListOfShape myShapes;
        //! Returns the type as a term of the ShapeEnum enum :
        //! VERTEX, EDGE, WIRE, FACE, ....
        public abstract TopAbs_ShapeEnum ShapeType();

        //! Returns the number of direct sub-shapes (children).
        //! @sa TopoDS_Iterator for accessing sub-shapes
        public int NbChildren() { return myShapes.Size(); }

        //! Returns the convexness flag.
        public bool Convex() { return ((myFlags & (int)TopoDS_TShape_Flags.TopoDS_TShape_Flags_Convex) != 0); }

        //! Sets the convexness flag.
        public void Convex(bool theIsConvex) { setFlag(TopoDS_TShape_Flags.TopoDS_TShape_Flags_Convex, theIsConvex); }


        //! Returns a copy  of the  TShape  with no sub-shapes.
        public abstract TopoDS_TShape EmptyCopy();

        //! Returns the checked flag.
        public bool Checked() { return ((myFlags & (int)TopoDS_TShape_Flags.TopoDS_TShape_Flags_Checked) != 0); }


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
        //! Returns the orientability flag.
        public bool Orientable() { return ((myFlags & (int)TopoDS_TShape_Flags.TopoDS_TShape_Flags_Orientable) != 0); }

        //! Returns the free flag.
        public bool Free() { return ((myFlags & (int)TopoDS_TShape_Flags.TopoDS_TShape_Flags_Free) != 0); }

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
        public bool Modified() { return ((myFlags & (int)TopoDS_TShape_Flags.TopoDS_TShape_Flags_Modified) != 0); }

        //! Returns the locked flag.
        public bool Locked() { return ((myFlags & (int)TopoDS_TShape_Flags.TopoDS_TShape_Flags_Locked) != 0); }

        //! Sets the locked flag.
        public void Locked(bool theIsLocked) { setFlag(TopoDS_TShape_Flags.TopoDS_TShape_Flags_Locked, theIsLocked); }


    }
}