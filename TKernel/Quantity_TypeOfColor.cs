namespace TKernel
{//! Generic matrix of 4 x 4 elements.
 //! Identifies color definition systems.
    public enum Quantity_TypeOfColor
    {

        //Identifies color definition systems.

        Quantity_TOC_RGB,

        //Normalized linear RGB (red, green, blue) values within range [0..1] for each component.
        Quantity_TOC_sRGB

        //Normalized non-linear gamma-shifted RGB (red, green, blue) values within range [0..1] for each component. 
    }
}
