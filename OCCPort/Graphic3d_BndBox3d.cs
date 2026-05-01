using System;

namespace OCCPort
{
    public class Graphic3d_BndBox3d : BVH_Box
    {


		public Graphic3d_BndBox3d()
		{
			myIsInited = false;
		}
		public Graphic3d_BndBox3d(BVH_VecNt min, BVH_VecNt max)
		{
			myIsInited = true;
			myMaxPoint = max;
			myMinPoint = min;
		}
        public Graphic3d_BndBox3d(BVH_VecNt min)
        {
            myIsInited = true;
            myMaxPoint = min;
            myMinPoint = min;
        }
        public Graphic3d_BndBox3d(Graphic3d_Vec3d min, Graphic3d_Vec3d max)
        {
            myIsInited = true;
            myMaxPoint = new BVH_VecNt ( max);
            myMinPoint = new  BVH_VecNt( min);
        }
    }
}
