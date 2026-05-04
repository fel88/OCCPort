using OCCPort.Tester;

namespace OCCPort
{
    //! This    is  the  root     class for     all  shape
    //! constructions.  It stores the result.
    //!
    //! It  provides deferred methods to trace the history
    //! of sub-shapes.
    public class BRepLib_MakeShape : BRepLib_Command
    {
        public TopoDS_Shape Shape()
        {
            if (!IsDone())
            {
                // the following is const cast away
                Build();
                Check();
            }
            return myShape;
        }

        public virtual void Build()
        {
            
        }

        protected TopoDS_Shape myShape = new TopoDS_Shape();
        protected TopTools_ListOfShape myGenFaces = new TopTools_ListOfShape();
        protected TopTools_ListOfShape myNewFaces = new TopTools_ListOfShape();
        protected TopTools_ListOfShape myEdgFaces = new TopTools_ListOfShape();

    }
}