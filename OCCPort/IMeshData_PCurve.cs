namespace OCCPort
{

    //! Interface class representing pcurve of edge associated with discrete face.
    //! Indexation of points starts from zero.
    public interface IMeshData_PCurve
    {
        //! Returns orientation of the edge associated with current pcurve.
        TopAbs_Orientation GetOrientation();

        //! Returns discretization point with the given index.
        gp_Pnt2d GetPoint(int theIndex);
        int ParametersNb();


    }
}