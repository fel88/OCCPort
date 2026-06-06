using OCCPort.Common;
using TKernel;
using TKG3d;
using TKMath;

namespace TKMesh
{
    //! Auxiliary class to find circles shot by the given point.
    public class BRepMesh_CircleInspector : NCollection_CellFilter_InspectorXY, IInspector
    {
        //! Returns list of circles shot by the reference point.
        public ListOfInteger GetShotCircles()
        {
            return myResIndices;
        }


        //! Returns circle with the given index.
        //! @param theIndex index of circle.
        //! @return circle with the given index.
        public BRepMesh_Circle Circle(int theIndex)
        {
            return myCircles[theIndex];
        }

        //! Adds the circle to vector of circles at the given position.
        //! @param theIndex position of circle in the vector.
        //! @param theCircle circle to be added.
        public void Bind(int theIndex,
              BRepMesh_Circle theCircle)
        {
            myCircles.SetValue(theIndex, theCircle);
        }

        //! Set reference point to be checked.
        //! @param thePoint bullet point.
        public void SetPoint(gp_XY thePoint)
        {
            myResIndices.Clear();
            myPoint = thePoint;
        }

        //! Performs inspection of a circle with the given index.
        //! @param theTargetIndex index of a circle to be checked.
        //! @return status of the check.
        public NCollection_CellFilter_Action Inspect(int theTargetIndex)
        {
            BRepMesh_Circle aCircle = myCircles.Value(theTargetIndex);
            double aRadius = aCircle.Radius();
            if (aRadius < 0.0)
                return NCollection_CellFilter_Action.CellFilter_Purge;

            gp_XY aLoc = (gp_XY)(aCircle.Location());

            double aDX = myPoint.ChangeCoord(1) - aLoc.ChangeCoord(1);
            double aDY = myPoint.ChangeCoord(2) - aLoc.ChangeCoord(2);

            //This check is wrong. It is better to use 
            //  
            //  const Standard_Real aR = aRadius + aToler;
            //  if ((aDX * aDX + aDY * aDY) <= aR * aR)
            //  {
            //    ...
            //  }

            //where aToler = sqrt(mySqTolerance). Taking into account the fact
            //that the input parameter of the class (see constructor) is linear
            //(not quadratic) tolerance there is no point in square root computation.
            //Simply, we do not need to compute square of the input tolerance and to
            //assign it to mySqTolerance. The input linear tolerance is needed to be used.

            //However, this change leads to hangs the test case "perf mesh bug27119".
            //So, this correction is better to be implemented in the future.

            if ((aDX * aDX + aDY * aDY) - (aRadius * aRadius) <= mySqTolerance)
                myResIndices.Append(theTargetIndex);

            return NCollection_CellFilter_Action.CellFilter_Keep;
        }

        double mySqTolerance;
        ListOfInteger myResIndices;
        VectorOfCircle myCircles;
        gp_XY myPoint;


        //! Constructor.
        //! @param theTolerance tolerance to be used for identification of shot circles.
        //! @param theReservedSize size to be reserved for vector of circles.
        //! @param theAllocator memory allocator to be used by internal collections.
        public BRepMesh_CircleInspector(double theTolerance, int theReservedSize)
        {
            mySqTolerance = (theTolerance * theTolerance);
            myResIndices = new ListOfInteger();
            //myCircles = new VectorOfCircle(theReservedSize);
            myCircles = new VectorOfCircle();
        }
    }
    public class VectorOfCircle : NCollection_Vector<BRepMesh_Circle>
    {
        internal void SetValue(int theIndex, BRepMesh_Circle theCircle)
        {
            //expandV
            while (Count <= theIndex)
            {
                Add(null);
            }

            base[theIndex] = theCircle;
       
        
        }
    }


    //! Default implementation of curve data model entity.
    public class BRepMeshData_PCurve : AbstractMeshData_PCurve, IMeshData_PCurve
    {
        //! Returns orientation of the edge associated with current pcurve.
        //! Returns forward flag of this pcurve.
        public bool IsForward()
        {
            return (myOrientation != TopAbs_Orientation.TopAbs_REVERSED);
        }
        public TopAbs_Orientation GetOrientation()
        {
            return myOrientation;
        }
        public int GetIndex(int theIndex)
        {
            Standard_OutOfRange_Raise_if(
              theIndex < 0 || theIndex >= myIndices.Count,
              "BRepMeshData_PCurve::GetIndex");
            return myIndices[theIndex];
        }

        //! Returns discrete face pcurve is associated to.
        public IMeshData_Face GetFace()
        {
            return myDFace;
        }
        public void InsertPoint(int thePosition, gp_Pnt2d thePoint, double theParamOnPCurve)
        {
            myPoints2d.Insert(thePosition, thePoint);
            myParameters.Insert(thePosition, theParamOnPCurve);
            myIndices.Insert(thePosition, 0);
        }

        //! Adds new discretization point to pcurve.
        public void AddPoint(gp_Pnt2d thePoint, double theParamOnPCurve)
        {
            myPoints2d.Add(thePoint);
            myParameters.Add(theParamOnPCurve);
            myIndices.Add(0);
        }

        public gp_Pnt2d GetPoint(int theIndex)
        {
            Standard_OutOfRange_Raise_if(
    theIndex < 0 || theIndex >= myPoints2d.Count, "BRepMeshData_PCurve::GetPoint");
            return myPoints2d[theIndex];
        }

        _IMeshData.Model.SequenceOfPnt2d myPoints2d = new();

        private void Standard_OutOfRange_Raise_if(bool v1, string v2)
        {
            if (v1)
                throw new Exception(v2);
        }

        _IMeshData.Model.SequenceOfReal myParameters = new();
        _IMeshData.Model.SequenceOfInteger myIndices = new();

        public BRepMeshData_PCurve(IMeshData_Face theDFace, TopAbs_Orientation theOrientation) : base(theDFace, theOrientation)
        {
        }

        public int ParametersNb()
        {
            return myParameters.Count;
        }

        public void SetIndex(int theIndex, int val)
        {
            myIndices[theIndex] = val;
        }

        public double GetParameter(int theIndex)
        {
            Standard_OutOfRange_Raise_if(
   theIndex < 0 || theIndex >= ParametersNb(),
   "BRepMeshData_PCurve::GetParameter");
            return myParameters[theIndex];
        }

        public void Clear(bool isKeepEndPoints)
        {
            if (!isKeepEndPoints)
            {
                myPoints2d.clear();
                myParameters.clear();
                myIndices.clear();
            }
            else if (ParametersNb() > 2)
            {
                myPoints2d.erase(1, (myPoints2d.size() - 1));
                myParameters.erase(1, (myParameters.size() - 1));
                myIndices.erase(1, (myIndices.size() - 1));
            }
        }
    }
    public abstract class AbstractMeshData_PCurve
    {

        public AbstractMeshData_PCurve(IMeshData_Face theDFace, TopAbs_Orientation theOrientation)
        {
            myOrientation = theOrientation;
            myDFace = theDFace;
        }

        protected IMeshData_Face myDFace;
        protected TopAbs_Orientation myOrientation;


    }
}


