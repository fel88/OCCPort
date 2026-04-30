using System;

namespace OCCPort.Tester
{
    internal class Graphic3d_ArrayOfTriangles: Graphic3d_ArrayOfPrimitives
    {

        //! Creates an array of triangles (Graphic3d_TOPA_TRIANGLES).
        //! @param theMaxVertexs  defines the maximum allowed vertex number in the array
        //! @param theMaxEdges    defines the maximum allowed edge   number in the array
        //! @param theHasVNormals when TRUE,  AddVertex(Point,Normal), AddVertex(Point,Normal,Color) or AddVertex(Point,Normal,Texel) should be used to specify vertex normal;
        //!                       vertex normals should be specified coherent to triangle orientation (defined by order of vertexes within triangle) for proper rendering
        //! @param theHasVColors  when TRUE,  AddVertex(Point,Color) or AddVertex(Point,Normal,Color) should be used to specify vertex color
        //! @param theHasVTexels  when TRUE,  AddVertex(Point,Texel) or AddVertex(Point,Normal,Texel) should be used to specify vertex UV coordinates
      public  Graphic3d_ArrayOfTriangles(int theMaxVertexs,
                                    int theMaxEdges = 0,
                                    bool theHasVNormals = false,
                                    bool theHasVColors = false,
                                    bool theHasVTexels = false)
            :base(Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_TRIANGLES, theMaxVertexs, 0, theMaxEdges,
                                 (theHasVNormals? Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_VertexNormal : Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_None)
                               | (theHasVColors? Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_VertexColor : Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_None)
                               | (theHasVTexels? Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_VertexTexel : Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_None)) {}
        

        
    }
}