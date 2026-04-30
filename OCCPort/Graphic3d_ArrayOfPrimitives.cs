using System;
using System.Security.AccessControl;

namespace OCCPort
{
    public class Graphic3d_ArrayOfPrimitives
    {
        public Graphic3d_ArrayOfPrimitives()
        {
        }

        //! Convenience method, adds three vertex indices (a triangle) in the range [1,VertexNumber()] in the array.
        //! @return the actual edges number
        public int AddEdges(int theVertexIndex1,
                                   int theVertexIndex2,
                                   int theVertexIndex3)
        {
            AddEdge(theVertexIndex1);
            AddEdge(theVertexIndex2);
            return AddEdge(theVertexIndex3);
        }

        public int AddEdge(int theVertexIndex)
        {
            Exceptions.Standard_OutOfRange_Raise_if(myIndices == null || myIndices.NbElements >= myIndices.NbMaxElements(), "TOO many EDGE");
            Exceptions.Standard_OutOfRange_Raise_if(theVertexIndex < 1 || theVertexIndex > myAttribs.NbElements, "BAD VERTEX index");
            int aVertIndex = theVertexIndex - 1;
            myIndices.SetIndex(myIndices.NbElements, aVertIndex);
            return ++myIndices.NbElements;
        }

        //! Returns optional index buffer.
        internal Graphic3d_IndexBuffer Indices()
        {
            return myIndices;
        }

        public Graphic3d_ArrayOfPrimitives(Graphic3d_TypeOfPrimitiveArray theType,
                               int theMaxVertexs,
                               int theMaxBounds,
                               int theMaxEdges,
                               Graphic3d_ArrayFlags theArrayFlags)
        {
            /*myNormData = (null);
            myTexData = (null);
            myColData = (null);*/
            myPosStride = (0);
            myNormStride = (0);/*
            myTexStride = (0);/*
            myColStride = (0);*/
            myType = Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_UNDEFINED;

            init(theType, theMaxVertexs, theMaxBounds, theMaxEdges, theArrayFlags);
        }

        int myNormStride;
        int myPosStride;
        //! Returns the number of defined vertex
        public int VertexNumber() { return myAttribs.NbElements; }

