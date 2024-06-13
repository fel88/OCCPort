namespace OCCPort
{
    class STEPControl_Reader : XSControl_Reader
    {

        //! Determines the list of root entities from Model which are candidate for
        //! a transfer to a Shape (type of entities is PRODUCT)
        public override int NbRootsForTransfer()
        {
            if (therootsta)
                return theroots.Length();

            therootsta = true;

            //theroots.Clear();
            int nb = Model().NbEntities();
            for (int i = 1; i <= nb; i++)
            {
                var ent = Model().Value(i);
               /* if (Interface_Static.IVal("read.step.all.shapes") == 1)
                {
                    // Special case to read invalid shape_representation without links to shapes.
                    if (ent->IsKind(STANDARD_TYPE(StepShape_ManifoldSolidBrep)))
                    {
                        Interface_EntityIterator aShareds = WS().Graph().Sharings(ent);
                        if (!aShareds.More())
                        {
                            theroots.Append(ent);
                            WS().TransferReader().TransientProcess().RootsForTransfer().Append(ent);
                        }
                    }
                }*/
            }
            return 0;
        }
    }
}