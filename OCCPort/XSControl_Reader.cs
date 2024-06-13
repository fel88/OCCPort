namespace OCCPort
{
    internal abstract class XSControl_Reader
    {
        protected bool therootsta;
        protected TColStd_SequenceOfTransient theroots;

        public Interface_InterfaceModel Model()
        {
            return thesession.Model();
        }


        //=======================================================================
        //function : WS
        //purpose  : 
        //=======================================================================

        protected XSControl_WorkSession WS()
        {
            return thesession;
        }


        private XSControl_WorkSession thesession;
        public abstract int NbRootsForTransfer();
    }
}