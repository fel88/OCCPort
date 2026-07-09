using Microsoft.VisualBasic;
using OCCPort.Common;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using TKernel;

namespace TKernel
{
    //! This class allows the definition of an RGB color as triplet of 3 normalized floating point values (red, green, blue).
    //!
    //! Although Quantity_Color can be technically used for pass-through storage of RGB triplet in any color space,
    //! other OCCT interfaces taking/returning Quantity_Color would expect them in linear space.
    //! Therefore, take a look into methods converting to and from non-linear sRGB color space, if needed;
    //! for instance, application usually providing color picking within 0..255 range in sRGB color space.
    public struct Quantity_Color
    {
        //! Creates the color from enumeration value.
        public Quantity_Color(Quantity_NameOfColor theName)
        {
            myRgb = (valuesOf(theName, Quantity_TypeOfColor.Quantity_TOC_RGB));
        }
        //! Creates a color according to the definition system theType.
        //! Throws exception if values are out of range.
        public Quantity_Color(double theC1,
                                   double theC2,
                                   double theC3,
                                   Quantity_TypeOfColor theType)
        {
            SetValues(theC1, theC2, theC3, theType);

        }
        public Quantity_Color(NCollection_Vec3<float> theRgb)
        {
            myRgb = new NCollection_Vec3<float>(theRgb);
            Quantity_ColorValidateRgbRange(theRgb.r(), theRgb.g(), theRgb.b());
        }

        // Throw exception if RGB values are out of range.
        void Quantity_ColorValidateRgbRange(double theR, double theG, double theB)
        {
            if (theR < 0.0 || theR > 1.0
   || theG < 0.0 || theG > 1.0
   || theB < 0.0 || theB > 1.0) { throw new Standard_OutOfRange("Color out"); }
        }
        //! Convert sRGB component into linear RGB using OpenGL specs formula (double precision), also known as gamma correction.
        static double Convert_sRGB_To_LinearRGB(double thesRGBValue)
        {
            return thesRGBValue <= 0.04045
                 ? thesRGBValue / 12.92
                 : Math.Pow((thesRGBValue + 0.055) / 1.055, 2.4);
        }
        void SetValues(double theC1, double theC2, double theC3,
                                 Quantity_TypeOfColor theType)
        {
            switch (theType)
            {
                case Quantity_TypeOfColor.Quantity_TOC_RGB:
                    {
                        Quantity_ColorValidateRgbRange(theC1, theC2, theC3);
                        myRgb.SetValues((float)(theC1), (float)(theC2), (float)(theC3));
                        break;
                    }
                case Quantity_TypeOfColor.Quantity_TOC_sRGB:
                    {
                        Quantity_ColorValidateRgbRange(theC1, theC2, theC3);
                        myRgb.SetValues((float)Convert_sRGB_To_LinearRGB(theC1),
                                           (float)Convert_sRGB_To_LinearRGB(theC2),
                                           (float)Convert_sRGB_To_LinearRGB(theC3));
                        break;
                    }
                case Quantity_TypeOfColor.Quantity_TOC_HLS:
                    {
                        throw new NotImplementedException();
                        //Quantity_ColorValidateHlsRange(theC1, theC2, theC3);
                        // myRgb = Convert_HLS_To_LinearRGB(NCollection_Vec3<float>(float(theC1), float(theC2), float(theC3)));
                        break;
                    }
                case Quantity_TypeOfColor.Quantity_TOC_CIELab:
                    {
                        throw new NotImplementedException();

                        //  Quantity_ColorValidateLabRange(theC1, theC2, theC3);
                        //   myRgb = Convert_Lab_To_LinearRGB(NCollection_Vec3<float>(float(theC1), float(theC2), float(theC3)));
                        break;
                    }
                case Quantity_TypeOfColor.Quantity_TOC_CIELch:
                    {
                        throw new NotImplementedException();

                        // Quantity_ColorValidateLchRange(theC1, theC2, theC3);
                        // myRgb = Convert_Lab_To_LinearRGB(Convert_Lch_To_Lab(NCollection_Vec3<float>(float(theC1), float(theC2), float(theC3))));
                        break;
                    }
            }
        }

        public static implicit operator NCollection_Vec3<float>(Quantity_Color f)
        {
            return new NCollection_Vec3<float>(f.myRgb);
        }


