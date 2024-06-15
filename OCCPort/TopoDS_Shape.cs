using System;
using System.Runtime.Remoting.Messaging;

namespace OCCPort
{
    public class TopoDS_Shape
    {
        //! Returns the free flag.
        public bool Free() { return myTShape.Free(); }

        //! Sets the free flag.
        public void Free(bool theIsFree) { myTShape.Free(theIsFree); }

        internal uint ShapeType()
        {
            throw new NotImplementedException();
        }
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

        internal void Reverse()
        {
            throw new NotImplementedException();
        }

        internal void Move(object v1, bool v2)
        {
            throw new NotImplementedException();
        }
    }
}