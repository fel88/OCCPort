namespace TKernel
{//! Generic matrix of 4 x 4 elements.
 //! Raw color for defining list of standard color
    public class Quantity_StandardColor
    {
        public string StringName;
        public NCollection_Vec3<float> sRgbValues = new NCollection_Vec3<float>();
        public NCollection_Vec3<float> RgbValues = new NCollection_Vec3<float>();
        public Quantity_NameOfColor EnumName;

        public Quantity_StandardColor(Quantity_NameOfColor theName,
                            string theStringName,
                            NCollection_Vec3<float> thesRGB,
                            NCollection_Vec3<float> theRGB)
        {
            StringName = (theStringName);
            sRgbValues = (thesRGB);
            RgbValues = (theRGB);
            EnumName = (theName);
        }
    };
}
