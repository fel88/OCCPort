namespace OCCPort
{
    public enum Graphic3d_DisplayPriority
    {
        //Structure priority - range(do not change this range!). Values are between 0 and 10, with 5 used by default. A structure of priority 10 is displayed the last and appears over the others(considering depth test).
        Graphic3d_DisplayPriority_INVALID = -1,
        Graphic3d_DisplayPriority_Bottom = 0,
        Graphic3d_DisplayPriority_AlmostBottom = 1,
        Graphic3d_DisplayPriority_Below2 = 2,
        Graphic3d_DisplayPriority_Below1 = 3,
        Graphic3d_DisplayPriority_Below = 4,
        Graphic3d_DisplayPriority_Normal = 5,
        Graphic3d_DisplayPriority_Above = 6,
        Graphic3d_DisplayPriority_Above1 = 7,
        Graphic3d_DisplayPriority_Above2 = 8,
        Graphic3d_DisplayPriority_Highlight = 9,
        Graphic3d_DisplayPriority_Topmost = 10,
        //enum    { Graphic3d_DisplayPriority_NB = Graphic3d_DisplayPriority_Topmost - Graphic3d_DisplayPriority_Bottom + 1 }
    }
}
