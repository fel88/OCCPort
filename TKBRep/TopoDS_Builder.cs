using OCCPort;
using OCCPort.Common;
using System.Reflection.Metadata;
using TKG2d;
using TKG3d;
using TKMath;

namespace OCCPort
{
    public class TopoDS_Builder
    {
        public void MakeWire(TopoDS_Wire W)
        {
            TopoDS_TWire TW = new TopoDS_TWire();
            MakeShape(W, TW);
        }


        //! Transfert the parameters  of   Vin on  Ein as  the
        //! parameter of Vout on Eout.
        public void Transfert(TopoDS_Edge Ein, TopoDS_Edge Eout, TopoDS_Vertex Vin, TopoDS_Vertex Vout)
        {
            double tol = BRep_Tool.Tolerance(Vin);
            double parin = BRep_Tool.Parameter(Vin, Ein);
            UpdateVertex(Vout, parin, Eout, tol);
        }

        //=======================================================================
        //function : UpdateVertex
        //purpose  : update vertex with parameter on edge
        //=======================================================================

        public void UpdateVertex(TopoDS_Vertex V,
                                   double Par,
                                   TopoDS_Edge E,
                                   double Tol)
        {
            if (Precision.IsPositiveInfinite(Par) ||
                Precision.IsNegativeInfinite(Par))
                throw new Standard_DomainError("BRep_Builder::Infinite parameter");

            BRep_TVertex TV = (BRep_TVertex)V.TShape();
            BRep_TEdge TE = (BRep_TEdge)E.TShape();

            if (TV.Locked() || TE.Locked())
            {
                throw new TopoDS_LockedShape("BRep_Builder::UpdateVertex");
            }

            TopLoc_Location L = E.Location().Predivided(V.Location());

            // Search the vertex in the edge
            TopAbs_Orientation ori = TopAbs_Orientation.TopAbs_INTERNAL;

            TopoDS_Iterator itv = new TopoDS_Iterator(E.Oriented(TopAbs_Orientation.TopAbs_FORWARD));

            // if the edge has no vertices
            // and is degenerated use the vertex orientation
            // RLE, june 94

            if (!itv.More() && TE.Degenerated())
                ori = V.Orientation();

            while (itv.More())
            {
                TopoDS_Shape Vcur = itv.Value();
                if (V.IsSame(Vcur))
                {
                    ori = Vcur.Orientation();
                    if (ori == V.Orientation()) break;
                }
                itv.Next();
            }

            //BRep_ListOfCurveRepresentation lcr = TE->ChangeCurves();
            //BRep_ListIteratorOfListOfCurveRepresentation itcr(lcr);
            BRep_GCurve GC;
            foreach (var item in TE.ChangeCurves())
            {
                GC = (BRep_GCurve)item;
                if (GC != null)
                {
                    if (ori == TopAbs_Orientation.TopAbs_FORWARD)
                        GC.First(Par);
                    else if (ori == TopAbs_Orientation.TopAbs_REVERSED)
                        GC.Last(Par);
                    else
                    {
                        BRep_ListOfPointRepresentation lpr = TV.ChangePoints();
                        TopLoc_Location GCloc = GC.Location();
                        TopLoc_Location LGCloc = L * GCloc;
                        if (GC.IsCurve3D())
                        {
                            Geom_Curve GC3d = GC.Curve3D();
                            UpdatePoints(lpr, Par, GC3d, LGCloc);
                        }
                        else if (GC.IsCurveOnSurface())
                        {
                            Geom2d_Curve GCpc = GC.PCurve();
                            Geom_Surface GCsu = GC.Surface();
                            UpdatePoints(lpr, Par, GCpc, GCsu, LGCloc);
                        }
                    }
                }
                //itcr.Next();
            }

            if ((ori != TopAbs_Orientation.TopAbs_FORWARD) && (ori != TopAbs_Orientation.TopAbs_REVERSED))
                TV.Modified(true);

            TV.UpdateTolerance(Tol);
            TE.Modified(true);
        }


        public static void UpdatePoints(BRep_ListOfPointRepresentation lpr,
                                  double p,
                           Geom2d_Curve PC,
                           Geom_Surface S,
                           TopLoc_Location L)
        {
            BRep_ListIteratorOfListOfPointRepresentation itpr = new BRep_ListIteratorOfListOfPointRepresentation(lpr);
            while (itpr.More())
            {
                BRep_PointRepresentation pr = itpr.Value();
                bool isponcons = pr.IsPointOnCurveOnSurface(PC, S, L);
                if (isponcons) break;
                itpr.Next();
            }

            if (itpr.More())
            {
                BRep_PointRepresentation pr = itpr.Value();
                pr.Parameter(p);
            }
            else
            {
                BRep_PointOnCurveOnSurface POCS =
                  new BRep_PointOnCurveOnSurface(p, PC, S, L);
                lpr.Append(POCS);
            }
        }


