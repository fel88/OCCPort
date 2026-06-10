namespace TKXSBASE
{
    //! Gives access to
    //! - entities in a STEP file,
    //! - the STEP file header.
    public class StepData_StepModel : Interface_InterfaceModel
    {

        //! erases specific labels, i.e. clears the map (entity-ident)
        public override void ClearLabels()
        {
            theidnums = null;
        }

        public override void ClearHeader()
        {
            throw new NotImplementedException();
        }

        Interface_EntityList theheader;
        string theidnums;
        Resource_FormatType mySourceCodePage;
        bool myReadUnitIsInitialized;
        double myWriteUnit;

    }
}
