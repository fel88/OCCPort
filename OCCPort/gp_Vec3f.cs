namespace OCCPort
{
    public class gp_Vec3f : NCollection_Vec3_float
    {
        //! Initialize ALL components of vector within specified value.
        public gp_Vec3f(float v1)
        {
            v[0] = v[1] = v[2] = v1;
        }

        public gp_Vec3f(float value1, float value2, float value3) : base(value1, value2, value3)
        {
        }

        public static gp_Vec3f operator +(gp_Vec3f v, gp_Vec3f v2)
        {
            return new gp_Vec3f(
                v.v[0] + v2.v[0],
                v.v[1] + v2.v[1],
                v.v[2] + v2.v[2]);
        }
        public static gp_Vec3f operator /(gp_Vec3f v, float v2)
        {
            return new gp_Vec3f(
                v.v[0] / v2,
                v.v[1] / v2,
                v.v[2] / v2);
        }
    }
}