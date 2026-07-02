using OpenTK.Mathematics;
using System.Threading;
using TKernel;

namespace OCCPort
{
    public class Quantity_ColorRGBA
    {

        //! Return alpha value (1.0 means opaque, 0.0 means fully transparent).
        public float Alpha() { return myAlpha; }
        //! Assign RGB color components without affecting alpha value.
        public void SetRGB(Quantity_Color theRgb) { myRgb = theRgb; }

        public static Vector4 Convert_LinearRGB_To_sRGB(Vector4 theRGB)
        {
            return new Vector4(Quantity_Color.Convert_LinearRGB_To_sRGB(theRGB.X),
                                   Quantity_Color.Convert_LinearRGB_To_sRGB(theRGB.Y),
                                   Quantity_Color.Convert_LinearRGB_To_sRGB(theRGB.Z),
                                   theRGB.W);
        }

        public void ChangeRGB(Quantity_Color theColor)
        {
            myRgb = theColor;
        }


        //! Return RGB color value.
        public Quantity_Color GetRGB() { return myRgb; }



        //! Creates the color with specified RGB value.
        public Quantity_ColorRGBA(Quantity_Color theRgb)
        {
            myRgb = (theRgb);
            myAlpha = (1.0f);
        }


        //! Creates the color from RGBA values.
        public Quantity_ColorRGBA(float theRed, float theGreen, float theBlue, float theAlpha)
        {
            myRgb = new(theRed, theGreen, theBlue, Quantity_TypeOfColor.Quantity_TOC_RGB);
    myAlpha = (theAlpha);

        }

        Quantity_Color myRgb;

        float myAlpha;
    }
}