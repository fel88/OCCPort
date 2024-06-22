namespace OCCPort
{
	internal class Aspect_RectangularGrid : Aspect_Grid
	{
		public Aspect_RectangularGrid(

							   double aXStep,
							   double aYStep = 0,
							   double anXOrigin = 0,
							   double anYOrigin = 0,
							   double aFirstAngle = 0,
							   double aSecondAngle = 0,
							   double aRotationAngle = 0)
: base(anXOrigin, anYOrigin, aRotationAngle)

		{
			//myXStep(aXStep),myYStep(aYStep),myFirstAngle(aFirstAngle),mySecondAngle(aSecondAngle)
			//Standard_NumericError_Raise_if(!CheckAngle(aFirstAngle, mySecondAngle),
			//			   "networks are parallel");

			/*Standard_NegativeValue_Raise_if(aXStep < 0. , "invalid x step");
			Standard_NegativeValue_Raise_if(aYStep < 0. , "invalid y step");
			Standard_NullValue_Raise_if(aXStep == 0. , "invalid x step");
			Standard_NullValue_Raise_if(aYStep == 0. , "invalid y step");*/
		}

	}
}