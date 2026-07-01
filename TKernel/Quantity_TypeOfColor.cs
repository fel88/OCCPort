namespace TKernel
{//! Generic matrix of 4 x 4 elements.
 //! Identifies color definition systems.
    public enum Quantity_TypeOfColor
    {

        
//Normalized linear RGB (red, green, blue) values within range [0..1] for each component.
        Quantity_TOC_RGB,

        //Normalized non-linear gamma-shifted RGB (red, green, blue) values within range [0..1] for each component. 
        Quantity_TOC_sRGB,

        

        //! Hue + light + saturation components, where:
        //! - First component is the Hue (H) angle in degrees within range [0.0; 360.0], 0.0 being Red;
        //!   value -1.0 is a special value reserved for grayscale color (S should be 0.0).
        //! - Second component is the Lightness (L) within range [0.0; 1.0]
        //! - Third component is the Saturation (S) within range [0.0; 1.0]
  Quantity_TOC_HLS,

        //! CIE L*a*b* color space, constructed to be perceptually uniform for human eye.
        //! The values are assumed to be with respect to D65 2&deg; white point. 
        //!
        //! The color is defined by:
        //! - L: lightness in range [0, 100] (from black to white)
        //! - a: green-to-red axis, approximately in range [-90, 100]
        //! - b: blue-to-yellow axis, approximately in range [-110, 95]
        //!
        //! Note that not all combinations of L, a, and b values represent visible
        //! colors, and RGB cube takes only part of visible color space.
        //!
        //! When Lab color is converted to RGB, a and b components may be reduced
        //! (with the same proportion) to fit the result into the RGB range.
        Quantity_TOC_CIELab,

        //! CIE L*c*h* color space, same as L*a*b* in cylindrical coordinates:
        //! - L: lightness in range [0, 100] (from black to white)
        //! - c: chroma, approximately in range [0, 135], 0 corresponds to greyscale
        //! - h: hue angle, in range [0., 360.]
        //!
        //! The hue values of standard colors are approximately:
        //! - red at 40, 
        //! - yellow at 103,
        //! - green at 136,
        //! - cyan at 196,
        //! - blue at 306,
        //! - magenta at 328.
        //!
        //! When Lch color is converted to RGB, chroma component may be reduced
        //! to fit the color into the RGB range.
        Quantity_TOC_CIELch
    }
}
