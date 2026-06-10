namespace TKXSBASE
{
    //! A TransferReader performs, manages, handles results of,
    //! transfers done when reading a file (i.e. from entities of an
    //! InterfaceModel, to objects for Imagine)
    //!
    //! Running is organised around basic tools : TransientProcess and
    //! its Actor, results are Binders and CheckIterators. It implies
    //! control by a Controller (which prepares the Actor as required)
    //!
    //! Getting results can be done directly on TransientProcess, but
    //! these are immediate "last produced" results. Each transfer of
    //! an entity gives a final result, but also possible intermediate
    //! data, and checks, which can be attached to sub-entities.
    //!
    //! Hence, final results (which intermediates and checks) are
    //! recorded as ResultFromModel and can be queried individually.
    //!
    //! Some more direct access are given for results which are
    //! Transient or Shapes
    public class XSControl_TransferReader
    {
    }

}