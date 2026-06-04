using OCCPort.Common;
using TKernel;
using TKMath;

namespace TKService
{
    //! This class allows the definition of groups
    //! of primitives inside of graphic objects (presentations).
    //! A group contains the primitives and attributes
    //! for which the range is limited to this group.
    //! The primitives of a group can be globally suppressed.
    //!
    //! There are two main group usage models:
    //!
    //! 1) Non-modifiable, or unbounded, group ('black box').
    //! Developers can repeat a sequence of
    //! SetPrimitivesAspect() with AddPrimitiveArray() methods arbitrary number of times
    //! to define arbitrary number of primitive "blocks" each having individual apect values.
    //! Any modification of such a group is forbidden, as aspects and primitives are mixed
    //! in memory without any high-level logical structure, and any modification is very likely to result
    //! in corruption of the group internal data.
    //! It is necessary to recreate such a group as a whole when some attribute should be changed.
    //! (for example, in terms of AIS it is necessary to re-Compute() the whole presentation each time).
    //! 2) Bounded group. Developers should specify the necessary group aspects with help of
    //! SetGroupPrimitivesAspect() and then add primitives to the group.
    //! Such a group have simplified organization in memory (a single block of attributes
    //! followed by a block of primitives) and therefore it can be modified, if it is necessary to
    //! change parameters of some aspect that has already been set, using methods:
    //! IsGroupPrimitivesAspectSet() to detect which aspect was set for primitives;
    //! GroupPrimitivesAspect() to read current aspect values
    //! and SetGroupPrimitivesAspect() to set new values.
    //!
    //! Developers are strongly recommended to take all the above into account when filling Graphic3d_Group
    //! with aspects and primitives and choose the group usage model beforehand out of application needs.
    //! Note that some Graphic3d_Group class virtual methods contain only base implementation
    //! that is extended by the descendant class in OpenGl package.
    public abstract class Graphic3d_Group
    {
        public Graphic3d_Group(Graphic3d_Structure theStruct)
        {
            myStructure = theStruct;
            myIsClosed = (false);
        }
        //! Return transformation persistence.
        public Graphic3d_TransformPers TransformPersistence() { return myTrsfPers; }

        /*public Graphic3d_Group(Graphic3d_StructureManager m)
        {
            myStructureManager = m;
        }*/
        protected Graphic3d_TransformPers myTrsfPers; //!< current transform persistence
        protected Graphic3d_Structure myStructure;     //!< pointer to the parent structure
        //Graphic3d_BndBox4f myBounds;        //!< bounding box
        bool myIsClosed;      //!< flag indicating closed volume
                              //! Adds an array of primitives for display

        Graphic3d_BndBox4f myBounds = new Graphic3d_BndBox4f();        //!< bounding box

        //! Modifies the context for all the face primitives of the group.
        public abstract void SetGroupPrimitivesAspect(Graphic3d_Aspects theAspect);

        public void AddPrimitiveArray(Graphic3d_ArrayOfPrimitives thePrim, Graphic3d_IndexBuffer graphic3d_IndexBuffer, bool theToEvalMinMax = true)
        {
            if (IsDeleted()
            || !thePrim.IsValid())
            {
                return;
            }

            AddPrimitiveArray(thePrim.Type(), thePrim.Indices(), thePrim.Attributes(), thePrim.Bounds(), theToEvalMinMax);
        }

