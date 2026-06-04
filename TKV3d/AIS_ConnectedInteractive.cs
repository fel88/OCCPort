using OCCPort;

namespace TKV3d
{

    //! Creates an arbitrary located instance of another Interactive Object,
    //! which serves as a reference.
    //! This allows you to use the Connected Interactive
    //! Object without having to recalculate presentation,
    //! selection or graphic structure. These are deduced
    //! from your reference object.
    //! The relation between the connected interactive object
    //! and its source is generally one of geometric transformation.
    //! AIS_ConnectedInteractive class supports selection mode 0 for any InteractiveObject and
    //! all standard modes if its reference based on AIS_Shape.
    //! Descendants may redefine ComputeSelection() though.
    //! Also ConnectedInteractive will handle HLR if its reference based on AIS_Shape.
    public class AIS_ConnectedInteractive : AIS_InteractiveObject
    {

        AIS_InteractiveObject myReference;
        TopoDS_Shape myShape;
        //! Returns true if there is a connection established
        //! between the presentation and its source reference.
        public bool HasConnection() { return myReference != null; }

        public override void Compute(PrsMgr_PresentationManager thePrsMgr,
            Prs3d_Presentation thePrs, int theMode)
        {
            if (HasConnection())
            {
                thePrs.Clear(false);
                //thePrs.DisconnectAll(Graphic3d_TOC_DESCENDANT);

                if (!myReference.HasInteractiveContext())
                {
                    //   myReference.SetContext(GetContext());
                }
                thePrsMgr.Connect(this, myReference, theMode, theMode);
                // if (thePrsMgr.Presentation(myReference, theMode)->MustBeUpdated())
                {
                    //thePrsMgr.Update(myReference, theMode);
                }
            }

            if (thePrs != null)
            {
                thePrs.ReCompute();
            }
        }

        public override void ComputeSelection(SelectMgr_Selection theSelection, int theMode)
        {
            throw new NotImplementedException();
        }
    }
}

