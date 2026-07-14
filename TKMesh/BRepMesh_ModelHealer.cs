using OCCPort;
using OCCPort.Common;
using System.Security.Cryptography;
using TKBRep;
using TKernel;
using TKMath;

namespace TKMesh
{
    //! Class implements functionality of model healer tool.
    //! Iterates over model's faces and checks consistency of their wires, 
    //! i.e.whether wires are closed and do not contain self - intersections.
    //! In case if wire contains disconnected parts, ends of adjacent edges
    //! forming the gaps are connected in parametric space forcibly. The notion
    //! of this operation is to create correct discrete model defined relatively
    //! parametric space of target face taking into account connectivity and 
    //! tolerances of 3D space only. This means that there are no specific 
    //! computations are made for the sake of determination of U and V tolerance.
    //! Registers intersections on edges forming the face's shape and tries to
    //! amplify discrete representation by decreasing of deflection for the target edge.
    //! Checks can be performed in parallel mode.
    public class BRepMesh_ModelHealer : IMeshTools_ModelAlgo
    {

        IMeshData_Model myModel;
        IMeshTools_Parameters myParameters;
        DMapOfIFacePtrsMapOfIEdgePtrs myFaceIntersectingEdges;

        TopoDS_Vertex getCommonVertex(
   IEdgeHandle theEdge1,
   IEdgeHandle theEdge2)
        {
            TopoDS_Vertex aVertex1_1 = new TopoDS_Vertex(), aVertex1_2 = new TopoDS_Vertex();
            TopExp.Vertices(theEdge1.GetEdge(), ref aVertex1_1, ref aVertex1_2);

            //Test bugs moddata_2 bug428.
            //  restore [locate_data_file OCC428.brep] rr
            //  explode rr f
            //  explode rr_91 w
            //  explode rr_91_2 e
            //  nbshapes rr_91_2_2
            //  # 0 vertices; 1 edge

            //This shape is invalid and can lead to exception in this code.

            if (aVertex1_1.IsNull() || aVertex1_2.IsNull())
                return new TopoDS_Vertex();

            if (theEdge1.GetEdge().IsSame(theEdge2.GetEdge()))
            {
                return aVertex1_1.IsSame(aVertex1_2) ? aVertex1_1 : new TopoDS_Vertex();
            }

            TopoDS_Vertex aVertex2_1 = new TopoDS_Vertex(), aVertex2_2 = new TopoDS_Vertex();
            TopExp.Vertices(theEdge2.GetEdge(), ref aVertex2_1, ref aVertex2_2);

            if (aVertex2_1.IsNull() || aVertex2_2.IsNull())
                return new TopoDS_Vertex();

            if (isSameWithSomeOf(aVertex1_1, aVertex2_1, aVertex2_2))
            {
                return aVertex1_1;
            }
            else if (isSameWithSomeOf(aVertex1_2, aVertex2_1, aVertex2_2))
            {
                return aVertex1_2;
            }

            gp_Pnt aPnt1_1 = BRep_Tool.Pnt(aVertex1_1);
            gp_Pnt aPnt1_2 = BRep_Tool.Pnt(aVertex1_2);
            double aTol1_1 = BRep_Tool.Tolerance(aVertex1_1);
            double aTol1_2 = BRep_Tool.Tolerance(aVertex1_2);

            gp_Pnt aPnt2_1 = BRep_Tool.Pnt(aVertex2_1);
            gp_Pnt aPnt2_2 = BRep_Tool.Pnt(aVertex2_2);
            double aTol2_1 = BRep_Tool.Tolerance(aVertex2_1);
            double aTol2_2 = BRep_Tool.Tolerance(aVertex2_2);

            if (isInToleranceWithSomeOf(aPnt1_1, aPnt2_1, aPnt2_2, aTol1_1 + Math.Max(aTol2_1, aTol2_2)))
            {
                return aVertex1_1;
            }
            else if (isInToleranceWithSomeOf(aPnt1_2, aPnt2_1, aPnt2_2, aTol1_2 + Math.Max(aTol2_1, aTol2_2)))
            {
                return aVertex1_2;
            }

            return new TopoDS_Vertex();
        }

        //! Returns True if some of two vertcies is same with reference one.
        bool isSameWithSomeOf(
    TopoDS_Vertex theRefVertex,
    TopoDS_Vertex theVertex1,
    TopoDS_Vertex theVertex2)
        {
            return (theRefVertex.IsSame(theVertex1) ||
                    theRefVertex.IsSame(theVertex2));
        }

