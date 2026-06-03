namespace TKMath
{
    //! The geometric processor package, called gp, provides an
    //! implementation of entities used  :
    //! . for algebraic calculation such as "XYZ" coordinates, "Mat"
    //! matrix
    //! . for basis analytic geometry such as Transformations, point,
    //! vector, line, plane, axis placement, conics, and elementary
    //! surfaces.
    //! These entities are defined in 2d and 3d space.
    //! All the classes of this package are non-persistent.
    public static class gp
    {

        public static double Resolution()
        {
            //2.2250738585072014e-308
            return double.Epsilon;
        }
        public static gp_Dir2d DY2d()
        {
            gp_Dir2d gp_DY2d = new gp_Dir2d(0, 1);
            return gp_DY2d;
        }
        public static gp_Dir2d DX2d()
        {
            gp_Dir2d gp_DY2d = new gp_Dir2d(1, 0);
            return gp_DY2d;
        }


        public static gp_Dir DX()
        {
            gp_Dir gp_DX = new gp_Dir(1, 0, 0);
            return gp_DX;

        }

        public static gp_Dir DY()
        {
            gp_Dir gp_DY = new gp_Dir(0, 1, 0);
            return gp_DY;

        }

        public static gp_Dir DZ()
        {
            gp_Dir gp_DZ = new gp_Dir(0, 0, 1);
            return gp_DZ;

        }

        public static gp_Pnt Origin()
        {
            gp_Pnt gp_Origin = new gp_Pnt(0, 0, 0);
            return gp_Origin;

        }
    }
}
