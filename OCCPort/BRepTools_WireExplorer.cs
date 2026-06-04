using OCCPort;
using OpenTK.Audio.OpenAL;
using OpenTK.Core.Exceptions;
using System;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using TKBRep;
using TKMath;

namespace OCCPort
{
    internal class BRepTools_WireExplorer
    {

        //! Constructs an empty explorer (which can be initialized using Init)
        public BRepTools_WireExplorer()
        {
            myReverse = (false);
            myTolU = 0.0;
            myTolV = 0.0;
        }

        //! IInitializes an exploration  of the wire <W>.
        public BRepTools_WireExplorer(TopoDS_Wire W)
        {
            TopoDS_Face F = new TopoDS_Face();
            Init(W, F);
        }
        public void Init(TopoDS_Wire W,
                                    TopoDS_Face F)
        {
            myEdge = new TopoDS_Edge();
            myVertex = new TopoDS_Vertex();
            myMap.Clear();
            myDoubles.Clear();

            if (W.IsNull())
                return;

            double UMin = (0.0), UMax = (0.0), VMin = (0.0), VMax = (0.0);
            if (!F.IsNull())
            {
                // For the faces based on Cone, BSpline and Bezier compute the
                // UV bounds to precise the UV tolerance values
                GeomAbs_SurfaceType aSurfType = new BRepAdaptor_Surface(F, false)._GetType();
                if (aSurfType == GeomAbs_SurfaceType.GeomAbs_Cone ||
                    aSurfType == GeomAbs_SurfaceType.GeomAbs_BSplineSurface ||
                    aSurfType == GeomAbs_SurfaceType.GeomAbs_BezierSurface)
                {
                    BRepTools.UVBounds(F, ref UMin, ref UMax, ref VMin, ref VMax);
                }
            }

            Init(W, F, UMin, UMax, VMin, VMax);
        }
        public bool SelectDouble(TopTools_MapOfShape Doubles,
                  TopTools_ListOfShape L,
                  TopoDS_Edge E)
        {
            //TopTools_ListIteratorOfListOfShape it(L);
            foreach (var item in L)
            {
                TopoDS_Shape CE = item;
                if (Doubles.Contains(CE) && (!E.IsSame(CE)))
                {
                    E = TopoDS.Edge(CE);
                    L.Remove(item);
                    return true;
                }
            }
            return false;
        }
        public bool SelectDegenerated(TopTools_ListOfShape L,
                   TopoDS_Edge E)
        {
            //TopTools_ListIteratorOfListOfShape it = new TopTools_ListIteratorOfListOfShape(L);
            foreach (var item in L)
            {
                if (!item.IsSame(E))
                {
                    E = TopoDS.Edge(item);
                    if (BRep_Tool.Degenerated(E))
                    {
                        L.Remove(item);
                        return true;
                    }
                }

            }
            return false;
        }