        public virtual void AddPrimitiveArray(Graphic3d_TypeOfPrimitiveArray theType,
                                                Graphic3d_IndexBuffer theIndices,
                                                Graphic3d_Buffer theAttribs,
                                                Graphic3d_BoundBuffer theBounds,
                                                bool theToEvalMinMax = true)
        {
            //(void)theType;
            if (IsDeleted()
             || theAttribs == null)
            {
                return;
            }

            if (!theToEvalMinMax)
            {
                Update();
                return;
            }

            int aNbVerts = theAttribs.NbElements;
            int anAttribIndex = 0;
            int anAttribStride = 0;
            byte[] aDataPtr = theAttribs.AttributeData(Graphic3d_TypeOfAttribute.Graphic3d_TOA_POS, ref anAttribIndex, ref anAttribStride);
            if (aDataPtr == null)
            {
                Update();
                return;
            }

            switch (theAttribs.Attribute(anAttribIndex).DataType)
            {
                case Graphic3d_TypeOfData.Graphic3d_TOD_VEC2:
                    {
                        for (int aVertIter = 0; aVertIter < aNbVerts; ++aVertIter)
                        {
                            var offset = anAttribStride * aVertIter;
                            Graphic3d_Vec2 aVert = new Graphic3d_Vec2(aDataPtr, offset, new FloatExtractor());

                            myBounds.Add(new Graphic3d_Vec4(aVert.x(), aVert.y(), 0.0f, 1.0f));
                        }
                        break;
                    }
                case Graphic3d_TypeOfData.Graphic3d_TOD_VEC3:
                case Graphic3d_TypeOfData.Graphic3d_TOD_VEC4:
                    {
                        for (int aVertIter = 0; aVertIter < aNbVerts; ++aVertIter)
                        {
                            var offset = anAttribStride * aVertIter;
                            Graphic3d_Vec3 aVert = BinaryHelper.Get_Vec3(aDataPtr, offset);

                            myBounds.Add(new Graphic3d_Vec4(aVert.x(), aVert.y(), aVert.z(), 1.0f));
                        }
                        break;
                    }
                default: break;
            }
            Update();
        }

        public void Update()
        {
            if (IsDeleted())
            {
                return;
            }

            myStructure.StructureManager().Update();
        }


        //! Returns Standard_True if the group <me> is deleted.
        //! <me> is deleted after the call Remove (me) or the
        //! associated structure is deleted.
        public bool IsDeleted()
        {
            return myStructure == null || myStructure.IsDeleted();
        }

        public void AddPrimitiveArray(Graphic3d_ArrayOfPrimitives thePrim, bool theToEvalMinMax = true)
        {
            if (IsDeleted() || !thePrim.IsValid())
                return;

            AddPrimitiveArray(thePrim.Type(), thePrim.Indices(), thePrim.Attributes(), thePrim.Bounds(), theToEvalMinMax);
        }

        //! Modifies the current context of the group to give another aspect for all the primitives created after this call in the group.
        public abstract void SetPrimitivesAspect(Graphic3d_Aspects theAspect);

        //! Changes property shown that primitive arrays within this group form closed volume (do no contain open shells).
        public void SetClosed(bool theIsClosed) { myIsClosed = theIsClosed; }

        //! Return true if primitive arrays within this graphic group form closed volume (do no contain open shells).
        public bool IsClosed() { return myIsClosed; }
        //! Returns boundary box of the group <me> without transformation applied,
        public Graphic3d_BndBox4f BoundingBox()
        {
            return myBounds;
        }
    }


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

            //memset(myAttribs->ChangeData(), 0, size_t(myAttribs->Stride) * size_t(myAttribs->NbMaxElements()));
            if ((theArrayOptions & Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_AttribsMutable) == 0
             && (theArrayOptions & Graphic3d_ArrayFlags.Graphic3d_ArrayFlags_AttribsDeinterleaved) == 0)
            {
                myAttribs.NbElements = 0;
            }

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
                var aVec3 = BinaryHelper.Get_Vec3(myNormData, myNormStride);
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
            Graphic3d_Vec3 aVec = BinaryHelper.Get_Vec3(myAttribs.Data(), theIndex * myPosStride);
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

        //! Returns vertex attributes buffer (colors, normals, texture coordinates).
        internal Graphic3d_Buffer Attributes()
        {
            return myAttribs;
        }

