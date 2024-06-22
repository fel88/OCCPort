namespace OCCPort
{
	//! Raw color for defining list of standard color
	public class Quantity_StandardColor
	{
		public string StringName;
		public float[] sRgbValues;
		public float[] RgbValues;
		public Quantity_NameOfColor EnumName;

		/*Quantity_StandardColor(Quantity_NameOfColor theName,
							const char* theStringName,
							const NCollection_Vec3<float>& thesRGB,
                            const NCollection_Vec3<float>& theRGB)
    : StringName(theStringName),
      sRgbValues(thesRGB),
      RgbValues(theRGB),
      EnumName(theName) { }*/
	};

}