        // =======================================================================
        // function : Convert_LinearRGB_To_Lab
        // purpose  : convert RGB color to CIE Lab color
        // see https://www.easyrgb.com/en/math.php
        // =======================================================================
        NCollection_Vec3<float> Convert_LinearRGB_To_Lab(NCollection_Vec3<float> theRgb)
        {
            double aR = theRgb[0];
            double aG = theRgb[1];
            double aB = theRgb[2];

            // convert to XYZ normalized to D65 / 2 deg (CIE 1931) standard illuminant intensities
            // see http://www.brucelindbloom.com/index.html?Equations.html
            double aX = (aR * 0.4124564 + aG * 0.3575761 + aB * 0.1804375) * 100.0 / 95.047;
            double aY = (aR * 0.2126729 + aG * 0.7151522 + aB * 0.0721750) * 100.0 / 100.000;
            double aZ = (aR * 0.0193339 + aG * 0.1191920 + aB * 0.9503041) * 100.0 / 108.883;

            // convert to Lab
            double afX = CIELab_f(aX);
            double afY = CIELab_f(aY);
            double afZ = CIELab_f(aZ);

            double aL = 116.0 * afY - 16.0;
            double aa = 500.0 * (afX - afY);
            double ab = 200.0 * (afY - afZ);

            return new NCollection_Vec3<float>((float)aL, (float)aa, (float)ab);
        }

        // =======================================================================
        // function : CIELab_f
        // purpose  : non-linear function transforming XYZ coordinates to CIE Lab
        // see http://www.brucelindbloom.com/index.html?Equations.html
        // =======================================================================
        static double CIELab_f(double theValue)
        {
            return theValue > 0.008856451679035631 ? Math.Pow(theValue, 1.0 / 3.0) : (7.787037037037037 * theValue) + 16.0 / 116.0;
        }
        public void Values(ref double theR1, ref double theR2, ref double theR3,
                              Quantity_TypeOfColor theType)
        {
            switch (theType)
            {
                case Quantity_TypeOfColor.Quantity_TOC_RGB:
                    {
                        theR1 = myRgb.r();
                        theR2 = myRgb.g();
                        theR3 = myRgb.b();
                        break;
                    }
                case Quantity_TypeOfColor.Quantity_TOC_sRGB:
                    {
                        theR1 = Convert_LinearRGB_To_sRGB((double)myRgb.r());
                        theR2 = Convert_LinearRGB_To_sRGB((double)myRgb.g());
                        theR3 = Convert_LinearRGB_To_sRGB((double)myRgb.b());
                        break;
                    }
                case Quantity_TypeOfColor.Quantity_TOC_HLS:
                    {
                        NCollection_Vec3<float> aHls = Convert_LinearRGB_To_HLS(myRgb);
                        theR1 = aHls[0];
                        theR2 = aHls[1];
                        theR3 = aHls[2];
                        break;
                    }
                case Quantity_TypeOfColor.Quantity_TOC_CIELab:
                    {
                        NCollection_Vec3<float> aLab = Convert_LinearRGB_To_Lab(myRgb);
                        theR1 = aLab[0];
                        theR2 = aLab[1];
                        theR3 = aLab[2];
                        break;
                    }
                case Quantity_TypeOfColor.Quantity_TOC_CIELch:
                    {
                        NCollection_Vec3<float> aLch = Convert_Lab_To_Lch(Convert_LinearRGB_To_Lab(myRgb));
                        theR1 = aLch[0];
                        theR2 = aLch[1];
                        theR3 = aLch[2];
                        break;
                    }
            }

        }


        // =======================================================================
        // function : Convert_Lab_To_Lch
        // purpose  : convert CIE Lab color to CIE Lch color
        // see https://www.easyrgb.com/en/math.php
        // =======================================================================
        NCollection_Vec3<float> Convert_Lab_To_Lch(NCollection_Vec3<float> theLab)
        {
            double aa = theLab[1];
            double ab = theLab[2];

            double aC = Math.Sqrt(aa * aa + ab * ab);
            double aH = (aC > TheEpsilon ? Math.Atan2(ab, aa) * 180.0 / Math.PI : 0.0);

            if (aH < 0.0)
                aH += 360.0;

            return new NCollection_Vec3<float>(theLab[0], (float)aC, (float)aH);
        }

        NCollection_Vec3<float> myRgb = new NCollection_Vec3<float>();

        //! Return the color as vector of 3 float elements.
        public NCollection_Vec3<float> Rgb() { return myRgb; }

