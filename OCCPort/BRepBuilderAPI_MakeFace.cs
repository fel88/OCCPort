using TKBRep;
using TKTopAlgo;

namespace OCCPort.Tester
{
    //! Provides methods to build faces.
    //!
    //! A face may be built :
    //!
    //! * From a surface.
    //!
    //! - Elementary surface from gp.
    //!
    //! - Surface from Geom.
    //!
    //! * From a surface and U,V values.
    //!
    //! * From a wire.
    //!
    //! - Find the surface automatically if possible.
    //!
    //! * From a surface and a wire.
    //!
    //! - A flag Inside is given, when this flag is True
    //! the  wire is  oriented to bound a finite area on
    //! the surface.
    //!
    //! * From a face and a wire.
    //!
    //! - The new wire is a perforation.
    public class BRepBuilderAPI_MakeFace : BRepBuilderAPI_MakeShape
    {
        //! Find a surface from the wire and make a face.
     //! if <OnlyPlane> is true, the computed surface will be
     //! a plane. If it is not possible to find a plane, the
     //! flag NotDone will be set.
        public BRepBuilderAPI_MakeFace(TopoDS_Wire W, bool OnlyPlane = false)
        {
            myMakeFace = new BRepLib_MakeFace(W, OnlyPlane);
            if (myMakeFace.IsDone())
            {
                Done();
                myShape = myMakeFace.Shape();
            }
        }
        
        public static implicit operator TopoDS_Face(BRepBuilderAPI_MakeFace f)
        {
           return f.Face();
        }

        public TopoDS_Face Face()
        {
            return myMakeFace.Face();
        }
        BRepLib_MakeFace myMakeFace;


    }
}