        public static void UpdatePoints(BRep_ListOfPointRepresentation lpr,
                                 double p,
                          Geom_Curve C,
                          TopLoc_Location L)
        {
            BRep_ListIteratorOfListOfPointRepresentation itpr = new BRep_ListIteratorOfListOfPointRepresentation(lpr);
            while (itpr.More())
            {
                BRep_PointRepresentation pr = itpr.Value();
                bool isponc = pr.IsPointOnCurve(C, L);
                if (isponc)
                    break;
                itpr.Next();
            }

            if (itpr.More())
            {
                BRep_PointRepresentation pr = itpr.Value();
                pr.Parameter(p);
            }
            else
            {
                BRep_PointOnCurve POC = new BRep_PointOnCurve(p, C, L);
                lpr.Append(POC);
            }
        }

        //! Add the Shape C in the Shape S.
        //! Exceptions
        //! - TopoDS_FrozenShape if S is not free and cannot be modified.
        //! - TopoDS__UnCompatibleShapes if S and C are not compatible.
        public void Add(TopoDS_Shape aShape, TopoDS_Shape aComponent)
        {

            //=======================================================================
            //function : Add
            //purpose  : insert aComponent in aShape
            //=======================================================================



            // From now the Component cannot be edited
            aComponent.TShape().Free(false);

            // Note that freezing aComponent before testing if aShape is free
            // prevents from self-insertion
            // but aShape will be frozen when the Exception is raised
            //if (aShape.Free())
            if (true)
            {
                uint[] aTb =                 {
      //COMPOUND to:
      (1<<((int)TopAbs_ShapeEnum.TopAbs_COMPOUND)),
      //COMPSOLID to:
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_COMPOUND)),
      //SOLID to:
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_COMPOUND)) |
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_COMPSOLID)),
      //SHELL to:
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_COMPOUND)) |
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_SOLID)),
      //FACE to:
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_COMPOUND)) |
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_SHELL)),
      //WIRE to:
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_COMPOUND)) |
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_FACE)),
      //EDGE to:
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_COMPOUND)) |
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_SOLID)) |
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_WIRE)),
      //VERTEX to:
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_COMPOUND)) |
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_SOLID)) |
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_FACE)) |
      (1 << ((int)TopAbs_ShapeEnum.TopAbs_EDGE)),
      //SHAPE to:
      0
                  };
                //
                uint iC = (uint)aComponent.ShapeType();
                int iS = (int)aShape.ShapeType();
                //
                if ((aTb[iC] & (1 << iS)) != 0)
                {
                    TopoDS_ListOfShape L = aShape.TShape().myShapes;
                    L.Append(aComponent);
                    TopoDS_Shape S = L.Last();
                    //
                    // compute the relative Orientation
                    if (aShape.Orientation() == TopAbs_Orientation.TopAbs_REVERSED)
                        S.Reverse();
                    //
                    // and the Relative Location
                    TopLoc_Location aLoc = aShape.Location();
                    if (!aLoc.IsIdentity())
                        S.Move(aLoc.Inverted(), false);
                    //
                    // Set the TShape as modified.
                    aShape.TShape().Modified(true);
                }
                else
                {
                    throw new TopoDS_UnCompatibleShapes("TopoDS_Builder::Add");
                }
            }
            else
            {
                throw new TopoDS_FrozenShape("TopoDS_Builder::Add");
            }
        }

        //! Make a Solid covering the whole 3D space.
        public void MakeSolid(TopoDS_Solid S)
        {
            TopoDS_TSolid TS = new TopoDS_TSolid();
            MakeShape(S, TS);
        }

        //=======================================================================
        public void MakeShape(TopoDS_Shape S,
                                   TopoDS_TShape T)
        {
            S.TShape(T);
            S.Location(new TopLoc_Location());
            S.Orientation(TopAbs_Orientation.TopAbs_FORWARD);
        }


        public void MakeShell(TopoDS_Shell S)
        {
            TopoDS_TShell TS = new TopoDS_TShell();
            MakeShape(S, TS);
        }

    }
}