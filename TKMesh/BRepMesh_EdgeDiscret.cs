global using TColStd_Array1OfInteger = TKernel.NCollection_Array1<int>;

using OCCPort;
using OCCPort.Common;
using TKBRep;
using TKernel;
using TKG2d;
using TKG3d;
using TKGeomBase;
using TKMath;

namespace TKMesh
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

                BRepMesh_Deflection.ComputeDeflection(aDEdge, myModel.GetMaxSize(), myParameters);

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

                        BRepMesh_ShapeTool.CheckAndUpdateFlags(aDEdge, aPCurve);
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
  int theMinPointsNb = 2)
        {
            return theDEdge.GetSameParam() ?
              new BRepMesh_CurveTessellator(theDEdge, theParameters, theMinPointsNb) :
              new BRepMesh_CurveTessellator(theDEdge, theOrientation, theDFace, theParameters, theMinPointsNb);
        }

        private void Tessellate2d(IMeshData_Edge theDEdge, bool theUpdateEnds)
        {

            var aCurve = theDEdge.GetCurve();
            for (int aPCurveIt = 0; aPCurveIt < theDEdge.PCurvesNb(); ++aPCurveIt)
            {
                var aPCurve = theDEdge.GetPCurve(aPCurveIt);
                var aDFace = aPCurve.GetFace();
                ICurveArrayAdaptor aCurveArray = new ICurveArrayAdaptor(new ICurveArrayAdaptor(aCurve));
                BRepMesh_EdgeParameterProvider aProvider = new BRepMesh_EdgeParameterProvider(
                  theDEdge, aPCurve.GetOrientation(), aDFace, aCurveArray);

                Adaptor2d_Curve2d aGeomPCurve = aProvider.GetPCurve();

                int aParamIdx, aParamNb;
                if (theUpdateEnds)
                {
                    aParamIdx = 0;
                    aParamNb = aCurve.ParametersNb();
                }
                else
                {
                    aParamIdx = 1;
                    aParamNb = aCurve.ParametersNb() - 1;
                }

                for (; aParamIdx < aParamNb; ++aParamIdx)
                {
                    double aParam = aProvider.Parameter(aParamIdx, aCurve.GetPoint(aParamIdx));

                    gp_Pnt2d aPoint2d = new gp_Pnt2d();
                    aGeomPCurve.D0(aParam, ref aPoint2d);
                    if (theUpdateEnds)
                    {
                        aPCurve.AddPoint(aPoint2d, aParam);
                    }
                    else
                    {
                        aPCurve.InsertPoint(aPCurve.ParametersNb() - 1, aPoint2d, aParam);
                    }
                }
            }
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
            TopoDS_Vertex aFirstVertex = new TopoDS_Vertex(), aLastVertex = new TopoDS_Vertex();
            TopExp.Vertices(aEdge, ref aFirstVertex, ref aLastVertex);

            if (aFirstVertex.IsNull() || aLastVertex.IsNull())
                return;

            if (theUpdateEnds)
            {
                gp_Pnt aPoint;
                double aParam;
                theTessellator.Value(1, out aPoint, out aParam);
                aCurve.AddPoint(BRep_Tool.Pnt(aFirstVertex), aParam);
            }

            if (!theDEdge.GetDegenerated())
            {
                for (int i = 2; i < theTessellator.PointsNb(); ++i)
                {
                    gp_Pnt aPoint;
                    double aParam;
                    if (!theTessellator.Value(i, out aPoint, out aParam))
                        continue;

                    if (theUpdateEnds)
                    {
                        aCurve.AddPoint(aPoint, aParam);
                    }
                    else
                    {
                        aCurve.InsertPoint(aCurve.ParametersNb() - 1, aPoint, aParam);
                    }
                }
            }

            if (theUpdateEnds)
            {
                gp_Pnt aPoint;
                double aParam;
                theTessellator.Value(theTessellator.PointsNb(), out aPoint, out aParam);
                aCurve.AddPoint(BRep_Tool.Pnt(aLastVertex), aParam);
            }
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

    //! Auxiliary class implements functionality retrieving tessellated
    //! representation of an edge stored in polygon.
    public class BRepMesh_EdgeTessellationExtractor : IMeshTools_CurveTessellator
    {
        public BRepMesh_EdgeTessellationExtractor(IMeshData_Edge theEdge, IMeshData_Face theFace)
        {
            Poly_Triangulation aTriangulation =
    BRep_Tool.Triangulation(theFace.GetFace(), ref myLoc);

            Poly_PolygonOnTriangulation aPolygon =
              BRep_Tool.PolygonOnTriangulation(theEdge.GetEdge(), aTriangulation, myLoc);

            myTriangulation = aTriangulation;
            myIndices = aPolygon.Nodes();
            myProvider.Init(theEdge, TopAbs_Orientation.TopAbs_FORWARD, theFace, aPolygon.Parameters());
        }

        BRepMesh_EdgeParameterProvider myProvider;

        public bool Value(int theIndex, out gp_Pnt thePoint, out double theParameter)
        {
            gp_Pnt aRefPnt = myTriangulation.Node(myIndices.Value(theIndex));
            thePoint = BRepMesh_ShapeTool.UseLocation(aRefPnt, myLoc);

            theParameter = myProvider.Parameter(theIndex, thePoint);
            return true;
        }

        TColStd_Array1OfInteger myIndices;
        TopLoc_Location myLoc;
        Poly_Triangulation myTriangulation;

        public int PointsNb()
        {
            return myIndices.Length();
        }
    }
    //! Auxiliary class provides correct parameters 
    //! on curve regarding SameParameter flag.

    public class BRepMesh_EdgeParameterProvider
    {
        //! Constructor. Initializes empty provider.
        public BRepMesh_EdgeParameterProvider()
        {
            myIsSameParam = (false);
            myFirstParam = (0.0);
            myOldFirstParam = (0.0);
            myScale = (0.0);
            myCurParam = (0.0);
            myFoundParam = 0.0;
        }

        //! Returns pcurve used to compute parameters.
        public Adaptor2d_Curve2d GetPCurve()
        {
            return myCurveAdaptor.CurveOnSurface().GetCurve();
        }

        //! Returns parameter according to SameParameter flag of the edge.
        //! If SameParameter is TRUE returns value from parameters w/o changes,
        //! elsewhere scales initial parameter and tries to determine resulting
        //! value using projection of the corresponded 3D point on PCurve.
        public double Parameter(int theIndex,
                          gp_Pnt thePoint3d)
        {
            if (myIsSameParam)
            {
                return myParameters.Value(theIndex);
            }

            // Use scaled
            double aParam = myParameters.Value(theIndex);

            double aPrevParam = myCurParam;
            myCurParam = myFirstParam + myScale * (aParam - myOldFirstParam);

            double aPrevFoundParam = myFoundParam;
            myFoundParam += (myCurParam - aPrevParam);

            myProjector.Perform(thePoint3d, myFoundParam);
            if (myProjector.IsDone())
            {
                double aFoundParam = myProjector.Point().Parameter();
                if ((aPrevFoundParam < myFoundParam && aPrevFoundParam < aFoundParam) ||
                    (aPrevFoundParam > myFoundParam && aPrevFoundParam > aFoundParam))
                {
                    // Rude protection against case when amplified parameter goes before 
                    // previous one due to period or other reason occurred in projector.
                    // Using parameter returned by projector as is can produce self-intersections.
                    myFoundParam = aFoundParam;
                }
            }

            return myFoundParam;
        }
        IParametersCollection myParameters;

        //! Constructor.
        //! @param theEdge edge which parameters should be processed.
        //! @param theFace face the parametric values are defined for.
        //! @param theParameters parameters corresponded to discretization points.
        public BRepMesh_EdgeParameterProvider(
    IMeshData_Edge theEdge,
    TopAbs_Orientation theOrientation,
IMeshData_Face theFace,
    IParametersCollection theParameters)
        {
            Init(theEdge, theOrientation, theFace, theParameters);
        }

        //! Initialized provider by the given data.
        public void Init(
        IMeshData_Edge theEdge,
        TopAbs_Orientation theOrientation,
        IMeshData_Face theFace,
        IParametersCollection theParameters)
        {
            myParameters = theParameters;
            myIsSameParam = theEdge.GetSameParam();
            myScale = 1.0;

            // Extract actual parametric values
            TopoDS_Edge aEdge = TopoDS.Edge(theEdge.GetEdge().Oriented(theOrientation));

            myCurveAdaptor.Initialize(aEdge, theFace.GetFace());
            if (myIsSameParam)
            {
                return;
            }

            myFirstParam = myCurveAdaptor.FirstParameter();
            double aLastParam = myCurveAdaptor.LastParameter();

            myFoundParam = myCurParam = myFirstParam;

            // Extract parameters stored in polygon
            myOldFirstParam = myParameters.Value(myParameters.Lower());
            double aOldLastParam = myParameters.Value(myParameters.Upper());

            // Calculate scale factor between actual and stored parameters
            if ((myOldFirstParam != myFirstParam || aOldLastParam != aLastParam) &&
                myOldFirstParam != aOldLastParam)
            {
                myScale = (aLastParam - myFirstParam) / (aOldLastParam - myOldFirstParam);
            }

            myProjector.Initialize(myCurveAdaptor, myCurveAdaptor.FirstParameter(),
                                   myCurveAdaptor.LastParameter(), Precision.PConfusion());
        }


        bool myIsSameParam;
        double myFirstParam;

        double myOldFirstParam;
        double myScale;

        double myCurParam;
        double myFoundParam;

        BRepAdaptor_Curve myCurveAdaptor = new BRepAdaptor_Curve();

        Extrema_LocateExtPC myProjector = new Extrema_LocateExtPC();
    }
}


