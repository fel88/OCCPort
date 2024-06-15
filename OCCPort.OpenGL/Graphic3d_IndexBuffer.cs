using System;

namespace OCCPort.OpenGL
{
    public class Graphic3d_IndexBuffer : Graphic3d_Buffer
    {
        //! Access index at specified position
        public int Index(int theIndex)
        {
            return Stride == sizeof(ushort)
                 ? (int)((ushort)(theIndex))
                 : (int)((uint)(theIndex));
        }

        //! Change index at specified position
        public void SetIndex(int theIndex, int theValue)
        {
            if (Stride == sizeof(ushort))
            {
                //ChangeValue < unsigned short> (theIndex) = (unsigned short )theValue;
            }
            else
            {
                //ChangeValue < unsigned int> (theIndex) = (unsigned int   )theValue;
            }
        }

    }
}