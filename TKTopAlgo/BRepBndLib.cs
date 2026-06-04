using OCCPort;
using TKBRep;
using TKG3d;
using TKGeomBase;
using TKMath;

namespace TKTopAlgo
{
    //! This package provides the bounding boxes for curves
    //! and surfaces from BRepAdaptor.
    //! Functions to add a topological shape to a bounding box
    public class BRepBndLib
    {
        //

        //! Adds the shape S to the bounding box B.
        //! More precisely are successively added to B:
        //! -   each face of S; the triangulation of the face is used if it exists,
        //! -   then each edge of S which does not belong to a face,
        //! the polygon of the edge is used if it exists
        //! -   and last each vertex of S which does not belong to an edge.
        //! After each elementary operation, the bounding box B is
        //! enlarged by the tolerance value of the relative sub-shape.
        //! When working with the triangulation of a face this value of
        //! enlargement is the sum of the triangulation deflection and
        //! the face tolerance. When working with the
        //! polygon of an edge this value of enlargement is
        //! the sum of the polygon deflection and the edge tolerance.
        //! Warning
        //! -   This algorithm is time consuming if triangulation has not
        //! been inserted inside the data structure of the shape S.
        //! -   The resulting bounding box may be somewhat larger than the object.
        //=======================================================================
        //function : Add
        //purpose  : Add a shape bounding to a box
        //=======================================================================
        public static void Add(TopoDS_Shape S, Bnd_Box B, bool useTriangulation = true)
        {
            TopExp_Explorer ex = new TopExp_Explorer();

            // Add the faces
            BRepAdaptor_Surface BS = new BRepAdaptor_Surface();
            TopLoc_Location l = new TopLoc_Location(), aDummyLoc = new TopLoc_Location();
            int i, nbNodes;
            BRepAdaptor_Curve BC = new BRepAdaptor_Curve();

            for (ex.Init(S, TopAbs_ShapeEnum.TopAbs_FACE); ex.More(); ex.Next())
            {
                TopoDS_Face F = TopoDS.Face(ex.Current());
                Poly_Triangulation T2 = BRep_Tool.Triangulation(F, ref l);
                Geom_Surface GS = BRep_Tool.Surface(F, out aDummyLoc);
                if ((useTriangulation || GS == null) && T2 != null && T2.MinMax(B, l.Transformation()))
                {
                    //       B.Enlarge(T->Deflection());
                    B.Enlarge(T2.Deflection() + BRep_Tool.Tolerance(F));
                }
                else
                {
                    if (GS != null)
                    {
                        BS.Initialize(F, false);
                        if (BS._GetType() != GeomAbs_SurfaceType.GeomAbs_Plane)
                        {
                            BS.Initialize(F);
                            BndLib_AddSurface.Add(BS, BRep_Tool.Tolerance(F), B);
                        }
                        else
                        {
                            // on travaille directement sur les courbes 3d.
                            TopExp_Explorer ex2 = new TopExp_Explorer(F, TopAbs_ShapeEnum.TopAbs_EDGE);
                            if (!ex2.More())
                            {
                                BS.Initialize(F);
                                BndLib_AddSurface.Add(BS, BRep_Tool.Tolerance(F), B);
                            }
                            else
                            {
                                for (; ex2.More(); ex2.Next())
                                {
                                    TopoDS_Edge anEdge = TopoDS.Edge(ex2.Current());
                                    if (BRep_Tool.IsGeometric(anEdge))
                                    {
                                        BC.Initialize(anEdge);
                                        BndLib_Add3dCurve.Add(BC, BRep_Tool.Tolerance(anEdge), B);
                                    }
                                }
                                B.Enlarge(BRep_Tool.Tolerance(F));
                            }
                        }
                    }
                }
            }

            // Add the edges not in faces
            //TColStd_HArray1OfInteger HIndices;
            Poly_PolygonOnTriangulation Poly;
            Poly_Triangulation T;
            for (ex.Init(S, TopAbs_ShapeEnum.TopAbs_EDGE, TopAbs_ShapeEnum.TopAbs_FACE); ex.More(); ex.Next())
            {
                TopoDS_Edge E = TopoDS.Edge(ex.Current());
                //   Poly_Polygon3D P3d = BRep_Tool.Polygon3D(E, l);
                //if (P3d != null && P3d.NbNodes() > 0)
                {
                    // TColgp_Array1OfPnt Nodes = P3d.Nodes();
                    // nbNodes = P3d.NbNodes();
                    // for (i = 1; i <= nbNodes; i++)
                    {
                        // if (l.IsIdentity()) B.Add(Nodes[i]);

                        //else B.Add(Nodes[i].Transformed(l));
                    }
                    //       B.Enlarge(P3d->Deflection());
                    //  B.Enlarge(P3d, Deflection() + BRep_Tool.Tolerance(E));
                }
                // else
                {
                    //  BRep_Tool.PolygonOnTriangulation(E, Poly, T, l);
                    // if (useTriangulation && Poly != null && T != null && T.NbNodes() > 0)
                    {
                        /*   TColStd_Array1OfInteger Indices = Poly.Nodes();
                           nbNodes = Indices.Length();
                           if (l.IsIdentity())
                           {
                               for (i = 1; i <= nbNodes; i++)
                               {
                                   B.Add(T.Node(Indices[i]));
                               }
                           }
                           else
                           {
                               for (i = 1; i <= nbNodes; i++)
                               {
                                   B.Add(T.Node(Indices[i]).Transformed(l));
                               }
                           }*/
                        // 	B.Enlarge(T->Deflection());
                        //B.Enlarge(Poly.Deflection() + BRep_Tool.Tolerance(E));
                    }
                    // else
                    {
                        // if (BRep_Tool.IsGeometric(E))
                        {
                            //  BC.Initialize(E);
                            //  BndLib_Add3dCurve.Add(BC, BRep_Tool.Tolerance(E), B);
                        }
                    }
                }
            }

            // Add the vertices not in edges

            //for (ex.Init(S, TopAbs_ShapeEnum.TopAbs_VERTEX, TopAbs_ShapeEnum.TopAbs_EDGE); ex.More(); ex.Next())
            {
                //      B.Add(BRep_Tool.Pnt(TopoDS.Vertex(ex.Current())));
                //    B.Enlarge(BRep_Tool.Tolerance(TopoDS.Vertex(ex.Current())));
            }
        }
    }
}
