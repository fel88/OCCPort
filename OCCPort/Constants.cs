namespace OCCPort
{
    public static class Constants
    {
        public const int Graphic3d_DisplayPriority_NB = Graphic3d_DisplayPriority.Graphic3d_DisplayPriority_Topmost - Graphic3d_DisplayPriority.Graphic3d_DisplayPriority_Bottom + 1;
        public const int AIS_RotationMode_LOWER = 0;
        public const int AIS_RotationMode_UPPER = (int)AIS_RotationMode.AIS_RotationMode_BndBoxScene;
    }
}
