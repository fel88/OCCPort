namespace OCCPort
{
    //! Defines a continuity between two surfaces.
    public class BRep_CurveOn2Surfaces : BRep_CurveRepresentation
    {
        public BRep_CurveOn2Surfaces(TopLoc_Location L) : base(L)
        {
        }

        public override BRep_CurveRepresentation Copy()
        {
            throw new System.NotImplementedException();
        }
    }
}