        //! Returns True if some of two vertcies is within tolerance of reference one.
        bool isInToleranceWithSomeOf(
     gp_Pnt theRefPoint,
     gp_Pnt thePoint1,
     gp_Pnt thePoint2,
     double theTol)
        {
            double aSqTol = theTol * theTol;
            return (theRefPoint.SquareDistance(thePoint1) < aSqTol ||
                    theRefPoint.SquareDistance(thePoint2) < aSqTol);
        }

        bool connectClosestPoints(
   IMeshData_PCurve thePrevDEdge,
   IMeshData_PCurve theCurrDEdge,
   IMeshData_PCurve theNextDEdge)
        {
            if (thePrevDEdge.IsInternal() ||
                theCurrDEdge.IsInternal() ||
                theNextDEdge.IsInternal())
            {
                return true;
            }

            gp_Pnt2d aPrevFirstUV = thePrevDEdge.GetPoint(0);
            gp_Pnt2d aPrevLastUV = thePrevDEdge.GetPoint(thePrevDEdge.ParametersNb() - 1);

            if (thePrevDEdge == theCurrDEdge)
            {
                // Wire consists of a single edge.
                aPrevFirstUV = aPrevLastUV;
                return true;
            }

            gp_Pnt2d aCurrFirstUV = theCurrDEdge.GetPoint(0);
            gp_Pnt2d aCurrLastUV = theCurrDEdge.GetPoint(theCurrDEdge.ParametersNb() - 1);

            gp_Pnt2d? aPrevUV = null, aCurrPrevUV = null;
            //wrap to objects here
            double aPrevSqDist = closestPoints(aPrevFirstUV, aPrevLastUV,
                                                            aCurrFirstUV, aCurrLastUV,
                                                           out aPrevUV, out aCurrPrevUV);

            gp_Pnt2d? aNextUV = null, aCurrNextUV = null;
            if (thePrevDEdge == theNextDEdge)
            {
                // Wire consists of two edges. Connect both ends.
                //  aNextUV = (aPrevUV == aPrevFirstUV) ? aPrevLastUV : aPrevFirstUV;
                //aCurrNextUV = (aCurrPrevUV == aCurrFirstUV) ? aCurrLastUV : aCurrFirstUV;

                aNextUV = aCurrNextUV;
                aPrevUV = aCurrPrevUV;
                return true;
            }

            gp_Pnt2d aNextFirstUV = theNextDEdge.GetPoint(0);
            gp_Pnt2d aNextLastUV = theNextDEdge.GetPoint(theNextDEdge.ParametersNb() - 1);

            double aNextSqDist = closestPoints(aNextFirstUV, aNextLastUV,
                                                           aCurrFirstUV, aCurrLastUV,
                                                          out aNextUV, out aCurrNextUV);



            // Connect closest points first. This can help to identify 
            // which ends should be connected in case of gap.
            if (aPrevSqDist - aNextSqDist > gp.Resolution())
            {
                adjustSamePoints(ref aCurrNextUV, aNextUV, ref aCurrPrevUV, ref aPrevUV, aCurrFirstUV, aCurrLastUV, aPrevFirstUV, aPrevLastUV);
            }
            else
            {
                adjustSamePoints(ref aCurrPrevUV, aPrevUV, ref aCurrNextUV, ref aNextUV, aCurrFirstUV, aCurrLastUV, aNextFirstUV, aNextLastUV);
            }

            return true;
        }

