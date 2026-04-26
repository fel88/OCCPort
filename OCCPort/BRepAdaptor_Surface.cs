using System.Reflection.Metadata;

namespace OCCPort
{
    //! The Surface from BRepAdaptor allows to  use a Face
    //! of the BRep topology look like a 3D surface.
    //!
    //! It  has  the methods  of  the class   Surface from
    //! Adaptor3d.
    //!
    //! It is created or initialized with a Face. It takes
    //! into account the local coordinates system.
    //!
    //! The  u,v parameter range is   the minmax value for
    //! the  restriction,  unless  the flag restriction is
    //! set to false.
    public class BRepAdaptor_Surface : Adaptor3d_Surface
    {
        //! Returns the type of the surface : Plane, Cylinder,
        //! Cone,      Sphere,        Torus,    BezierSurface,
        //! BSplineSurface,               SurfaceOfRevolution,
        //! SurfaceOfExtrusion, OtherSurface
        public override GeomAbs_SurfaceType _GetType() { return mySurf._GetType(); }

        //! Creates a surface to  access the geometry  of <F>.
        //! If  <Restriction> is  true  the parameter range is
        //! the  parameter  range  in   the  UV space  of  the
        //! restriction.
        public BRepAdaptor_Surface(TopoDS_Face F, bool R = true)
        {
            Initialize(F, R);

        }

        public override Adaptor3d_Surface BasisSurface()
        {
            GeomAdaptor_Surface HS = new GeomAdaptor_Surface();
            HS.Load((Geom_Surface)(mySurf.Surface().Transformed(myTrsf)));
            return HS.BasisSurface();
        }

        public override double FirstVParameter() { return mySurf.FirstVParameter(); }

        public override double LastVParameter() { return mySurf.LastVParameter(); }


        public override double FirstUParameter() { return mySurf.FirstUParameter(); }
        public override double LastUParameter() { return mySurf.LastUParameter(); }

        GeomAdaptor_Surface mySurf = new GeomAdaptor_Surface();
        gp_Trsf myTrsf;
        TopoDS_Face myFace;
        void Initialize(TopoDS_Face F,
                      bool Restriction)
        {
            myFace = F;
            TopLoc_Location L;
            Geom_Surface aSurface = BRep_Tool.Surface(F, out L);
            if (aSurface == null)
                return;

            if (Restriction)
            {
                double umin = 0, umax = 0, vmin = 0, vmax = 0;
                BRepTools.UVBounds(F, ref umin, ref umax, ref vmin, ref vmax);
                mySurf.Load(aSurface, umin, umax, vmin, vmax);
            }
            else
                mySurf.Load(aSurface);
            myTrsf = L.Transformation();
        }

        public override gp_Pln Plane()
        {
            return mySurf.Plane().Transformed(myTrsf);
        }

        public override bool IsVPeriodic()
        {
            throw new System.NotImplementedException();
        }

        public override bool IsUPeriodic()
        {
            throw new System.NotImplementedException();
        }

        public override double UPeriod()
        {
            throw new System.NotImplementedException();
        }

        public override double VPeriod()
        {
            throw new System.NotImplementedException();
        }

        public override double VResolution(double v)
        {
            throw new System.NotImplementedException();
        }

        public override double UResolution(double v)
        {
            throw new System.NotImplementedException();
        }

        public override void D1(double U, double V, out gp_Pnt P, out gp_Vec D1U, out gp_Vec D1V)
        {
            throw new System.NotImplementedException();
        }

        public override int NbVKnots()
        {
            throw new System.NotImplementedException();
        }

        public override void D0(double u, double v, ref gp_Pnt pnt)
        {
            throw new System.NotImplementedException();
        }
    }
}