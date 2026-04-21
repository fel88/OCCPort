using System;
using System.Drawing;

namespace OCCPort
{
    internal class Cell
    {
        public Cell(gp_XY thePnt, NCollection_Array1<double> myCellSize)
        {
            //index=(theCellSize.Size()),
        }
        public ListNode Objects;

        

        internal int index(int idim)
        {            
            ListNode top = Objects;
            for (int i = 0; i < idim; i++)
            {
                if (top.Object == idim)
                    return i;
                top = top.Next;
            }
            return -1;
        }

        internal void index(int idim, int i)
        {
            throw new NotImplementedException();
        }
    }
}