using System;
using System.Drawing;

namespace OCCPort
{
    /**
     * Auxiliary structure representing a cell in the space.
     * Cells are stored in the map, each cell contains list of objects 
     * that belong to that cell.
     */
    internal class Cell
    {
        public Cell(gp_XY thePnt, NCollection_Array1<double> theCellSize)
        {
            index = new int[theCellSize.Size()];
            Objects = null;
            for (int i = 0; i < theCellSize.Size(); i++)
            {
                // double aVal = (Standard_Real)(Inspector::Coord(i, thePnt) / theCellSize[theCellSize.Lower() + i]);
                //If the value of index is greater than
                //INT_MAX it is decreased correspondingly for the value of INT_MAX. If the value
                //of index is less than INT_MIN it is increased correspondingly for the absolute
                //value of INT_MIN.
                //index[i] = Cell_IndexType((aVal > INT_MAX - 1) ? fmod(aVal, (Standard_Real)INT_MAX)
                //            : (aVal < INT_MIN + 1) ? fmod(aVal, (Standard_Real)INT_MIN)
                //                    : aVal);
            }
        }
        public ListNode Objects;



        public int[] index = new int[10];


        /*internal int index(int idim)
        {
            ListNode top = Objects;
            for (int i = 0; i < idim; i++)
            {
                if (top.Object == idim)
                    return i;
                top = top.Next;
            }
            return -1;
        }*/

        /*internal void index(int idim, int i)
        {
            throw new NotImplementedException();
        }*/
    }
}