namespace OCCPort
{
    public enum Graphic3d_TypeOfPrimitiveArray
    {

        //        The type of primitive array in a group in a structure.

        Graphic3d_TOPA_UNDEFINED,

        //undefined primitive type
        Graphic3d_TOPA_POINTS,

        //individual points
        Graphic3d_TOPA_SEGMENTS,

        //segments array - each 2 vertexes define 1 segment
        Graphic3d_TOPA_POLYLINES,

        //line strip - each new vertex in array defines segment with previous one
        Graphic3d_TOPA_TRIANGLES,

        //triangle array - each 3 vertexes define 1 triangle
        Graphic3d_TOPA_TRIANGLESTRIPS,

        //triangle strip - each new vertex in array defines triangle with 2 previous vertexes
        Graphic3d_TOPA_TRIANGLEFANS,

        //triangle fan - each new vertex in array define triangle with the previous vertex and the very first vertex(fan center)
        Graphic3d_TOPA_LINES_ADJACENCY,

        //ADVANCED - same as Graphic3d_TOPA_SEGMENTS, but each pair of vertexes defining 1 segment is preceded by 1 extra vertex and followed by 1 extra vertex which are not actually rendered.
        Graphic3d_TOPA_LINE_STRIP_ADJACENCY,

        //ADVANCED - same as Graphic3d_TOPA_POLYLINES, but each sequence of vertexes defining 1 polyline is preceded by 1 extra vertex and followed by 1 extra vertex which are not actually rendered.
        Graphic3d_TOPA_TRIANGLES_ADJACENCY,

        //ADVANCED - same as Graphic3d_TOPA_TRIANGLES, but each vertex defining of triangle is followed by 1 extra adjacent vertex which is not actually rendered.
        Graphic3d_TOPA_TRIANGLE_STRIP_ADJACENCY,

        //ADVANCED - same as Graphic3d_TOPA_TRIANGLESTRIPS, but with extra adjacent vertexes.
        Graphic3d_TOPA_QUADRANGLES,

        //DEPRECATED - triangle array should be used instead; array of quads - each 4 vertexes define single quad.
        Graphic3d_TOPA_QUADRANGLESTRIPS,

        //DEPRECATED - triangle array should be used instead; quad strip - each 2 new vertexes define a quad shared 2 more vertexes of previous quad.
        Graphic3d_TOPA_POLYGONS,

        //DEPRECATED - triangle array should be used instead; array defines a polygon.
    }
}