        void init(Graphic3d_TypeOfPrimitiveArray theType,
                                            int theMaxVertexs,
                                            int theMaxBounds,
                                            int theMaxEdges,
                                            Graphic3d_ArrayFlags theArrayOptions)
        {
            myType = theType;
            /*myNormData = NULL;
            myTexData = NULL;
            myColData = NULL;*/
            myAttribs = null;

            myIndices = null;/*
            myBounds.Nullify();*/

            NCollection_BaseAllocator anAlloc = Graphic3d_Buffer.DefaultAllocator();

            if ((theArrayOptions & Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_AttribsMutable) != 0
             || (theArrayOptions & Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_AttribsDeinterleaved) != 0)
            {
                Graphic3d_AttribBuffer _anAttribs = new Graphic3d_AttribBuffer(anAlloc);
                _anAttribs.SetMutable((theArrayOptions & Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_AttribsMutable) != 0);
                _anAttribs.SetInterleaved((theArrayOptions & Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_AttribsDeinterleaved) == 0);
                myAttribs = _anAttribs;
            }
            else
            {
                myAttribs = new Graphic3d_Buffer(anAlloc);
            }
            if (theMaxVertexs < 1)
            {
                return;
            }

            if (theMaxEdges > 0)
            {
                if ((theArrayOptions & Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_IndexesMutable) != 0)
                {
                    myIndices = new Graphic3d_MutableIndexBuffer(anAlloc);
                }
                else
                {
                    myIndices = new Graphic3d_IndexBuffer(anAlloc);
                }
                if (theMaxVertexs < (int)(ushort.MaxValue))
                {
                    if (!myIndices.Init(sizeof(ushort), theMaxEdges))//init ushort
                    {
                        myIndices = null;
                        return;
                    }
                }
                else
                {
                    if (!myIndices.Init(sizeof(uint), (theMaxEdges))) //init uint
                    {
                        myIndices = null;
                        return;
                    }
                }
                myIndices.NbElements = 0;
            }

            Graphic3d_Attribute[] anAttribs = new Graphic3d_Attribute[4];
            for (int i = 0; i < 3; i++)
            {
                anAttribs[i] = new Graphic3d_Attribute();
            }
            int aNbAttribs = 0;
            anAttribs[aNbAttribs].Id = Graphic3d_TypeOfAttribute.Graphic3d_TOA_POS;
            anAttribs[aNbAttribs].DataType = Graphic3d_TypeOfData.Graphic3d_TOD_VEC3;
            ++aNbAttribs;
            if ((theArrayOptions & Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_VertexNormal) != 0)
            {
                anAttribs[aNbAttribs].Id = Graphic3d_TypeOfAttribute.Graphic3d_TOA_NORM;
                anAttribs[aNbAttribs].DataType = Graphic3d_TypeOfData.Graphic3d_TOD_VEC3;
                ++aNbAttribs;
            }
            if ((theArrayOptions & Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_VertexTexel) != 0)
            {
                anAttribs[aNbAttribs].Id = Graphic3d_TypeOfAttribute.Graphic3d_TOA_UV;
                anAttribs[aNbAttribs].DataType = Graphic3d_TypeOfData.Graphic3d_TOD_VEC2;
                ++aNbAttribs;
            }
            if ((theArrayOptions & Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_VertexColor) != 0)
            {
                anAttribs[aNbAttribs].Id = Graphic3d_TypeOfAttribute.Graphic3d_TOA_COLOR;
                anAttribs[aNbAttribs].DataType = Graphic3d_TypeOfData.Graphic3d_TOD_VEC4UB;
                ++aNbAttribs;
            }

            if (!myAttribs.Init(theMaxVertexs, anAttribs, aNbAttribs))
            {
                myAttribs = null;
                myIndices = null;
                return;
            }

            int anAttribDummy = 0;
            myAttribs.AttributeData(Graphic3d_TypeOfAttribute.Graphic3d_TOA_POS, ref anAttribDummy, ref myPosStride);
            myNormData = myAttribs.AttributeData(Graphic3d_TypeOfAttribute.Graphic3d_TOA_NORM, ref anAttribDummy, ref myNormStride);
            //myTexData = myAttribs->ChangeAttributeData(Graphic3d_TOA_UV, anAttribDummy, myTexStride);
            //myColData = myAttribs->ChangeAttributeData(Graphic3d_TOA_COLOR, anAttribDummy, myColStride);

            //      memset(myAttribs->ChangeData(), 0, size_t(myAttribs->Stride) * size_t(myAttribs->NbMaxElements()));
            //      if ((theArrayOptions & Graphic3d_ArrayFlags_AttribsMutable) == 0
            //       && (theArrayOptions & Graphic3d_ArrayFlags_AttribsDeinterleaved) == 0)
            //      {
            //          myAttribs->NbElements = 0;
            //}

            if (theMaxBounds > 0)
            {
                //myBounds = new Graphic3d_BoundBuffer(anAlloc);
                myBounds = new Graphic3d_BoundBuffer();
                if (!myBounds.Init(theMaxBounds, (theArrayOptions & Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_BoundColor) != 0))
                {
                    myAttribs = null;
                    myIndices = null;
                    myBounds = null;
                    return;
                }
                myBounds.NbBounds = 0;
            }
        }

