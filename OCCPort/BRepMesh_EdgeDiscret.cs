using OCCPort;
using OCCPort.Interfaces;
using System;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace OCCPort
{
    //! Class implements functionality of edge discret tool.
    //! Performs check of the edges for existing Poly_PolygonOnTriangulation.
    //! In case if it fits specified deflection, restores data structure using
    //! it, else clears edges from outdated data.
    public class BRepMesh_EdgeDiscret : IMeshTools_ModelAlgo
    {
        public void process(int theEdgeIndex)
        {
            var aDEdge = myModel.GetEdge(theEdgeIndex);
            try
            {
                //OCC_CATCH_SIGNALS

                //BRepMesh_Deflection.ComputeDeflection(aDEdge, myModel.GetMaxSize(), myParameters);

                IMeshTools_CurveTessellator aEdgeTessellator = null;
                if (!aDEdge.IsFree())
                {
                    // Iterate over pcurves and check deflection on corresponding face.
                    double aMinDeflection = Standard_Real.RealLast();
                    int aMinPCurveIndex = -1;
                    for (int aPCurveIt = 0; aPCurveIt < aDEdge.PCurvesNb(); ++aPCurveIt)
                    {
                        var aPCurve = aDEdge.GetPCurve(aPCurveIt);
                        double aTmpDeflection = checkExistingPolygonAndUpdateStatus(aDEdge, aPCurve);
                        if (aTmpDeflection < aMinDeflection)
                        {
                            // Identify pcurve with the smallest deflection in order to
                            // retrieve polygon that represents the most smooth discretization.
                            aMinDeflection = aTmpDeflection;
                            aMinPCurveIndex = aPCurveIt;
                        }

                        //BRepMesh_ShapeTool.CheckAndUpdateFlags(aDEdge, aPCurve);
                    }

                    if (aMinPCurveIndex != -1)
                    {
                        aDEdge.SetDeflection(aMinDeflection);
                        var aDFace = aDEdge.GetPCurve(aMinPCurveIndex).GetFace();
                        aEdgeTessellator = CreateEdgeTessellationExtractor(aDEdge, aDFace);
                    }
                    else
                    {
                        var aPCurve = aDEdge.GetPCurve(0);
                        var aDFace = aPCurve.GetFace();
                        aEdgeTessellator = CreateEdgeTessellator(
                          aDEdge, aPCurve.GetOrientation(), aDFace, myParameters);
                    }
                }
                else
                {
                    TopLoc_Location aLoc = new TopLoc_Location();
                    Poly_Polygon3D aPoly3D = BRep_Tool.Polygon3D(aDEdge.GetEdge(), ref aLoc);
                    if (aPoly3D != null)
                    {
                        //    if (aPoly3D.HasParameters() &&
                        //        BRepMesh_Deflection::IsConsistent(aPoly3D->Deflection(),
                        //                                           aDEdge->GetDeflection(),
                        //                                           myParameters.AllowQualityDecrease))
                        //    {
                        //        // Edge already has suitable 3d polygon.
                        //        aDEdge->SetStatus(IMeshData_Reused);
                        //        return;
                        //    }
                        //    else
                        //    {
                        //        aDEdge->SetStatus(IMeshData_Outdated);
                        //    }
                    }

                    //aEdgeTessellator = CreateEdgeTessellator(aDEdge, myParameters);
                }

                Tessellate3d(aDEdge, aEdgeTessellator, true);
                if (!aDEdge.IsFree())
                {
                    Tessellate2d(aDEdge, true);
                }
            }
            catch (Standard_Failure ex)
            {
                aDEdge.SetStatus(IMeshData_Status.IMeshData_Failure);
            }
        }
        IMeshTools_CurveTessellator CreateEdgeTessellator(
  IMeshData_Edge theDEdge,
  TopAbs_Orientation theOrientation,
  IMeshData_Face theDFace,
  IMeshTools_Parameters theParameters,
  int theMinPointsNb=2)
        {
            return theDEdge.GetSameParam() ?
              new BRepMesh_CurveTessellator(theDEdge, theParameters, theMinPointsNb) :
              new BRepMesh_CurveTessellator(theDEdge, theOrientation, theDFace, theParameters, theMinPointsNb);
        }

        private void Tessellate2d(IMeshData_Edge theDEdge, bool theUpdateEnds)
        {

            var aCurve = theDEdge.GetCurve();
            //for (int aPCurveIt = 0; aPCurveIt < theDEdge.PCurvesNb(); ++aPCurveIt)
            //{
            //    IPCurveHandle aPCurve = theDEdge.GetPCurve(aPCurveIt);
            //    IFaceHandle aDFace = aPCurve.GetFace();
            //    ICurveArrayAdaptorHandle aCurveArray = new ICurveArrayAdaptorHandle(new IMeshData::ICurveArrayAdaptor(aCurve));
            //    BRepMesh_EdgeParameterProvider<IMeshData::ICurveArrayAdaptorHandle> aProvider(
            //      theDEdge, aPCurve->GetOrientation(), aDFace, aCurveArray);

            //    Adaptor2d_Curve2d aGeomPCurve = aProvider.GetPCurve();

            //    int aParamIdx, aParamNb;
            //    if (theUpdateEnds)
            //    {
            //        aParamIdx = 0;
            //        aParamNb = aCurve.ParametersNb();
            //    }
            //    else
            //    {
            //        aParamIdx = 1;
            //        aParamNb = aCurve.ParametersNb() - 1;
            //    }

            //    for (; aParamIdx < aParamNb; ++aParamIdx)
            //    {
            //        double aParam = aProvider.Parameter(aParamIdx, aCurve.GetPoint(aParamIdx));

            //        gp_Pnt2d aPoint2d;
            //        aGeomPCurve.D0(aParam, out aPoint2d);
            //        if (theUpdateEnds)
            //        {
            //            aPCurve.AddPoint(aPoint2d, aParam);
            //        }
            //        else
            //        {
            //            aPCurve.InsertPoint(aPCurve.ParametersNb() - 1, aPoint2d, aParam);
            //        }
            //    }
            //}
        }



        public IMeshTools_CurveTessellator CreateEdgeTessellationExtractor(
          IMeshData_Edge theDEdge,
          IMeshData_Face theDFace)
        {
            return new BRepMesh_EdgeTessellationExtractor(theDEdge, theDFace);
        }

        public void Tessellate3d(
  IMeshData_Edge theDEdge,
  IMeshTools_CurveTessellator theTessellator,
  bool theUpdateEnds)
        {
            // Create 3d polygon.
            var aCurve = theDEdge.GetCurve();

            TopoDS_Edge aEdge = theDEdge.GetEdge();
            TopoDS_Vertex aFirstVertex=new TopoDS_Vertex (), aLastVertex=new TopoDS_Vertex ();
            TopExp.Vertices(aEdge, ref aFirstVertex, ref aLastVertex);

            if (aFirstVertex.IsNull() || aLastVertex.IsNull())
                return;

            if (theUpdateEnds)
            {
                gp_Pnt aPoint;
                double aParam;
                //  theTessellator.Value(1, out aPoint, out aParam);
                //aCurve.AddPoint(BRep_Tool.Pnt(aFirstVertex), aParam);
            }

            //if (!theDEdge.GetDegenerated())
            //{
            //    for (int i = 2; i < theTessellator.PointsNb(); ++i)
            //    {
            //        gp_Pnt aPoint;
            //        double aParam;
            //        if (!theTessellator.Value(i, aPoint, aParam))
            //            continue;

            //        if (theUpdateEnds)
            //        {
            //            aCurve->AddPoint(aPoint, aParam);
            //        }
            //        else
            //        {
            //            aCurve->InsertPoint(aCurve->ParametersNb() - 1, aPoint, aParam);
            //        }
            //    }
            //}

            //if (theUpdateEnds)
            //{
            //    gp_Pnt aPoint;
            //    double aParam;
            //    theTessellator.Value(theTessellator.PointsNb(), out aPoint, out aParam);
            //    aCurve.AddPoint(BRep_Tool::Pnt(aLastVertex), aParam);
            //}
        }

        public override bool performInternal(IMeshData_Model theModel, IMeshTools_Parameters theParameters, Message_ProgressRange theRange)
        {
            //(void)theRange;??
            myModel = theModel;
            myParameters = theParameters;

            if (myModel == null)
            {
                return false;
            }

            //OSD_Parallel::For(0, myModel->EdgesNb(), *this, !myParameters.InParallel);
            /*Parallel.For(0, myModel.EdgesNb(), (z) =>
            {
                process(z);
                //??
            });*/
            for (int i = 0; i < myModel.EdgesNb(); i++)
            {
                process(i);
            }

            myModel = null; // Do not hold link to model.
            return true;
        }

        IMeshData_Model myModel;
        IMeshTools_Parameters myParameters;
        public double checkExistingPolygonAndUpdateStatus(
  IMeshData_Edge theDEdge,
  IMeshData_PCurve thePCurve)
        {
            TopoDS_Edge aEdge = theDEdge.GetEdge();
            TopoDS_Face aFace = thePCurve.GetFace().GetFace();

            TopLoc_Location aLoc = new TopLoc_Location();
            Poly_Triangulation aFaceTriangulation = BRep_Tool.Triangulation(aFace, ref aLoc);

            double aDeflection = Standard_Real.RealLast();
            if (aFaceTriangulation == null)
            {
                return aDeflection;
            }

            Poly_PolygonOnTriangulation aPolygon =
              BRep_Tool.PolygonOnTriangulation(aEdge, aFaceTriangulation, aLoc);

            if (aPolygon != null)
            {
                bool isConsistent = aPolygon.HasParameters() &&
                                  BRepMesh_Deflection.IsConsistent(aPolygon.Deflection(),
                                                                     theDEdge.GetDeflection(),
                                                                     myParameters.AllowQualityDecrease);

                if (!isConsistent)
                {
                    // Nullify edge data and mark discrete pcurve to 
                    // notify necessity to mesh the entire face.
                    theDEdge.SetStatus(IMeshData_Status.IMeshData_Outdated);
                }
                else
                {
                    aDeflection = aPolygon.Deflection();
                }
            }

            return aDeflection;
        }

    }

}