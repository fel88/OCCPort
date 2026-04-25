using OCCPort.Interfaces;
using System.Linq;

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

        public int PCurvesNb()
        {
            return myPCurves.Count;
        }

        public IPCurveHandle AddPCurve(IMeshData_Face theDFace, TopAbs_Orientation theOrientation)
        {

            int aPCurveIndex = PCurvesNb();
            // Add pcurve to list of pcurves
            IMeshData_PCurve aPCurve = new BRepMeshData_PCurve(theDFace, theOrientation);
            myPCurves.Append(aPCurve);

            // Map pcurve to faces.
            if (!myPCurvesMap.IsBound(theDFace))
            {
                myPCurvesMap.Bind(theDFace, new ListOfInteger());

            }

            ListOfInteger aListOfPCurves = myPCurvesMap.ChangeFind(theDFace);
            aListOfPCurves.Append(aPCurveIndex);

            return GetPCurve(aPCurveIndex);

        }

        public IPCurveHandle GetPCurve(int theIndex)
        {
            return myPCurves[theIndex];
        }

        public BRepMeshData_Edge(TopoDS_Edge theEdge) : base(theEdge)
        {
            myPCurves = new VectorOfIPCurveHandles(256);
            myPCurvesMap = new DMapOfIFacePtrsListOfInteger(1);

        }
    }
}