using System.Reflection.Metadata;

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

        }
    }
}