        Graphic3d_IndexBuffer myIndices;
        Graphic3d_Buffer myAttribs;
        Graphic3d_BoundBuffer myBounds;
        //! Adds a vertice and vertex normal in the vertex array.
        //! Warning: theNormal is ignored when the hasVNormals constructor parameter is FALSE.
        //! @return the actual vertex number
        public int AddVertex(gp_Pnt theVertex, gp_Dir theNormal)
        {
            return AddVertex(theVertex.X(), theVertex.Y(), theVertex.Z(),
                              theNormal.X(), theNormal.Y(), theNormal.Z());
        }
        //! Adds a vertice and vertex normal in the vertex array.
        //! Warning: Normal is ignored when the hasVNormals constructor parameter is FALSE.
        //! @return the actual vertex number
        public int AddVertex(double theX, double theY, double theZ,
                             double theNX, double theNY, double theNZ)
        {
            return AddVertex(RealToShortReal(theX), RealToShortReal(theY), RealToShortReal(theZ),
                              (float)(theNX), (float)(theNY), (float)(theNZ));
        }//! Adds a vertice and vertex normal in the vertex array.
         //! Warning: Normal is ignored when the hasVNormals constructor parameter is FALSE.
         //! @return the actual vertex number
        public int AddVertex(float theX, float theY, float theZ,
                              float theNX, float theNY, float theNZ)
        {
            int anIndex = myAttribs.NbElements + 1;
            SetVertice(anIndex, theX, theY, theZ);
            SetVertexNormal(anIndex, theNX, theNY, theNZ);
            return anIndex;
        }
        byte[] myNormData;

        //! Change the vertex normal in the array.
        //! @param[in] theIndex node index within [1, VertexNumberAllocated()] range
        //! @param[in] theNX surface normal X component
        //! @param[in] theNY surface normal Y component
        //! @param[in] theNZ surface normal Z component
        void SetVertexNormal(int theIndex, double theNX, double theNY, double theNZ)
        {
            Exceptions.Standard_OutOfRange_Raise_if(theIndex < 1 || theIndex > myAttribs.NbMaxElements(), "BAD VERTEX index");
            if (myNormData != null)
            {
                //Graphic3d_Vec3 aVec = (Graphic3d_Vec3)(myNormData + myNormStride * ((Standard_Size)theIndex - 1));
                var aVec3 = new Graphic3d_Vec3(myNormData, myNormStride);
                aVec3.SetValues((float)(theNX),
                 (float)(theNY),
                 (float)(theNZ));
            }
            myAttribs.NbElements = Math.Max(theIndex, myAttribs.NbElements);
        }
        const float FLT_MAX = 3.402823466e+38F; // max value

        public float RealToShortReal(double theVal)
        {
            return theVal < -FLT_MAX ? -FLT_MAX
              : theVal > FLT_MAX ? FLT_MAX
              : (float)theVal;
        }

        //! Adds a vertice in the array.
        //! @return the actual vertex number
        public int AddVertex(gp_Pnt theVertex)
        {
            return AddVertex(theVertex.X(), theVertex.Y(), theVertex.Z());
        }
        //! Adds a vertice in the array.
        //! @return the actual vertex number
        public int AddVertex(double theX, double theY, double theZ)
        {
            return AddVertex(Standard_Real.RealToShortReal(theX), Standard_Real.RealToShortReal(theY), Standard_Real.RealToShortReal(theZ));
        }

        public int AddVertex(float theX, float theY, float theZ)
        {

            int anIndex = myAttribs.NbElements + 1;
            SetVertice(anIndex, theX, theY, theZ);
            return anIndex;
        }

        private void SetVertice(int theIndex, float theX, float theY, float theZ)
        {
            Exceptions.Standard_OutOfRange_Raise_if(theIndex < 1 || theIndex > myAttribs.NbMaxElements(), "BAD VERTEX index");
            //Graphic3d_Vec3 aVec = *reinterpret_cast<Graphic3d_Vec3*>(myAttribs->ChangeData() + myPosStride * ((Standard_Size)theIndex - 1));
            Graphic3d_Vec3 aVec = new Graphic3d_Vec3(myAttribs.Data(), theIndex * myPosStride);
            aVec.SetValues(theX, theY, theZ);
            if (myAttribs.NbElements < theIndex)
            {
                myAttribs.NbElements = theIndex;
            }

        }