        //! Returns optional bounds buffer.
        internal Graphic3d_BoundBuffer Bounds()
        {
            return myBounds;

        }
        Graphic3d_TypeOfPrimitiveArray myType;
        //! Returns the type of this primitive
        public Graphic3d_TypeOfPrimitiveArray Type() { return myType; }

    }

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

    public class Graphic3d_BoundBuffer
    {
        public Graphic3d_Vec4[] Colors;      //!< pointer to facet color values
        public int[] Bounds;      //!< pointer to bounds array
        public int NbBounds;    //!< number of bounds
        public int NbMaxBounds; //!< number of allocated bounds
                                //! Empty constructor.
        public Graphic3d_BoundBuffer(/*NCollection_BaseAllocator theAlloc*/)
        {
            //NCollection_Buffer(theAlloc);
            Colors = (null);
            Bounds = (null);
            NbBounds = (0);
            NbMaxBounds = (0);
        }


        //! Allocates new empty array
        public bool Init(int theNbBounds,
              bool theHasColors)
        {
            Colors = null;
            Bounds = null;
            NbBounds = 0;
            NbMaxBounds = 0;
            //   Free();
            if (theNbBounds < 1)
            {
                return false;
            }

            /*  int aBoundsSize = sizeof(Standard_Integer) * theNbBounds;
              int aColorsSize = theHasColors
                                      ? sizeof(Graphic3d_Vec4) * theNbBounds
                                      : 0;*/
            // if (!Allocate(aColorsSize + aBoundsSize))
            {
                //   Free();
                //     return false;
            }

            NbBounds = theNbBounds;
            NbMaxBounds = theNbBounds;
            //  Colors = theHasColors ? reinterpret_cast<Graphic3d_Vec4*>(myData) : NULL;
            //Bounds = reinterpret_cast<Standard_Integer*>(theHasColors ? (myData + aColorsSize) : myData);
            return true;
        }


    }

    //! Graphic3d_ArrayFlags bitmask values.
    [Flags]
    public enum Graphic3d_ArrayFlags : int
    {
        Graphic3d_ArrayFlags_None = 0x00,  //!< no flags
        Graphic3d_ArrayFlags_VertexNormal = 0x01,  //!< per-vertex normal attribute
        Graphic3d_ArrayFlags_VertexColor = 0x02,  //!< per-vertex color  attribute
        Graphic3d_ArrayFlags_VertexTexel = 0x04,  //!< per-vertex texel coordinates (UV) attribute
        Graphic3d_ArrayFlags_BoundColor = 0x10,
        // advanced
        Graphic3d_ArrayFlags_AttribsMutable = 0x20,  //!< mutable array, which can be invalidated during lifetime without re-creation
        Graphic3d_ArrayFlags_AttribsDeinterleaved = 0x40,  //!< non-interleaved vertex attributes packed into single array
        Graphic3d_ArrayFlags_IndexesMutable = 0x80,  //!< mutable index array, which can be invalidated during lifetime without re-creation
    };
    public class BinaryHelper
    {
        public static Graphic3d_Vec3 Get_Vec3(byte[] aDataPtr, int offset)
        {
            return new Graphic3d_Vec3(BitConverter.ToSingle(aDataPtr, offset),
                BitConverter.ToSingle(aDataPtr, offset + sizeof(float)),
                BitConverter.ToSingle(aDataPtr, offset + sizeof(float) * 2));


        }
    }

    //! Redefines BVH_Box<Standard_ShortReal, 4> for AABB representation
    //! Describes rendering parameters and effects.

    public class Graphic3d_BndBox4f : BVH_Box<Graphic3d_Vec4, Graphic3d_Vec4_BoxMinMax>//<float,4>
    {

    }
    public static class ByteHelper
    {

    }

    public class FloatExtractor : TKernel.IElementBinaryExtractor<float>
    {
        public float Get(byte[] data, int idx)
        {
            var offset = idx;//??
            return BitConverter.ToSingle(data, offset);
        }
    }

    //! Definition of line types
    public enum Aspect_TypeOfLine
    {
        Aspect_TOL_EMPTY = -1, //!< hidden
        Aspect_TOL_SOLID = 0, //!< continuous
        Aspect_TOL_DASH,       //!< dashed 2.0,1.0 (MM)
        Aspect_TOL_DOT,        //!< dotted 0.2,0.5 (MM)
        Aspect_TOL_DOTDASH,    //!< mixed  10.0,1.0,2.0,1.0 (MM)
        Aspect_TOL_USERDEFINED //!< defined by Users
    };


    public class BVH_Box<BVH_VecNt, MinMax> where BVH_VecNt : struct
       where MinMax : IBoxMinMax<BVH_VecNt>, new()
    {

        public BVH_VecNt myMinPoint; //!< Minimum point of bounding box
        public BVH_VecNt myMaxPoint; //!< Maximum point of bounding box
        protected bool myIsInited; //!< Is bounding box initialized?
                                   //! Returns minimum point of bounding box.

        MinMax minMax = new MinMax();

        //! Appends new point to the bounding box.
        public void Add(BVH_VecNt thePoint)
        {
            if (!myIsInited)
            {
                myMinPoint = thePoint;
                myMaxPoint = thePoint;
                myIsInited = true;
            }
            else
            {
                myMinPoint = minMax.CwiseMin(myMinPoint, thePoint);
                myMaxPoint = minMax.CwiseMax(myMaxPoint, thePoint);
            }
        }


        public BVH_VecNt CornerMin() { return myMinPoint; }

        //! Returns maximum point of bounding box.
        public BVH_VecNt CornerMax() { return myMaxPoint; }

        public void Combine(BVH_Box<BVH_VecNt, MinMax> theBox)
        {
            if (theBox.myIsInited)
            {
                if (!myIsInited)
                {
                    myMinPoint = theBox.myMinPoint;
                    myMaxPoint = theBox.myMaxPoint;
                    myIsInited = true;
                }
                else
                {
                    minMax.CwiseMin(ref myMinPoint, theBox.myMinPoint);
                    minMax.CwiseMax(ref myMaxPoint, theBox.myMaxPoint);
                }
            }
        }

        //! Is bounding box valid?
        public bool IsValid() { return myIsInited; }
    }


    //! Tool class for calculate component-wise vector minimum
    //! and maximum (optimized version).
    //! \tparam T Numeric data type
    //! \tparam N Vector dimension
    public interface IBoxMinMax<BVH_VecNt>
    {

        BVH_VecNt CwiseMin(BVH_VecNt theVec1, BVH_VecNt theVec2);
        void CwiseMin(ref BVH_VecNt theVec1, BVH_VecNt theVec2);
        //{

        //    theVec1.X = Math.Min(theVec1.X, theVec2.X);
        //    theVec1.Y = Math.Min(theVec1.Y, theVec2.Y);
        //    theVec1.Z = Math.Min(theVec1.Z, theVec2.Z);
        //}

        BVH_VecNt CwiseMax(BVH_VecNt theVec1, BVH_VecNt theVec2);
        void CwiseMax(ref BVH_VecNt theVec1, BVH_VecNt theVec2);
        //{
        //    theVec1.X = Math.Max(theVec1.X, theVec2.X);
        //    theVec1.Y = Math.Max(theVec1.Y, theVec2.Y);
        //    theVec1.Z = Math.Max(theVec1.Z, theVec2.Z);
        //}
    }


    //! Buffer of vertex attributes.
    //! This class is intended for advanced usage allowing invalidation of entire buffer content or its sub-part.
    class Graphic3d_AttribBuffer : Graphic3d_Buffer
    {
        // =======================================================================
        public Graphic3d_AttribBuffer(NCollection_BaseAllocator theAlloc) : base(theAlloc)
        {
            myIsInterleaved = (true);
            myIsMutable = false;
        }

        //! Return number of initially allocated elements which can fit into this buffer,
        //! while NbElements can be overwritten to smaller value.
        public int NbMaxElements() { return Stride != 0 ? (int)(mySize / (int)(Stride)) : 0; }

        public void SetInterleaved(bool theIsInterleaved)
        {
            if (NbMaxElements() != 0)
            {
                throw new Standard_ProgramError("Graphic3d_AttribBuffer::SetInterleaved() should not be called for allocated buffer");
            }
            myIsInterleaved = theIsInterleaved;
        }
        public void SetMutable(bool theMutable)
        {
            if (mySize > Standard_Integer.IntegerLast()
             && theMutable)
            {
                throw new Standard_OutOfRange("Graphic3d_AttribBuffer::SetMutable(), Mutable flag cannot be used for buffer exceeding 32-bit address space");
            }
            myIsMutable = theMutable;
        }

        Graphic3d_BufferRange myInvalidatedRange; //!< invalidated buffer data range (as byte offsets)
        bool myIsInterleaved;    //!< flag indicating the vertex attributes being interleaved
        bool myIsMutable;        //!< flag indicating that data can be invalidated
    }

    internal class Graphic3d_BufferRange
    {
    }

    //! Interior types for primitive faces.
    public enum Aspect_InteriorStyle
    {
        Aspect_IS_EMPTY = -1, //!< no interior
        Aspect_IS_SOLID = 0, //!< normally filled surface interior
        Aspect_IS_HATCH,      //!< hatched surface interior
        Aspect_IS_HIDDENLINE, //!< interior is filled with viewer background color
        Aspect_IS_POINT,      //!< display only vertices of surface (obsolete)

        // obsolete aliases
        Aspect_IS_HOLLOW = Aspect_IS_EMPTY, //!< transparent surface interior
    };


    public class Graphic3d_MaterialAspect
    {
        public float Alpha()
        {
            throw new NotImplementedException();
        }
        //! Returns TRUE if the reflection mode is active, FALSE otherwise.
        public bool ReflectionMode(Graphic3d_TypeOfReflection theType)
        {
            return !myColors[(int)theType].IsEqual(Quantity_NameOfColor.Quantity_NOC_BLACK);
        }
        const int Graphic3d_TypeOfReflection_NB = 4;
        Quantity_Color[] myColors = new Quantity_Color[Graphic3d_TypeOfReflection_NB];

    }//! Nature of the reflection of a material.


    public enum Graphic3d_TypeOfShadingModel
    {
        //! Use Shading Model, specified as default for entire Viewer.
        Graphic3d_TypeOfShadingModel_DEFAULT = -1,

        //! Unlit Shading (or shadeless), lighting is ignored and facet is fully filled by its material color.
        //! This model is useful for artificial/auxiliary objects, not intended to be lit,
        //! or for objects with pre-calculated lighting information (e.g. captured by camera).
        Graphic3d_TypeOfShadingModel_Unlit = 0,

        //! Flat Shading for Phong material model, calculated using triangle normal.
        //! Could be useful for mesh element analysis.
        //! This shading model does NOT require normals to be defined within vertex attributes.
        Graphic3d_TypeOfShadingModel_PhongFacet,

        //! Gouraud shading uses the same material definition as Phong reflection model,
        //! but emulates an obsolete per-vertex calculations with result color interpolated across fragments,
        //! as implemented by T&L hardware blocks on old graphics hardware.
        //! This shading model requires normals to be defined within vertex attributes.
        Graphic3d_TypeOfShadingModel_Gouraud,

        //! Phong reflection model, an empirical model defined by Diffuse/Ambient/Specular/Shininess components.
        //! Lighting is calculated per-fragment basing on nodal normal (normal is interpolated across fragments of triangle).
        //! This shading model requires normals to be defined within vertex attributes.
        Graphic3d_TypeOfShadingModel_Phong,

        //! Metallic-roughness physically based (PBR) illumination system.
        Graphic3d_TypeOfShadingModel_Pbr,

        //! Same as Graphic3d_TypeOfShadingModel_Pbr but using flat per-triangle normal.
        Graphic3d_TypeOfShadingModel_PbrFacet,

        // obsolete aliases
        Graphic3d_TOSM_DEFAULT = Graphic3d_TypeOfShadingModel_DEFAULT,
        Graphic3d_TOSM_UNLIT = Graphic3d_TypeOfShadingModel_Unlit,
        Graphic3d_TOSM_FACET = Graphic3d_TypeOfShadingModel_PhongFacet,
        Graphic3d_TOSM_VERTEX = Graphic3d_TypeOfShadingModel_Gouraud,
        Graphic3d_TOSM_FRAGMENT = Graphic3d_TypeOfShadingModel_Phong,
        Graphic3d_TOSM_PBR = Graphic3d_TypeOfShadingModel_Pbr,
        Graphic3d_TOSM_PBR_FACET = Graphic3d_TypeOfShadingModel_PbrFacet,
        //
        Graphic3d_TOSM_NONE = Graphic3d_TOSM_UNLIT,
        V3d_COLOR = Graphic3d_TOSM_NONE,
        V3d_FLAT = Graphic3d_TOSM_FACET,
        V3d_GOURAUD = Graphic3d_TOSM_VERTEX,
        V3d_PHONG = Graphic3d_TOSM_FRAGMENT
    }

    

    public enum Graphic3d_AlphaMode
    {

        //Defines how alpha value of base color / texture should be treated.

        Graphic3d_AlphaMode_Opaque,

        //rendered output is fully opaque and alpha value is ignored
        Graphic3d_AlphaMode_Mask,

        //rendered output is either fully opaque or fully transparent depending on the alpha value and the alpha cutoff value
        Graphic3d_AlphaMode_Blend,

        //rendered output is combined with the background
        Graphic3d_AlphaMode_MaskBlend,
        //performs in-place blending (without implicit reordering of opaque objects) with alpha-test
        Graphic3d_AlphaMode_BlendAuto,

        //special value defined for backward compatibility - it is equal to Graphic3d_AlphaMode_Blend when Material transparency is not zero and Graphic3d_AlphaMode_Opaque otherwise;
    }

    internal class Graphic3d_MutableIndexBuffer : Graphic3d_IndexBuffer
    {
        public Graphic3d_MutableIndexBuffer(NCollection_BaseAllocator theAlloc) : base(theAlloc)
        {
        }
    }

    public enum Graphic3d_TypeOfReflection
    {
        Graphic3d_TOR_AMBIENT = 0,
        Graphic3d_TOR_DIFFUSE,
        Graphic3d_TOR_SPECULAR,
        Graphic3d_TOR_EMISSION
    };

    public class Graphic3d_Vec4_BoxMinMax : IBoxMinMax<Graphic3d_Vec4>
    {
        public void CwiseMax(ref Graphic3d_Vec4 theVec1, Graphic3d_Vec4 theVec2)
        {
            theVec1.X = Math.Max(theVec1.X, theVec2.X);
            theVec1.Y = Math.Max(theVec1.Y, theVec2.Y);
            theVec1.Z = Math.Max(theVec1.Z, theVec2.Z);
            theVec1.W = Math.Max(theVec1.W, theVec2.W);
        }

        public Graphic3d_Vec4 CwiseMax(Graphic3d_Vec4 theVec1, Graphic3d_Vec4 theVec2)
        {
            theVec1.X = Math.Max(theVec1.X, theVec2.X);
            theVec1.Y = Math.Max(theVec1.Y, theVec2.Y);
            theVec1.Z = Math.Max(theVec1.Z, theVec2.Z);
            theVec1.W = Math.Max(theVec1.W, theVec2.W);
            return theVec1;
        }

        public void CwiseMin(ref Graphic3d_Vec4 theVec1, Graphic3d_Vec4 theVec2)
        {
            theVec1.X = Math.Min(theVec1.X, theVec2.X);
            theVec1.Y = Math.Min(theVec1.Y, theVec2.Y);
            theVec1.Z = Math.Min(theVec1.Z, theVec2.Z);
            theVec1.W = Math.Min(theVec1.W, theVec2.W);
        }

        public Graphic3d_Vec4 CwiseMin(Graphic3d_Vec4 theVec1, Graphic3d_Vec4 theVec2)
        {
            theVec1.X = Math.Min(theVec1.X, theVec2.X);
            theVec1.Y = Math.Min(theVec1.Y, theVec2.Y);
            theVec1.Z = Math.Min(theVec1.Z, theVec2.Z);
            theVec1.W = Math.Min(theVec1.W, theVec2.W);
            return theVec1;
        }
    }
}
