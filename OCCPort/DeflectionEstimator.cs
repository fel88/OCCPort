using OCCPort.Interfaces;

namespace OCCPort
{
    //! Estimates and updates deflection of triangulations for corresponding faces.
    public class DeflectionEstimator
    {
        IMeshData_Model myModel;
        Poly_TriangulationParameters myParams;
        public DeflectionEstimator(IMeshData_Model theModel, IMeshTools_Parameters theParams)
        {
            myModel = theModel;
            myParams = (new Poly_TriangulationParameters(
          theParams.Deflection, theParams.Angle, theParams.MinSize));
        }

        internal void Run(int theFaceIndex)
        {
            var aDFace = myModel.GetFace(theFaceIndex);
            if (aDFace.IsSet(IMeshData_Status.IMeshData_Failure) ||
                aDFace.IsSet(IMeshData_Status.IMeshData_Reused))
            {
                return;
            }

            BRepLib.UpdateDeflection(aDFace.GetFace());

            TopLoc_Location aLoc = new TopLoc_Location();
            Poly_Triangulation aTriangulation =
              BRep_Tool.Triangulation(aDFace.GetFace(), ref aLoc);

            if (aTriangulation != null)
            {
                aTriangulation.Parameters(myParams);
            }
        }
    }
}