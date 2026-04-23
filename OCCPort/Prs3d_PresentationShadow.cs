namespace OCCPort
{
    //! Defines a "shadow" of existing presentation object with custom aspects.
    public class Prs3d_PresentationShadow : Graphic3d_Structure
    {
        //! Returns the id of the parent presentation
        public int ParentId() { return myParentStructId; }
        int myParentStructId;

    }
}