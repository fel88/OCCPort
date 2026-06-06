using OCCPort;
using OpenTK.Compute.OpenCL;
using System.Reflection.Metadata;
using TKMath;
using TKTopAlgo;

namespace OCCPort
{
    //! Provides methods to build edges.
    //!
    //! The   methods have  the  following   syntax, where
    //! TheCurve is one of Lin, Circ, ...
    //!
    //! Create(C : TheCurve)
    //!
    //! Makes an edge on  the whole curve.  Add vertices
    //! on finite curves.
    //!
    //! Create(C : TheCurve; p1,p2 : Real)
    //!
    //! Make an edge  on the curve between parameters p1
    //! and p2. if p2 < p1 the edge will be REVERSED. If
    //! p1  or p2 is infinite the  curve will be open in
    //! that  direction. Vertices are created for finite
    //! values of p1 and p2.
    //!
    //! Create(C : TheCurve; P1, P2 : Pnt from gp)
    //!
    //! Make an edge on the curve  between the points P1
    //! and P2. The  points are projected on   the curve
    //! and the   previous method is  used. An  error is
    //! raised if the points are not on the curve.
    //!
    //! Create(C : TheCurve; V1, V2 : Vertex from TopoDS)
    //!
    //! Make an edge  on the curve  between the vertices
    //! V1 and V2. Same as the  previous but no vertices
    //! are created. If a vertex is  Null the curve will
    //! be open in this direction.
    public class BRepBuilderAPI_MakeEdge : BRepBuilderAPI_MakeShape
    {
        public BRepBuilderAPI_MakeEdge(gp_Pnt P1, gp_Pnt P2)
        {
            myMakeEdge = new BRepLib_MakeEdge(P1, P2);

            if (myMakeEdge.IsDone())
            {
                Done();
                myShape = myMakeEdge.Shape();
            }
        }
        public static implicit operator TopoDS_Edge(BRepBuilderAPI_MakeEdge f)
        {
            return f.Edge();
        }

        public TopoDS_Edge Edge()
        {
            return myMakeEdge.Edge();
        }

        BRepLib_MakeEdge myMakeEdge;


    }
}