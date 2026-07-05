namespace TKService
{
    // Enumeration for polygon offset modes
    public enum Aspect_PolygonOffsetMode
    {
        Aspect_POM_Off = 0x00,  /* all polygon offset modes disabled                     */
        Aspect_POM_Fill = 0x01,  /* GL_POLYGON_OFFSET_FILL enabled (shaded polygons)      */
        Aspect_POM_Line = 0x02,  /* GL_POLYGON_OFFSET_LINE enabled (polygons as outlines) */
        Aspect_POM_Point = 0x04,  /* GL_POLYGON_OFFSET_POINT enabled (polygons as vertices)*/
        Aspect_POM_All = Aspect_POM_Fill | Aspect_POM_Line | Aspect_POM_Point,
        Aspect_POM_None = 0x08,  /* do not change current polygon offset mode             */
        Aspect_POM_Mask = Aspect_POM_All | Aspect_POM_None
    }
}
