namespace OCCPort
{
    //! This  package  provides  classes  and services  to
    //! handle :
    //!
    //! * 3D triangular polyhedrons.
    //!
    //! * 3D polygons.
    //!
    //! * 2D polygon.
    //!
    //! * Tools to dump, save and restore those objects.
    public class Poly
    {//=======================================================================
        public static void ComputeNormals(Poly_Triangulation theTri)
        {
            theTri.ComputeNormals();
        }

    }
}