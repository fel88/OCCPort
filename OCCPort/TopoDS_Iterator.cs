using System;

namespace OCCPort
{
    public class TopoDS_Iterator
    {

        public void Next()
        {
            myShapes.Next();
            if (More())
            {
                myShape = myShapes.Value();
                myShape.Orientation(TopAbs.Compose(myOrientation, myShape.Orientation()));
                if (!myLocation.IsIdentity())
                    myShape.Move(myLocation, false);
            }
        }

        //! Returns true if there is another sub-shape in the
        //! shape which this iterator is scanning.
        public bool More() { return myShapes.More(); }


        TopoDS_Shape myShape;
        protected TopoDS_ListIteratorOfListOfShape myShapes;
        TopAbs_Orientation myOrientation;
        TopLoc_Location myLocation;

    }
}