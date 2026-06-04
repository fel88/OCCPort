namespace TKService
{
    //! Contains segments array definition.
    public class Graphic3d_ArrayOfSegments : Graphic3d_ArrayOfPrimitives
    {


        public Graphic3d_ArrayOfSegments(int theMaxVertexs,
                             int theMaxEdges = 0,
                             bool theHasVColors = false)
            : base(Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_SEGMENTS, theMaxVertexs, 0, theMaxEdges,
                 theHasVColors ? Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_VertexColor :
               Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_None)
        {

        }

        Graphic3d_IndexBuffer myIndices;

        public int AddEdge(int theVertexIndex)
        {
            //Standard_OutOfRange_Raise_if(myIndices.IsNull() || myIndices->NbElements >= myIndices->NbMaxElements(), "TOO many EDGE");
            //Standard_OutOfRange_Raise_if(theVertexIndex < 1 || theVertexIndex > myAttribs->NbElements, "BAD VERTEX index");
            int aVertIndex = theVertexIndex - 1;
            myIndices.SetIndex(myIndices.NbElements, aVertIndex);
            return ++myIndices.NbElements;
        }


    }
    public enum Aspect_TypeOfHighlightMethod
    {

        //	Definition of a highlight method.

        //TOHM_COLOR drawn in the highlight color(default white) TOHM_BOUNDBOX enclosed by the boundary box(default white)

        Aspect_TOHM_COLOR,
        Aspect_TOHM_BOUNDBOX
    }

}
