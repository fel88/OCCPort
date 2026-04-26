using OCCPort.Interfaces;
using System;
using System.Reflection.Metadata;
using System.Security.Cryptography;

namespace OCCPort
{
    //! Auxiliary class performing tessellation of passed edge according to specified parameters.
    public class BRepMesh_CurveTessellator : IMeshTools_CurveTessellator
    {
        public BRepMesh_CurveTessellator(
  IMeshData_Edge theEdge,
  IMeshTools_Parameters theParameters,
  int theMinPointsNb)
        {

            myDEdge = (theEdge);
            myParameters = (theParameters);
            myEdge = (theEdge.GetEdge());
            myCurve = new BRepAdaptor_Curve(myEdge);//??
            myMinPointsNb = (theMinPointsNb);
            init();
        }

        public BRepMesh_CurveTessellator(
  IMeshData_Edge theEdge,
  TopAbs_Orientation theOrientation,
  IMeshData_Face theFace,
  IMeshTools_Parameters theParameters,
  int theMinPointsNb)

        {
            myDEdge = (theEdge);
            myParameters = (theParameters);
            myEdge = (TopoDS.Edge(theEdge.GetEdge().Oriented(theOrientation)));
            myCurve = new BRepAdaptor_Curve(myEdge, theFace.GetFace());
            myMinPointsNb = theMinPointsNb;
            init();
        }
        void init()
        {
            if (myParameters.MinSize <= 0.0)
            {
                Standard_Failure.Raise("The structure \"myParameters\" is not initialized");
            }

            TopExp.Vertices(myEdge, out myFirstVertex, out myLastVertex);

            double aPreciseAngDef = 0.5 * myDEdge.GetAngularDeflection();
            double aPreciseLinDef = 0.5 * myDEdge.GetDeflection();
            if (myEdge.Orientation() == TopAbs_Orientation.TopAbs_INTERNAL)
            {
                aPreciseLinDef *= 0.5;
            }

            aPreciseLinDef = Math.Max(aPreciseLinDef, Precision.Confusion());
            aPreciseAngDef = Math.Max(aPreciseAngDef, Precision.Angular());

            double aMinSize = myParameters.MinSize;
            //if (myParameters.AdjustMinSize)
            //{
            //    aMinSize = Math.Min(aMinSize, myParameters.RelMinSize() * GCPnts_AbscissaPoint::Length(
            //      myCurve, myCurve.FirstParameter(), myCurve.LastParameter(), aPreciseLinDef));
            //}

            mySquareEdgeDef = aPreciseLinDef * aPreciseLinDef;
            mySquareMinSize = Math.Max(mySquareEdgeDef, aMinSize * aMinSize);

            myEdgeSqTol = BRep_Tool.Tolerance(myEdge);
            myEdgeSqTol *= myEdgeSqTol;

            int aMinPntNb = Math.Max(myMinPointsNb,
              (myCurve._GetType() == GeomAbs_CurveType.GeomAbs_Circle) ? 4 : 2); //OCC287

            /*myDiscretTool.Initialize(myCurve,
                                      myCurve.FirstParameter(), myCurve.LastParameter(),
                                      aPreciseAngDef, aPreciseLinDef, aMinPntNb,
                                      Precision.PConfusion(), aMinSize);*/

            if (myCurve.IsCurveOnSurface())
            {
                //Adaptor3d_CurveOnSurface aCurve = myCurve.CurveOnSurface();
                //Adaptor3d_Surface aSurface = aCurve.GetSurface();

                //double aTol = Precision.Confusion();
                //double aDu = aSurface.UResolution(aTol);
                //double aDv = aSurface.VResolution(aTol);

                //myFaceRangeU[0] = aSurface.FirstUParameter() - aDu;
                //myFaceRangeU[1] = aSurface.LastUParameter() + aDu;

                //myFaceRangeV[0] = aSurface.FirstVParameter() - aDv;
                //myFaceRangeV[1] = aSurface.LastVParameter() + aDv;
            }

            addInternalVertices();
            splitByDeflection2d();
        }

        private void splitByDeflection2d()
        {
            throw new NotImplementedException();
        }

        private void addInternalVertices()
        {
            throw new NotImplementedException();
        }

        IMeshData_Edge myDEdge;
        IMeshTools_Parameters myParameters;
        TopoDS_Edge myEdge;
        BRepAdaptor_Curve myCurve;
        int myMinPointsNb;
        GCPnts_TangentialDeflection myDiscretTool;
        TopoDS_Vertex myFirstVertex;
        TopoDS_Vertex myLastVertex;
        double mySquareEdgeDef;
        double mySquareMinSize;
        double myEdgeSqTol;
        double[] myFaceRangeU = new double[2];
        double[] myFaceRangeV = new double[2];
    }
}