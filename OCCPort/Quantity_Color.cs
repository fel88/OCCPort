using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace OCCPort
{
	public class Quantity_Color
	{
		//! Creates the color from enumeration value.
		public Quantity_Color(Quantity_NameOfColor theName)
		{
			myRgb = (valuesOf(theName, Quantity_TypeOfColor.Quantity_TOC_RGB));
		}
		float[] myRgb = new float[3];

		//! Returns the values of a predefined color according to the mode.
		static float[] valuesOf(Quantity_NameOfColor theName,

														   Quantity_TypeOfColor theType)
		{

			if ((int)theName < 0 || (int)theName > (int)Quantity_NameOfColor.Quantity_NOC_WHITE)
			{
				throw new Standard_OutOfRange("Bad name");
			}

			float[] anRgb = THE_COLORS(theName).RgbValues;
			switch (theType)
			{
				case Quantity_TypeOfColor.Quantity_TOC_RGB: return anRgb;
				case Quantity_TypeOfColor.Quantity_TOC_sRGB: return Convert_LinearRGB_To_sRGB(anRgb);
					//case Quantity_TOC_HLS: return Convert_LinearRGB_To_HLS(anRgb);
					//	case Quantity_TOC_CIELab: return Convert_LinearRGB_To_Lab(anRgb);
					//case Quantity_TOC_CIELch: return Convert_Lab_To_Lch(Convert_LinearRGB_To_Lab(anRgb));
			}
			throw new Standard_ProgramError("Internal error");

		}

		//todo: replace with dictionary
		static Quantity_StandardColor[] _the_colors = null;
		private static Quantity_StandardColor THE_COLORS(Quantity_NameOfColor theName)
		{
			if (_the_colors == null)
			{
				//parse color from pxx table
				var rr = ResourceHelper.ReadResourceTxt("Quantity_ColorTable.pxx");
				List<Quantity_StandardColor> l = new List<Quantity_StandardColor>();
				var ss = new StringReader(rr);
				string str = null;
				while ((str = ss.ReadLine()) != null)
				{
					//parse str
				}
			}
			var fr = _the_colors.First(z => theName.ToString().Contains(z.StringName));
			return fr;
		}

		private static float[] Convert_LinearRGB_To_sRGB(float[] anRgb)
		{
			throw new NotImplementedException();
		}
	}

}