namespace OCCPort.Tester
{
    public class AIS_Shape: AIS_InteractiveObject
    {
        private object solid;
        public virtual void Compute(PrsMgr_PresentationManager thePrsMgr,
                                   Prs3d_Presentation thePrs,
                                   int theMode)
        {

        }
        public AIS_Shape() { }
        public AIS_Shape(object solid)
        {
            this.solid = solid;
        }

     protected   TopoDS_Shape myshape;    //!< shape to display

        protected Bnd_Box myBB;       //!< cached bounding box of the shape
        protected  gp_Pnt2d myUVOrigin; //!< UV origin vector for generating texture coordinates
        protected  gp_Pnt2d myUVRepeat; //!< UV repeat vector for generating texture coordinates
        protected  gp_Pnt2d myUVScale;  //!< UV scale  vector for generating texture coordinates
        double myInitAng;
        bool myCompBB;   //!< if TRUE, then bounding box should be recomputed



    }
}