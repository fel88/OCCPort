using System;
using System.Reflection.Metadata;

namespace OCCPort
{
    //! Constructs point datums to be used in construction of
    //! composite shapes. The datum is displayed as the plus marker +.
    public class AIS_Point : AIS_InteractiveObject
    {
        public override void Compute(PrsMgr_PresentationManager myPresentationManager, Prs3d_Presentation prsMgr_Presentation, int aDispMode)
        {
            throw new NotImplementedException();
        }

        public override void ComputeSelection(SelectMgr_Selection aSelection,
                                 int aMode)
        {
            //SelectMgr_EntityOwner eown = new SelectMgr_EntityOwner(this, 10);
            //Select3D_SensitivePoint sp = new Select3D_SensitivePoint(eown,
            //                                 myComponent->Pnt());
            //aSelection.Add(sp);
        }

    }
}
