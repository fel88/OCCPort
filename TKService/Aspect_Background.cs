using TKernel;

namespace TKService
{
    //! This class allows the definition of
    //! a window background.
    public class Aspect_Background
    {
        public Aspect_Background()
        {

            Quantity_Color MatraGray=new Quantity_Color (Quantity_NameOfColor.Quantity_NOC_MATRAGRAY);

            MyColor = MatraGray;
        }

        public Quantity_Color MyColor;
        public Quantity_Color Color()
        {

            return (MyColor);

        }
    }
}



