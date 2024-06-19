namespace OCCPort
{
    public enum Graphic3d_DisplayPriority
    {
        //Structure priority - range(do not change this range!). Values are between 0 and 10, with 5 used by default. A structure of priority 10 is displayed the last and appears over the others(considering depth test).

        Graphic3d_DisplayPriority_INVALID,
        Graphic3d_DisplayPriority_Bottom,
        Graphic3d_DisplayPriority_AlmostBottom,
        Graphic3d_DisplayPriority_Below2,
        Graphic3d_DisplayPriority_Below1,
        Graphic3d_DisplayPriority_Below,
        Graphic3d_DisplayPriority_Normal,
        Graphic3d_DisplayPriority_Above,
        Graphic3d_DisplayPriority_Above1,
        Graphic3d_DisplayPriority_Above2,
        Graphic3d_DisplayPriority_Highlight,
        Graphic3d_DisplayPriority_Topmost
        //enum    { Graphic3d_DisplayPriority_NB = Graphic3d_DisplayPriority_Topmost - Graphic3d_DisplayPriority_Bottom + 1 }
    }
}
