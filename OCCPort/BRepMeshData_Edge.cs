using OCCPort.Interfaces;

namespace OCCPort
{
    public class BRepMeshData_Edge : AbstractMeshData_TessellatedShape, IMeshData_Edge
    {
        DMapOfIFacePtrsListOfInteger myPCurvesMap;
        VectorOfIPCurveHandles myPCurves;

        public IPCurveHandle GetPCurve(
  IFacePtr theDFace,
  TopAbs_Orientation theOrientation)
        {
            ListOfInteger aListOfPCurves = myPCurvesMap.Find(theDFace);
            IPCurveHandle aPCurve1 = myPCurves.get(aListOfPCurves.First());
            return (aPCurve1.GetOrientation() == theOrientation) ?
              aPCurve1 :
              myPCurves.get(aListOfPCurves.Last());
        }

        public ICurveHandle GetCurve()
        {
            throw new System.NotImplementedException();
        }

        //! Returns pcurve for the specified discrete face.
        public IPCurveHandle GetPCurve(IMeshData_Face theDFace, TopAbs_Orientation theOrientation)
        {
            ListOfInteger aListOfPCurves = myPCurvesMap.Find(theDFace);
            IPCurveHandle aPCurve1 = myPCurves.get(aListOfPCurves.First());
            return (aPCurve1.GetOrientation() == theOrientation) ?
    aPCurve1 : myPCurves.get(aListOfPCurves.Last());
        }

        public BRepMeshData_Edge(TopoDS_Edge theEdge) : base(theEdge)
        {
        }
    }
}