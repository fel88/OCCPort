using System;
using System.Reflection.Metadata;

namespace OCCPort
{
    //! Class defines presentation properties.

    public class Graphic3d_PresentationAttributes
    {
        public Graphic3d_PresentationAttributes()
        {
            //! Empty constructor.
            myBasicColor = new Quantity_ColorRGBA((new Quantity_Color(Quantity_NameOfColor.Quantity_NOC_WHITE)));
            myHiMethod = (Aspect_TypeOfHighlightMethod.Aspect_TOHM_COLOR);
            myZLayer = Graphic3d_ZLayerId.Graphic3d_ZLayerId_Default;
            myDispMode = (0);
        }

        //! Returns presentation Zlayer, Graphic3d_ZLayerId_Default by default.
        //! Graphic3d_ZLayerId_UNKNOWN means undefined (a layer of main presentation to be used).
        public Graphic3d_ZLayerId ZLayer() { return myZLayer; }

        //! Sets presentation Zlayer.
        public virtual void SetZLayer(Graphic3d_ZLayerId theLayer) { myZLayer = theLayer; }

        //! Sets display mode.
        public virtual void SetDisplayMode(int theMode) { myDispMode = theMode; }

        Graphic3d_AspectFillArea3d myBasicFillAreaAspect; //!< presentation fill area aspect
        Quantity_ColorRGBA myBasicColor;          //!< presentation color
        Aspect_TypeOfHighlightMethod myHiMethod;            //!< box or color highlighting
        Graphic3d_ZLayerId myZLayer;              //!< Z-layer
        int myDispMode;            //!< display mode
                                   //! Returns display mode, 0 by default.
                                   //! -1 means undefined (main display mode of presentation to be used).
        public int DisplayMode() { return myDispMode; }

        public Quantity_ColorRGBA ColorRGBA()
        {
            return myBasicColor;
        }

        //! Return basic presentation fill area aspect, NULL by default.
        //! When set, might be used instead of Color() property.
        public Graphic3d_AspectFillArea3d BasicFillAreaAspect() { return myBasicFillAreaAspect; }
    }
}