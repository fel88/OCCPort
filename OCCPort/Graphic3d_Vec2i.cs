using System;
using System.Reflection.Emit;
using System.Xml.Linq;

namespace OCCPort
{
    public class Graphic3d_Vec2i : NCollection_Vec2i
    {
        public Graphic3d_Vec2i() { }
        public Graphic3d_Vec2i(int x, int y) : base(x, y) { }
        public Graphic3d_Vec2i(int xy) : base(xy) { }
        //! Compute per-component division by scale factor.
        public static Graphic3d_Vec2i operator /(Graphic3d_Vec2i vv, int theInvFactor)
        {
            return new Graphic3d_Vec2i(vv.v[0] / theInvFactor,
                    vv.v[1] / theInvFactor);
        }

    }


}