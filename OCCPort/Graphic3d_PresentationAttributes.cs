using System;

namespace OCCPort
{
	public class Graphic3d_PresentationAttributes
	{
		public Graphic3d_PresentationAttributes()
		{
			//! Empty constructor.
			//myBasicColor = (Quantity_NameOfColor.Quantity_NOC_WHITE);
			myHiMethod = (Aspect_TypeOfHighlightMethod.Aspect_TOHM_COLOR);
			myZLayer = Graphic3d_ZLayerId.Graphic3d_ZLayerId_Default;
			myDispMode = (0);
		}

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

		public object BasicFillAreaAspect()
		{
			throw new NotImplementedException();
		}
	}
}