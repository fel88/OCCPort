using OCCPort.Interfaces;
using System;
using System.Collections.Generic;

namespace OCCPort
{

    //! Default implementation of curve data model entity.
    internal class BRepMeshData_PCurve : AbstractMeshData_PCurve, IMeshData_PCurve
    {
        public TopAbs_Orientation GetOrientation()
        {
            throw new System.NotImplementedException();
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

        public BRepMeshData_PCurve(IMeshData_Face theDFace, TopAbs_Orientation theOrientation):base(theDFace,theOrientation)
        { 
        }

        public int ParametersNb()
        {
            return myParameters.Count;
        }
    }
}