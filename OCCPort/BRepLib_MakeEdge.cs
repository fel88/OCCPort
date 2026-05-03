using System;

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
    public class BRepLib_MakeEdge : BRepLib_MakeShape
    {
        public BRepLib_MakeEdge(gp_Pnt P1, gp_Pnt P2)
        {
            double l = P1.Distance(P2);
            if (l <= gp.Resolution())
            {
                myError = BRepLib_EdgeError.BRepLib_LineThroughIdenticPoints;
                return;
            }
            gp_Lin L = new gp_Lin(P1, new gp_Vec(P1, P2).To_gp_Dir());
            Geom_Line GL = new Geom_Line(L);
            Init(GL, P1, P2, 0, l);
        }

        public TopoDS_Edge Edge()
        {
            return TopoDS.Edge(Shape());
        }


        public void Init(Geom_Curve C,

                 gp_Pnt P1,
                 gp_Pnt P2,
                 double p1,
                 double p2)
        {
            double Tol = BRepLib.Precision();
            BRep_Builder B = new BRep_Builder();

            TopoDS_Vertex V1 = new TopoDS_Vertex(), V2 = new TopoDS_Vertex();
            B.MakeVertex(V1, P1, Tol);
            if (P1.Distance(P2) < Tol)
                V2 = V1;
            else
                B.MakeVertex(V2, P2, Tol);

            Init(C, V1, V2, p1, p2);
        }
        //=======================================================================
        //function : Init
        //purpose  : this one really makes the job ...
        //=======================================================================

        void Init(Geom_Curve CC,
                  TopoDS_Vertex VV1,
                  TopoDS_Vertex VV2,
                  double pp1,

                  double pp2)
        {
            // kill trimmed curves
            Geom_Curve C = CC;
            Geom_TrimmedCurve CT = C as Geom_TrimmedCurve;
            while (CT != null)
            {
                C = CT.BasisCurve();
                CT = (Geom_TrimmedCurve)C;
            }

            // check parameters
            double p1 = pp1;
            double p2 = pp2;
            double cf = C.FirstParameter();
            double cl = C.LastParameter();
            double epsilon = Precision.PConfusion();
            bool periodic = C.IsPeriodic();
            GeomAdaptor_Curve aCA = new GeomAdaptor_Curve(C);

            TopoDS_Vertex V1, V2;
            if (periodic)
            {
                // adjust in period
                ElCLib.AdjustPeriodic(cf, cl, epsilon, ref p1, ref p2);
                V1 = VV1;
                V2 = VV2;
            }
            else
            {
                // reordonate
                if (p1 < p2)
                {
                    V1 = VV1;
                    V2 = VV2;
                }
                else
                {
                    V2 = VV1;
                    V1 = VV2;
                    double x = p1;
                    p1 = p2;
                    p2 = x;
                }

                // check range
                if ((cf - p1 > epsilon) || (p2 - cl > epsilon))
                {
                    myError = BRepLib_EdgeError.BRepLib_ParameterOutOfRange;
                    return;
                }

                // check ponctuallity
                if ((p2 - p1) <= gp.Resolution())
                {
                    myError = BRepLib_EdgeError.BRepLib_LineThroughIdenticPoints;
                    return;
                }
            }

            // compute points on the curve
            bool p1inf = Precision.IsNegativeInfinite(p1);
            bool p2inf = Precision.IsPositiveInfinite(p2);
            gp_Pnt P1 = new gp_Pnt(), P2 = new gp_Pnt();
            if (!p1inf) P1 = aCA.Value(p1);
            if (!p2inf) P2 = aCA.Value(p2);

            double preci = BRepLib.Precision();
            BRep_Builder B = new BRep_Builder();

            // check for closed curve
            bool closed = false;
            bool degenerated = false;
            if (!p1inf && !p2inf)
                closed = (P1.Distance(P2) <= preci);

            // check if the vertices are on the curve
            if (closed)
            {
                if (V1.IsNull() && V2.IsNull())
                {
                    B.MakeVertex(V1, P1, preci);
                    V2 = V1;
                }
                else if (V1.IsNull())
                    V1 = V2;
                else if (V2.IsNull())
                    V2 = V1;
                else
                {
                    if (!V1.IsSame(V2))
                    {
                        myError = BRepLib_EdgeError.BRepLib_DifferentPointsOnClosedCurve;
                        return;
                    }
                    else if (P1.Distance(BRep_Tool.Pnt(V1)) >
                     Math.Max(preci, BRep_Tool.Tolerance(V1)))
                    {
                        myError = BRepLib_EdgeError.BRepLib_DifferentPointsOnClosedCurve;
                        return;
                    }
                    else
                    {
                        gp_Pnt PM = aCA.Value((p1 + p2) / 2);
                        if (P1.Distance(PM) < preci)
                            degenerated = true;
                    }
                }
            }

            else
            {    // not closed

                if (p1inf)
                {
                    if (!V1.IsNull())
                    {
                        myError = BRepLib_EdgeError.BRepLib_PointWithInfiniteParameter;
                        return;
                    }
                }
                else
                {
                    if (V1.IsNull())
                    {
                        B.MakeVertex(V1, P1, preci);
                    }
                    else if (P1.Distance(BRep_Tool.Pnt(V1)) >
                         Math.Max(preci, BRep_Tool.Tolerance(V1)))
                    {
                        myError = BRepLib_EdgeError.BRepLib_DifferentsPointAndParameter;
                        return;
                    }
                }

                if (p2inf)
                {
                    if (!V2.IsNull())
                    {
                        myError = BRepLib_EdgeError.BRepLib_PointWithInfiniteParameter;
                        return;
                    }
                }
                else
                {
                    if (V2.IsNull())
                    {
                        B.MakeVertex(V2, P2, preci);
                    }
                    else if (P2.Distance(BRep_Tool.Pnt(V2)) >
                       Math.Max(preci, BRep_Tool.Tolerance(V2)))
                    {
                        myError = BRepLib_EdgeError.BRepLib_DifferentsPointAndParameter;
                        return;
                    }
                }
            }

            V1.Orientation(TopAbs_Orientation.TopAbs_FORWARD);
            V2.Orientation(TopAbs_Orientation.TopAbs_REVERSED);
            myVertex1 = V1;
            myVertex2 = V2;

            TopoDS_Edge E = TopoDS.Edge(myShape);
            B.MakeEdge(E, C, preci);
            if (!V1.IsNull())
            {
                B.Add(E, V1);
            }
            if (!V2.IsNull())
            {
                B.Add(E, V2);
            }
            B.Range(E, p1, p2);
            B.Degenerated(E, degenerated);

            myError = BRepLib_EdgeError.BRepLib_EdgeDone;
            Done();
        }

        BRepLib_EdgeError myError;
        TopoDS_Vertex myVertex1;
        TopoDS_Vertex myVertex2;

    }
}