        //! Chooses the most closest points among the given to reference one from the given pair.
        //! Returns square distance between reference point and closest one as 
        //! well as pointer to closest point.
        double closestPoints(
          gp_Pnt2d theFirstPnt1,
          gp_Pnt2d theSecondPnt1,
          gp_Pnt2d theFirstPnt2,
          gp_Pnt2d theSecondPnt2,
         out gp_Pnt2d? theClosestPnt1,
     out gp_Pnt2d? theClosestPnt2)
        {
            gp_Pnt2d? aCurrPrevUV1 = null, aCurrPrevUV2 = null;
            double aSqDist1 = closestPoint(theFirstPnt1, theFirstPnt2, theSecondPnt2, out aCurrPrevUV1);
            double aSqDist2 = closestPoint(theSecondPnt1, theFirstPnt2, theSecondPnt2, out aCurrPrevUV2);
            if (aSqDist1 - aSqDist2 < gp.Resolution())
            {
                theClosestPnt1 = theFirstPnt1;
                theClosestPnt2 = aCurrPrevUV1;
                return aSqDist1;
            }

            theClosestPnt1 = theSecondPnt1;
            theClosestPnt2 = aCurrPrevUV2;
            return aSqDist2;
        }
        //! Adjusts the given pair of points supposed to be the same.
        //! In addition, adjusts another end-point of an edge in order
        //! to perform correct matching in case of gap.
        void adjustSamePoints(
                  ref gp_Pnt2d? theMajorSamePnt1,
                  gp_Pnt2d? theMinorSamePnt1,
                 ref gp_Pnt2d? theMajorSamePnt2,
                 ref gp_Pnt2d? theMinorSamePnt2,
                  gp_Pnt2d? theMajorFirstPnt,
                  gp_Pnt2d? theMajorLastPnt,
                  gp_Pnt2d? theMinorFirstPnt,
                  gp_Pnt2d? theMinorLastPnt)
        {
            //if (theMajorSamePnt2 == theMajorSamePnt1)//ref or value???
            //{
            //    theMajorSamePnt2 = (theMajorSamePnt2 == theMajorFirstPnt) ? theMajorLastPnt : theMajorFirstPnt;
            //    closestPoint(theMajorSamePnt2, theMinorFirstPnt, theMinorLastPnt,out theMinorSamePnt2);
            //}

            theMajorSamePnt1 = theMinorSamePnt1;
            theMajorSamePnt2 = theMinorSamePnt2;
        }

        //! Chooses the most closest point to reference one from the given pair.
        //! Returns square distance between reference point and closest one as 
        //! well as pointer to closest point.
        double closestPoint(
          gp_Pnt2d theRefPnt,
          gp_Pnt2d theFristPnt,
          gp_Pnt2d theSecondPnt,
        out gp_Pnt2d? theClosestPnt)
        {
            // Find the most closest end-points.
            double aSqDist1 = theRefPnt.SquareDistance(theFristPnt);
            double aSqDist2 = theRefPnt.SquareDistance(theSecondPnt);
            if (aSqDist1 < aSqDist2)
            {
                theClosestPnt = theFristPnt;
                return aSqDist1;
            }

            theClosestPnt = theSecondPnt;
            return aSqDist2;
        }
        //! Connects ends of pcurves of face's wires according to topological coherency.
        void fixFaceBoundaries(IMeshData_Face theDFace)
        {

            for (int aWireIt = 0; aWireIt < theDFace.WiresNb(); ++aWireIt)
            {
                IWireHandle aDWire = theDFace.GetWire(aWireIt);
                BRepMesh_Deflection.ComputeDeflection(aDWire, myParameters);
                for (int aEdgeIt = 0; aEdgeIt < aDWire.EdgesNb(); ++aEdgeIt)
                {
                    int aPrevEdgeIt = (aEdgeIt + aDWire.EdgesNb() - 1) % aDWire.EdgesNb();
                    int aNextEdgeIt = (aEdgeIt + 1) % aDWire.EdgesNb();

                    IEdgeHandle aPrevEdge = aDWire.GetEdge(aPrevEdgeIt);
                    IEdgeHandle aCurrEdge = aDWire.GetEdge(aEdgeIt);
                    IEdgeHandle aNextEdge = aDWire.GetEdge(aNextEdgeIt);

                    bool isConnected = !getCommonVertex(aCurrEdge, aNextEdge).IsNull() &&
                                                   !getCommonVertex(aPrevEdge, aCurrEdge).IsNull();

                    if (isConnected)
                    {
                        var aPrevPCurve =
                         aPrevEdge.GetPCurve(theDFace, aDWire.GetEdgeOrientation(aPrevEdgeIt));

                        var aCurrPCurve =
                         aCurrEdge.GetPCurve(theDFace, aDWire.GetEdgeOrientation(aEdgeIt));

                        var aNextPCurve =
                         aNextEdge.GetPCurve(theDFace, aDWire.GetEdgeOrientation(aNextEdgeIt));

                        isConnected = connectClosestPoints(aPrevPCurve, aCurrPCurve, aNextPCurve);


                    }

                    if (!isConnected || aCurrEdge.IsSet(IMeshData_Status.IMeshData_Outdated))
                    {
                        // We have to clean face from triangulation.
                        theDFace.SetStatus(IMeshData_Status.IMeshData_Outdated);

                        if (!isConnected)
                        {
                            // Just mark wire as open, but continue fixing other inconsistencies
                            // in hope that this data could be suitable to build mesh somehow.
                            aDWire.SetStatus(IMeshData_Status.IMeshData_OpenWire);
                        }
                    }
                }
            }

            BRepMesh_Deflection.ComputeDeflection(theDFace, myParameters);
        }

