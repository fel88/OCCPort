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


       

    }
}
