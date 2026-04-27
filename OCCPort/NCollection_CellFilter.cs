using System;
using System.Drawing;
using System.Reflection.Metadata;

namespace OCCPort
{
    /**
     * A data structure for sorting geometric objects (called targets) in 
     * n-dimensional space into cells, with associated algorithm for fast checking 
     * of coincidence (overlapping, intersection, etc.) with other objects 
     * (called here bullets).
     *
     * Description
     * 
     * The algorithm is based on hash map, thus it has linear time of initialization 
     * (O(N) where N is number of cells covered by added targets) and constant-time 
     * search for one bullet (more precisely, O(M) where M is number of cells covered 
     * by the bullet).
     *
     * The idea behind the algorithm is to separate each coordinate of the space
     * into equal-size cells. Note that this works well when cell size is 
     * approximately equal to the characteristic size of the involved objects 
     * (targets and bullets; including tolerance eventually used for coincidence 
     * check). 
     *
     * Usage
     *
     * The target objects to be searched are added to the tool by methods Add(); 
     * each target is classified as belonging to some cell(s). The data on cells 
     * (list of targets found in each one) are stored in the hash map with key being 
     * cumulative index of the cell by all coordinates.
     * Thus the time needed to find targets in some cell is O(1) * O(number of 
     * targets in the cell).
     *
     * As soon as all the targets are added, the algorithm is ready to check for 
     * coincidence.
     * To find the targets coincident with any given bullet, it checks all the 
     * candidate targets in the cell(s) covered by the bullet object 
     * (methods Inspect()).
     *
     * The methods Add() and Inspect() have two flavours each: one accepts
     * single point identifying one cell, another accept two points specifying
     * the range of cells. It should be noted that normally at least one of these
     * methods is called as for range of cells: either due to objects having non-
     * zero size, or in order to account for the tolerance when objects are points.
     *
     * The set of targets can be modified during the process: new targets can be
     * added by Add(), existing targets can be removed by Remove().
     *
     * Implementation
     *
     * The algorithm is implemented as template class, thus it is capable to 
     * work with objects of any type. The only argument of the template should be 
     * the specific class providing all necessary features required by the 
     * algorithm:
     *
     * - typedef "Target" defining type of target objects.
     *   This type must have copy constructor
     *
     * - typedef "Point" defining type of geometrical points used 
     *
     * - enum Dimension whose value must be dimension of the point
     *
     * - method Coord() returning value of the i-th coordinate of the point:
     *
     *   static Standard_Real Coord (int i, const Point& thePnt);
     *
     *   Note that index i is from 0 to Dimension-1.
     *
     * - method IsEqual() used by Remove() to identify objects to be removed:
     *
     *   Standard_Boolean IsEqual (const Target& theT1, const Target& theT2);
     *
     * - method Inspect() performing necessary actions on the candidate target 
     *   object (usially comparison with the currently checked bullet object):
     *
     *   NCollection_CellFilter_Action Inspect (const Target& theObject);
     *
     *   The returned value can be used to command CellFilter
     *   to remove the inspected item from the current cell; this allows
     *   to exclude the items that has been processed and are not needed any 
     *   more in further search (for better performance).
     *
     *   Note that method Inspect() can be const and/or virtual.
     */

    internal class NCollection_CellFilter<T> where T : IInspector
    {
        //! Constructor when dimenstion count is known at compilation time.
        public NCollection_CellFilter(double theCellSize = 0,
            int dimension = 2
                           )
        {
            myCellSize = new NCollection_Array1<double>(0, dimension - 1);
            myDim = dimension;
            Reset(theCellSize);
        }

        //! Clear the data structures, set new cell size and allocator
        public void Reset(double theCellSize)
        {
            for (int i = 0; i < myDim; i++)
                myCellSize[i] = theCellSize;
            //resetAllocator(theAlloc);
        }

        //! Inspect all targets in the cell corresponding to the given point
        public void Inspect(gp_XY thePnt, T theInspector)
        {
            Cell aCell = new Cell(thePnt, myCellSize);
            inspect(aCell, theInspector);
        }
        NCollection_Map<Cell> myCells = new NCollection_Map<Cell>();
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

        //! Adds a target object for further search at a point (into only one cell)
        public void Add(int theTarget, gp_XY thePnt)
        {
            Cell aCell = new Cell(thePnt, myCellSize);
            add(aCell, theTarget);
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
            int aStart = theCellMin.index[idim];
            int anEnd = theCellMax.index[idim];
            for (int i = aStart; i <= anEnd; ++i)
            {
                theCell.index[idim]= i;
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