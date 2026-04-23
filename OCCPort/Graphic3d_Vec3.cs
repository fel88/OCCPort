using System;

namespace OCCPort
{
    public class Graphic3d_Vec3 : NCollection_Vec3_float
    {
        public Graphic3d_Vec3(byte[] aDataPtr, int offset) : base(
            BitConverter.ToSingle(aDataPtr, offset),
            BitConverter.ToSingle(aDataPtr, offset + sizeof(float)),
            BitConverter.ToSingle(aDataPtr, offset + sizeof(float) * 2))
        {
        }
    }
}