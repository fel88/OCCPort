namespace OCCPort
{
    //! Projects elementary curves on a plane.
    public class ProjLib_Plane : ProjLib_Projector
    {
        //! Projection on the plane <Pl>.
        public ProjLib_Plane(gp_Pln Pl)
        {
            Init(Pl);

        }
        gp_Pln myPlane;

        public void Init(gp_Pln Pl)
        {
            myType = GeomAbs_CurveType.GeomAbs_OtherCurve;
            isDone = false;
            myIsPeriodic = false;
            myPlane = Pl;
        }
    }

}