using System;
using System.Net.NetworkInformation;

namespace OCCPort
{
    internal class ShapeAnalysis_Wire
    {
        public ShapeAnalysis_Wire(ShapeExtend_WireData aWireData, TopoDS_Face topoDS_Face, double v)
        {
        }
        public bool CheckOrder(bool isClosed, bool mode3d)
        {
            ShapeAnalysis_WireOrder sawo = new ShapeAnalysis_WireOrder();
            CheckOrder(sawo, isClosed, mode3d, false);
            myStatusOrder = myStatus;
            return StatusOrder(ShapeExtend_Status.ShapeExtend_DONE);
        }

        public bool StatusOrder(ShapeExtend_Status Status)
        {
            return ShapeExtend.DecodeStatus(myStatusOrder, Status);
        }

        //! Analyzes the order of the edges in the wire,
        //! uses class WireOrder for that purpose.
        //! Flag <isClosed> defines if the wire is closed or not
        //! Flag <theMode3D> defines 3D or 2d mode.
        //! Flag <theModeBoth> defines miscible mode and the flag <theMode3D> is ignored.
        //! Returns False if wire is already ordered (tail-to-head),
        //! True otherwise.
        //! Use returned WireOrder object for deeper analysis.
        //! Status:
        //! OK   : the same edges orientation, the same edges sequence
        //! DONE1: the same edges orientation, not the same edges sequence
        //! DONE2: as DONE1 and gaps more than myPrecision
        //! DONE3: not the same edges orientation (some need to be reversed)
        //! DONE4: as DONE3 and gaps more than myPrecision
        //! FAIL : algorithm failed (could not detect order)
        public bool CheckOrder(ShapeAnalysis_WireOrder sawo,
                                                    bool isClosed = true,
                                                    bool theMode3D = true,
                                                    bool theModeBoth = false)
        {
            if ((!theMode3D || theModeBoth) && myFace.IsNull())
            {
                myStatus = ShapeExtend.EncodeStatus(ShapeExtend_Status.ShapeExtend_FAIL2);
                return false;
            }
            myStatus = ShapeExtend.EncodeStatus(ShapeExtend_Status.ShapeExtend_OK);
            sawo.SetMode(theMode3D, 0.0, theModeBoth);
            int nb = myWire.NbEdges();
            ShapeAnalysis_Edge EA = new ShapeAnalysis_Edge();
            bool isAll2dEdgesOk = true;
            for (int i = 1; i <= nb; i++)
            {
                TopoDS_Edge E = myWire.Edge(i);
                gp_XYZ aP1XYZ = new gp_XYZ(), aP2XYZ = new gp_XYZ();
                gp_XY aP1XY = new gp_XY(), aP2XY = new gp_XY();
                if (theMode3D || theModeBoth)
                {
                    TopoDS_Vertex V1 = EA.FirstVertex(E);
                    TopoDS_Vertex V2 = EA.LastVertex(E);
                    if (V1.IsNull() || V2.IsNull())
                    {
                        myStatus = ShapeExtend.EncodeStatus(ShapeExtend_Status.ShapeExtend_FAIL2);
                        return false;
                    }
                    else
                    {
                        aP1XYZ = BRep_Tool.Pnt(V1).XYZ();
                        aP2XYZ = BRep_Tool.Pnt(V2).XYZ();
                    }
                }
                if (!theMode3D || theModeBoth)
                {
                    double f = 0, l = 0;
                    Geom2d_Curve c2d = null;
                    TopoDS_Shape tmpF = myFace.Oriented(TopAbs_Orientation.TopAbs_FORWARD);
                    if (!EA.PCurve(E, TopoDS.Face(tmpF), c2d, ref f, ref l))
                    {
                        // if mode is 2d, then we can nothing to do, else we can switch to 3d mode
                        if (!theMode3D && !theModeBoth)
                        {
                            myStatus = ShapeExtend.EncodeStatus(ShapeExtend_Status.ShapeExtend_FAIL2);
                            return false;
                        }
                        else
                        {
                            isAll2dEdgesOk = false;
                        }
                    }
                    else
                    {
                        aP1XY = c2d.Value(f).XY();
                        aP2XY = c2d.Value(l).XY();
                    }
                }
                if (theMode3D && !theModeBoth)
                {
                    sawo.Add(aP1XYZ, aP2XYZ);
                }
                else if (!theMode3D && !theModeBoth)
                {
                    sawo.Add(aP1XY, aP2XY);
                }
                else
                {
                    sawo.Add(aP1XYZ, aP2XYZ, aP1XY, aP2XY);
                }
            }
            // need to switch to 3d mode
            if (theModeBoth && !isAll2dEdgesOk)
            {
                sawo.SetMode(true, 0.0, false);
            }
            sawo.Perform(isClosed);
            int stat = sawo.Status();
            switch (stat)
            {
                case 0: myStatus = ShapeExtend.EncodeStatus(ShapeExtend_Status.ShapeExtend_OK); break;
                case 1: myStatus = ShapeExtend.EncodeStatus(ShapeExtend_Status.ShapeExtend_DONE1); break;
                case 2: myStatus = ShapeExtend.EncodeStatus(ShapeExtend_Status.ShapeExtend_DONE2); break; // this value is not returned
                case -1: myStatus = ShapeExtend.EncodeStatus(ShapeExtend_Status.ShapeExtend_DONE3); break;
                case -2: myStatus = ShapeExtend.EncodeStatus(ShapeExtend_Status.ShapeExtend_DONE4); break; // this value is not returned
                case 3: myStatus = ShapeExtend.EncodeStatus(ShapeExtend_Status.ShapeExtend_DONE5); break; // only shifted
                case -10: myStatus = ShapeExtend.EncodeStatus(ShapeExtend_Status.ShapeExtend_FAIL1); break; // this value is not returned
            }
            return LastCheckStatus(ShapeExtend_Status.ShapeExtend_DONE);
        }
        ShapeExtend_WireData myWire;
        TopoDS_Face myFace;
        ShapeAnalysis_Surface mySurf;
        double myPrecision;
        double myMin3d;
        double myMin2d;
        double myMax3d;
        double myMax2d;
        int myStatusOrder;
        int myStatusConnected;
        int myStatusEdgeCurves;
        int myStatusDegenerated;
        int myStatusClosed;
        int myStatusSmall;
        int myStatusSelfIntersection;
        int myStatusLacking;
        int myStatusGaps3d;
        int myStatusGaps2d;
        int myStatusCurveGaps;
        int myStatusLoop;
        int myStatus;
        public bool LastCheckStatus(ShapeExtend_Status Status)
        {
            return ShapeExtend.DecodeStatus(myStatus, Status);
        }

    }
}