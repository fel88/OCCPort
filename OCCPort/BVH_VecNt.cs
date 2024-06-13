using System;

namespace OCCPort
{
    public class BVH_VecNt
    {
		public BVH_VecNt(double x, double y, double z)
		{
			v[0] = x;
			v[1] = y;
			v[2] = z;
		}

		double[] v = new double[3];
        internal double x()
        {
			return v[0];
        }

        internal double y()
        {
			return v[1];
        }

        internal double z()
        {
			return v[2];
        }
    }
}