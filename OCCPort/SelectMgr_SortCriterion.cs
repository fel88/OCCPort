namespace OCCPort
{
    public class SelectMgr_SortCriterion
    {
        public Select3D_SensitiveEntity Entity; //!< detected entity
        public gp_Pnt Point;           //!< 3D point
        public Graphic3d_Vec3 Normal;          //!< surface normal or 0 vector if undefined
        public double Depth;           //!< distance from the view plane to the entity
        public double MinDist;         //!< distance from the clicked point to the entity on the view plane
        public double Tolerance;       //!< tolerance used for selecting candidates
        public int Priority;        //!< selection priority
        public int ZLayerPosition;  //!< ZLayer rendering order index, stronger than a depth
        public int NbOwnerMatches;  //!< overall number of entities collected for the same owner
    }
}