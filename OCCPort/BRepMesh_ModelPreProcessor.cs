using OCCPort.Interfaces;
using System.Reflection.Metadata;
using System.Security.Cryptography;

namespace OCCPort
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
}