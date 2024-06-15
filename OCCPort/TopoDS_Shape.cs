using System;

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

        internal TopLoc_Location Location()
        {
            throw new NotImplementedException();
        }

        internal TopAbs_Orientation Orientation()
        {
            throw new NotImplementedException();
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