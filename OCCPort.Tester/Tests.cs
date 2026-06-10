global using AIS_ListOfInteractive = TKernel.NCollection_List<TKV3d.AIS_InteractiveObject>;
global using AIS_ListIteratorOfListOfInteractive = TKernel.NCollection_List<TKV3d.AIS_InteractiveObject>.Iterator;
using OCCPort.Common;
using OCCPort.Enums;
using OpenTK.Mathematics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using TKBRep;
using TKernel;
using TKG3d;
using TKMath;
using TKMesh;
using TKV3d;
using static OpenTK.Graphics.OpenGL.GL;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace OCCPort.Tester
{
    public static class Tests
    {
        public static DelaBella_Triangle TriangulateTest1(Vector2d[] points)
        {
            Bnd_B2d aBox = new Bnd_B2d();

            //List<Standard_Real> aPoints(2 * (aNodesNb + 4));
            var aNodesNb = points.Length;
            double[] aPoints = new double[(2 * (aNodesNb + 4))];
            //double[] aPoints = new double[(2 * (aNodesNb ))];
            for (int aNodeIt = 0; aNodeIt < aNodesNb; ++aNodeIt)
            {
                var aVertex = points[aNodeIt];

                int aBaseIdx = 2 * (aNodeIt);
                aPoints[aBaseIdx + 0] = aVertex.X;
                aPoints[aBaseIdx + 1] = aVertex.Y;

                aBox.Add(new gp_Pnt2d(aVertex.X, aVertex.Y));
            }

            aBox.Enlarge(0.1 * (aBox.CornerMax() - aBox.CornerMin()).Modulus());
            gp_XY aMin = aBox.CornerMin();
            gp_XY aMax = aBox.CornerMax();

            aPoints[2 * aNodesNb + 0] = aMin.X();
            aPoints[2 * aNodesNb + 1] = aMin.Y();


            aPoints[2 * aNodesNb + 2] = aMax.X();
            aPoints[2 * aNodesNb + 3] = aMin.Y();


            aPoints[2 * aNodesNb + 4] = aMax.X();
            aPoints[2 * aNodesNb + 5] = aMax.Y();


            aPoints[2 * aNodesNb + 6] = aMin.X();
            aPoints[2 * aNodesNb + 7] = aMax.Y();


            double aDiffX = (aMax.X() - aMin.X());
            double aDiffY = (aMax.Y() - aMin.Y());
            for (int i = 0; i < aPoints.Length; i += 2)
            {
                aPoints[i + 0] = (aPoints[i + 0] - aMin.X()) / aDiffX - 0.5;
                aPoints[i + 1] = (aPoints[i + 1] - aMin.Y()) / aDiffY - 0.5;
            }

            //IDelaBella aTriangulator = new ShewchukTriangulator();
            IDelaBella aTriangulator = new CDelaBella();
            if (aTriangulator == null) // should never happen
            {
                throw new Standard_ProgramError("BRepMesh_DelabellaBaseMeshAlgo::buildBaseTriangulation: unable creating a triangulation algorithm");
            }

            //          aTriangulator->SetErrLog(logDelabella2Occ, NULL);
            //          try
            //          {
            int aVerticesNb = aTriangulator.Triangulate((int)(aPoints.Length / 2), aPoints);
            DelaBella_Triangle ret = null;
            if (aVerticesNb > 0)
            {
                DelaBella_Triangle aTrianglePtr = aTriangulator.GetFirstDelaunayTriangle();
                if (ret == null)
                    ret = aTrianglePtr;
                while (aTrianglePtr != null)
                {
                    int[] aNodes = {
                    aTrianglePtr.v[0].i + 1,
                    aTrianglePtr.v[2].i + 1,
                    aTrianglePtr.v[1].i + 1
                  };

                    int[] aEdges = new int[3];
                    bool[] aOrientations = new bool[3];
                    for (int k = 0; k < 3; ++k)
                    {
                        BRepMesh_Edge aLink = new BRepMesh_Edge(aNodes[k], aNodes[(k + 1) % 3], BRepMesh_DegreeOfFreedom.BRepMesh_Free);

                        //int aLinkInfo = aStructure.AddLink(aLink);
                        //aEdges[k] = Math.Abs(aLinkInfo);
                        // aOrientations[k] = aLinkInfo > 0;
                    }

                    BRepMesh_Triangle aTriangle = new BRepMesh_Triangle(aEdges, aOrientations, BRepMesh_DegreeOfFreedom.BRepMesh_Free);
                    //aStructure.AddElement(aTriangle);

                    aTrianglePtr = aTrianglePtr.next;
                }
            }
            return ret;
        }


        public static AIS_InteractiveObject findObject(int bindId, AIS_InteractiveContext ctx, TopTools_IndexedMapOfShape _map_shape_int)
        {


            AIS_ListOfInteractive aList = new AIS_ListOfInteractive();
            AIS_ListOfInteractive aList2 = new AIS_ListOfInteractive();

            ctx.DisplayedObjects(aList);
            ctx.ErasedObjects(aList2);
            AIS_ListIteratorOfListOfInteractive it = new AIS_ListIteratorOfListOfInteractive(aList);
            AIS_ListIteratorOfListOfInteractive it2 = new AIS_ListIteratorOfListOfInteractive(aList2);
            //iterate on list:
            while (it.More())
            {
                var sin = it.Value();

                AIS_Shape aAIS_Shape = (AIS_Shape)sin;
                if (aAIS_Shape != null)
                {
                    TopoDS_Shape aTopoDS_Shape = aAIS_Shape.Shape();
                    // Use aTopoDS_Shape here

                    if (_map_shape_int.Contains(aTopoDS_Shape))
                    {
                        if (_map_shape_int.FindIndex(aTopoDS_Shape) == bindId)
                            return sin;
                    }
                }

                //do something with the current item : it.Value ()
                it.Next();
            }

            while (it2.More())
            {
                var sin = it2.Value();

                AIS_Shape aAIS_Shape = (AIS_Shape)sin;
                if (aAIS_Shape != null)
                {
                    TopoDS_Shape aTopoDS_Shape = aAIS_Shape.Shape();
                    // Use aTopoDS_Shape here

                    if (_map_shape_int.Contains(aTopoDS_Shape))
                    {
                        if (_map_shape_int.FindIndex(aTopoDS_Shape) == bindId)
                            return sin;
                    }
                }

                //do something with the current item : it.Value ()
                it2.Next();
            }

            return default;
            //return reinterpret_cast<AIS_InteractiveObject*> (handle.handle);
        }

        public static List<double> IteratePoly(int hId, bool useLocalTransform, bool useWholeShapeTransform, AIS_InteractiveContext ctx, TopTools_IndexedMapOfShape _map_shape_int)
        {
            var obj = findObject(hId, ctx, _map_shape_int);
            var shape = ((AIS_Shape)obj).Shape();

            //std::vector<QVector3D> vertices;
            List<double> ret = new List<double>();
            //std::vector<QVector3D> normals;
            //std::vector<QVector2D> uvs2;
            List<uint> indices = new List<uint>();
            uint idxCounter = 0;
            //TopoDS_Shape shape = MakeBottle(100, 300, 20);
            double aDeflection = 0.1;

            //BRepMesh_IncrementalMesh(*shape, 1);
            //bm.Perform();
            //auto shape2 = bm.Shape();

            int aIndex = 1, nbNodes = 0;
            if (useWholeShapeTransform && useLocalTransform)
                shape = shape.Located(new TopLoc_Location(obj.LocalTransformation()));
            //TColgp_SequenceOfPnt aPoints, aPoints1;

            for (TopExp_Explorer aExpFace = new TopExp_Explorer(shape, TopAbs_ShapeEnum.TopAbs_FACE); aExpFace.More(); aExpFace.Next())
            {
                var face = aExpFace.Current();

                if (!useWholeShapeTransform && useLocalTransform)
                    face = face.Located(new TopLoc_Location(obj.Transformation()));

                TopoDS_Face aFace = TopoDS.Face(face);

                TopAbs_Orientation faceOrientation = aFace.Orientation();

                TopLoc_Location aLocation = new TopLoc_Location();

                Poly_Triangulation aTr = BRep_Tool.Triangulation(aFace, ref aLocation);

                if (aTr != null)
                {
                    //const TColgp_Array1OfPnt& aNodes = aTr->NbNodes();
                    Poly_Array1OfTriangle triangles = aTr.Triangles();

                    //const TColgp_Array1OfPnt2d& uvNodes = aTr->UVNodes();

                    TColgp_Array1OfPnt aPoints = new TColgp_Array1OfPnt(1, aTr.NbNodes());
                    NCollection_Array1<gp_Dir> aNormals = new NCollection_Array1<gp_Dir>(1, aTr.NbNodes());

                    for (int i = 1; i < aTr.NbNodes() + 1; i++)
                    {
                        aPoints[i] = aTr.Node(i).Transformed(aLocation);
                        aNormals[i] = aTr.Normal(i).Transformed(aLocation);
                    }


                    int nnn = aTr.NbTriangles();
                    int nt = 0, n1 = 0, n2 = 0, n3 = 0;

                    for (nt = 1; nt < nnn + 1; nt++)
                    {

                        triangles[nt].Get(out n1, out n2, out n3);
                        gp_Pnt aPnt1 = aPoints[n1];
                        gp_Pnt aPnt2 = aPoints[n2];
                        gp_Pnt aPnt3 = aPoints[n3];

                        gp_Dir aDir1 = aNormals[n1];
                        gp_Dir aDir2 = aNormals[n2];
                        gp_Dir aDir3 = aNormals[n3];
                        if (faceOrientation == TopAbs_Orientation.TopAbs_REVERSED)
                        {
                            aDir1.Reverse();
                            aDir2.Reverse();
                            aDir3.Reverse();
                        }
                        /*gp_Pnt2d uv1 = uvNodes(n1);
                        gp_Pnt2d uv2 = uvNodes(n2);
                        gp_Pnt2d uv3 = uvNodes(n3);*/

                        //QVector3D p1, p2, p3;

                        //if (faceOrientation == TopAbs_Orientation::TopAbs_FORWARD)
                        {
                            ret.Add(aPnt1.X());
                            ret.Add(aPnt1.Y());
                            ret.Add(aPnt1.Z());

                            ret.Add(aDir1.X());
                            ret.Add(aDir1.Y());
                            ret.Add(aDir1.Z());

                            ret.Add(aPnt2.X());
                            ret.Add(aPnt2.Y());
                            ret.Add(aPnt2.Z());

                            ret.Add(aDir2.X());
                            ret.Add(aDir2.Y());
                            ret.Add(aDir2.Z());

                            ret.Add(aPnt3.X());
                            ret.Add(aPnt3.Y());
                            ret.Add(aPnt3.Z());

                            ret.Add(aDir3.X());
                            ret.Add(aDir3.Y());
                            ret.Add(aDir3.Z());

                            /*p1 = QVector3D(aPnt1.X(), aPnt1.Y(), aPnt1.Z());
                            p2 = QVector3D(aPnt2.X(), aPnt2.Y(), aPnt2.Z());
                            p3 = QVector3D(aPnt3.X(), aPnt3.Y(), aPnt3.Z());*/
                        }

                    }
                }

            }
            return ret;
        }
    }
}
