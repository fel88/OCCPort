namespace OCCPort
{
    //! Interface class providing API for edge tessellation tools.
    public interface IMeshTools_CurveTessellator
    {

        //! Returns parameters of solution with the given index.
        //! @param theIndex index of tessellation point.
        //! @param thePoint tessellation point.
        //! @param theParameter parameters on PCurve corresponded to the solution.
        //! @return True in case of valid result, false elewhere.

        //! Returns number of tessellation points.
        int PointsNb();

        bool Value(
     int theIndex,
   out gp_Pnt                thePoint,
    out double         theParameter) ;

    }

}