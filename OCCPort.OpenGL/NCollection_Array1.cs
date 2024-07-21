using System;
using System.Collections.Generic;
using System.Linq;

namespace OCCPort.OpenGL
{
    internal class NCollection_Array1
    {
        List<int> list = new List<int>();
        //! Empty constructor; should be used with caution.
        //! @sa methods Resize() and Move().
        public NCollection_Array1() {
            myLowerBound = (1);
            myUpperBound = (0);
    //myDeletable(Standard_False),
  //  myData(NULL)
        
            //
        }

        int myLowerBound;
        int myUpperBound;
        internal void Init(int val)
        {
            for (int i = myLowerBound; i <= myUpperBound; i++)
            {
                list[i] = val;
            }
        }

        internal int Lower()
        {
            return myLowerBound;
        }

        internal void SetValue(int v, int aDrawBuffer)
        {
            list[v] = aDrawBuffer;
        }

        internal int Upper()
        {
            return myUpperBound;
        }
        internal int Value(int index)
        {
            return list[index];
        }
    }
}