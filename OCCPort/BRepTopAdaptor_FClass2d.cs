using System;

namespace OCCPort.Tester
{
    public class BRepTopAdaptor_FClass2d
    {
        //   BRepTopAdaptor_SeqOfPtr TabClass;
        //TColStd_SequenceOfInteger TabOrien;
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

            throw new NotImplementedException();
        //    int dedans;
        //    int nbtabclass = TabClass.Length();

        //    if (nbtabclass == 0)
        //    {
        //        return TopAbs_State.TopAbs_IN;
        //    }

        //    //-- U1 is the First Param and U2 in this case is U1+Period
        //    double u = _Puv.X();
        //    double v = _Puv.Y();
        //    double uu = u, vv = v;

        //    BRepAdaptor_Surface surf = new BRepAdaptor_Surface();
        //    surf->Initialize(Face, Standard_False);
        //    const Standard_Boolean IsUPer = surf->IsUPeriodic();
        //    const Standard_Boolean IsVPer = surf->IsVPeriodic();
        //    const Standard_Real uperiod = IsUPer ? surf->UPeriod() : 0.0;
        //    const Standard_Real vperiod = IsVPer ? surf->VPeriod() : 0.0;
        //    TopAbs_State aStatus = TopAbs_UNKNOWN;
        //    bool urecadre = false, vrecadre = false;

        //    if (RecadreOnPeriodic)
        //    {
        //        if (IsUPer)
        //        {
        //            if (uu < Umin)
        //                while (uu < Umin)
        //                    uu += uperiod;
        //            else
        //            {
        //                while (uu >= Umin)
        //                    uu -= uperiod;
        //                uu += uperiod;
        //            }
        //        }
        //        if (IsVPer)
        //        {
        //            if (vv < Vmin)
        //                while (vv < Vmin)
        //                    vv += vperiod;
        //            else
        //            {
        //                while (vv >= Vmin)
        //                    vv -= vperiod;
        //                vv += vperiod;
        //            }
        //        }
        //    }

        //    for (; ; )
        //    {
        //        dedans = 1;
        //        gp_Pnt2d Puv = new gp_Pnt2d(u, v);

        //        if (TabOrien(1) != -1)
        //        {
        //            for (int n = 1; n <= nbtabclass; n++)
        //            {
        //                int cur = ((CSLib_Class2d)TabClass[n]).SiDans(Puv);
        //                if (cur == 1)
        //                {
        //                    if (TabOrien(n) == 0)
        //                    {
        //                        dedans = -1;
        //                        break;
        //                    }
        //                }
        //                else if (cur == -1)
        //                {
        //                    if (TabOrien(n) == 1)
        //                    {
        //                        dedans = -1;
        //                        break;
        //                    }
        //                }
        //                else
        //                {
        //                    dedans = 0;
        //                    break;
        //                }
        //            }
        //            if (dedans == 0)
        //            {
        //                BRepClass_FaceClassifier aClassifier;
        //                Standard_Real m_Toluv = (Toluv > 4.0) ? 4.0 : Toluv;
        //                //aClassifier.Perform(Face,Puv,Toluv);
        //                aClassifier.Perform(Face, Puv, m_Toluv);
        //                aStatus = aClassifier.State();
        //            }
        //            if (dedans == 1)
        //            {
        //                aStatus = TopAbs_State.TopAbs_IN;
        //            }
        //            if (dedans == -1)
        //            {
        //                aStatus = TopAbs_State.TopAbs_OUT;
        //            }
        //        }
        //        else
        //        {  //-- TabOrien(1)=-1    False Wire
        //            BRepClass_FaceClassifier aClassifier;
        //            aClassifier.Perform(Face, Puv, Toluv);
        //            aStatus = aClassifier.State();
        //        }

        //        if (!RecadreOnPeriodic || (!IsUPer && !IsVPer))
        //            return aStatus;
        //        if (aStatus == TopAbs_IN || aStatus == TopAbs_ON)
        //            return aStatus;

        //        if (!urecadre)
        //        {
        //            u = uu;
        //            urecadre = Standard_True;
        //        }
        //        else
        //      if (IsUPer)
        //            u += uperiod;
        //        if (u > Umax || !IsUPer)
        //        {
        //            if (!vrecadre)
        //            {
        //                v = vv;
        //                vrecadre = Standard_True;
        //            }
        //            else
        //              if (IsVPer)
        //                v += vperiod;

        //            u = uu;

        //            if (v > Vmax || !IsVPer)
        //                return aStatus;
        //        }
        //    } //for (;;)
        }

    }
}
