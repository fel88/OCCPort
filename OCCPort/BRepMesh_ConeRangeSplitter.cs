using OCCPort.Interfaces;
using System;

namespace OCCPort
{
    //! Auxiliary class extending default range splitter in
    //! order to generate internal nodes for conical surface.
    public class BRepMesh_ConeRangeSplitter : BRepMesh_DefaultRangeSplitter
    {
        internal void AddPoint(gp_Pnt2d aPnt2d)
        {
            throw new NotImplementedException();
        }

        internal (double, double) GetSplitSteps(IMeshTools_Parameters myParameters, out (int, int) aStepsNb)
        {
            throw new NotImplementedException();
        }

        internal void Reset(IMeshData_Face theDFace, IMeshTools_Parameters myParameters)
        {
            throw new NotImplementedException();
        }
    }
}