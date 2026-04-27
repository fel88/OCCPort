using OCCPort;
using OCCPort.Interfaces;
using System;
using System.Security.Cryptography;

namespace OCCPort
{
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
}