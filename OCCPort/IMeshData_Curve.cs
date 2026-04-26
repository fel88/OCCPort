namespace OCCPort
{
    //! Interface class representing discrete 3d curve of edge.
    //! Indexation of points starts from zero.
    public interface IMeshData_Curve : IMeshData_ParametersList, IParametersListPtrType
    {
        //! Returns discretization point with the given index.
        gp_Pnt GetPoint(int theIndex);
        //! Adds new discretization point to curve.
        void AddPoint(
   gp_Pnt thePoint,
   double theParamOnCurve);
        //! Inserts new discretization point at the given position.
          void InsertPoint(
     int thePosition,
     gp_Pnt         thePoint,
     double theParamOnPCurve) ;

    }
}