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
                List<Quantity_StandardColor> ret = new List<Quantity_StandardColor>();
                while ((str = ss.ReadLine()) != null)
                {/*
                  
// Note that HTML/hex sRGB representation is ignored
#define RawColor(theName, theHex, SRGB, sR, sG, sB, RGB, theR, theG, theB) \
  Quantity_StandardColor(Quantity_NOC_##theName, #theName, NCollection_Vec3<float>(sR##f, sG##f, sB##f), NCollection_Vec3<float>(theR##f, theG##f, theB##f))
*/

                    var spl = str.Replace("RawColor", "").Split(new char[] { '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries).Select(z => z.Trim()).Where(z => !string.IsNullOrEmpty(z)).ToArray();


                    var name = "Quantity_NOC_" + spl[0];
                    var r = float.Parse(spl[7]);
                    var g = float.Parse(spl[8]);
                    var b = float.Parse(spl[9]);
                    float[] rgb = new float[] {
                         r,g,b
                    };

                    r = float.Parse(spl[3]);
                    g = float.Parse(spl[4]);
                    b = float.Parse(spl[5]);
                    var s = Convert.ToUInt32(spl[2], 16);
                    float[] srgb = new float[] {
                        s, r,g,b
                    };


                    ret.Add(new Quantity_StandardColor()
                    {
                        StringName = name,
                        RgbValues = rgb,
                        sRgbValues = srgb,
                        EnumName = (Quantity_NameOfColor)Enum.Parse(typeof(Quantity_NameOfColor), name)
                    });



                    //parse str
                }
                _the_colors = ret.ToArray();
            }
            var fr = _the_colors.First(z => theName.ToString().Contains(z.StringName));
            return fr;
        }
        static double TheEpsilon = 0.0001;

        double Epsilon()
        {
            return TheEpsilon;
        }
        //! Returns TRUE if the distance between two colors is no greater than Epsilon().
        public bool IsEqual(Quantity_Color theOther) { return (SquareDistance(theOther) <= Epsilon() * Epsilon()); }
        public bool IsEqual(Quantity_NameOfColor theOther) { return IsEqual(new Quantity_Color(theOther)); }
        //! Returns the square of distance between two colors.
        public double SquareDistance(Quantity_Color theColor)
        {
            return (new NCollection_Vec3(myRgb) - new NCollection_Vec3(theColor.myRgb)).SquareModulus();
        }

        private static float[] Convert_LinearRGB_To_sRGB(float[] anRgb)
        {
            throw new NotImplementedException();
        }
    }

}