        //! Checks existing discretization of the face and updates data model.
        void process(IFaceHandle theDFace)
        {
            try
            {
                //OCC_CATCH_SIGNALS

                var aIntersections = myFaceIntersectingEdges.ChangeFind(theDFace);
                aIntersections=null;

                fixFaceBoundaries(theDFace);

                if (!theDFace.IsSet(IMeshData_Status.IMeshData_Failure))
                {
                    BRepMesh_FaceChecker aChecker = new(theDFace, myParameters);
                    if (!aChecker.Perform())
                    {

                        aIntersections = aChecker.GetIntersectingEdges();
                    }
                    else
                    {
                        if (theDFace.WiresNb() == 1)
                        {
                            IWireHandle aDWire = theDFace.GetWire(0);

                            if (aDWire.EdgesNb() == 2)
                            {
                                var aDEdge0 = aDWire.GetEdge(0);
                                var aDEdge1 = aDWire.GetEdge(1);

                                var aPCurve0 = aDEdge0.GetPCurve(theDFace, aDWire.GetEdgeOrientation(0));
                                var aPCurve1 = aDEdge1.GetPCurve(theDFace, aDWire.GetEdgeOrientation(1));

                                if (aPCurve0.ParametersNb() == 2 && aPCurve1.ParametersNb() == 2)
                                {
                                    //typedef NCollection_Shared<NCollection_Map<IEdgePtr, WeakEqual<IMeshData_Edge> > >                            MapOfIEdgePtr;

                                    aIntersections = new NCollection_Map<IMeshData_Edge, WeakEqual<IMeshData_Edge>>();
                                    // a kind of degenerated face - 1 wire, 2 edges and both edges are very small
                                    aIntersections.Add(aDEdge0);
                                    aIntersections.Add(aDEdge1);
                                }
                            }
                        }
                    }
                }
            }
            catch (Standard_Failure)
            {
                theDFace.SetStatus(IMeshData_Status.IMeshData_Failure);
            }
        }
        //! Checks existing discretization of the face and updates data model.
        void process(int theFaceIndex)
        {
            IFaceHandle aDFace = myModel.GetFace(theFaceIndex);
            process(aDFace);
        }
        public override bool performInternal(IMeshData_Model theModel, IMeshTools_Parameters theParameters, Message_ProgressRange theRange)
        {
            //(void)theRange;
            myModel = theModel;
            myParameters = theParameters;
            if (myModel == null)
            {
                return false;
            }

            // MinSize is made as a constant. It is connected with
            // the fact that too rude discretisation can lead to 
            // self-intersecting polygon, which cannot be fixed.
            // As result the face will not be triangulated at all.
            // E.g. see "Test mesh standard_mesh C7", the face #17.
            myParameters.MinSize = Precision.Confusion();

            myFaceIntersectingEdges = new DMapOfIFacePtrsMapOfIEdgePtrs();
            for (int aFaceInd = 0; aFaceInd < myModel.FacesNb(); ++aFaceInd)
            {
                //   myFaceIntersectingEdges.Bind(myModel.GetFace(aFaceInd).get(), null);
            }

            // TODO: Here we can process edges in order to remove close discrete points.
            //OSD_Parallel::For(0, myModel->FacesNb(), *this, !isParallel());
            for (int i = 0; i < myModel.FacesNb(); i++)
            {                
                 amplifyEdges();
            }

            //MapOfIFacePtrsMapOfIEdgePtrs::Iterator aFaceIt(*myFaceIntersectingEdges);
            //for (; aFaceIt.More(); aFaceIt.Next())
            //{
            //    if (!aFaceIt.Value().IsNull())
            //    {
            //        const IMeshData::IFaceHandle aDFace = aFaceIt.Key();
            //        aDFace->SetStatus(IMeshData_SelfIntersectingWire);
            //        aDFace->SetStatus(IMeshData_Failure);
            //    }
            //}

            myFaceIntersectingEdges = null;
            myModel = null; // Do not hold link to model.
            return true;
        }

        //! Amplifies discretization of edges in case if self-intersection problem has been found.
        private void amplifyEdges()
        {
            //todo
        }
    }
}

