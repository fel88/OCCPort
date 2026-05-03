using OCCPort;
using System;
using System.Formats.Asn1;
using System.Reflection.Metadata;
using TriangleNet.Topology.DCEL;

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
    public class BRepLib_MakeFace : BRepLib_MakeShape
    {

        //! Find a surface from the wire and make a face.
        //! if <OnlyPlane> is true, the computed surface will be
        //! a plane. If it is not possible to find a plane, the
        //! flag NotDone will be set.
        public BRepLib_MakeFace(TopoDS_Wire W, bool OnlyPlane = false)
        {
            // Find a surface through the wire
            BRepLib_FindSurface FS = new BRepLib_FindSurface(W, -1, OnlyPlane, true);
            if (!FS.Found())
            {
                myError = BRepLib_FaceError.BRepLib_NotPlanar;
                return;
            }

            // build the face and add the wire
            BRep_Builder B;
            myError = BRepLib_FaceError.BRepLib_FaceDone;

            double tol = Math.Max(1.2 * FS.ToleranceReached(), FS.Tolerance());

       //     B.MakeFace(TopoDS.Face(myShape), FS.Surface(), FS.Location(), tol);
            Add(W);
            //
            BRepLib.UpdateTolerances(myShape);
            //
         //  BRepLib.SameParameter(myShape, tol, true);
            //
            if (BRep_Tool.IsClosed(W))
                CheckInside();
        }
        void CheckInside()
        {
            // compute the area and return the face if the area is negative
            TopoDS_Face F = TopoDS.Face(myShape);
            //BRepTopAdaptor_FClass2d FClass = new BRepTopAdaptor_FClass2d(F, 0.0);
          //  if (FClass.PerformInfinitePoint() == TopAbs_IN)
            {
                BRep_Builder B = new BRep_Builder();
               // TopoDS_Shape S = myShape.EmptyCopied();
                TopoDS_Iterator it = new TopoDS_Iterator(myShape);
                while (it.More())
                {
               //     B.Add(S, it.Value().Reversed());
                    it.Next();
                }
               // myShape = S;
            }
        }

        public void Add(TopoDS_Wire W)
        {
            BRep_Builder B = new BRep_Builder();
            B.Add(myShape, W);
            B.NaturalRestriction(TopoDS.Face(myShape), false);
            Done();
        }

        internal TopoDS_Face Face()
        {
            return TopoDS.Face(myShape);

        }

        BRepLib_FaceError myError;

    }
}
