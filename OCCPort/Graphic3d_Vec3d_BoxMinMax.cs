using System;

namespace OCCPort
{
    public class Graphic3d_Vec3d_BoxMinMax : IBoxMinMax<Graphic3d_Vec3d>
    {
        public void CwiseMax(ref Graphic3d_Vec3d theVec1, Graphic3d_Vec3d theVec2)
        {
            theVec1.X = Math.Max(theVec1.X, theVec2.X);
            theVec1.Y = Math.Max(theVec1.Y, theVec2.Y);
            theVec1.Z = Math.Max(theVec1.Z, theVec2.Z);
        }

        public Graphic3d_Vec3d CwiseMax(Graphic3d_Vec3d theVec1, Graphic3d_Vec3d theVec2)
        {
            throw new NotImplementedException();
        }

        public void CwiseMin(ref Graphic3d_Vec3d theVec1, Graphic3d_Vec3d theVec2)
        {
            theVec1.X = Math.Min(theVec1.X, theVec2.X);
            theVec1.Y = Math.Min(theVec1.Y, theVec2.Y);
            theVec1.Z = Math.Min(theVec1.Z, theVec2.Z);
        }

        public Graphic3d_Vec3d CwiseMin(Graphic3d_Vec3d theVec1, Graphic3d_Vec3d theVec2)
        {
            throw new NotImplementedException();
        }
    }
}
