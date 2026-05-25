using System;

namespace OCCPort
{
    public class Graphic3d_BndBox3d : BVH_Box<Graphic3d_Vec3d, Graphic3d_Vec3d_BoxMinMax>
    {


        public Graphic3d_BndBox3d()
        {
            myIsInited = false;
        }
        public Graphic3d_BndBox3d(Graphic3d_Vec3d min, Graphic3d_Vec3d max)
        {
            myIsInited = true;
            myMaxPoint = max;
            myMinPoint = min;
        }
        public Graphic3d_BndBox3d(Graphic3d_Vec3d min)
        {
            myIsInited = true;
            myMaxPoint = min;
            myMinPoint = min;
        }
        
    }
}