        public void Init(TopoDS_Wire W,
                                  TopoDS_Face F,
                                 double UMin,
                                 double UMax,
                                 double VMin,
                                 double VMax)
        {
            myEdge = new TopoDS_Edge();
            myVertex = new TopoDS_Vertex();
            myMap.Clear();
            myDoubles.Clear();

            if (W.IsNull())
                return;

            myFace = F;
            double dfVertToler = 0.0;
            myReverse = false;

            if (!myFace.IsNull())
            {
                TopLoc_Location aL = new TopLoc_Location();
                Geom_Surface aSurf = BRep_Tool.Surface(myFace, out aL);
                GeomAdaptor_Surface aGAS = new GeomAdaptor_Surface(aSurf);
                TopExp_Explorer anExp = new TopExp_Explorer(W, TopAbs_ShapeEnum.TopAbs_VERTEX);
                for (; anExp.More(); anExp.Next())
                {
                    TopoDS_Vertex aV = TopoDS.Vertex(anExp.Current());
                    dfVertToler = Math.Max(BRep_Tool.Tolerance(aV), dfVertToler);
                }
                if (dfVertToler < Precision.Confusion())
                {
                    // Use tolerance of edges
                    TopoDS_Iterator _it = new TopoDS_Iterator(W);
                    for (; _it.More(); _it.Next())
                        dfVertToler = Math.Max(BRep_Tool.Tolerance(TopoDS.Edge(_it.Value())), dfVertToler);

                    if (dfVertToler < Precision.Confusion())
                        // empty wire
                        return;
                }
                myTolU = 2.0 * aGAS.UResolution(dfVertToler);
                myTolV = 2.0 * aGAS.VResolution(dfVertToler);

                // uresolution for cone with infinite vmin vmax is too small.
                //if (aGAS.GetType() == GeomAbs_Cone)
                //{
                //    gp_Pnt aP;
                //    gp_Vec aD1U, aD1V = new gp_Vec();
                //    aGAS.D1(UMin, VMin, aP, aD1U, aD1V);
                //    double tol1, tol2, maxtol = .0005 * (UMax - UMin);
                //    double a = aD1U.Magnitude();

                //    if (a <= Precision.Confusion())
                //        tol1 = maxtol;
                //    else
                //        tol1 = Min(maxtol, dfVertToler / a);

                //    aGAS.D1(UMin, VMax, ref aP, ref aD1U, ref aD1V);
                //    a = aD1U.Magnitude();
                //    if (a <= Precision.Confusion())
                //        tol2 = maxtol;
                //    else
                //        tol2 = Math.Min(maxtol, dfVertToler / a);

                //    myTolU = 2.0 * Math.Max(tol1, tol2);
                //}

                if (aGAS._GetType() == GeomAbs_SurfaceType.GeomAbs_BSplineSurface ||
                  aGAS._GetType() == GeomAbs_SurfaceType.GeomAbs_BezierSurface)
                {
                    double maxTol = Math.Max(myTolU, myTolV);
                    gp_Pnt aP = new gp_Pnt();
                    gp_Vec aDU = new gp_Vec(), aDV = new gp_Vec();
                    aGAS.D1((UMax - UMin) / 2.0, (VMax - VMin) / 2.0, out aP, out aDU, out aDV);
                    double mod = Math.Sqrt(aDU * aDU + aDV * aDV);
                    if (mod > gp.Resolution())
                    {
                        if (mod * maxTol / dfVertToler < 1.5)
                        {
                            maxTol = 1.5 * dfVertToler / mod;
                        }
                        myTolU = maxTol;
                        myTolV = maxTol;
                    }
                }

                myReverse = (myFace.Orientation() == TopAbs_Orientation.TopAbs_REVERSED);
            }

            // map of vertices to know if the wire is open
            TopTools_IndexedMapOfShape vmap = new TopTools_IndexedMapOfShape();
            //  map of infinite edges
            TopTools_MapOfShape anInfEmap = new TopTools_MapOfShape();

            // list the vertices
            TopoDS_Vertex V1 = new TopoDS_Vertex(), V2 = new TopoDS_Vertex();
            TopTools_ListOfShape empty = new TopTools_ListOfShape();

            TopoDS_Iterator it = new TopoDS_Iterator(W);
            while (it.More())
            {
                TopoDS_Edge E = TopoDS.Edge(it.Value());
                TopAbs_Orientation Eori = E.Orientation();
                if (Eori == TopAbs_Orientation.TopAbs_INTERNAL || Eori == TopAbs_Orientation.TopAbs_EXTERNAL)
                {
                    it.Next();
                    continue;
                }
                TopExp.Vertices(E, ref V1, ref V2, true);

                if (!V1.IsNull())
                {
                    if (!myMap.IsBound(V1))
                        //myMap.Bind(V1, empty);
                        myMap.Bind(V1, new TopTools_ListOfShape());// ????
                    myMap[V1].Append(E);

                    // add or remove in the vertex map
                    V1.Orientation(TopAbs_Orientation.TopAbs_FORWARD);
                    int currsize = vmap.Extent(),
                                   ind = vmap.Add(V1);
                    if (currsize >= ind)
                    {
                        vmap.RemoveKey(V1);
                    }
                }

                if (!V2.IsNull())
                {
                    V2.Orientation(TopAbs_Orientation.TopAbs_REVERSED);
                    int currsize = vmap.Extent(),
                                    ind = vmap.Add(V2);
                    if (currsize >= ind)
                    {
                        vmap.RemoveKey(V2);
                    }
                }

                if (V1.IsNull() || V2.IsNull())
                {
                    double aF = 0.0, aL = 0.0;
                    BRep_Tool.Range(E, out aF, out aL);

                    if (Eori == TopAbs_Orientation.TopAbs_FORWARD)
                    {
                        if (aF == -Precision.Infinite())
                            anInfEmap.Add(E);
                    }
                    else
                    { // Eori == TopAbs_REVERSED
                        if (aL == Precision.Infinite())
                            anInfEmap.Add(E);
                    }
                }
                it.Next();
            }

            //Construction of the set of double edges.
            TopoDS_Iterator it2 = new TopoDS_Iterator(W);
            TopTools_MapOfShape emap = new TopTools_MapOfShape();
            while (it2.More())
            {
                if (!emap.Add(it2.Value()))
                    myDoubles.Add(it2.Value());
                it2.Next();
            }

            // if vmap is not empty the wire is open, let us find the first vertex
            if (!vmap.IsEmpty())
            {
                //TopTools_MapIteratorOfMapOfShape itt(vmap);
                //while (itt.Key().Orientation() != TopAbs_FORWARD) {
                //  itt.Next();
                //  if (!itt.More()) break;
                //}
                //if (itt.More()) V1 = TopoDS.Vertex(itt.Key());
                int ind = 0;
                for (ind = 1; ind <= vmap.Extent(); ++ind)
                {
                    if (vmap[ind].Orientation() == TopAbs_Orientation.TopAbs_FORWARD)
                    {
                        V1 = TopoDS.Vertex(vmap[ind]);
                        break;
                    }
                }
            }
            else
            {
                //   The wire is infinite Try to find the first vertex. It may be NULL.
                if (!anInfEmap.IsEmpty())
                {
                    TopTools_MapIteratorOfMapOfShape itt = new TopTools_MapIteratorOfMapOfShape(anInfEmap);

                    for (; itt.More(); itt.Next())
                    {
                        TopoDS_Edge anEdge = TopoDS.Edge(itt.Key());
                        TopAbs_Orientation anOri = anEdge.Orientation();
                        double aF;
                        double aL;

                        BRep_Tool.Range(anEdge, out aF, out aL);
                        if ((anOri == TopAbs_Orientation.TopAbs_FORWARD && aF == -Precision.Infinite()) ||
                          (anOri == TopAbs_Orientation.TopAbs_REVERSED && aL == Precision.Infinite()))
                        {
                            myEdge = anEdge;
                            myVertex = new TopoDS_Vertex();

                            return;
                        }
                    }
                }

                // use the first vertex in iterator
                it.Initialize(W);
                while (it.More())
                {
                    TopoDS_Edge E = TopoDS.Edge(it.Value());
                    TopAbs_Orientation Eori = E.Orientation();
                    if (Eori == TopAbs_Orientation.TopAbs_INTERNAL || Eori == TopAbs_Orientation.TopAbs_EXTERNAL)
                    {
                        // JYL 10-03-97 : waiting for correct processing 
                        // of INTERNAL/EXTERNAL edges
                        it.Next();
                        continue;
                    }
                    TopExp.Vertices(E, ref V1, ref V2, true);
                    break;
                }
            }

            if (V1.IsNull()) return;
            if (!myMap.IsBound(V1)) return;

            TopTools_ListOfShape l = myMap[V1];
            myEdge = TopoDS.Edge(l.First());
            l.RemoveFirst();
            myVertex = TopExp.FirstVertex(myEdge, true);

        }


