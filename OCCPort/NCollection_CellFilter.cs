using System;
using System.Drawing;

namespace OCCPort
{
    internal class NCollection_CellFilter<T> where T : IInspector
    {
        //! Inspect all targets in the cell corresponding to the given point
        public void Inspect(gp_XY thePnt, T theInspector)
        {
            Cell aCell = new Cell(thePnt, myCellSize);
            inspect(aCell, theInspector);
        }
        NCollection_Map<Cell> myCells;
        int myDim;

        //! Adds a target object for further search in the range of cells 
        //! defined by two points (the first point must have all coordinates equal or
        //! less than the same coordinate of the second point)
        public void Add(int theTarget, gp_XY thePntMin, gp_XY thePntMax)
        {
            // get cells range by minimal and maximal coordinates
            Cell aCellMin = new Cell(thePntMin, myCellSize);
            Cell aCellMax = new Cell(thePntMax, myCellSize);
            Cell aCell = aCellMin;
            // add object recursively into all cells in range
            iterateAdd(myDim - 1, aCell, aCellMin, aCellMax, theTarget);
        }

        //! Internal addition function, performing iteration for adjacent cells
        //! by one dimension; called recursively to cover all dimensions
        void iterateAdd(int idim, Cell theCell,

            Cell theCellMin, Cell theCellMax,
                    int theTarget)
        {
            /*
  //! Cell index type.
  typedef Standard_Integer Cell_IndexType;*/
            int aStart = theCellMin.index(idim);
            int anEnd = theCellMax.index(idim);
            for (int i = aStart; i <= anEnd; ++i)
            {
                theCell.index(idim, i);
                if (idim != 0) // recurse
                {
                    iterateAdd(idim - 1, theCell, theCellMin, theCellMax, theTarget);
                }
                else // add to this cell
                {
                    add(theCell, theTarget);
                }
            }
        }
        NCollection_BaseAllocator myAllocator;


        //! Add a new target object into the specified cell
        void add(Cell theCell, int theTarget)
        {
            // add a new cell or get reference to existing one
            Cell aMapCell = (Cell)myCells.Added(theCell);

            // create a new list node and add it to the beginning of the list
            //ListNode aNode = (ListNode)myAllocator.Allocate(sizeof(ListNode));
            ListNode aNode = new ListNode();
            //new(&aNode->Object) Target(theTarget);
            aNode.Object = theTarget;
            
            aNode.Next = aMapCell.Objects;
            aMapCell.Objects = aNode;
        }

        NCollection_Array1<double> myCellSize;
        //! Inspect the target objects in the specified cell.
        public void inspect(Cell theCell, T theInspector)
        {
            // check if any objects are recorded in that cell
            if (!myCells.Contains(theCell))
                return;

            // iterate by objects in the cell and check each
            Cell aMapCell = (Cell)myCells.Added(theCell);
            ListNode aNode = aMapCell.Objects;
            ListNode aPrev = null;
            while (aNode != null)
            {
                ListNode aNext = aNode.Next;
                NCollection_CellFilter_Action anAction =
                  theInspector.Inspect(aNode.Object);
                // delete items requested to be purged
                if (anAction == NCollection_CellFilter_Action.CellFilter_Purge)
                {
                    //aNode->Object.~Target();
                    //(aPrev ? aPrev->Next : aMapCell.Objects) = aNext;
                    // note that aNode itself need not to be freed, since IncAllocator is used
                }
                else
                    aPrev = aNode;
                aNode = aNext;
            }
        }
    }
}