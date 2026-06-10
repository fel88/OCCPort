namespace TKXSBASE
{
    //! Defines a Check, as a list of Fail or Warning Messages under
    //! a literal form, which can be empty. A Check can also bring an
    //! Entity, which is the Entity to which the messages apply
    //! (this Entity may be any Transient Object).
    //!
    //! Messages can be stored in two forms : the definitive form
    //! (the only one by default), and another form, the original
    //! form, which can be different if it contains values to be
    //! inserted (integers, reals, strings)
    //! The original form can be more suitable for some operations
    //! such as counting messages
    public class Interface_Check
    {//! Clears a check, in order to receive information from transfer
     //! (Messages and Entity)
        public void Clear()
        {
            thefails = null; thefailo = null;
            thewarns = null; thewarno = null;
            theinfos = null; theinfoo = null;
            theent = null;
        }

        string thefails;
        string thefailo;
        string thewarns;
        string thewarno;
        string theinfos;
        string theinfoo;
        object theent;
    }
}
