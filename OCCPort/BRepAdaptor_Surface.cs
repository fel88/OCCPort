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
        public virtual new GeomAbs_SurfaceType GetType() { return mySurf.GetType(); }

        //! Creates a surface to  access the geometry  of <F>.
        //! If  <Restriction> is  true  the parameter range is
        //! the  parameter  range  in   the  UV space  of  the
        //! restriction.
        public BRepAdaptor_Surface(TopoDS_Face F, bool R = true)
        {
            Initialize(F, R);

        }
        GeomAdaptor_Surface mySurf;
        gp_Trsf myTrsf;
        TopoDS_Face myFace;
        void Initialize(TopoDS_Face F,
                      bool Restriction)
        {
            myFace = F;
            TopLoc_Location L = new TopLoc_Location();
            Geom_Surface aSurface = BRep_Tool.Surface(F, ref L);
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


    }
}