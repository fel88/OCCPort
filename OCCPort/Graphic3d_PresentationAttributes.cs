namespace OCCPort
{
    public class Graphic3d_PresentationAttributes
    {  //! Sets display mode.
        public virtual void SetDisplayMode(int theMode) { myDispMode = theMode; }

        Graphic3d_AspectFillArea3d myBasicFillAreaAspect; //!< presentation fill area aspect
        Quantity_ColorRGBA myBasicColor;          //!< presentation color
        Aspect_TypeOfHighlightMethod myHiMethod;            //!< box or color highlighting
        Graphic3d_ZLayerId myZLayer;              //!< Z-layer
        int myDispMode;            //!< display mode
        //! Returns display mode, 0 by default.
        //! -1 means undefined (main display mode of presentation to be used).
        public int DisplayMode() { return myDispMode; }


    }
}