        public double GetNextParamOnPC(Geom2d_Curve aPC,

                   gp_Pnt2d aPRef,
                   double fP,
                   double lP,
                   double tolU,
                   double tolV,
                   bool reverse)
        {
            double result = (reverse) ? fP : lP;
            double dP = Math.Abs(lP - fP) / 1000.0; // was / 16.;
            if (reverse)
            {
                double startPar = fP;
                bool nextPntOnEdge = false;
                while (!nextPntOnEdge && startPar < lP)
                {
                    gp_Pnt2d pnt = new gp_Pnt2d();
                    startPar += dP;
                    aPC.D0(startPar, ref pnt);
                    if (Math.Abs(aPRef.X() - pnt.X()) < tolU && Math.Abs(aPRef.Y() - pnt.Y()) < tolV)
                        continue;
                    else
                    {
                        result = startPar;
                        nextPntOnEdge = true;
                        break;
                    }
                }

                if (!nextPntOnEdge)
                    result = lP;

                if (result > lP)
                    result = lP;
            }
            else
            {
                double startPar = lP;
                bool nextPntOnEdge = false;
                while (!nextPntOnEdge && startPar > fP)
                {
                    gp_Pnt2d pnt = new gp_Pnt2d();
                    startPar -= dP;
                    aPC.D0(startPar, ref pnt);
                    if (Math.Abs(aPRef.X() - pnt.X()) < tolU && Math.Abs(aPRef.Y() - pnt.Y()) < tolV)
                        continue;
                    else
                    {
                        result = startPar;
                        nextPntOnEdge = true;
                        break;
                    }
                }

                if (!nextPntOnEdge)
                    result = fP;

                if (result < fP)
                    result = fP;
            }

            return result;
        }

