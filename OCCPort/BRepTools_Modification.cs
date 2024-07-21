namespace OCCPort
{
    //! Defines geometric modifications to a shape, i.e.
    //! changes to faces, edges and vertices.
    public interface BRepTools_Modification

    {
        //! Returns true if the face, F, has been modified.
        //! If the face has been modified:
        //! - S is the new geometry of the face,
        //! - L is its new location, and
        //! - Tol is the new tolerance.
        //! The flag, RevWires, is set to true when the
        //! modification reverses the normal of the surface, (i.e.
        //! the wires have to be reversed).
        //! The flag, RevFace, is set to true if the orientation of
        //! the modified face changes in the shells which contain it.
        //! If the face has not been modified this function returns
        //! false, and the values of S, L, Tol, RevWires and
        //! RevFace are not significant.
        bool NewSurface(TopoDS_Face F, Geom_Surface S, TopLoc_Location L, double Tol, bool RevWires, bool RevFace);

        //! Returns true if the face has been modified according to changed triangulation.
        //! If the face has been modified:
        //! - T is a new triangulation on the face
        bool NewTriangulation(TopoDS_Face F, Poly_Triangulation T);

    }

}