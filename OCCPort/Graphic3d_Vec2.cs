using System;
using System.Linq;

namespace OCCPort
{
    internal class Graphic3d_Vec2 : NCollection_Vec2<float>
    {
        public Graphic3d_Vec2(byte[] aDataPtr, int offset) : base(BitConverter.ToSingle(aDataPtr, offset),
            BitConverter.ToSingle(aDataPtr, offset + sizeof(float)))
        {
        }
    }
}