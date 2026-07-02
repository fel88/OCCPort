namespace TKV3d
{
    //! Defines an Interactive Object by gathering together
    //! several object presentations. This is done through a
    //! list of interactive objects. These can also be
    //! Connected objects. That way memory-costly
    //! calculations of presentation are avoided.
    public class AIS_MultipleConnectedInteractive : AIS_InteractiveObject
    {
        public override void Compute(PrsMgr_PresentationManager myPresentationManager, Prs3d_Presentation prsMgr_Presentation, int aDispMode)
        {
            throw new NotImplementedException();
        }

        public override void ComputeSelection(SelectMgr_Selection theSelection, int theMode)
        {
            throw new NotImplementedException();
        }
    }
}

