using OCCPort.Interfaces;
using System;
using System.Collections.Generic;

namespace OCCPort
{

    //! Default implementation of curve data model entity.
    internal class BRepMeshData_PCurve : AbstractMeshData_PCurve, IMeshData_PCurve
    {
        //! Returns orientation of the edge associated with current pcurve.

        public TopAbs_Orientation GetOrientation()
        {
            return myOrientation;
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

        List<gp_Pnt2d> myPoints2d = new List<gp_Pnt2d>();

        private void Standard_OutOfRange_Raise_if(bool v1, string v2)
        {
            if (v1)
                throw new Exception(v2);
        }

        List<double> myParameters = new List<double>();
        List<int> myIndices = new List<int>();

        public BRepMeshData_PCurve(IMeshData_Face theDFace, TopAbs_Orientation theOrientation) : base(theDFace, theOrientation)
        {
        }

        public int ParametersNb()
        {
            return myParameters.Count;
        }


    }
}