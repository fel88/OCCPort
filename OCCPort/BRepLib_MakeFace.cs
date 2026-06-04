using OCCPort;
using OCCPort.Tester;
using System;
using System.Formats.Asn1;
using System.Reflection.Metadata;
using TKBRep;
using TriangleNet.Topology.DCEL;

namespace OCCPort
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
        public static implicit operator TopoDS_Face(BRepLib_MakeFace f)
        {
            return f.Face();
        }

        //! Make a face from a Surface. Accepts tolerance value (TolDegen)
        //! for resolution of degenerated edges.        

        public BRepLib_MakeFace(Geom_Surface S,
                                   double TolDegen)
        {
            myShape = new TopoDS_Face();//not origin code!

            Init(S, true, TolDegen);
        }
        public void Init(Geom_Surface S,
                               bool Bound,
                               double TolDegen)
        {
            myError = BRepLib_FaceError.BRepLib_FaceDone;
            if (Bound)
            {
                double UMin, UMax, VMin, VMax;
                S.Bounds(out UMin, out UMax, out VMin, out VMax);
                Init(S, UMin, UMax, VMin, VMax, TolDegen);
            }
            else
            {
                BRep_Builder _B = new BRep_Builder();
                _B.MakeFace(TopoDS.Face(myShape), S, Precision.Confusion());
            }

            BRep_Builder B = new BRep_Builder();
            B.NaturalRestriction(TopoDS.Face(myShape), true);
        }


        //=======================================================================
        //function : IsDegenerated
        //purpose  : Checks whether the passed curve is degenerated with the
        //           passed tolerance value
        //=======================================================================

        public bool IsDegenerated(
    Geom_Curve theCurve,
    double theMaxTol,
           double theActTol)
        {
            GeomAdaptor_Curve AC = new GeomAdaptor_Curve(theCurve);
            double aConfusion = Precision.Confusion();
            theActTol = aConfusion;
            GeomAbs_CurveType Type = AC._GetType();

            //if (Type == GeomAbs_Circle)
            //{
            //    gp_Circ Circ = AC.Circle();
            //    if (Circ.Radius() > theMaxTol)
            //        return Standard_False;
            //    theActTol = Max(Circ.Radius(), aConfusion);
            //    return Standard_True;
            //}
            //else if (Type == GeomAbs_BSplineCurve)
            //{
            //    Handle(Geom_BSplineCurve) BS = AC.BSpline();
            //    Standard_Integer NbPoles = BS->NbPoles();
            //    Standard_Real aMaxPoleDist2 = 0.0, aMaxTol2 = theMaxTol * theMaxTol;
            //    gp_Pnt P1, P2;
            //    P1 = BS->Pole(1);
            //    for (Standard_Integer i = 2; i <= NbPoles; i++)
            //    {
            //        P2 = BS->Pole(i);
            //        Standard_Real aPoleDist2 = P1.SquareDistance(P2);
            //        if (aPoleDist2 > aMaxTol2)
            //            return Standard_False;
            //        if (aPoleDist2 > aMaxPoleDist2)
            //            aMaxPoleDist2 = aPoleDist2;
            //    }
            //    theActTol = Max(1.000001 * Sqrt(aMaxPoleDist2), aConfusion);
            //    return Standard_True;
            //}
            //else if (Type == GeomAbs_BezierCurve)
            //{
            //    Handle(Geom_BezierCurve) BZ = AC.Bezier();
            //    Standard_Integer NbPoles = BZ->NbPoles();
            //    Standard_Real aMaxPoleDist2 = 0.0, aMaxTol2 = theMaxTol * theMaxTol;
            //    gp_Pnt P1, P2;
            //    P1 = BZ->Pole(1);
            //    for (Standard_Integer i = 2; i <= NbPoles; i++)
            //    {
            //        P2 = BZ->Pole(i);
            //        Standard_Real aPoleDist2 = P1.SquareDistance(P2);
            //        if (aPoleDist2 > aMaxTol2)
            //            return Standard_False;
            //        if (aPoleDist2 > aMaxPoleDist2)
            //            aMaxPoleDist2 = aPoleDist2;
            //    }
            //    theActTol = Max(1.000001 * Sqrt(aMaxPoleDist2), aConfusion);
            //    return Standard_True;
            //}

            return false;
        }


        public void Init(Geom_Surface SS,
                           double Um,
                           double UM,
                           double Vm,
                           double VM,
                           double TolDegen)
        {
            myError = BRepLib_FaceError.BRepLib_FaceDone;

            double UMin = Um;
            double UMax = UM;
            double VMin = Vm;
            double VMax = VM;

            double umin, umax, vmin, vmax, T;

            Geom_Surface S = SS, BS = SS;
            Geom_RectangularTrimmedSurface RS =
              S as Geom_RectangularTrimmedSurface;
            if (RS != null)
                BS = RS.BasisSurface();

            var OffsetSurface =
              (BS is Geom_OffsetSurface);

            // adjust periodical surface or reordonate
            // check if the values are in the natural range
            var epsilon = Precision.PConfusion();

            BS.Bounds(out umin, out umax, out vmin, out vmax);

            if (OffsetSurface)
            {
                Geom_OffsetSurface OS = (Geom_OffsetSurface)BS;
                Geom_Surface Base = OS.BasisSurface();

                //if (Base is Geom_SurfaceOfLinearExtrusion)
                //{
                //    if (Precision::IsInfinite(umin) || Precision::IsInfinite(umax))
                //        S = new Geom_RectangularTrimmedSurface(OS, UMin, UMax, VMin, VMax);
                //    else
                //        S = new Geom_RectangularTrimmedSurface(OS, VMin, VMax, Standard_False);
                //}
                //else if (Base->DynamicType() == STANDARD_TYPE(Geom_SurfaceOfRevolution))
                //{
                //    if (Precision::IsInfinite(vmin) || Precision::IsInfinite(vmax))
                //        S = new Geom_RectangularTrimmedSurface(OS, VMin, VMax, Standard_False);
                //}
            }

            if (S.IsUPeriodic())
            {
                ElCLib.AdjustPeriodic(umin, umax, epsilon, ref UMin, ref UMax);
            }
            else if (UMin > UMax)
            {
                T = UMin;
                UMin = UMax;
                UMax = T;
                if ((umin - UMin > epsilon) || (UMax - umax > epsilon))
                {
                    myError = BRepLib_FaceError.BRepLib_ParametersOutOfRange;
                    return;
                }
            }

            if (S.IsVPeriodic())
            {
                ElCLib.AdjustPeriodic(vmin, vmax, epsilon, ref VMin, ref VMax);
            }
            else if (VMin > VMax)
            {
                T = VMin;
                VMin = VMax;
                VMax = T;
                if ((vmin - VMin > epsilon) || (VMax - vmax > epsilon))
                {
                    myError = BRepLib_FaceError.BRepLib_ParametersOutOfRange;
                    return;
                }
            }

            // compute infinite flags
            bool umininf = Precision.IsNegativeInfinite(UMin);
            bool umaxinf = Precision.IsPositiveInfinite(UMax);
            bool vmininf = Precision.IsNegativeInfinite(VMin);
            bool vmaxinf = Precision.IsPositiveInfinite(VMax);

            // closed flag
            bool uclosed =
              S.IsUClosed() &&
                Math.Abs(UMin - umin) < epsilon &&
              Math.Abs(UMax - umax) < epsilon;

            bool vclosed =
              S.IsVClosed() &&
                Math.Abs(VMin - vmin) < epsilon &&
              Math.Abs(VMax - vmax) < epsilon;


            // compute 3d curves and degenerate flag
            double maxTol = TolDegen;
            Geom_Curve Cumin = null, Cumax = null, Cvmin = null, Cvmax = null;
            bool Dumin, Dumax, Dvmin, Dvmax;
            Dumin = Dumax = Dvmin = Dvmax = false;
            double uminTol = Precision.Confusion(),
                          umaxTol = Precision.Confusion(),
                          vminTol = Precision.Confusion(),
                          vmaxTol = Precision.Confusion();

            if (!umininf)
            {
                Cumin = S.UIso(UMin);
                Dumin = IsDegenerated(Cumin, maxTol, uminTol);
            }
            if (!umaxinf)
            {
                Cumax = S.UIso(UMax);
                Dumax = IsDegenerated(Cumax, maxTol, umaxTol);
            }
            if (!vmininf)
            {
                Cvmin = S.VIso(VMin);
                Dvmin = IsDegenerated(Cvmin, maxTol, vminTol);
            }
            if (!vmaxinf)
            {
                Cvmax = S.VIso(VMax);
                Dvmax = IsDegenerated(Cvmax, maxTol, vmaxTol);
            }

            // compute vertices
            BRep_Builder B = new BRep_Builder();

            TopoDS_Vertex V00 = new TopoDS_Vertex(),
                V10 = new TopoDS_Vertex(),
                V11 = new TopoDS_Vertex(),
                V01 = new TopoDS_Vertex();

            if (!umininf)
            {
                if (!vmininf) B.MakeVertex(V00, S.Value(UMin, VMin), Math.Max(uminTol, vminTol));
                if (!vmaxinf) B.MakeVertex(V01, S.Value(UMin, VMax), Math.Max(uminTol, vmaxTol));
            }
            if (!umaxinf)
            {
                if (!vmininf) B.MakeVertex(V10, S.Value(UMax, VMin), Math.Max(umaxTol, vminTol));
                if (!vmaxinf) B.MakeVertex(V11, S.Value(UMax, VMax), Math.Max(umaxTol, vmaxTol));
            }

            if (uclosed)
            {
                V10 = V00;
                V11 = V01;
            }

            if (vclosed)
            {
                V01 = V00;
                V11 = V10;
            }

            if (Dumin) V00 = V01;
            if (Dumax) V10 = V11;
            if (Dvmin) V00 = V10;
            if (Dvmax) V01 = V11;

            // make the lines
            Geom2d_Line Lumin = null, Lumax = null, Lvmin = null, Lvmax = null;
            if (!umininf)
                Lumin = new Geom2d_Line(new gp_Pnt2d(UMin, 0), new gp_Dir2d(0, 1));
            if (!umaxinf)
                Lumax = new Geom2d_Line(new gp_Pnt2d(UMax, 0), new gp_Dir2d(0, 1));
            if (!vmininf)
                Lvmin = new Geom2d_Line(new gp_Pnt2d(0, VMin), new gp_Dir2d(1, 0));
            if (!vmaxinf)
                Lvmax = new Geom2d_Line(new gp_Pnt2d(0, VMax), new gp_Dir2d(1, 0));

            // make the face
            TopoDS_Face F = TopoDS.Face(myShape);
            B.MakeFace(F, S, Precision.Confusion());

            // make the edges
            TopoDS_Edge eumin = new TopoDS_Edge(), eumax = new TopoDS_Edge(), evmin = new TopoDS_Edge(), evmax = new TopoDS_Edge();

            if (!umininf)
            {
                if (!Dumin)
                    B.MakeEdge(eumin, Cumin, uminTol);
                else
                    B.MakeEdge(eumin);
                if (uclosed)
                {
                    //   B.UpdateEdge(eumin, Lumax, Lumin, F, Math.Max(uminTol, umaxTol));
                }
                else
                    B.UpdateEdge(eumin, Lumin, F, uminTol);
                B.Degenerated(eumin, Dumin);
                if (!vmininf)
                {
                    V00.Orientation(TopAbs_Orientation.TopAbs_FORWARD);
                    B.Add(eumin, V00);
                }
                if (!vmaxinf)
                {
                    V01.Orientation(TopAbs_Orientation.TopAbs_REVERSED);
                    B.Add(eumin, V01);
                }
                B.Range(eumin, VMin, VMax);
            }

            if (!umaxinf)
            {
                if (uclosed)
                    eumax = eumin;
                else
                {
                    if (!Dumax)
                        B.MakeEdge(eumax, Cumax, umaxTol);
                    else
                        B.MakeEdge(eumax);
                    B.UpdateEdge(eumax, Lumax, F, umaxTol);
                    B.Degenerated(eumax, Dumax);
                    if (!vmininf)
                    {
                        V10.Orientation(TopAbs_Orientation.TopAbs_FORWARD);
                        B.Add(eumax, V10);
                    }
                    if (!vmaxinf)
                    {
                        V11.Orientation(TopAbs_Orientation.TopAbs_REVERSED);
                        B.Add(eumax, V11);
                    }
                    B.Range(eumax, VMin, VMax);
                }
            }

            if (!vmininf)
            {
                if (!Dvmin)
                    B.MakeEdge(evmin, Cvmin, vminTol);
                else
                    B.MakeEdge(evmin);
                if (vclosed)
                {
                    //   B.UpdateEdge(evmin, Lvmin, Lvmax, F, Math.Max(vminTol, vmaxTol));
                }
                else
                    B.UpdateEdge(evmin, Lvmin, F, vminTol);
                B.Degenerated(evmin, Dvmin);
                if (!umininf)
                {
                    V00.Orientation(TopAbs_Orientation.TopAbs_FORWARD);
                    B.Add(evmin, V00);
                }
                if (!umaxinf)
                {
                    V10.Orientation(TopAbs_Orientation.TopAbs_REVERSED);
                    B.Add(evmin, V10);
                }
                B.Range(evmin, UMin, UMax);
            }

            if (!vmaxinf)
            {
                if (vclosed)
                    evmax = evmin;
                else
                {
                    if (!Dvmax)
                        B.MakeEdge(evmax, Cvmax, vmaxTol);
                    else
                        B.MakeEdge(evmax);
                    B.UpdateEdge(evmax, Lvmax, F, vmaxTol);
                    B.Degenerated(evmax, Dvmax);
                    if (!umininf)
                    {
                        V01.Orientation(TopAbs_Orientation.TopAbs_FORWARD);
                        B.Add(evmax, V01);
                    }
                    if (!umaxinf)
                    {
                        V11.Orientation(TopAbs_Orientation.TopAbs_REVERSED);
                        B.Add(evmax, V11);
                    }
                    B.Range(evmax, UMin, UMax);
                }
            }

            // make the wires and add them to the face
            eumin.Orientation(TopAbs_Orientation.TopAbs_REVERSED);
            evmax.Orientation(TopAbs_Orientation.TopAbs_REVERSED);

            TopoDS_Wire W = new TopoDS_Wire();

            if (!umininf && !umaxinf && vmininf && vmaxinf)
            {
                // two wires in u
                B.MakeWire(W);
                B.Add(W, eumin);
                B.Add(F, W);
                B.MakeWire(W);
                B.Add(W, eumax);
                B.Add(F, W);
                F.Closed(uclosed);
            }

            else if (umininf && umaxinf && !vmininf && !vmaxinf)
            {
                // two wires in v
                B.MakeWire(W);
                B.Add(W, evmin);
                B.Add(F, W);
                B.MakeWire(W);
                B.Add(W, evmax);
                B.Add(F, W);
                F.Closed(vclosed);
            }

            else if (!umininf || !umaxinf || !vmininf || !vmaxinf)
            {
                // one wire
                B.MakeWire(W);
                if (!umininf) B.Add(W, eumin);
                if (!vmininf) B.Add(W, evmin);
                if (!umaxinf) B.Add(W, eumax);
                if (!vmaxinf) B.Add(W, evmax);
                B.Add(F, W);
                W.Closed(!umininf && !umaxinf && !vmininf && !vmaxinf);
                F.Closed(uclosed && vclosed);
            }

            if (OffsetSurface)
            {
                // Les Isos sont Approximees a Precision::Approximation()
                // et on code Precision::Confusion() dans l'arete.
                // ==> Un petit passage dans SamePrameter pour regler les tolerances.
                BRepLib.SameParameter(F, Precision.Confusion(), true);
            }

            Done();
        }


        //! Find a surface from the wire and make a face.
        //! if <OnlyPlane> is true, the computed surface will be
        //! a plane. If it is not possible to find a plane, the
        //! flag NotDone will be set.
        public BRepLib_MakeFace(TopoDS_Wire W, bool OnlyPlane = false)
        {
            myShape = new TopoDS_Face();//not origin code!
            // Find a surface through the wire
            BRepLib_FindSurface FS = new BRepLib_FindSurface(W, -1, OnlyPlane, true);
            if (!FS.Found())
            {
                myError = BRepLib_FaceError.BRepLib_NotPlanar;
                return;
            }

            // build the face and add the wire
            BRep_Builder B = new BRep_Builder();
            myError = BRepLib_FaceError.BRepLib_FaceDone;

            double tol = Math.Max(1.2 * FS.ToleranceReached(), FS.Tolerance());

            B.MakeFace(TopoDS.Face(myShape), FS.Surface(), FS.Location(), tol);
            Add(W);
            //
            BRepLib.UpdateTolerances(myShape);
            //
            BRepLib.SameParameter(myShape, tol, true);
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
