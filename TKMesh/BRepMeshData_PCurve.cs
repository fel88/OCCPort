using TKG3d;
using TKMath;

namespace TKMesh
{
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
}


