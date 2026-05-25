using System;

namespace OCCPort
{
    public class Graphic3d_Vec4_BoxMinMax : IBoxMinMax<Graphic3d_Vec4>
    {
        public void CwiseMax(ref Graphic3d_Vec4 theVec1, Graphic3d_Vec4 theVec2)
        {
            theVec1.X = Math.Max(theVec1.X, theVec2.X);
            theVec1.Y = Math.Max(theVec1.Y, theVec2.Y);
            theVec1.Z = Math.Max(theVec1.Z, theVec2.Z);
            theVec1.W = Math.Max(theVec1.W, theVec2.W);
        }

        public Graphic3d_Vec4 CwiseMax(Graphic3d_Vec4 theVec1, Graphic3d_Vec4 theVec2)
        {
            theVec1.X = Math.Max(theVec1.X, theVec2.X);
            theVec1.Y = Math.Max(theVec1.Y, theVec2.Y);
            theVec1.Z = Math.Max(theVec1.Z, theVec2.Z);
            theVec1.W = Math.Max(theVec1.W, theVec2.W);
            return theVec1;
        }

        public void CwiseMin(ref Graphic3d_Vec4 theVec1, Graphic3d_Vec4 theVec2)
        {
            theVec1.X = Math.Min(theVec1.X, theVec2.X);
            theVec1.Y = Math.Min(theVec1.Y, theVec2.Y);
            theVec1.Z = Math.Min(theVec1.Z, theVec2.Z);
            theVec1.W = Math.Min(theVec1.W, theVec2.W);
        }

        public Graphic3d_Vec4 CwiseMin(Graphic3d_Vec4 theVec1, Graphic3d_Vec4 theVec2)
        {
            theVec1.X = Math.Min(theVec1.X, theVec2.X);
            theVec1.Y = Math.Min(theVec1.Y, theVec2.Y);
            theVec1.Z = Math.Min(theVec1.Z, theVec2.Z);
            theVec1.W = Math.Min(theVec1.W, theVec2.W);
            return theVec1;
        }
    }
}
