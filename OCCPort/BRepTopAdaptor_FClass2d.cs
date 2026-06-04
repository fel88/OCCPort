using System;
using System.Formats.Asn1;
using TKBRep;
using TKMath;
using TriangleNet.Geometry;

namespace OCCPort
{
    public class BRepTopAdaptor_FClass2d
    {

        BRepTopAdaptor_SeqOfPtr TabClass = new BRepTopAdaptor_SeqOfPtr();
        TColStd_SequenceOfInteger TabOrien = new TColStd_SequenceOfInteger();
        double Toluv;
        TopoDS_Face Face;
        double U1;
        double V1;
        double U2;
        double V2;
        double Umin;
        double Umax;
        double Vmin;
        double Vmax;


        public BRepTopAdaptor_FClass2d(TopoDS_Face aFace, double TolUV)

        {
            Toluv = TolUV;
            Face = (aFace);
            U1 = (0.0);
            V1 = (0.0);
            U2 = (0.0);
            V2 = 0.0;
#if LBRCOMPT
  STAT.NbConstrShape++;
#endif

            //-- dead end on surfaces defined on more than one period

            Face.Orientation(TopAbs_Orientation.TopAbs_FORWARD);
            BRepAdaptor_Surface surf = new BRepAdaptor_Surface();
            surf.Initialize(aFace, false);

            TopoDS_Edge edge;
            TopAbs_Orientation Or;
            double u, du, Tole = 0.0, Tol = 0.0;
            BRepTools_WireExplorer WireExplorer = new BRepTools_WireExplorer();
            TopExp_Explorer FaceExplorer = new TopExp_Explorer();

            Umin = Vmin = 0.0; //RealLast();
            Umax = Vmax = -Umin;

            int aNbE = 0;
            double eps = 1e-10;
            int BadWire = 0;
            for (FaceExplorer.Init(Face, TopAbs_ShapeEnum.TopAbs_WIRE); (FaceExplorer.More() && BadWire == 0); FaceExplorer.Next())
            {
                int nbpnts = 0;
                TColgp_SequenceOfPnt2d SeqPnt2d = new TColgp_SequenceOfPnt2d();
                int firstpoint = 1;
                double FlecheU = 0.0;
                double FlecheV = 0.0;
                bool WireIsNotEmpty = false;
                int NbEdges = 0;

                TopExp_Explorer Explorer = new TopExp_Explorer();
                for (Explorer.Init(FaceExplorer.Current(), TopAbs_ShapeEnum.TopAbs_EDGE); Explorer.More(); Explorer.Next()) NbEdges++;
                aNbE = NbEdges;

                gp_Pnt Ancienpnt3d = new(0, 0, 0);
                bool Ancienpnt3dinitialise = false;

                for (WireExplorer.Init(TopoDS.Wire(FaceExplorer.Current()), Face); WireExplorer.More(); WireExplorer.Next())
                {

                    NbEdges--;
                    edge = (TopoDS_Edge)WireExplorer.Current();
                    Or = edge.Orientation();
                    if (Or == TopAbs_Orientation.TopAbs_FORWARD || Or == TopAbs_Orientation.TopAbs_REVERSED)
                    {
                        double pfbid = 0, plbid = 0;
                        if (BRep_Tool.CurveOnSurface(edge, Face, ref pfbid, ref plbid) == null)
                            return;

                        BRepAdaptor_Curve2d C = new BRepAdaptor_Curve2d(edge, Face);

                        //                        //-- ----------------------------------------
                        bool degenerated = false;
                        if (BRep_Tool.Degenerated(edge)) degenerated = true;
                        if (BRep_Tool.IsClosed(edge, Face)) degenerated = true;
                        TopoDS_Vertex Va = new TopoDS_Vertex(), Vb = new TopoDS_Vertex();
                        TopExp.Vertices(edge, ref Va, ref Vb);
                        double TolVertex1 = 0.0, TolVertex = 0.0;
                        if (Va.IsNull()) degenerated = true;
                        else TolVertex1 = BRep_Tool.Tolerance(Va);
                        if (Vb.IsNull()) degenerated = true;
                        else TolVertex = BRep_Tool.Tolerance(Vb);
                        if (TolVertex < TolVertex1) TolVertex = TolVertex1;
                        BRepAdaptor_Curve C3d = new BRepAdaptor_Curve();

                        if (Math.Abs(plbid - pfbid) < 1e-9)
                            continue;

                        //if(degenerated==false)
                        //  C3d.Initialize(edge,Face);

                        //-- Check cases when it was forgotten to code degenerated :  PRO17410 (janv 99)
                        if (degenerated == false)
                        {
                            C3d.Initialize(edge, Face);
                            du = (plbid - pfbid) * 0.1;
                            u = pfbid + du;
                            gp_Pnt P3da = C3d.Value(u);
                            degenerated = true;
                            u += du;
                            do
                            {

                                gp_Pnt P3db = C3d.Value(u);
                                // 		      if(P3da.SquareDistance(P3db)) { degenerated=false; break; }
                                if (P3da.SquareDistance(P3db) > Precision.Confusion()) { degenerated = false; break; }
                                u += du;
                            }
                            while (u < plbid);
                        }

                        //-- ----------------------------------------

                        Tole = BRep_Tool.Tolerance(edge);
                        if (Tole > Tol) Tol = Tole;

                        //int nbs = 1 + Geom2dInt_Geom2dCurveTool.NbSamples(C);
                        int nbs = Geom2dInt_Geom2dCurveTool.NbSamples(C);
                        //-- Attention to rational bsplines of degree 3. (ends of circles among others)
                        if (nbs > 2) nbs *= 4;
                        du = (plbid - pfbid) / (double)(nbs - 1);

                        if (Or == TopAbs_Orientation.TopAbs_FORWARD) u = pfbid;
                        else { u = plbid; du = -du; }

                        //                        //-- ------------------------------------------------------------
                        //                        //-- Check distance uv between the start point of the edge
                        //                        //-- and the last point registered in SeqPnt2d
                        //                        //-- Try to remote the first point of the current edge 
                        //                        //-- from the last saved point
                        //                        //# ifdef OCCT_DEBUG
                        //                        //                        gp_Pnt2d Pnt2dDebutEdgeCourant = C.Value(u); (void)Pnt2dDebutEdgeCourant;
                        //                        //#endif

                        //                        //double Baillement2dU=0;
                        //                        //double Baillement2dV=0;
                        //#if AFFICHAGE
                        //	      if(nbpnts>1) printf("\nTolVertex %g ",TolVertex);
                        //#endif

                        if (firstpoint == 2) u += du;
                        int Avant = nbpnts;
                        for (int e = firstpoint; e <= nbs; e++)
                        {
                            gp_Pnt2d P2d = C.Value(u);
                            if (P2d.X() < Umin) Umin = P2d.X();
                            if (P2d.X() > Umax) Umax = P2d.X();
                            if (P2d.Y() < Vmin) Vmin = P2d.Y();
                            if (P2d.Y() > Vmax) Vmax = P2d.Y();

                            double dist3dptcourant_ancienpnt = 1e+20;//RealLast();
                            gp_Pnt P3d = new gp_Pnt();
                            if (degenerated == false)
                            {
                                P3d = C3d.Value(u);
                                if (nbpnts > 1 && Ancienpnt3dinitialise) dist3dptcourant_ancienpnt = P3d.Distance(Ancienpnt3d);
                            }
                            bool IsRealCurve3d = true; //patch
                            if (dist3dptcourant_ancienpnt < Precision.Confusion())
                            {
                                gp_Pnt MidP3d = C3d.Value(u - du / 2.0);
                                if (P3d.Distance(MidP3d) < Precision.Confusion()) IsRealCurve3d = false;
                            }
                            if (IsRealCurve3d)
                            {
                                if (degenerated == false) { Ancienpnt3d = P3d; Ancienpnt3dinitialise = true; }
                                nbpnts++;
                                SeqPnt2d.Append(P2d);
                            }
                            //#if AFFICHAGE
                            //                  else { static int mm=0; printf("\npoint p%d  %g %g %g",++mm,P3d.X(),P3d.Y(),P3d.Z());	}
                            //#endif
                            u += du;
                            int ii = nbpnts;
                            //-- printf("\n nbpnts:%4d  u=%7.5g   FlecheU=%7.5g  FlecheV=%7.5g  ii=%3d  Avant=%3d ",nbpnts,u,FlecheU,FlecheV,ii,Avant);
                            // 		  if(ii>(Avant+4))
                            //  Modified by Sergey KHROMOV - Fri Apr 19 09:46:12 2002 Begin
                            if (ii > (Avant + 4) && SeqPnt2d[ii - 2].SquareDistance(SeqPnt2d[ii]) != 0)
                            //  Modified by Sergey KHROMOV - Fri Apr 19 09:46:13 2002 End
                            {
                                gp_Lin2d Lin = new gp_Lin2d(SeqPnt2d[ii - 2], new gp_Dir2d(new gp_Vec2d(SeqPnt2d[ii - 2], SeqPnt2d[ii])));
                                double ul = ElCLib.Parameter(Lin, SeqPnt2d[ii - 1]);
                                gp_Pnt2d Pp = ElCLib.Value(ul, Lin);
                                double dU = Math.Abs(Pp.X() - SeqPnt2d[ii - 1].X());
                                double dV = Math.Abs(Pp.Y() - SeqPnt2d[ii - 1].Y());
                                //-- printf(" (du=%7.5g   dv=%7.5g)",dU,dV);
                                if (dU > FlecheU) FlecheU = dU;
                                if (dV > FlecheV) FlecheV = dV;
                            }
                        }//for(e=firstpoint
                        if (firstpoint == 1) firstpoint = 2;
                        WireIsNotEmpty = true;
                    }//if(Or==FORWARD,REVERSED
                } //-- Edges -> for(Ware.Explorer

                if (NbEdges != 0)
                { //-- on compte ++ with a normal explorer and with the Wire Explorer
                    ///*
                    //#ifdef OCCT_DEBUG

                    //      std.cout << std.endl;
                    //      std.cout << "*** BRepTopAdaptor_Fclass2d  ** Wire Probablement FAUX **" << std.endl;
                    //      std.cout << "*** WireExplorer does not find all edges " << std.endl;
                    //      std.cout << "*** Connect old classifier" << std.endl;
                    //#endif
                    //*/
                    TColgp_Array1OfPnt2d PClass = new TColgp_Array1OfPnt2d(1, 2);
                    //// modified by jgv, 28.04.2009 ////
                    PClass.Init(new gp_Pnt2d(0.0, 0.0));
                    /////////////////////////////////////
                    TabClass.Append(new CSLib_Class2d(PClass, FlecheU, FlecheV, Umin, Vmin, Umax, Vmax));
                    BadWire = 1;
                    TabOrien.Append(-1);
                }
                else if (WireIsNotEmpty)
                {
                    //double anglep=0,anglem=0;
                    TColgp_Array1OfPnt2d PClass = new TColgp_Array1OfPnt2d(1, nbpnts);
                    double square = 0.0;

                    //-------------------------------------------------------------------
                    //-- ** The mode of calculation was somewhat changed 
                    //-- Before Oct 31 97 , the total angle of  
                    //-- rotation of the wire was evaluated on all angles except for the last 
                    //-- ** Now, exactly the angle of rotation is evaluated
                    //-- If a value remote from 2PI or -2PI is found, it means that there is 
                    //-- an uneven number of loops

                    if (nbpnts > 3)
                    {
                        //	      int im2=nbpnts-2;
                        int im1 = nbpnts - 1;
                        int im0 = 1;
                        //	      PClass(im2)=SeqPnt2d.Value(im2);
                        PClass[im1] = SeqPnt2d.Value(im1);
                        PClass[nbpnts] = SeqPnt2d.Value(nbpnts);

                        double aPer = 0.0;
                        //	      for(int ii=1; ii<nbpnts; ii++,im0++,im1++,im2++)
                        for (int ii = 1; ii < nbpnts; ii++, im0++, im1++)
                        {
                            //		  if(im2>=nbpnts) im2=1;
                            if (im1 >= nbpnts) im1 = 1;
                            PClass[ii] = SeqPnt2d.Value(ii);
                            //		  gp_Vec2d A(PClass(im2),PClass(im1));
                            //		  gp_Vec2d B(PClass(im1),PClass(im0));
                            //		  double N = A.Magnitude() * B.Magnitude();

                            square += (PClass[im0].X() - PClass[im1].X()) * (PClass[im0].Y() + PClass[im1].Y()) * .5;
                            aPer += (PClass[im0].XY() - PClass[im1].XY()).Modulus();

                            //		  if(N>1e-16){ double a=A.Angle(B); angle+=a; }
                        }

                        double anExpThick = Math.Max(2.0 * Math.Abs(square) / aPer, 1e-7);
                        double aDefl = Math.Max(FlecheU, FlecheV);
                        double aDiscrDefl = Math.Min(aDefl * 0.1, anExpThick * 10.0);
                        while (aDefl > anExpThick && aDiscrDefl > 1e-7)
                        {
                            //                            // Deflection of the polygon is too much for this ratio of area and perimeter,
                            //                            // and this might lead to self-intersections.
                            //                            // Discretize the wire more tightly to eliminate the error.
                            //                            firstpoint = 1;
                            //                            SeqPnt2d.Clear();
                            //                            FlecheU = 0.0;
                            //                            FlecheV = 0.0;
                            for (WireExplorer.Init(TopoDS.Wire(FaceExplorer.Current()), Face);
                              WireExplorer.More(); WireExplorer.Next())
                            {
                                edge = (TopoDS_Edge)WireExplorer.Current();
                                Or = edge.Orientation();
                                if (Or == TopAbs_Orientation.TopAbs_FORWARD || Or == TopAbs_Orientation.TopAbs_REVERSED)
                                {
                                    //double pfbid, plbid;
                                    //BRep_Tool.Range(edge, Face, pfbid, plbid);
                                    //if (Abs(plbid - pfbid) < 1.e - 9) continue;
                                    //BRepAdaptor_Curve2d C(edge, Face);
                                    //GCPnts_QuasiUniformDeflection aDiscr(C, aDiscrDefl);
                                    //if (!aDiscr.IsDone())
                                    //    break;
                                    //int nbp = aDiscr.NbPoints();
                                    //int iStep = 1, i = 1, iEnd = nbp + 1;
                                    //if (Or == TopAbs_REVERSED)
                                    //{
                                    //    iStep = -1;
                                    //    i = nbp;
                                    //    iEnd = 0;
                                    //}
                                    //if (firstpoint == 2)
                                    //    i += iStep;
                                    //for (; i != iEnd; i += iStep)
                                    //{
                                    //    gp_Pnt2d aP2d = C.Value(aDiscr.Parameter(i));
                                    //    SeqPnt2d.Append(aP2d);
                                    //}
                                    //if (nbp > 2)
                                    //{
                                    //    int ii = SeqPnt2d.Length();
                                    //    gp_Lin2d Lin = new gp_Lin2d(SeqPnt2d(ii - 2), gp_Dir2d(gp_Vec2d(SeqPnt2d(ii - 2), SeqPnt2d(ii))));
                                    //    double ul = ElCLib.Parameter(Lin, SeqPnt2d(ii - 1));
                                    //    gp_Pnt2d Pp = ElCLib.Value(ul, Lin);
                                    //    double dU = Abs(Pp.X() - SeqPnt2d(ii - 1).X());
                                    //    double dV = Abs(Pp.Y() - SeqPnt2d(ii - 1).Y());
                                    //    if (dU > FlecheU) FlecheU = dU;
                                    //    if (dV > FlecheV) FlecheV = dV;
                                    //}
                                    firstpoint = 2;
                                }
                            }
                            nbpnts = SeqPnt2d.Length();
                            PClass.Resize(1, nbpnts, false);
                            im1 = nbpnts - 1;
                            im0 = 1;
                            PClass[im1] = SeqPnt2d.Value(im1);
                            PClass[nbpnts] = SeqPnt2d.Value(nbpnts);
                            square = 0.0;
                            aPer = 0.0;
                            for (int ii = 1; ii < nbpnts; ii++, im0++, im1++)
                            {
                                if (im1 >= nbpnts) im1 = 1;
                                PClass[ii] = SeqPnt2d.Value(ii);
                                square += (PClass[im0].X() - PClass[im1].X()) * (PClass[im0].Y() + PClass[im1].Y()) * .5;
                                aPer += (PClass[im0].XY() - PClass[im1].XY()).Modulus();
                            }

                            anExpThick = Math.Max(2.0 * Math.Abs(square) / aPer, 1e-7);
                            aDefl = Math.Max(FlecheU, FlecheV);
                            aDiscrDefl = Math.Min(aDiscrDefl * 0.1, anExpThick * 10.0);
                        }

                        //-- FlecheU*=10.0;
                        //-- FlecheV*=10.0;
                        if (aNbE == 1 && FlecheU < eps && FlecheV < eps && Math.Abs(square) < eps)
                        {
                            TabOrien.Append(1);
                        }
                        else
                        {
                            TabOrien.Append(((square < 0.0) ? 1 : 0));
                        }

                        if (FlecheU < Toluv) FlecheU = Toluv;
                        if (FlecheV < Toluv) FlecheV = Toluv;
                        //-- std.cout<<" U:"<<FlecheU<<" V:"<<FlecheV<<std.endl;
                        TabClass.Append(new CSLib_Class2d(PClass, FlecheU, FlecheV, Umin, Vmin, Umax, Vmax));

                        //	      if((angle<2 && angle>-2)||(angle>10)||(angle<-10))
                        //		{
                        //		  BadWire=1;
                        //		  TabOrien.Append(-1);
                        //#ifdef OCCT_DEBUG
                        //		  std.cout << std.endl;
                        //		  std.cout << "*** BRepTopAdaptor_Fclass2d  ** Wire Probably FALSE **" << std.endl;
                        //		  std.cout << "*** Total rotation angle of the wire : " << angle << std.endl;
                        //		  std.cout << "*** Connect the old classifier" << std.endl;
                        //#endif
                        //		} 
                        //	      else TabOrien.Append(((angle>0.0)? 1 : 0));
                    }//if(nbpoints>3


                    else
                    {
                        //# ifdef OCCT_DEBUG
                        //                        std.cout << std.endl;
                        //                        std.cout << "*** BRepTopAdaptor_Fclass2d  ** Wire Probably FALSE **" << std.endl;
                        //                        std.cout << "*** The sample wire contains less than 3 points" << std.endl;
                        //                        std.cout << "*** Connect the old classifier" << std.endl;
                        //#endif
                        BadWire = 1;
                        TabOrien.Append(-1);
                        TColgp_Array1OfPnt2d xPClass = new TColgp_Array1OfPnt2d(1, 2);
                        xPClass[1] = SeqPnt2d[1];
                        xPClass[2] = SeqPnt2d[2];
                        TabClass.Append(new CSLib_Class2d(xPClass, FlecheU, FlecheV, Umin, Vmin, Umax, Vmax));
                    }
                }//else if(WareIsNotEmpty
            }//for(FaceExplorer

            int nbtabclass = TabClass.Length();

            if (nbtabclass > 0)
            {
                //    //-- If an error was detected on a wire: set all TabOrien to -1
                if (BadWire != 0) TabOrien[1] = -1;

                if (surf._GetType() == GeomAbs_SurfaceType.GeomAbs_Cone
               || surf._GetType() == GeomAbs_SurfaceType.GeomAbs_Cylinder
               || surf._GetType() == GeomAbs_SurfaceType.GeomAbs_Torus
               || surf._GetType() == GeomAbs_SurfaceType.GeomAbs_Sphere
               || surf._GetType() == GeomAbs_SurfaceType.GeomAbs_SurfaceOfRevolution)

                {
                    double uuu = Math.PI + Math.PI - (Umax - Umin);
                    if (uuu < 0) uuu = 0;
                    U1 = 0.0;  // modified by NIZHNY-OFV  Thu May 31 14:24:10 2001 ---> //Umin-uuu*0.5;
                    U2 = 2 * Math.PI; // modified by NIZHNY-OFV  Thu May 31 14:24:35 2001 ---> //U1+M_PI+M_PI;
                }
                else { U1 = U2 = 0.0; }

                if (surf._GetType() == GeomAbs_SurfaceType.GeomAbs_Torus)
                {
                    double uuu = Math.PI + Math.PI - (Vmax - Vmin);
                    if (uuu < 0) uuu = 0;
                    V1 = 0.0;  // modified by NIZHNY-OFV  Thu May 31 14:24:55 2001 ---> //Vmin-uuu*0.5;
                    V2 = 2 * Math.PI; // modified by NIZHNY-OFV  Thu May 31 14:24:59 2001 ---> //V1+M_PI+M_PI;
                }
                else { V1 = V2 = 0.0; }
            }
        }
        //   BRepTopAdaptor_SeqOfPtr TabClass;
        //TColStd_SequenceOfInteger TabOrien;

