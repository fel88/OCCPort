namespace OCCPort
{
    public class PrsMgr_PresentableObject
    {

        //! Return view affinity mask.
        public Graphic3d_ViewAffinity ViewAffinity() { return myViewAffinity; }


        protected PrsMgr_PresentableObject myParent;                  //!< pointer to the parent object
        protected PrsMgr_Presentations myPresentations;           //!< list of presentations
        protected Graphic3d_ViewAffinity myViewAffinity;            //!< view affinity mask
        protected Graphic3d_SequenceOfHClipPlane myClipPlanes;              //!< sequence of object-specific clipping planes
        protected Prs3d_Drawer myDrawer;                  //!< main presentation attributes
        protected Prs3d_Drawer myHilightDrawer;           //!< (optional) custom presentation attributes for highlighting selected object

    }
}