using TKBRep;
using TKernel;
using TKG3d;
using TKMath;

namespace TKMesh
{
    //! Class implements functionality of model pre-processing tool.
    //! Nullifies existing polygonal data in case if model elements
    //! have IMeshData_Outdated status.
    public class BRepMesh_ModelPreProcessor : IMeshTools_ModelAlgo
    {
        public override bool performInternal(IMeshData_Model theModel, IMeshTools_Parameters theParameters, Message_ProgressRange theRange)
        {

            if (theModel == null)
            {
                return false;
            }

            int aFacesNb = theModel.FacesNb();
            bool isOneThread = !theParameters.InParallel;
            SeamEdgeAmplifier seam = new SeamEdgeAmplifier(theModel, theParameters);
            //OSD_Parallel::For(0, aFacesNb, SeamEdgeAmplifier(theModel, theParameters), isOneThread);
            //OSD_Parallel::For(0, aFacesNb, TriangulationConsistency(theModel, theParameters.AllowQualityDecrease), isOneThread);
            for (int i = 0; i < aFacesNb; i++)
            {
                seam.func(i);
            }

            // Clean edges and faces from outdated polygons.

            NCollection_Map<IMeshData_Face> aUsedFaces = new NCollection_Map<IMeshData_Face>(1);
            for (int aEdgeIt = 0; aEdgeIt < theModel.EdgesNb(); ++aEdgeIt)
            {
                var aDEdge = theModel.GetEdge(aEdgeIt);
                if (aDEdge.IsFree())
                {
                    if (aDEdge.IsSet(IMeshData_Status.IMeshData_Outdated))
                    {
                        TopLoc_Location aLoc = new TopLoc_Location();
                        BRep_Tool.Polygon3D(aDEdge.GetEdge(), ref aLoc);
                        BRepMesh_ShapeTool.NullifyEdge(aDEdge.GetEdge(), ref aLoc);
                    }

                    continue;
                }

                for (int aPCurveIt = 0; aPCurveIt < aDEdge.PCurvesNb(); ++aPCurveIt)
                {
                    // Find adjacent outdated face.
                    var aDFace = aDEdge.GetPCurve(aPCurveIt).GetFace();
                    if (!aUsedFaces.Contains(aDFace))
                    {
                        aUsedFaces.Add(aDFace);
                        if (aDFace.IsSet(IMeshData_Status.IMeshData_Outdated))
                        {
                            TopLoc_Location aLoc = new TopLoc_Location();
                            var aTriangulation =
                              BRep_Tool.Triangulation(aDFace.GetFace(), ref aLoc);

                            // Clean all edges of oudated face.
                            for (int aWireIt = 0; aWireIt < aDFace.WiresNb(); ++aWireIt)
                            {
                                var aDWire = aDFace.GetWire(aWireIt);
                                for (int aWireEdgeIt = 0; aWireEdgeIt < aDWire.EdgesNb(); ++aWireEdgeIt)
                                {
                                    var aTmpDEdge = aDWire.GetEdge(aWireEdgeIt);
                                    BRepMesh_ShapeTool.NullifyEdge(aTmpDEdge.GetEdge(), aTriangulation, ref aLoc);
                                }
                            }

                            BRepMesh_ShapeTool.NullifyFace(aDFace.GetFace());
                        }
                    }
                }
            }


            return true;
        }

    }

    //! Adds additional points to seam edges on specific surfaces.
    public class SeamEdgeAmplifier
    {
        //! Constructor
        public SeamEdgeAmplifier(IMeshData_Model theModel,
                      IMeshTools_Parameters theParameters)


        {
            myModel = (theModel);
            myParameters = (theParameters);
        }

        //! Returns step for splitting seam edge of a cone.
        public double getConeStep(IMeshData_Face theDFace)
        {
            BRepMesh_ConeRangeSplitter aSplitter = new BRepMesh_ConeRangeSplitter();
            aSplitter.Reset(theDFace, myParameters);

            var aDWire = theDFace.GetWire(0);
            for (int aEdgeIt = 0; aEdgeIt < aDWire.EdgesNb(); ++aEdgeIt)
            {
                var aDEdge = aDWire.GetEdge(aEdgeIt);
                var aPCurve = aDEdge.GetPCurve(
                  theDFace, aDWire.GetEdgeOrientation(aEdgeIt));

                for (int aPointIt = 0; aPointIt < aPCurve.ParametersNb(); ++aPointIt)
                {
                    gp_Pnt2d aPnt2d = aPCurve.GetPoint(aPointIt);
                    aSplitter.AddPoint(aPnt2d);
                }
            }

            (int, int) aStepsNb;
            (double, double) aSteps = aSplitter.GetSplitSteps(myParameters, out aStepsNb);
            return aSteps.Item2;
        }

        IMeshData_Model myModel;
        IMeshTools_Parameters myParameters;
        //! Main functor.
        public void func(int theFaceIndex)
        {
            var aDFace = myModel.GetFace(theFaceIndex);
            if (aDFace.GetSurface()._GetType() != GeomAbs_SurfaceType.GeomAbs_Cone || aDFace.IsSet(IMeshData_Status.IMeshData_Failure))
            {
                return;
            }

            var aDWire = aDFace.GetWire(0);
            for (int aEdgeIdx = 0; aEdgeIdx < aDWire.EdgesNb() - 1; ++aEdgeIdx)
            {
                var aDEdge = aDWire.GetEdge(aEdgeIdx);

                if (aDEdge.GetPCurve(aDFace, TopAbs_Orientation.TopAbs_FORWARD) != aDEdge.GetPCurve(aDFace, TopAbs_Orientation.TopAbs_REVERSED))
                {
                    if (aDEdge.GetCurve().ParametersNb() == 2)
                    {
                        if (splitEdge(aDEdge, aDFace, Math.Abs(getConeStep(aDFace))))
                        {
                            TopLoc_Location aLoc = new TopLoc_Location();
                            var aTriangulation =
                                    BRep_Tool.Triangulation(aDFace.GetFace(), ref aLoc);

                            if (aTriangulation != null)
                            {
                                aDFace.SetStatus(IMeshData_Status.IMeshData_Outdated);
                            }
                        }
                    }
                    return;
                }
            }
        }

        private bool splitEdge(IMeshData_Edge aDEdge, IMeshData_Face aDFace, double v)
        {
            throw new NotImplementedException();
        }
    }

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

