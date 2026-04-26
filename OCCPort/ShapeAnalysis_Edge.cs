using OpenTK.Compute.OpenCL;
using System.Reflection.Metadata;

namespace OCCPort
{

    //! Tool for analyzing the edge.
    //! Queries geometrical representations of the edge (3d curve, pcurve
    //! on the given face or surface) and topological sub-shapes (bounding
    //! vertices).
    //! Provides methods for analyzing geometry and topology consistency
    //! (3d and pcurve(s) consistency, their adjacency to the vertices).
    internal class ShapeAnalysis_Edge
    {
        //! Returns start vertex of the edge (taking edge orientation
        //! into account).
        public TopoDS_Vertex FirstVertex(TopoDS_Edge edge)
        {
            TopoDS_Vertex V;
            if (edge.Orientation() == TopAbs_Orientation.TopAbs_REVERSED)
            {
                V = TopExp.LastVertex(edge);
                V.Reverse();
            }
            else
            {
                V = TopExp.FirstVertex(edge);
            }
            return V;
        }
        public bool PCurve(TopoDS_Edge edge,
                         TopoDS_Face face,
                       ref  Geom2d_Curve C2d,
                         ref double cf, ref double cl,
                         bool orient = true)
        {
            //:abv 20.05.02: take into account face orientation
            // COMMENTED BACK - NEEDS MORE CHANGES IN ALL SHAPEHEALING
            //   C2d = BRep_Tool::CurveOnSurface (edge, face, cf, cl);
            //   if (orient && edge.Orientation() == TopAbs_REVERSED) {
            //     Standard_Real tmp = cf; cf = cl; cl = tmp;
            //   }
            //   return !C2d.IsNull();
            TopLoc_Location L;
            Geom_Surface S = BRep_Tool.Surface(face, out L);
            return PCurve(edge, S, L,ref  C2d, ref cf, ref cl, orient);
        }
        //! Returns the pcurve and bounding parameteres for the edge
        //! lying on the surface.
        //! Returns False if the edge has no pcurve on this surface.
        //! If <orient> is True (default), takes orientation into account:
        //! if the edge is reversed, cf and cl are toggled
        public bool PCurve(TopoDS_Edge edge,
                         Geom_Surface surface,
                         TopLoc_Location location,
                       ref  Geom2d_Curve C2d,
                         ref double cf, ref double cl,
                         bool orient = true)
        {
            C2d = BRep_Tool.CurveOnSurface(edge, surface, location, ref cf, ref cl);
            if (orient && edge.Orientation() == TopAbs_Orientation.TopAbs_REVERSED)
            {
                double tmp = cf; cf = cl; cl = tmp;
            }
            return C2d != null;
        }

        //! Returns end vertex of the edge (taking edge orientation
        //! into account).
        public TopoDS_Vertex LastVertex(TopoDS_Edge edge)
        {
            TopoDS_Vertex V;
            if (edge.Orientation() == TopAbs_Orientation.TopAbs_REVERSED)
            {
                V = TopExp.FirstVertex(edge);
                V.Reverse();
            }
            else
            {
                V = TopExp.LastVertex(edge);
            }
            return V;
        }
    }
}