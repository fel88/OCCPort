using OCCPort.Interfaces;

namespace OCCPort
{
    public class BRepMeshData_Edge : AbstractMeshData_TessellatedShape, IMeshData_Edge
    {
        DMapOfIFacePtrsListOfInteger myPCurvesMap;
        VectorOfIPCurveHandles myPCurves;
        bool mySameParam;
        //! Gets value of angular deflection for the discrete model.
        public double GetAngularDeflection()
        {
            return myAngDeflection;
        }
        double myAngDeflection;

        //! Returns true in case if the edge is free one, i.e. it does not have pcurves.
        public bool IsFree()
        {
            return (PCurvesNb() == 0);
        }
        //! By default equals to flag stored in topological shape.
        public bool GetSameParam()
        {
            return mySameParam;
        }
        public IMeshData_PCurve GetPCurve(
  IFacePtr theDFace,
  TopAbs_Orientation theOrientation)
        {
            ListOfInteger aListOfPCurves = myPCurvesMap.Find(theDFace);
            IMeshData_PCurve aPCurve1 = myPCurves.get(aListOfPCurves.First());
            return (aPCurve1.GetOrientation() == theOrientation) ?
              aPCurve1 :
              myPCurves.get(aListOfPCurves.Last());
        }

        public ICurveHandle GetCurve()
        {
            throw new System.NotImplementedException();
        }
        public IMeshData_PCurve GetPCurve(
  int theIndex)
        {
            return myPCurves[theIndex];
        }


        //! Returns pcurve for the specified discrete face.
        public IMeshData_PCurve GetPCurve(IMeshData_Face theDFace, TopAbs_Orientation theOrientation)
        {
            ListOfInteger aListOfPCurves = myPCurvesMap.Find(theDFace);
            var aPCurve1 = myPCurves.get(aListOfPCurves.First());
            return (aPCurve1.GetOrientation() == theOrientation) ?
    aPCurve1 : myPCurves.get(aListOfPCurves.Last());
        }

        public int PCurvesNb()
        {
            return myPCurves.Size();
        }

        public IMeshData_PCurve AddPCurve(IMeshData_Face theDFace, TopAbs_Orientation theOrientation)
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


        public BRepMeshData_Edge(TopoDS_Edge theEdge) : base(theEdge)
        {
            myPCurves = new VectorOfIPCurveHandles(256);
            myPCurvesMap = new DMapOfIFacePtrsListOfInteger(1);

        }
    }
}