namespace OCCPort
{
    public enum Aspect_GradientFillMethod
    {

        //Defines the fill methods to write gradient background in a window.

        Aspect_GradientFillMethod_None,

        //fill method not specified
        Aspect_GradientFillMethod_Horizontal,

        //gradient directed from left (Color1) to right (Color2)
        Aspect_GradientFillMethod_Vertical,

        //gradient directed from top (Color1) to bottom (Color2)
        Aspect_GradientFillMethod_Diagonal1,

        //gradient directed from upper left corner (Color1) to lower right (Color2)
        Aspect_GradientFillMethod_Diagonal2,

        //gradient directed from upper right corner (Color1) to lower left (Color2)
        Aspect_GradientFillMethod_Corner1,
        //
        //highlights upper left corner with Color1
        Aspect_GradientFillMethod_Corner2,

        //highlights upper right corner with Color1
        Aspect_GradientFillMethod_Corner3,

        //highlights lower right corner with Color1
        Aspect_GradientFillMethod_Corner4,

        //highlights lower left corner with Color1
        Aspect_GradientFillMethod_Elliptical,

        //gradient directed from center (Color1) in all directions forming an elliptic shape (Color2)
        Aspect_GFM_NONE,
        Aspect_GFM_HOR,
        Aspect_GFM_VER,
        Aspect_GFM_DIAG1,
        Aspect_GFM_DIAG2,
        Aspect_GFM_CORNER1,
        Aspect_GFM_CORNER2,
        Aspect_GFM_CORNER3,
        Aspect_GFM_CORNER4
    }

}