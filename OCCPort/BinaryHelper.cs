using System;

namespace OCCPort
{
    public class BinaryHelper
    {
        public static Graphic3d_Vec3 Get_Vec3(byte[] aDataPtr, int offset)
        {
            return new Graphic3d_Vec3(BitConverter.ToSingle(aDataPtr, offset),
                BitConverter.ToSingle(aDataPtr, offset + sizeof(float)),
                BitConverter.ToSingle(aDataPtr, offset + sizeof(float) * 2));


        }
    }
}