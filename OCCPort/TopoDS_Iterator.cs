using System;

namespace OCCPort
{
    //! Iterates on the underlying shape underlying a given
    //! TopoDS_Shape object, providing access to its
    //! component sub-shapes. Each component shape is
    //! returned as a TopoDS_Shape with an orientation,
    //! and a compound of the original values and the relative values.
    public class TopoDS_Iterator
    {
        //! Creates an empty Iterator.
        public TopoDS_Iterator() { }

        //! Creates an Iterator on <S> sub-shapes.
        //! Note:
        //! - If cumOri is true, the function composes all
        //! sub-shapes with the orientation of S.
        //! - If cumLoc is true, the function multiplies all
        //! sub-shapes by the location of S, i.e. it applies to
        //! each sub-shape the transformation that is associated with S.
        public TopoDS_Iterator(TopoDS_Shape S,
                      bool cumOri = true,
                      bool cumLoc = true)
        {
            Initialize(S, cumOri, cumLoc);
        }

        public TopoDS_Iterator(TopoDS_Iterator topoDS_Iterator)
        {
        }

        //! Returns the current sub-shape in the shape which
        //! this iterator is scanning.
        //! Exceptions
        //! Standard_NoSuchObject if there is no current sub-shape.
        public TopoDS_Shape Value()
        {
            //Standard_NoSuchObject_Raise_if(!More(),"TopoDS_Iterator::Value");  
            return myShape;
        }
        //=======================================================================
        //function : Initialize
        //purpose  : 
        //=======================================================================
        void Initialize(TopoDS_Shape S,
                                      bool cumOri,
                                      bool cumLoc)
        {
            if (cumLoc)
                myLocation = S.Location();
            else
                myLocation.Identity();
            if (cumOri)
                myOrientation = S.Orientation();
            else
                myOrientation = TopAbs_Orientation.TopAbs_FORWARD;

            if (S.IsNull())
                myShapes = new TopoDS_ListIteratorOfListOfShape();
            else
                myShapes.Initialize(S.TShape().myShapes);

            if (More())
            {
                myShape = myShapes.Value();
                myShape.Orientation(TopAbs.Compose(myOrientation, myShape.Orientation()));
                if (!myLocation.IsIdentity())
                    myShape.Move(myLocation, false);
            }
        }

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
        protected TopoDS_ListIteratorOfListOfShape myShapes = new TopoDS_ListIteratorOfListOfShape();
        TopAbs_Orientation myOrientation;
        TopLoc_Location myLocation;

    }
}