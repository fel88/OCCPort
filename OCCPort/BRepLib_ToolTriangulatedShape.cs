using System.Reflection.Metadata;
using System.Xml.Linq;

namespace OCCPort
{
    internal class BRepLib_ToolTriangulatedShape
    {//! Computes nodal normals for Poly_Triangulation structure using UV coordinates and surface.
     //! Does nothing if triangulation already defines normals.
     //! @param[in] theFace the face
     //! @param[in] theTris the definition of a face triangulation
        public static void ComputeNormals(TopoDS_Face theFace,
                              Poly_Triangulation theTris)
        {
            Poly_Connect aPolyConnect = new Poly_Connect();
            ComputeNormals(theFace, theTris, aPolyConnect);
        }

        //! Computes nodal normals for Poly_Triangulation structure using UV coordinates and surface.
        //! Does nothing if triangulation already defines normals.
        //! @param[in] theFace the face
        //! @param[in] theTris the definition of a face triangulation
        //! @param[in,out] thePolyConnect optional, initialized tool for exploring triangulation
        public static void ComputeNormals(TopoDS_Face theFace,
                                                    Poly_Triangulation theTris,
                                                    Poly_Connect thePolyConnect)
        {
            if (theTris == null || theTris.HasNormals())
                return;

            // take in face the surface location
            TopoDS_Face aZeroFace = TopoDS.Face(theFace.Located(new TopLoc_Location()));
            Geom_Surface aSurf = BRep_Tool.Surface(aZeroFace);
            if (!theTris.HasUVNodes() || aSurf == null)
            {
                // compute normals by averaging triangulation normals sharing the same vertex
                Poly.ComputeNormals(theTris);
                return;
            }

            double aTol = Precision.Confusion();
            int[] aTri = new int[3];
            gp_Dir aNorm = new gp_Dir();
            theTris.AddNormals();
            for (int aNodeIter = 1; aNodeIter <= theTris.NbNodes(); ++aNodeIter)
            {
                // try to retrieve normal from real surface first, when UV coordinates are available
                if (GeomLib.NormEstim(aSurf, theTris.UVNode(aNodeIter), aTol, ref aNorm) > 1)
                {
                    if (thePolyConnect.Triangulation() != theTris)
                    {
                        thePolyConnect.Load(theTris);
                    }

                    // compute flat normals
                    gp_XYZ eqPlan = new gp_XYZ(0.0, 0.0, 0.0);
                    for (thePolyConnect.Initialize(aNodeIter); thePolyConnect.More(); thePolyConnect.Next())
                    {
                        theTris.Triangle(thePolyConnect.Value()).Get(ref aTri[0], ref aTri[1], ref aTri[2]);
                        gp_XYZ v1 = new gp_XYZ(theTris.Node(aTri[1]).Coord() - theTris.Node(aTri[0]).Coord());
                        gp_XYZ v2 = new gp_XYZ(theTris.Node(aTri[2]).Coord() - theTris.Node(aTri[1]).Coord());
                        gp_XYZ vv = v1 ^ v2;
                        double aMod = vv.Modulus();
                        if (aMod >= aTol)
                        {
                            eqPlan += vv / aMod;
                        }
                    }
                    double aModMax = eqPlan.Modulus();
                    aNorm = (aModMax > aTol) ? new gp_Dir(eqPlan) : gp.DZ();
                }

                theTris.SetNormal(aNodeIter, aNorm);
            }
        }
    }
}