namespace TKService
{
    //! Defines the fill methods to write gradient background in a window.
    public enum Aspect_GradientFillMethod
    {

        Aspect_GradientFillMethod_None,        //!< fill method not specified
        Aspect_GradientFillMethod_Horizontal,  //!< gradient directed from left (Color1) to right (Color2)
        Aspect_GradientFillMethod_Vertical,    //!< gradient directed from top (Color1) to bottom (Color2)
        Aspect_GradientFillMethod_Diagonal1,   //!< gradient directed from upper left corner (Color1) to lower right (Color2)
        Aspect_GradientFillMethod_Diagonal2,   //!< gradient directed from upper right corner (Color1) to lower left (Color2)
        Aspect_GradientFillMethod_Corner1,     //!< highlights upper left corner with Color1
        Aspect_GradientFillMethod_Corner2,     //!< highlights upper right corner with Color1
        Aspect_GradientFillMethod_Corner3,     //!< highlights lower right corner with Color1
        Aspect_GradientFillMethod_Corner4,     //!< highlights lower left corner with Color1
        Aspect_GradientFillMethod_Elliptical,  //!< gradient directed from center (Color1) in all directions forming an elliptic shape (Color2)

        // obsolete aliases
        Aspect_GFM_NONE = Aspect_GradientFillMethod_None,
        Aspect_GFM_HOR = Aspect_GradientFillMethod_Horizontal,
        Aspect_GFM_VER = Aspect_GradientFillMethod_Vertical,
        Aspect_GFM_DIAG1 = Aspect_GradientFillMethod_Diagonal1,
        Aspect_GFM_DIAG2 = Aspect_GradientFillMethod_Diagonal2,
        Aspect_GFM_CORNER1 = Aspect_GradientFillMethod_Corner1,
        Aspect_GFM_CORNER2 = Aspect_GradientFillMethod_Corner2,
        Aspect_GFM_CORNER3 = Aspect_GradientFillMethod_Corner3,
        Aspect_GFM_CORNER4 = Aspect_GradientFillMethod_Corner4
    }

    //! Helper class that implements transformation matrix functionality.
    public class Graphic3d_TransformUtils
    {
        public static void Ortho2D(Graphic3d_Mat4 theOut,
            float theLeft,
            float theRight, float theBottom, float theTop)
        {

            Ortho(theOut, theLeft, theRight, theBottom, theTop, -1.0f, 1.0f);
        }

        public static void Ortho(Graphic3d_Mat4 theOut,
                                      float theLeft,
                                      float theRight,
                                      float theBottom,
                                      float theTop,
                                      float theZNear,
                                      float theZFar)
        {
            theOut.InitIdentity();

            var aData = theOut.ChangeData();

            var anInvDx = (1.0) / (theRight - theLeft);
            var anInvDy = (1.0) / (theTop - theBottom);
            var anInvDz = (1.0) / (theZFar - theZNear);

            aData[0] = (float)((2.0) * anInvDx);
            aData[5] = (float)((2.0) * anInvDy);
            aData[10] = (float)((-2.0) * anInvDz);

            aData[12] = (float)(-(theRight + theLeft) * anInvDx);
            aData[13] = (float)(-(theTop + theBottom) * anInvDy);
            aData[14] = (float)(-(theZFar + theZNear) * anInvDz);
        }

    }
}
