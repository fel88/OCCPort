using OCCPort;
using TKBRep;
using TKernel;
using TKMath;

namespace TKMesh
{
    //! Class implements functionality of model post-processing tool.
    //! Stores polygons on triangulations to TopoDS_Edge.
    public class BRepMesh_ModelPostProcessor : IMeshTools_ModelAlgo
    {
        public override bool performInternal(IMeshData_Model theModel, IMeshTools_Parameters theParameters, Message_ProgressRange theRange)
        {
            if (theModel == null)
            {
                return false;
            }

            // TODO: Force single threaded solution due to data races on edges sharing the same TShape
            //OSD_Parallel::For(0, theModel->EdgesNb(), PolygonCommitter(theModel), Standard_True/*!theParameters.InParallel*/);
            for (int i = 0; i < theModel.EdgesNb(); i++)
            {
                new PolygonCommitter(theModel).Run(i);
            }
            // Estimate deflection here due to BRepLib::EstimateDeflection requires
            // existence of both Poly_Triangulation and Poly_PolygonOnTriangulation.
            //OSD_Parallel::For(0, theModel->FacesNb(), DeflectionEstimator(theModel, theParameters), !theParameters.InParallel);
            for (int i = 0; i < theModel.FacesNb(); i++)
            {
                new DeflectionEstimator(theModel, theParameters).Run(i);

            }
            return true;
        }
    }

    //! Commits 3D polygons and polygons on triangulations for corresponding edges.
    public class PolygonCommitter
    {
        public IMeshData_Model myModel;
        public PolygonCommitter(IMeshData_Model myModel)
        {
            this.myModel = myModel;
        }

        internal void Run(int theEdgeIndex) //main functor
        {
            var aDEdge = myModel.GetEdge(theEdgeIndex);
            if (aDEdge.GetCurve().ParametersNb() == 0)
                return;

            if (aDEdge.IsFree())
            {
                if (!aDEdge.IsSet(IMeshData_Status.IMeshData_Reused))
                {
                    commitPolygon3D(aDEdge);
                }
            }
            else
            {
                commitPolygons(aDEdge);
            }
        }

        //! Commits 3d polygon to topological edge
        void commitPolygon3D(IMeshData_Edge theDEdge)
        {
            var aCurve = theDEdge.GetCurve();

            TColgp_Array1OfPnt aNodes = new TColgp_Array1OfPnt(1, aCurve.ParametersNb());
            TColStd_Array1OfReal aUVNodes = new TColStd_Array1OfReal(1, aCurve.ParametersNb());
            for (int i = 1; i <= aCurve.ParametersNb(); ++i)
            {
                aNodes[i] = aCurve.GetPoint(i - 1);
                aUVNodes[i] = aCurve.GetParameter(i - 1);
            }

            Poly_Polygon3D aPoly3D = new Poly_Polygon3D(aNodes, aUVNodes);
            aPoly3D.Deflection(theDEdge.GetDeflection());

            BRepMesh_ShapeTool.UpdateEdge(theDEdge.GetEdge(), aPoly3D);
        }

        //! Commits all polygons on triangulations correspondent to the given edge.
        void commitPolygons(IMeshData_Edge theDEdge)
        {
            // Collect pcurves associated with the given edge on the specific surface.
            IDMapOfIFacePtrsListOfIPCurves aMapOfPCurves = new IDMapOfIFacePtrsListOfIPCurves();
            for (int aPCurveIt = 0; aPCurveIt < theDEdge.PCurvesNb(); ++aPCurveIt)
            {
                var aPCurve = theDEdge.GetPCurve(aPCurveIt);
                var aDFacePtr = aPCurve.GetFace();
                var aDFace = aDFacePtr;
                if (aDFace.IsSet(IMeshData_Status.IMeshData_Failure) ||
                    aDFace.IsSet(IMeshData_Status.IMeshData_Reused))
                {
                    continue;
                }

                if (!aMapOfPCurves.Contains(aDFacePtr))
                {
                    aMapOfPCurves.Add(aDFacePtr, new ListOfIPCurves());
                }

                ListOfIPCurves aPCurves = aMapOfPCurves.ChangeFromKey(aDFacePtr);
                aPCurves.Append(aPCurve);
            }

            // Commit polygons related to separate face.
            TopoDS_Edge aEdge = theDEdge.GetEdge();
            IDMapOfIFacePtrsListOfIPCurves.Iterator aPolygonIt = new IDMapOfIFacePtrsListOfIPCurves.Iterator(aMapOfPCurves);
            for (; aPolygonIt.More(); aPolygonIt.Next())
            {
                TopoDS_Face aFace = aPolygonIt.Key().GetFace();

                TopLoc_Location aLoc = new TopLoc_Location();
                Poly_Triangulation aTriangulation =
                  BRep_Tool.Triangulation(aFace, ref aLoc);

                if (aTriangulation != null)
                {
                    ListOfIPCurves aPCurves = aPolygonIt.Value();
                    if (aPCurves.Size() == 2)
                    {
                        BRepMesh_ShapeTool.UpdateEdge(
                          aEdge,
                          collectPolygon(aPCurves.First(), theDEdge.GetDeflection()),
                          collectPolygon(aPCurves.Last(), theDEdge.GetDeflection()),
                          aTriangulation, ref aLoc);
                    }
                    else
                    {
                        BRepMesh_ShapeTool.UpdateEdge(
                          aEdge,
                          collectPolygon(aPCurves.First(), theDEdge.GetDeflection()),
                          aTriangulation, ref aLoc);
                    }
                }
            }
        }
        //! Collects polygonal data for the given pcurve
        Poly_PolygonOnTriangulation collectPolygon(
      IMeshData_PCurve thePCurve,
      double theDeflection)
        {
            TColStd_Array1OfInteger aNodes = new TColStd_Array1OfInteger(1, thePCurve.ParametersNb());
            TColStd_Array1OfReal aParams = new TColStd_Array1OfReal(1, thePCurve.ParametersNb());
            for (int i = 1; i <= thePCurve.ParametersNb(); ++i)
            {
                aNodes[i] = thePCurve.GetIndex(i - 1);
                aParams[i] = thePCurve.GetParameter(i - 1);
            }

            Poly_PolygonOnTriangulation aPolygon =
              new Poly_PolygonOnTriangulation(aNodes, aParams);

            aPolygon.Deflection(theDeflection);
            return aPolygon;
        }
    }

    public class IDMapOfIFacePtrsListOfIPCurves : NCollection_IndexedDataMap<IFacePtr, ListOfIPCurves, WeakEqual<IMeshData_Face>>
    {

    }
    public class ListOfIPCurves : NCollection_List<IMeshData_PCurve>
    {
        public ListOfIPCurves()
        {
        }
    }
}

