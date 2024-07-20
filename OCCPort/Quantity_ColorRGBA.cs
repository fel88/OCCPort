namespace OCCPort
{
    public class Quantity_ColorRGBA
    {

        //! Return alpha value (1.0 means opaque, 0.0 means fully transparent).
        public float Alpha() { return myAlpha; }
        //! Assign RGB color components without affecting alpha value.
        public void SetRGB(Quantity_Color theRgb) { myRgb = theRgb; }

        //! Creates the color with specified RGB value.
        public Quantity_ColorRGBA(Quantity_Color theRgb)
        {
            myRgb = (theRgb);
            myAlpha = (1.0f);
        }


        Quantity_Color myRgb;

        float myAlpha;
    }

}