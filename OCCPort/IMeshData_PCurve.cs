using OCCPort.Interfaces;

namespace OCCPort
{

    //! Interface class representing pcurve of edge associated with discrete face.
    //! Indexation of points starts from zero.
    public interface IMeshData_PCurve
    {
        //! Returns orientation of the edge associated with current pcurve.
        TopAbs_Orientation GetOrientation();
        //! Returns discrete face pcurve is associated to.
        IMeshData_Face GetFace();
        //! Returns discretization point with the given index.
        gp_Pnt2d GetPoint(int theIndex);
        int ParametersNb();

        //! Adds new discretization point to pcurve.
        void AddPoint(gp_Pnt2d thePoint, double theParamOnPCurve);
        //! Inserts new discretization point at the given position.
        void InsertPoint(int thePosition, gp_Pnt2d thePoint, double theParamOnPCurve);

    }
}