using OCCPort;
using OCCPort.Tester;
using System;
using System.Reflection.Metadata;

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
            TopTools_ListOfShape empty;

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
                    //  if (!myMap.IsBound(V1))
                    //     myMap.Bind(V1, empty);
                    //  myMap(V1).Append(E);

                    // add or remove in the vertex map
                    V1.Orientation(TopAbs_Orientation.TopAbs_FORWARD);
                    // int currsize = vmap.Extent(),
                    //                ind = vmap.Add(V1);
                    // if (currsize >= ind)
                    {
                        // vmap.RemoveKey(V1);
                    }
                }

                if (!V2.IsNull())
                {
                    V2.Orientation(TopAbs_Orientation.TopAbs_REVERSED);
                    //   int currsize = vmap.Extent(),
                    //                   ind = vmap.Add(V2);
                    //  if (currsize >= ind)
                    {
                        // vmap.RemoveKey(V2);
                    }
                }

                if (V1.IsNull() || V2.IsNull())
                {
                    double aF = 0.0, aL = 0.0;
                    BRep_Tool.Range(E, out aF, out aL);

                    if (Eori == TopAbs_Orientation.TopAbs_FORWARD)
                    {
                        // if (aF == -Precision.Infinite())
                        //   anInfEmap.Add(E);
                    }
                    else
                    { // Eori == TopAbs_REVERSED
                      //   if (aL == Precision.Infinite())
                      //    anInfEmap.Add(E);
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
            //if (!myMap.IsBound(V1)) return;

            //   TopTools_ListOfShape l = myMap(V1);
            //  myEdge = TopoDS.Edge(l.First());
            //  l.RemoveFirst();
            //   myVertex = TopExp.FirstVertex(myEdge, true);

        }

        TopTools_DataMapOfShapeListOfShape myMap;
        TopoDS_Edge myEdge;
        TopoDS_Vertex myVertex;
        TopoDS_Face myFace;
        TopTools_MapOfShape myDoubles;
        bool myReverse;
        double myTolU;
        double myTolV;

        internal TopoDS_Shape Current()
        {
            throw new NotImplementedException();
        }

        internal bool More()
        {
            throw new NotImplementedException();
        }

        internal object Next()
        {
            throw new NotImplementedException();
        }
    }
}