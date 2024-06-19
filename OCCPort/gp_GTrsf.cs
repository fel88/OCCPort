namespace OCCPort
{
	public class gp_GTrsf
    {

		public gp_GTrsf(gp_Trsf theT)
		{
			shape = theT.Form();
			matrix = theT.matrix;
			loc = theT.TranslationPart();
			scale = theT.ScaleFactor();
		}

		gp_Mat matrix;
		gp_XYZ loc;
		gp_TrsfForm shape;
		double scale;

    }
}