        internal bool IsValid()
        {
            if (myAttribs == null)
            {
                return false;
            }

            int nvertexs = myAttribs.NbElements;
            int nbounds = myBounds == null ? 0 : myBounds.NbBounds;
            int nedges = myIndices == null ? 0 : myIndices.NbElements;
            switch (myType)
            {
                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_POINTS:
                    if (nvertexs < 1)
                    {
                        return false;
                    }
                    break;
                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_POLYLINES:
                    if (nedges > 0
                     && nedges < 2)
                    {
                        return false;
                    }
                    if (nvertexs < 2)
                    {
                        return false;
                    }
                    break;
                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_SEGMENTS:
                    if (nvertexs < 2)
                    {
                        return false;
                    }
                    break;
                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_POLYGONS:
                    if (nedges > 0
                     && nedges < 3)
                    {
                        return false;
                    }
                    if (nvertexs < 3)
                    {
                        return false;
                    }
                    break;
                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_TRIANGLES:
                    if (nedges > 0)
                    {
                        if (nedges < 3
                         || nedges % 3 != 0)
                        {
                            if (nedges <= 3)
                            {
                                return false;
                            }
                            myIndices.NbElements = 3 * (nedges / 3);
                        }
                    }
                    else if (nvertexs < 3
                          || nvertexs % 3 != 0)
                    {
                        if (nvertexs <= 3)
                        {
                            return false;
                        }
                        myAttribs.NbElements = 3 * (nvertexs / 3);
                    }
                    break;
                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_QUADRANGLES:
                    if (nedges > 0)
                    {
                        if (nedges < 4
                         || nedges % 4 != 0)
                        {
                            if (nedges <= 4)
                            {
                                return false;
                            }
                            myIndices.NbElements = 4 * (nedges / 4);
                        }
                    }
                    else if (nvertexs < 4
                          || nvertexs % 4 != 0)
                    {
                        if (nvertexs <= 4)
                        {
                            return false;
                        }
                        myAttribs.NbElements = 4 * (nvertexs / 4);
                    }
                    break;
                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_TRIANGLEFANS:
                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_TRIANGLESTRIPS:
                    if (nvertexs < 3)
                    {
                        return false;
                    }
                    break;
                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_QUADRANGLESTRIPS:
                    if (nvertexs < 4)
                    {
                        return false;
                    }
                    break;
                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_LINES_ADJACENCY:
                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_LINE_STRIP_ADJACENCY:
                    if (nvertexs < 4)
                    {
                        return false;
                    }
                    break;
                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_TRIANGLES_ADJACENCY:
                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_TRIANGLE_STRIP_ADJACENCY:
                    if (nvertexs < 6)
                    {
                        return false;
                    }
                    break;
                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_UNDEFINED:
                default:
                    return false;
            }

            // total number of edges(vertices) in bounds should be the same as variable
            // of total number of defined edges(vertices); if no edges - only vertices
            // could be in bounds.
            if (nbounds > 0)
            {
                int n = 0;
                for (int aBoundIter = 0; aBoundIter < nbounds; ++aBoundIter)
                {
                    n += myBounds.Bounds[aBoundIter];
                }
                if (nedges > 0
                 && n != nedges)
                {
                    if (nedges <= n)
                    {
                        return false;
                    }
                    myIndices.NbElements = n;
                }
                else if (nedges == 0
                      && n != nvertexs)
                {
                    if (nvertexs <= n)
                    {
                        return false;
                    }
                    myAttribs.NbElements = n;
                }
            }

            // check that edges (indexes to an array of vertices) are in range.
            if (nedges > 0)
            {
                for (int anEdgeIter = 0; anEdgeIter < nedges; ++anEdgeIter)
                {
                    if (myIndices.Index(anEdgeIter) >= myAttribs.NbElements)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal Graphic3d_Buffer Attributes()
        {
            throw new NotImplementedException();
        }

        internal Graphic3d_BoundBuffer Bounds()
        {
            throw new NotImplementedException();
        }
        Graphic3d_TypeOfPrimitiveArray myType;
        //! Returns the type of this primitive
        public Graphic3d_TypeOfPrimitiveArray Type() { return myType; }

    }

}