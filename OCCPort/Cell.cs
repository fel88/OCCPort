using System;
using System.Drawing;
using System.Linq;
using System.Security.Principal;

namespace OCCPort
{
    /**
     * Auxiliary structure representing a cell in the space.
     * Cells are stored in the map, each cell contains list of objects 
     * that belong to that cell.
     */
    internal class Cell
    {
        public double GetCoord(int ind, gp_XY p)
        {
            if (ind == 0)
                return p.X();
            return p.Y();
        }
        const int INT_MAX = 2147483647;
        const int INT_MIN = (-2147483647 - 1);

        //! Copy constructor: ensure that list is not deleted twice
        public Cell( Cell theOther)      
        {
            index = theOther.index.ToArray();
            Objects = theOther.Objects;//clone?
        }

        public Cell(gp_XY thePnt, NCollection_Array1<double> theCellSize)
        {
            index = new int[theCellSize.Size()];
            Objects = null;
            for (int i = 0; i < theCellSize.Size(); i++)
            {
                //double aVal = (double)(Inspector::Coord(i, thePnt) / theCellSize[theCellSize.Lower() + i]);
                double aVal = (double)(GetCoord(i, thePnt) / theCellSize[theCellSize.Lower() + i]);
                //If the value of index is greater than
                //INT_MAX it is decreased correspondingly for the value of INT_MAX. If the value
                //of index is less than INT_MIN it is increased correspondingly for the absolute
                //value of INT_MIN.
                index[i] = (int)((aVal > INT_MAX - 1) ? fmod(aVal, (double)INT_MAX)
                            : (aVal < INT_MIN + 1) ? fmod(aVal, (double)INT_MIN)
                                    : aVal);
            }
        }

        private double fmod(double aVal, double iNT_MAX)
        {
            return aVal % iNT_MAX;
        }

        public ListNode Objects;


        //! Compare cell with other one
        public override bool Equals(object obj)
        {
            var theOther = (Cell)obj;
            int aDim = (int)((theOther).index.Count());
            for (int i = 0; i < aDim; i++)
                if (index[i] != theOther.index[i])
                    return false;

            return true;
            //return base.Equals(obj);
        }

        public int[] index = new int[10];

        public override string ToString()
        {
            return $"Cell ({index[0]}; {index[1]})";
        }

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