namespace OCCPort
{
	public class Aspect_Grid
	{
		//! Returns TRUE when the grid is active.
		public bool IsActive() { return myIsActive; }
		double myRotationAngle;
		double myXOrigin;
		double myYOrigin;
		//Quantity_Color myColor;
		//	Quantity_Color myTenthColor;
		bool myIsActive;
		//Aspect_GridDrawMode myDrawMode;

		public Aspect_Grid(double theXOrigin = 0,
								double theYOrigin = 0,
								double theAngle = 0,
								Quantity_Color theColor = null,
								Quantity_Color theTenthColor = null)
		{
			if (theColor == null)
				theColor = new Quantity_Color(Quantity_NameOfColor.Quantity_NOC_GRAY50);

			if (theTenthColor == null)
				theTenthColor = new Quantity_Color(Quantity_NameOfColor.Quantity_NOC_GRAY70);

			myRotationAngle = (theAngle);
			myXOrigin = (theXOrigin);
			myYOrigin = (theYOrigin);
			//myColor = (theColor);
			//myTenthColor = (theTenthColor);
			myIsActive = (false);
			//myDrawMode = (Aspect_GDM_Lines);

			//
		}



	}
}