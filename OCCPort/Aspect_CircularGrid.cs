namespace OCCPort
{
	public class Aspect_CircularGrid : Aspect_Grid
	{  //! creates a new grid. By default this grid is not
	   //! active.
		public Aspect_CircularGrid(double aRadiusStep, int aDivisionNumber, double anXOrigin = 0, double anYOrigin = 0, double aRotationAngle = 0)
			:base(anXOrigin, anYOrigin, aRotationAngle)
		{
			myRadiusStep = (aRadiusStep);
			myDivisionNumber = aDivisionNumber;
		}

		double myRadiusStep;
		int myDivisionNumber;
		double myAlpha;
		double myA1;
		double myB1;

	}
}