using TKernel;

namespace TKService
{
    //! This class allows the definition of a window gradient background.
    public class Aspect_GradientBackground : Aspect_Background
    {

        public Aspect_GradientBackground(Quantity_Color AColor1,
                                                      Quantity_Color AColor2,
                                                      Aspect_GradientFillMethod AMethod)
        {
            SetColor(AColor1);
            MyColor2 = AColor2;
            MyGradientMethod = AMethod;
        }

        public Aspect_GradientFillMethod BgGradientFillMethod()
        {
            return MyGradientMethod;
        }

        //! Returns colours of the window gradient background.
        public  void Colors(out Quantity_Color AColor1, out Quantity_Color AColor2)
        {
            AColor1 = Color();
            AColor2 = MyColor2;
        }
       

        public void SetColor(Quantity_Color AColor)
        {
            MyColor = AColor;
        }

        Quantity_Color MyColor2;
        Aspect_GradientFillMethod MyGradientMethod;

    }
}



