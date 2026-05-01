namespace OCCPort
{
    public class Graphic3d_Vec3d : NCollection_Vec3_double
    {

        public Graphic3d_Vec3d() { }

        public Graphic3d_Vec3d(BVH_VecNt v) : base(v.X, v.Y, v.Z)
        {
        }

        public Graphic3d_Vec3d(double v1, double v2, double v3) : base(v1, v2, v3)
        {

        }


    }
}
