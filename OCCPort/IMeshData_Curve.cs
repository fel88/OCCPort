namespace OCCPort
{
    //! Interface class representing discrete 3d curve of edge.
    //! Indexation of points starts from zero.
    public interface IMeshData_Curve
    {
        //! Returns discretization point with the given index.
        gp_Pnt GetPoint(int theIndex);

    }
}