        TopTools_DataMapOfShapeListOfShape myMap = new TopTools_DataMapOfShapeListOfShape();
        TopoDS_Edge myEdge;
        TopoDS_Vertex myVertex;
        TopoDS_Face myFace;
        TopTools_MapOfShape myDoubles = new TopTools_MapOfShape();
        bool myReverse;
        double myTolU;
        double myTolV;

        public TopoDS_Shape Current()
        {
            return myEdge;
        }

        internal bool More()
        {
            return !myEdge.IsNull();
        }

        internal void Next()
        {
            myVertex = TopExp.LastVertex(myEdge, true);

            if (myVertex.IsNull())
            {
                myEdge = new TopoDS_Edge();
                return;
            }
            if (!myMap.IsBound(myVertex))
            {
                myEdge = new TopoDS_Edge();
                return;
            }

            TopTools_ListOfShape l = myMap[myVertex];

            if (l.IsEmpty())
            {
                myEdge = new TopoDS_Edge();
            }
            else if (l.Extent() == 1)
            {
                //  Modified by Sergey KHROMOV - Fri Jun 21 10:28:01 2002 OCC325 Begin
                TopoDS_Vertex aV1 = new TopoDS_Vertex();
                TopoDS_Vertex aV2 = new TopoDS_Vertex();
                TopoDS_Edge aNextEdge = TopoDS.Edge(l.First());

                TopExp.Vertices(aNextEdge, ref aV1, ref aV2, true);

                if (!aV1.IsSame(myVertex))
                {
                    myEdge = new TopoDS_Edge();
                    return;
                }
                if (!myFace.IsNull() && aV1.IsSame(aV2))
                {
                    Geom2d_Curve aPrevPC;
                    Geom2d_Curve aNextPC;
                    double aPar11 = 0, aPar12 = 0;
                    double aPar21 = 0, aPar22 = 0;
                    double aPrevPar;
                    double aNextFPar;
                    double aNextLPar;

                    aPrevPC = BRep_Tool.CurveOnSurface(myEdge, myFace, ref aPar11, ref aPar12);
                    aNextPC = BRep_Tool.CurveOnSurface(aNextEdge, myFace, ref aPar21, ref aPar22);

                    if (aPrevPC == null || aNextPC == null)
                    {
                        myEdge = new TopoDS_Edge();
                        return;
                    }

                    if (myEdge.Orientation() == TopAbs_Orientation.TopAbs_FORWARD)
                        aPrevPar = aPar12;
                    else
                        aPrevPar = aPar11;

                    if (aNextEdge.Orientation() == TopAbs_Orientation.TopAbs_FORWARD)
                    {
                        aNextFPar = aPar21;
                        aNextLPar = aPar22;
                    }
                    else
                    {
                        aNextFPar = aPar22;
                        aNextLPar = aPar21;
                    }

                    gp_Pnt2d aPPrev = aPrevPC.Value(aPrevPar);
                    gp_Pnt2d aPNextF = aNextPC.Value(aNextFPar);
                    gp_Pnt2d aPNextL = aNextPC.Value(aNextLPar);

                    if (aPPrev.SquareDistance(aPNextF) > aPPrev.SquareDistance(aPNextL))
                    {
                        myEdge = new TopoDS_Edge();
                        return;
                    }
                }
                //  Modified by Sergey KHROMOV - Fri Jun 21 11:08:16 2002 End
                myEdge = TopoDS.Edge(l.First());
                l.Clear();
            }
            else
            {
                if (myFace.IsNull())
                {
                    // Without Face - try to return edges
                    // as logically as possible
                    // At first degenerated edges.
                    TopoDS_Edge E = myEdge;
                    if (SelectDegenerated(l, E))
                    {
                        myEdge = E;
                        return;
                    }
                    // At second double edges.
                    E = myEdge;
                    if (SelectDouble(myDoubles, l, E))
                    {
                        myEdge = E;
                        return;
                    }

                    //TopTools_ListIteratorOfListOfShape it(l);
                    bool notfound = true;
                    foreach (var it in l)
                    {
                        if (!it.IsSame(myEdge))
                        {
                            myEdge = TopoDS.Edge(it);
                            l.Remove(it);
                            notfound = false;
                            break;
                        }

                    }

                    if (notfound)
                    {
                        myEdge = new TopoDS_Edge();
                        return;
                    }

                }
                else
                {
                    // If we have more than one edge attached to the list
                    // probably wire that we explore contains a loop or loops.
                    double dfFPar = 0.0, dfLPar = 0.0;
                    Geom2d_Curve aPCurve = BRep_Tool.CurveOnSurface(myEdge, myFace, ref dfFPar, ref dfLPar);
                    if (aPCurve == null)
                    {
                        myEdge = new TopoDS_Edge();
                        return;
                    }
                    // Note: current < myVertex > which is last on < myEdge >
                    //       equals in 2D to following 2D points:
                    //       edge is FORWARD  - point with MAX parameter on PCurve;
                    //       edge is REVERSED - point with MIN parameter on PCurve.

                    // Get 2D point equals to < myVertex > in 2D for current edge.
                    gp_Pnt2d PRef = new gp_Pnt2d();
                    if (myEdge.Orientation() == TopAbs_Orientation.TopAbs_REVERSED)
                        aPCurve.D0(dfFPar, ref PRef);
                    else
                        aPCurve.D0(dfLPar, ref PRef);

                    // Get next 2D point from current edge's PCurve with parameter
                    // F + dP (REV) or L - dP (FOR)
                    bool isrevese = (myEdge.Orientation() == TopAbs_Orientation.TopAbs_REVERSED);
                    double dfMPar = GetNextParamOnPC(aPCurve, PRef, dfFPar, dfLPar, myTolU, myTolV, isrevese);

                    gp_Pnt2d PRefm = new gp_Pnt2d();
                    aPCurve.D0(dfMPar, ref PRefm);
                    // Get vector from PRef to PRefm
                    gp_Vec2d anERefDir = new gp_Vec2d(PRef, PRefm);
                    if (anERefDir.SquareMagnitude() < gp.Resolution())
                    {
                        myEdge = new TopoDS_Edge();
                        return;
                    }

                    // Search the list of edges looking for the edge having hearest
                    // 2D point of connected vertex to current one and smallest angle.
                    // First process all degenerated edges, then - all others.

                    TopTools_ListIteratorOfListOfShape it = new TopTools_ListIteratorOfListOfShape();
                    int k = 1, kMin = 0, iDone = 0;
                    bool isDegenerated = true;
                    double dmin = Standard_Real.RealLast();
                    double dfMinAngle = 3.0 * Math.PI, dfCurAngle = 3.0 * Math.PI;

                    for (iDone = 0; iDone < 2; iDone++)
                    {
                        it.Initialize(l);
                        while (it.More())
                        {
                            TopoDS_Edge E = TopoDS.Edge(it.Value());
                            if (E.IsSame(myEdge))
                            {
                                it.Next();
                                k++;
                                continue;
                            }

                            TopoDS_Vertex aVert1 = new TopoDS_Vertex(), aVert2 = new TopoDS_Vertex();
                            TopExp.Vertices(E, ref aVert1, ref aVert2, true);
                            if (aVert1.IsNull() || aVert2.IsNull())
                            {
                                it.Next();
                                k++;
                                continue;
                            }

                            aPCurve = BRep_Tool.CurveOnSurface(E, myFace, ref dfFPar, ref dfLPar);
                            if (aPCurve == null)
                            {
                                it.Next();
                                k++;
                                continue;
                            }

                            gp_Pnt2d aPEb = new gp_Pnt2d(), aPEe = new gp_Pnt2d();
                            if (aVert1.IsSame(aVert2) == isDegenerated)
                            {
                                if (E.Orientation() == TopAbs_Orientation.TopAbs_REVERSED)
                                    aPCurve.D0(dfLPar, ref aPEb);
                                else
                                    aPCurve.D0(dfFPar, ref aPEb);

                                if (Math.Abs(dfLPar - dfFPar) > Precision.PConfusion())
                                {
                                    isrevese = (E.Orientation() == TopAbs_Orientation.TopAbs_REVERSED);
                                    isrevese = !isrevese;
                                    double aEPm = GetNextParamOnPC(aPCurve, aPEb, dfFPar, dfLPar, myTolU, myTolV, isrevese);

                                    aPCurve.D0(aEPm, ref aPEe);
                                    if (aPEb.SquareDistance(aPEe) <= gp.Resolution())
                                    {
                                        //seems to be very short curve
                                        gp_Vec2d aD = new gp_Vec2d();
                                        aPCurve.D1(aEPm, out aPEe, out aD);
                                        if (E.Orientation() == TopAbs_Orientation.TopAbs_REVERSED)
                                            aPEe.SetXY(aPEb.XY() - aD.XY());
                                        else
                                            aPEe.SetXY(aPEb.XY() + aD.XY());

                                        if (aPEb.SquareDistance(aPEe) <= gp.Resolution())
                                        {
                                            it.Next();
                                            k++;
                                            continue;
                                        }
                                    }
                                    gp_Vec2d anEDir = new gp_Vec2d(aPEb, aPEe);
                                    dfCurAngle = Math.Abs(anEDir.Angle(anERefDir));
                                }

                                if (dfCurAngle <= dfMinAngle)
                                {
                                    double d = PRef.SquareDistance(aPEb);
                                    if (d <= Precision.PConfusion())
                                        d = 0.0;
                                    if (Math.Abs(aPEb.X() - PRef.X()) < myTolU && Math.Abs(aPEb.Y() - PRef.Y()) < myTolV)
                                    {
                                        if (d <= dmin)
                                        {
                                            dfMinAngle = dfCurAngle;
                                            kMin = k;
                                            dmin = d;
                                        }
                                    }
                                }
                            }
                            it.Next();
                            k++;
                        }// while it

                        if (kMin == 0)
                        {
                            isDegenerated = false;
                            k = 1;
                            dmin = Standard_Real.RealLast();
                        }
                        else
                            break;
                    }// for iDone

                    if (kMin == 0)
                    {
                        // probably unclosed in 2d space wire
                        myEdge = new TopoDS_Edge();
                        return;
                    }

                    // Selection the edge.
                    it.Initialize(l);
                    k = 1;
                    while (it.More())
                    {
                        if (k == kMin)
                        {
                            myEdge = TopoDS.Edge(it.Value());
                            l.Remove(it.Value());
                            break;
                        }
                        it.Next();
                        k++;
                    }
                }//else face != NULL && l > 1
            }//else l > 1
        }
    }
}