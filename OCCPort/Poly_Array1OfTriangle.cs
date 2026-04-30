using OCCPort;
using System;
using System.Xml.Linq;

namespace OCCPort
{
    internal class Poly_Array1OfTriangle
    {
        public Poly_Triangle[] triangles;

        public Poly_Triangle Value(int index)
        {
            return triangles[index - 1];
        }
        internal void SetValue(int theIndex, Poly_Triangle theTriangle)
        {
            //todo: ???
            triangles[theIndex] = theTriangle;
        }
        //! Resizes the array to specified bounds.
        //! No re-allocation will be done if length of array does not change,
        //! but existing values will not be discarded if theToCopyData set to FALSE.
        //! @param theLower new lower bound of array
        //! @param theUpper new upper bound of array
        //! @param theToCopyData flag to copy existing data into new array
        public void Resize(int theLower,
                 int theUpper,
                 bool theToCopyData)
        {
            var old = triangles;
            triangles = new Poly_Triangle[theUpper - theLower + 1];
            if (theToCopyData)
                for (int i = 0; i < old.Length; i++)
                {
                    triangles[i] = old[i];
                }
            //Exceptions.Standard_RangeError_Raise_if(theUpper < theLower, "NCollection_Array1::Resize");
            //int anOldLen = Length();
            //i ntaNewLen = theUpper - theLower + 1;

            //TheItemType* aBeginOld = myData != NULL ? &myData[myLowerBound] : NULL;
            //myLowerBound = theLower;
            //myUpperBound = theUpper;
            //if (aNewLen == anOldLen)
            //{
            //    myData = aBeginOld - theLower;
            //    return;
            //}

            //if (!theToCopyData && myDeletable)
            //{
            //    delete[] aBeginOld;
            //}
            //TheItemType* aBeginNew = new TheItemType[aNewLen];
            //Standard_OutOfMemory_Raise_if(aBeginNew == NULL, "NCollection_Array1 : Allocation failed");
            //myData = aBeginNew - theLower;
            //if (!theToCopyData)
            //{
            //    myDeletable = Standard_True;
            //    return;
            //}

            //const Standard_Integer aLenCopy = Min(anOldLen, aNewLen);
            //for (Standard_Integer anIter = 0; anIter < aLenCopy; ++anIter)
            //{
            //    aBeginNew[anIter] = aBeginOld[anIter];
            //}
            //if (myDeletable)
            //{
            //    delete[] aBeginOld;
            //}
            //myDeletable = Standard_True;
        }
        public bool IsEmpty()
        {
            return triangles == null || triangles.Length == 0;//not origin code
        }
        public int Length()
        {
            if (triangles == null)//not origin code
                return 0;
            return triangles.Length;
        }
    }
}