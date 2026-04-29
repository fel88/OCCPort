using OpenTK.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace OCCPort
{
    public class NCollection_Array1<T>
    {
        public T[] list = null;
        //! Empty constructor; should be used with caution.
        //! @sa methods Resize() and Move().
        public NCollection_Array1()
        {
            myLowerBound = (1);
            myUpperBound = (0);
            //myDeletable(Standard_False),
            //  myData(NULL)

            //
        }
        public bool IsEmpty()
        {
            return list.Length == 0;
        }

        bool myDeletable; //!< Flag showing who allocated the array

        //! Constructor
        public NCollection_Array1(int theLower,
                     int theUpper)
        {
            myLowerBound = (theLower);
            myUpperBound = (theUpper);
            myDeletable = true;

            list = new T[theUpper - theLower + 1];
            //new Standard_RangeError_Raise_if(theUpper < theLower, "NCollection_Array1::Create");
            //TheItemType* pBegin = new TheItemType[Length()];
            //Standard_OutOfMemory_Raise_if(!pBegin, "NCollection_Array1 : Allocation failed");

            //myData = pBegin - theLower;
        }
        //! Copy constructor 
        public NCollection_Array1(T[] theOther, int lower, int upper)
        {
            myLowerBound = lower;
            myUpperBound = upper;
            list = theOther.ToArray();

        }
        public T this[int key]
        {
            get => list[key - Lower()];
            set => list[key - Lower()] = value;
        }

        int myLowerBound;
        int myUpperBound;
        public void Init(T val)
        {
            for (int i = myLowerBound; i <= myUpperBound; i++)
            {
                list[i] = val;
            }
        }

        public int Lower()
        {
            return myLowerBound;
        }

        public void SetValue(int v, T aDrawBuffer)
        {
            this[v] = aDrawBuffer;
        }

        public int Upper()
        {
            return myUpperBound;
        }
        public T Value(int index)
        {//Standard_OutOfRange_Raise_if(theIndex<myLowerBound || theIndex> myUpperBound, "NCollection_Array1::Value");
            return list[index];
        }

        public int Length()
        {
            return list.Length;
        }
        public void Resize(int v1, int v2, bool v3)
        {
            //throw new NotImplementedException();
        }

        internal int Size()
        {
            return list.Length ;
        }
    }
}