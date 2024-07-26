using OCCPort;
using System;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;

namespace OCCPort
{
    public class TopoDS_Shape
    {
        public TopoDS_Shape()
        {
            myOrient = TopAbs_Orientation.TopAbs_EXTERNAL;
        }
        public TopoDS_Shape(TopoDS_Shape theOther)
        {
            myOrient = theOther.myOrient;
            myTShape = theOther.myTShape;
            myLocation = theOther.myLocation.Clone();
        }

        public static int idx = 0;
        public int HashCode(int theUpperBound)
        {
            return idx++;
            // PKV
            //  //int aHS = ::HashCode(myTShape.get(), theUpperBound);
            //  int aHL = myLocation.HashCode(theUpperBound);
            //  return ::HashCode(aHS ^ aHL, theUpperBound);
        }

        //! Returns the checked flag.
        public bool Checked()
        {
            return myTShape.Checked();
        }

        //! Sets the checked flag.
        public void Checked(bool theIsChecked) { myTShape.Checked(theIsChecked); }

        //! Returns a  shape  similar to <me> with   the local
        //! coordinate system set to <Loc>.
        public TopoDS_Shape Located(TopLoc_Location theLoc, bool theRaiseExc = true)
        {
            TopoDS_Shape aShape = new TopoDS_Shape(this);
            aShape.Location(theLoc, theRaiseExc);
            return aShape;
        }


        //! Returns the locked flag.
        public bool Locked()
        {
            return myTShape.Locked();
        }

        //! Returns true if this shape is null. In other words, it
        //! references no underlying shape with the potential to
        //! be given a location and an orientation.
        public bool IsNull() { return myTShape == null; }

        //! Destroys the reference to the underlying shape
        //! stored in this shape. As a result, this shape becomes null.
        public void Nullify()
        {
            //myTShape.Nullify();
            myTShape = null;
            myLocation.Clear();
            myOrient = TopAbs_Orientation.TopAbs_EXTERNAL;
        }

        //! Sets the locked flag.
        public void Locked(bool theIsLocked) { myTShape.Locked(theIsLocked); }


        //! Sets the closedness flag.
        public void Closed(bool theIsClosed) { myTShape.Closed(theIsClosed); }

        //! Returns the closedness flag.
        public bool Closed()
        {
            return myTShape.Closed();
        }

        //! Returns the free flag.
        public bool Free() { return myTShape.Free(); }

        //! Sets the free flag.
        public void Free(bool theIsFree) { myTShape.Free(theIsFree); }

        //! Returns the value of the TopAbs_ShapeEnum
        //! enumeration that corresponds to this shape, for
        //! example VERTEX, EDGE, and so on.
        //! Exceptions
        //! Standard_NullObject if this shape is null.
        public TopAbs_ShapeEnum ShapeType() { return myTShape.ShapeType(); }

        public void TShape(TopoDS_TShape theTShape) { myTShape = theTShape; }

        TopoDS_TShape myTShape;
        TopLoc_Location myLocation;
        TopAbs_Orientation myOrient;


        //! Returns a handle to the actual shape implementation.
        public TopoDS_TShape TShape() { return myTShape; }

        //! Sets the shape local coordinate system.
        public void Location(TopLoc_Location theLoc, bool theRaiseExc = true)
        {
            gp_Trsf aTrsf = theLoc.Transformation();
            if ((Math.Abs(Math.Abs(aTrsf.ScaleFactor()) - 1.0) > TopLoc_Location.ScalePrec() || aTrsf.IsNegative()) && theRaiseExc)
            {
                //Exception
                throw new Standard_DomainError("Location with scaling transformation is forbidden");
            }
            else
            {
                myLocation = theLoc;
            }
        }

        //! Returns the shape local coordinate system.
        public TopLoc_Location Location() { return myLocation; }



        //! Sets the shape orientation.
        public void Orientation(TopAbs_Orientation theOrient) { myOrient = theOrient; }

        public TopAbs_Orientation Orientation()
        {
            return myOrient;
        }
        //! Reverses the orientation, using the Reverse method
        //! from the TopAbs package.
        public void Reverse() { myOrient = TopAbs.Reverse(myOrient); }


        internal void Move(object v1, bool v2)
        {
            throw new NotImplementedException();
        }


        //! Returns  a    shape  similar  to  <me>   with  the
        //! orientation set to <Or>.
        public TopoDS_Shape Oriented(TopAbs_Orientation theOrient)
        {
            TopoDS_Shape aShape = (TopoDS_Shape)MemberwiseClone();
            aShape.Orientation(theOrient);
            return aShape;
        }

        //! Returns the number of direct sub-shapes (children).
        //! @sa TopoDS_Iterator for accessing sub-shapes
        public int NbChildren()
        {
            return myTShape == null ? 0 : myTShape.NbChildren();
        }


        //! Returns True if two shapes are same, i.e.  if they
        //! share  the  same TShape  with the same  Locations.
        //! Orientations may differ.
        public bool IsSame(TopoDS_Shape theOther)
        {
            return myTShape == theOther.myTShape
                && myLocation == theOther.myLocation;
        }
    }
}