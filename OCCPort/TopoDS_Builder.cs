namespace OCCPort
{
    public class TopoDS_Builder
    {
        //! Make a Solid covering the whole 3D space.
        public void MakeSolid(TopoDS_Solid S)
        {
            TopoDS_TSolid TS = new TopoDS_TSolid();
            MakeShape(S, TS);
        }

        //=======================================================================
        public void MakeShape(TopoDS_Shape S,
                                   TopoDS_TShape T)
        {
            S.TShape(T);
            S.Location(new TopLoc_Location());
            S.Orientation(TopAbs_Orientation.TopAbs_FORWARD);
        }



    }
}