        //! Returns the values of a predefined color according to the mode.
        static NCollection_Vec3<float> valuesOf(Quantity_NameOfColor theName,

                                                           Quantity_TypeOfColor theType)
        {

            if ((int)theName < 0 || (int)theName > (int)Quantity_NameOfColor.Quantity_NOC_WHITE)
            {
                throw new Standard_OutOfRange("Bad name");
            }

            NCollection_Vec3<float> anRgb = THE_COLORS(theName).RgbValues;
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
                    var r = float.Parse(spl[7], NumberStyles.Any, CultureInfo.InvariantCulture);
                    var g = float.Parse(spl[8], NumberStyles.Any, CultureInfo.InvariantCulture);
                    var b = float.Parse(spl[9], NumberStyles.Any, CultureInfo.InvariantCulture);
                    NCollection_Vec3<float> rgb = new NCollection_Vec3<float>(r, g, b);

                    r = float.Parse(spl[3], NumberStyles.Any, CultureInfo.InvariantCulture);
                    g = float.Parse(spl[4], NumberStyles.Any, CultureInfo.InvariantCulture);
                    b = float.Parse(spl[5], NumberStyles.Any, CultureInfo.InvariantCulture);
                    var s = Convert.ToUInt32(spl[2], 16);
                    NCollection_Vec3<float> srgb = new NCollection_Vec3<float>(r, g, b);



                    ret.Add(new Quantity_StandardColor((Quantity_NameOfColor)Enum.Parse(typeof(Quantity_NameOfColor), name), name, rgb, srgb));



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
            return (new NCollection_Vec3<float>(myRgb) - new NCollection_Vec3<float>(theColor.myRgb)).SquareModulus();
        }

        private static NCollection_Vec3<float> Convert_LinearRGB_To_sRGB(NCollection_Vec3<float> theRGB)
        {
            return new NCollection_Vec3<float>(Convert_LinearRGB_To_sRGB(theRGB.r()),
                              Convert_LinearRGB_To_sRGB(theRGB.g()),
                              Convert_LinearRGB_To_sRGB(theRGB.b()));
        }


        //! Converts Linear RGB components into HLS ones.
        static NCollection_Vec3<float> Convert_LinearRGB_To_HLS(NCollection_Vec3<float> theRgb)
        {
            return Convert_sRGB_To_HLS(Convert_LinearRGB_To_sRGB(theRgb));
        }

        // =======================================================================
        // function : Convert_sRGB_To_HLS
        // purpose  : Reference: La synthese d'images, Collection Hermes
        // =======================================================================
        static NCollection_Vec3<float> Convert_sRGB_To_HLS(NCollection_Vec3<float> theRgb)
        {

            const float RGBHLS_H_UNDEFINED = -1.0f;

            float aPlus = 0.0f;
            float aDiff = theRgb.g() - theRgb.b();

            // compute maximum from RGB components, which will be a luminance
            float aMax = theRgb.r();
            if (theRgb.g() > aMax) { aPlus = 2.0f; aDiff = theRgb.b() - theRgb.r(); aMax = theRgb.g(); }
            if (theRgb.b() > aMax) { aPlus = 4.0f; aDiff = theRgb.r() - theRgb.g(); aMax = theRgb.b(); }

            // compute minimum from RGB components
            float min = theRgb.r();
            if (theRgb.g() < min) min = theRgb.g();
            if (theRgb.b() < min) min = theRgb.b();

            float aDelta = aMax - min;

            // compute saturation
            float aSaturation = 0.0f;
            if (aMax != 0.0f) aSaturation = aDelta / aMax;

            // compute hue
            float aHue = RGBHLS_H_UNDEFINED;
            if (aSaturation != 0.0f)
            {
                aHue = 60.0f * (aPlus + aDiff / aDelta);
                if (aHue < 0.0f) aHue += 360.0f;
            }
            return new NCollection_Vec3<float>(aHue, aMax, aSaturation);
        }

        //! Convert linear RGB component into sRGB using OpenGL specs formula (double precision), also known as gamma correction.
        static double Convert_LinearRGB_To_sRGB(double theLinearValue)
        {
            return theLinearValue <= 0.0031308
                 ? theLinearValue * 12.92
                 : Math.Pow(theLinearValue, 1.0 / 2.4) * 1.055 - 0.055;
        }
        //! Convert linear RGB component into sRGB using OpenGL specs formula (single precision), also known as gamma correction.
        internal static float Convert_LinearRGB_To_sRGB(float theLinearValue)
        {
            return theLinearValue <= 0.0031308f
        ? theLinearValue * 12.92f
        : (float)Math.Pow(theLinearValue, 1.0f / 2.4f) * 1.055f - 0.055f;
        }


        //! Returns TRUE if the distance between two colors is greater than Epsilon().
        public bool IsDifferent(Quantity_Color theOther) { return (SquareDistance(theOther) > Epsilon() * Epsilon()); }


    }

}