        public TopAbs_State PerformInfinitePoint()
        {

            if (Umax == -Standard_Real.RealLast()
                || Vmax == -Standard_Real.RealLast()
                || Umin == Standard_Real.RealLast()
                || Vmin == Standard_Real.RealLast())
            {
                return (TopAbs_State.TopAbs_IN);
            }
            gp_Pnt2d P = new gp_Pnt2d(Umin - (Umax - Umin), Vmin - (Vmax - Vmin));
            return (Perform(P, false));
        }

        public TopAbs_State Perform(gp_Pnt2d _Puv,
                          bool RecadreOnPeriodic)
        {
            int dedans;
            int nbtabclass = TabClass.Length();

            if (nbtabclass == 0)
            {
                return TopAbs_State.TopAbs_IN;
            }

            //-- U1 is the First Param and U2 in this case is U1+Period
            double u = _Puv.X();
            double v = _Puv.Y();
            double uu = u, vv = v;

            BRepAdaptor_Surface surf = new BRepAdaptor_Surface();
            surf.Initialize(Face, false);
            bool IsUPer = surf.IsUPeriodic();
            bool IsVPer = surf.IsVPeriodic();
            double uperiod = IsUPer ? surf.UPeriod() : 0.0;
            double vperiod = IsVPer ? surf.VPeriod() : 0.0;
            TopAbs_State aStatus = TopAbs_State.TopAbs_UNKNOWN;
            bool urecadre = false, vrecadre = false;

            if (RecadreOnPeriodic)
            {
                if (IsUPer)
                {
                    if (uu < Umin)
                        while (uu < Umin)
                            uu += uperiod;
                    else
                    {
                        while (uu >= Umin)
                            uu -= uperiod;
                        uu += uperiod;
                    }
                }
                if (IsVPer)
                {
                    if (vv < Vmin)
                        while (vv < Vmin)
                            vv += vperiod;
                    else
                    {
                        while (vv >= Vmin)
                            vv -= vperiod;
                        vv += vperiod;
                    }
                }
            }

            for (; ; )
            {
                dedans = 1;
                gp_Pnt2d Puv = new gp_Pnt2d(u, v);

                if (TabOrien[1] != -1)
                {
                    for (int n = 1; n <= nbtabclass; n++)
                    {
                        int cur = ((CSLib_Class2d)TabClass[n]).SiDans(Puv);
                        if (cur == 1)
                        {
                            if (TabOrien[n] == 0)
                            {
                                dedans = -1;
                                break;
                            }
                        }
                        else if (cur == -1)
                        {
                            if (TabOrien[n] == 1)
                            {
                                dedans = -1;
                                break;
                            }
                        }
                        else
                        {
                            dedans = 0;
                            break;
                        }
                    }
                    if (dedans == 0)
                    {
                        BRepClass_FaceClassifier aClassifier = new BRepClass_FaceClassifier();
                        double m_Toluv = (Toluv > 4.0) ? 4.0 : Toluv;
                        //aClassifier.Perform(Face,Puv,Toluv);
                        aClassifier.Perform(Face, Puv, m_Toluv);
                        aStatus = aClassifier.State();
                    }
                    if (dedans == 1)
                    {
                        aStatus = TopAbs_State.TopAbs_IN;
                    }
                    if (dedans == -1)
                    {
                        aStatus = TopAbs_State.TopAbs_OUT;
                    }
                }
                else
                {  //-- TabOrien(1)=-1    False Wire
                    BRepClass_FaceClassifier aClassifier = new BRepClass_FaceClassifier();
                    aClassifier.Perform(Face, Puv, Toluv);
                    aStatus = aClassifier.State();
                }

                if (!RecadreOnPeriodic || (!IsUPer && !IsVPer))
                    return aStatus;
                if (aStatus == TopAbs_State.TopAbs_IN || aStatus == TopAbs_State.TopAbs_ON)
                    return aStatus;

                if (!urecadre)
                {
                    u = uu;
                    urecadre = true;
                }
                else
              if (IsUPer)
                    u += uperiod;
                if (u > Umax || !IsUPer)
                {
                    if (!vrecadre)
                    {
                        v = vv;
                        vrecadre = true;
                    }
                    else
                      if (IsVPer)
                        v += vperiod;

                    u = uu;

                    if (v > Vmax || !IsVPer)
                        return aStatus;
                }
            } //for (;;)
        }

    }
}
