using System;
using System.Collections;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OCCPort
{
    public class Graphic3d_IndexBuffer : Graphic3d_Buffer
    {  //! Empty constructor.
        public Graphic3d_IndexBuffer(NCollection_BaseAllocator theAlloc)
     : base(theAlloc) { }


        //! Access index at specified position
        public int Index(int theIndex)
        {
            return Stride == sizeof(ushort)
                 ? (int)(Value(theIndex))
                 : (int)(Value(theIndex));
        }
        //! Allocates new empty index array

        public float[] DataFloat()
        {
            List<float> ret = new List<float>();
            for (int i = 0; i < myData.Length; i+=Stride)
            {
                ret.Add(BitConverter.ToSingle(myData, i));
            }
            return ret.ToArray();
        }
        public bool Init(int sizeOfElement, int theNbElems)
        {
            release();
            Stride = sizeOfElement;
            if (Stride != sizeof(ushort)
     && Stride != sizeof(uint))
            {
                return false;
            }

            NbElements = theNbElems;
            NbAttributes = 0;
            if (NbElements != 0
            && !Allocate(Stride * NbElements))
            {
                release();
                return false;
            }
            return true;
        }

      

        //! Change index at specified position
        public void SetIndex(int theIndex, int theValue)
        {
            if (Stride == sizeof(ushort))
            {
                long offset = Stride * theIndex;
                var bts = BitConverter.GetBytes((ushort)theValue);
                for (int i = 0; i < bts.Length; i++)
                {
                    myData[i + offset] = bts[i];
                }
                //ChangeValue < unsigned short> (theIndex) = (unsigned short )theValue;
            }
            else
            {
                //ChangeValue < unsigned int> (theIndex) = (unsigned int   )theValue;
                long offset = Stride * theIndex;
                var bts = BitConverter.GetBytes((uint)theValue);
                for (int i = 0; i < bts.Length; i++)
                {
                    myData[i + offset] = bts[i];
                }
            }
        }

        internal bool Init(uint theMaxEdges)
        {
            throw new NotImplementedException();
        }
    }
}