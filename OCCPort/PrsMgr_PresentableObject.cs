namespace OCCPort
{
    public class PrsMgr_PresentableObject
    {

        //! Return view affinity mask.
        public Graphic3d_ViewAffinity ViewAffinity() { return myViewAffinity; }
        Graphic3d_ViewAffinity myViewAffinity;            //!< view affinity mask

    }
}