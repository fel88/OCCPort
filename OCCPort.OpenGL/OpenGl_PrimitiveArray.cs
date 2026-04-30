using OCCPort.Enums;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Reflection.Metadata;
using System.Security.AccessControl;

namespace OCCPort.OpenGL
{
    public class OpenGl_PrimitiveArray : OpenGl_Element
    {
        public void InitBuffers(OpenGl_Context theContext,
                                         Graphic3d_TypeOfPrimitiveArray theType,

                                         Graphic3d_IndexBuffer theIndices,
                                         Graphic3d_Buffer theAttribs,
                                         Graphic3d_BoundBuffer theBounds)
        {
            // Release old graphic resources
            Release(theContext);

            myIndices = theIndices;
            myAttribs = theAttribs;
            myBounds = theBounds;
            if (theContext != null
              && theContext.GraphicsLibrary() == Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGLES)
            {
                processIndices(theContext);
            }

            setDrawMode(theType);
        }

        //=======================================================================
        public void setDrawMode(Graphic3d_TypeOfPrimitiveArray theType)
        {

            if (myAttribs == null)
            {
                myDrawMode = DRAW_MODE_NONE;
                myIsFillType = false;
                return;
            }

            switch (theType)
            {
                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_POINTS:
                    myDrawMode = GLConstants.GL_POINTS;
                    myIsFillType = false;
                    break;
                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_SEGMENTS:
                    myDrawMode = GLConstants.GL_LINES;
                    myIsFillType = false;
                    break;
                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_POLYLINES:
                    myDrawMode = GLConstants.GL_LINE_STRIP;
                    myIsFillType = false;
                    break;
                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_TRIANGLES:
                    myDrawMode = GLConstants.GL_TRIANGLES;
                    myIsFillType = true;
                    break;
                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_TRIANGLESTRIPS:
                    myDrawMode = GLConstants.GL_TRIANGLE_STRIP;
                    myIsFillType = true;
                    break;
                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_TRIANGLEFANS:
                    myDrawMode = GLConstants.GL_TRIANGLE_FAN;
                    myIsFillType = true;
                    break;

                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_LINES_ADJACENCY:
                    myDrawMode = GLConstants.GL_LINES_ADJACENCY;
                    myIsFillType = false;
                    break;
                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_LINE_STRIP_ADJACENCY:
                    myDrawMode = GLConstants.GL_LINE_STRIP_ADJACENCY;
                    myIsFillType = false;
                    break;
                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_TRIANGLES_ADJACENCY:
                    myDrawMode = GLConstants.GL_TRIANGLES_ADJACENCY;
                    myIsFillType = true;
                    break;
                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_TRIANGLE_STRIP_ADJACENCY:
                    myDrawMode = GLConstants.GL_TRIANGLE_STRIP_ADJACENCY;
                    myIsFillType = true;
                    break;


                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_QUADRANGLES:
                    myDrawMode = GLConstants.GL_QUADS;
                    myIsFillType = true;
                    break;
                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_QUADRANGLESTRIPS:
                    myDrawMode = GLConstants.GL_QUAD_STRIP;
                    myIsFillType = true;
                    break;
                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_POLYGONS:
                    myDrawMode = GLConstants.GL_POLYGON;
                    myIsFillType = true;
                    break;
                case Graphic3d_TypeOfPrimitiveArray.Graphic3d_TOPA_UNDEFINED:
                    myDrawMode = DRAW_MODE_NONE;
                    myIsFillType = false;
                    break;

            }
        }


        private void Release(OpenGl_Context theContext)
        {
            throw new NotImplementedException();
        }

        OpenGl_IndexBuffer myVboIndices;
        protected OpenGl_VertexBuffer myVboAttribs;

        protected Graphic3d_IndexBuffer myIndices;
        protected Graphic3d_Buffer myAttribs;
        Graphic3d_BoundBuffer myBounds;
        int myDrawMode;
        bool myIsFillType;
        bool myIsVboInit;

        int myUID; //!< Unique ID of primitive array. 

        public OpenGl_PrimitiveArray(OpenGl_GraphicDriver theDriver,
            Graphic3d_TypeOfPrimitiveArray theType,
            Graphic3d_IndexBuffer theIndices,
            Graphic3d_Buffer theAttribs,
            Graphic3d_BoundBuffer theBounds)
        {
            myIndices = (theIndices);
            myAttribs = (theAttribs);
            myBounds = (theBounds);
            //myDrawMode(DRAW_MODE_NONE),
            myIsFillType = (false);
            myIsVboInit = (false);

            if (myIndices != null && myIndices.NbElements < 1)
            {
                // dummy index buffer?
                myIndices = null;
            }

            if (theDriver != null)
            {
                myUID = theDriver.GetNextPrimitiveArrayUID();
                OpenGl_Context aCtx = theDriver.GetSharedContext();
                if (aCtx != null
                  && aCtx.GraphicsLibrary() == Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGLES)
                {
                    processIndices(aCtx);
                }
            }

            setDrawMode(theType);
        }

        //! OpenGL does not provide a constant for "none" draw mode.
        //! So we define our own one that does not conflict with GL constants and utilizes common GL invalid value.

        const int DRAW_MODE_NONE = -1;


        // =======================================================================
        // function : drawArray
        // purpose  :
        // =======================================================================
        void drawArray(OpenGl_Workspace theWorkspace,
                                       Graphic3d_Vec4[] theFaceColors,
                                       bool theHasVertColor)
        {
            OpenGl_Context aGlContext = theWorkspace.GetGlContext();
            if (myVboAttribs == null)
            {
                if (myDrawMode == (int)PrimitiveType.Points
                 && aGlContext.core11ffp != null)
                {
                    // extreme compatibility mode - without sprites but with markers
                    //drawMarkers(theWorkspace);
                }
                return;
            }

            var toHilight = theWorkspace.ToHighlight();
            var aDrawMode = aGlContext.ActiveProgram() != null
                                   && aGlContext.ActiveProgram().HasTessellationStage()
                                   ? (int)PrimitiveType.Patches
                                   : myDrawMode;
            myVboAttribs.BindAllAttributes(aGlContext);
            if (theHasVertColor && toHilight)
            {
                // disable per-vertex color
                //OpenGl_VertexBuffer.unbindAttribute(aGlContext, Graphic3d_TOA_COLOR);
            }
            if (myVboIndices != null)
            {
                myVboIndices.Bind(aGlContext);
                var anOffset = myVboIndices.GetDataOffset();
                if (myBounds != null)
                {
                    // draw primitives by vertex count with the indices
                    int aStride = myVboIndices.GetDataType() == All.UnsignedShort ? sizeof(ushort) : sizeof(uint);
                    for (int aGroupIter = 0; aGroupIter < myBounds.NbBounds; ++aGroupIter)
                    {
                        int aNbElemsInGroup = myBounds.Bounds[aGroupIter];
                        if (theFaceColors != null)
                            aGlContext.SetColor4fv(theFaceColors[aGroupIter]);
                        aGlContext.core11fwd.glDrawElements(aDrawMode, aNbElemsInGroup, myVboIndices.GetDataType(), anOffset);
                        anOffset += aStride * aNbElemsInGroup;
                    }
                }
                else
                {
                    // draw one (or sequential) primitive by the indices
                    aGlContext.core11fwd.glDrawElements(aDrawMode, myVboIndices.GetElemsNb(), myVboIndices.GetDataType(), anOffset);
                }
                myVboIndices.Unbind(aGlContext);
            }
            else if (myBounds != null)
            {
                int aFirstElem = 0;
                for (int aGroupIter = 0; aGroupIter < myBounds.NbBounds; ++aGroupIter)
                {
                    int aNbElemsInGroup = myBounds.Bounds[aGroupIter];
                    if (theFaceColors != null) aGlContext.SetColor4fv(theFaceColors[aGroupIter]);
                    aGlContext.core11fwd.glDrawArrays(aDrawMode, aFirstElem, aNbElemsInGroup);
                    aFirstElem += aNbElemsInGroup;
                }
            }
            else
            {
                if (myDrawMode == (int)PrimitiveType.Points)
                {
                    //drawMarkers(theWorkspace);
                }
                else
                {
                    aGlContext.core11fwd.glDrawArrays(aDrawMode, 0, myVboAttribs.GetElemsNb());
                }
            }

            // bind with 0
            myVboAttribs.UnbindAllAttributes(aGlContext);
        }

        public override void Render(OpenGl_Workspace theWorkspace)
        {
            if (myDrawMode == DRAW_MODE_NONE)
            {
                return;
            }

            OpenGl_Aspects anAspectFace = theWorkspace.Aspects();
            OpenGl_Context aCtx = theWorkspace.GetGlContext();

            bool toDrawArray = true, toSetLinePolygMode = false;
            int toDrawInteriorEdges = 0; // 0 - no edges, 1 - glsl edges, 2 - polygonMode

            // create VBOs on first render call
            if (!myIsVboInit)
            {
                // compatibility - keep data to draw markers using display lists
                bool toKeepData = myDrawMode == (int)All.Points
                                           && anAspectFace.IsDisplayListSprite(aCtx);
                if (aCtx.GraphicsLibrary() == Aspect_GraphicsLibrary.Aspect_GraphicsLibrary_OpenGLES)
                {
                    processIndices(aCtx);
                }
                buildVBO(aCtx, toKeepData);
                myIsVboInit = true;
            }
            else if ((myAttribs != null
                   && myAttribs.IsMutable())
                  || (myIndices != null
                   && myIndices.IsMutable()))
            {
                updateVBO(aCtx);
            }

            Graphic3d_TypeOfShadingModel aShadingModel = Graphic3d_TypeOfShadingModel.Graphic3d_TypeOfShadingModel_Unlit;

            OpenGl_TextureSet aTextureSet = theWorkspace.TextureSet();
            bool toEnableEnvMap = aTextureSet != null
                                      && aTextureSet == theWorkspace.EnvironmentTexture();

            if (toDrawArray)
            {
                bool hasColorAttrib = myVboAttribs != null && myVboAttribs.HasColorAttribute();
                bool toHilight = theWorkspace.ToHighlight();
                bool hasVertColor = hasColorAttrib && !toHilight;
                bool hasVertNorm = myVboAttribs != null && myVboAttribs.HasNormalAttribute();
                switch (myDrawMode)
                {
                    default:
                        {
                            aShadingModel = aCtx.ShaderManager().ChooseFaceShadingModel(anAspectFace.ShadingModel(), hasVertNorm);
                            aCtx.ShaderManager().BindFaceProgram(aTextureSet,
                                                                    aShadingModel,
                                                                    aCtx.ShaderManager().MaterialState().HasAlphaCutoff() ? Graphic3d_AlphaMode.Graphic3d_AlphaMode_Mask : Graphic3d_AlphaMode.Graphic3d_AlphaMode_Opaque,
                                                                    toDrawInteriorEdges == 1 ? anAspectFace.Aspect().InteriorStyle() : Aspect_InteriorStyle.Aspect_IS_SOLID,
                                                                    hasVertColor,
                                                                    toEnableEnvMap,
                                                                    toDrawInteriorEdges == 1,
                                                                    anAspectFace.ShaderProgramRes(aCtx));
                            if (toDrawInteriorEdges == 1)
                            {
                                //aCtx.ShaderManager().PushInteriorState(aCtx.ActiveProgram(), anAspectFace.Aspect());
                            }
                            else if (toSetLinePolygMode)
                            {
                                aCtx.SetPolygonMode((int)PolygonMode.Line);
                            }
                            break;
                        }
                }

                var aFaceColors = myBounds != null && !toHilight && anAspectFace.Aspect().InteriorStyle() != Aspect_InteriorStyle.Aspect_IS_HIDDENLINE
                                     ? myBounds.Colors
                                     : null;

                drawArray(theWorkspace, aFaceColors, hasColorAttrib);

            }


        }

        private void processIndices(OpenGl_Context aCtx)
        {
            throw new NotImplementedException();
        }

        private void updateVBO(OpenGl_Context aCtx)
        {
            throw new NotImplementedException();
        }


        // =======================================================================
        // function : buildVBO
        // purpose  :
        // =======================================================================
        public bool buildVBO(OpenGl_Context theCtx,
                                                   bool theToKeepData)
        {
            bool isNormalMode = theCtx.ToUseVbo();
            clearMemoryGL(theCtx);
            if (myAttribs == null
             || myAttribs.IsEmpty()
             || myAttribs.NbElements < 1
             || myAttribs.NbAttributes < 1
             || myAttribs.NbAttributes > 10)
            {
                // vertices should be always defined - others are optional
                return false;
            }

            if (isNormalMode
             && initNormalVbo(theCtx))
            {
                if (!theCtx.caps.keepArrayData
                 && !theToKeepData
                 && !myAttribs.IsMutable())
                {
                    myIndices = null; ;
                    myAttribs = null; ;
                }
                else
                {
                    myAttribs.Validate();
                }
                return true;
            }

            OpenGl_VertexBufferCompat aVboAttribs = new OpenGl_VertexBufferCompat();
            switch (myAttribs.NbAttributes)
            {
                case 1: aVboAttribs = new OpenGl_VertexBufferT_OpenGl_VertexBufferCompat(1, myAttribs); break;
                case 2: aVboAttribs = new OpenGl_VertexBufferT_OpenGl_VertexBufferCompat(2, myAttribs); break;
                case 3: aVboAttribs = new OpenGl_VertexBufferT_OpenGl_VertexBufferCompat(3, myAttribs); break;
                case 4: aVboAttribs = new OpenGl_VertexBufferT_OpenGl_VertexBufferCompat(4, myAttribs); break;
                case 5: aVboAttribs = new OpenGl_VertexBufferT_OpenGl_VertexBufferCompat(5, myAttribs); break;
                case 6: aVboAttribs = new OpenGl_VertexBufferT_OpenGl_VertexBufferCompat(6, myAttribs); break;
                case 7: aVboAttribs = new OpenGl_VertexBufferT_OpenGl_VertexBufferCompat(7, myAttribs); break;
                case 8: aVboAttribs = new OpenGl_VertexBufferT_OpenGl_VertexBufferCompat(8, myAttribs); break;
                case 9: aVboAttribs = new OpenGl_VertexBufferT_OpenGl_VertexBufferCompat(9, myAttribs); break;
                case 10: aVboAttribs = new OpenGl_VertexBufferT_OpenGl_VertexBufferCompat(10, myAttribs); break;
            }
            aVboAttribs.initLink(myAttribs, 0, myAttribs.NbElements, (int)All.None);
            if (myIndices != null)
            {
                OpenGl_IndexBufferCompat aVboIndices = new OpenGl_IndexBufferCompat();
                switch (myIndices.Stride)
                {
                    case 2:
                        {
                            aVboIndices.initLink(myIndices, 1, myIndices.NbElements, (int)All.UnsignedShort);
                            break;
                        }
                    case 4:
                        {
                            aVboIndices.initLink(myIndices, 1, myIndices.NbElements, (int)All.UnsignedInt);
                            break;
                        }
                    default:
                        {
                            return false;
                        }
                }
                //todo!!myVboIndices = aVboIndices;
            }
            //todo!!myVboAttribs = aVboAttribs;
            if (!theCtx.caps.keepArrayData
             && !theToKeepData)
            {
                // does not make sense for compatibility mode
                //myIndices.Nullify();
                //myAttribs.Nullify();
            }

            return true;
        }

        private void clearMemoryGL(OpenGl_Context theCtx)
        {
            throw new NotImplementedException();
        }
        //! Initialize normal (OpenGL-provided) VBO
        private bool initNormalVbo(OpenGl_Context theCtx)
        {
            switch (myAttribs.NbAttributes)
            {
                case 1: myVboAttribs = new OpenGl_VertexBufferT_OpenGl_VertexBuffer(1, myAttribs); break;
                case 2: myVboAttribs = new OpenGl_VertexBufferT_OpenGl_VertexBuffer(2, myAttribs); break;
                case 3: myVboAttribs = new OpenGl_VertexBufferT_OpenGl_VertexBuffer(3, myAttribs); break;
                case 4: myVboAttribs = new OpenGl_VertexBufferT_OpenGl_VertexBuffer(4, myAttribs); break;
                case 5: myVboAttribs = new OpenGl_VertexBufferT_OpenGl_VertexBuffer(5, myAttribs); break;
                case 6: myVboAttribs = new OpenGl_VertexBufferT_OpenGl_VertexBuffer(6, myAttribs); break;
                case 7: myVboAttribs = new OpenGl_VertexBufferT_OpenGl_VertexBuffer(7, myAttribs); break;
                case 8: myVboAttribs = new OpenGl_VertexBufferT_OpenGl_VertexBuffer(8, myAttribs); break;
                case 9: myVboAttribs = new OpenGl_VertexBufferT_OpenGl_VertexBuffer(9, myAttribs); break;
                case 10: myVboAttribs = new OpenGl_VertexBufferT_OpenGl_VertexBuffer(10, myAttribs); break;
            }

            bool isAttribMutable = myAttribs.IsMutable();
            bool isAttribInterleaved = myAttribs.IsInterleaved();
            if (myAttribs.NbElements != myAttribs.NbMaxElements()
             && myIndices == null
             && (!isAttribInterleaved || isAttribMutable))
            {
                throw new Standard_ProgramError("OpenGl_PrimitiveArray::buildVBO() - vertex attribute data with reserved size is not supported");
            }

            // specify data type as Byte and NbComponents as Stride, so that OpenGl_VertexBuffer::EstimatedDataSize() will return correct value
            int aNbVertexes = (isAttribMutable || !isAttribInterleaved) ? myAttribs.NbMaxElements() : myAttribs.NbElements;
            if (!myVboAttribs.init(theCtx, (uint)myAttribs.Stride, aNbVertexes, myAttribs.Data(), All.UnsignedByte, myAttribs.Stride))
            {
                string aMsg = ("VBO creation for Primitive Array has failed for ") + aNbVertexes + " vertices. Out of memory?";
                theCtx.PushMessage(All.DebugSourceApplication, All.DebugTypePerformance, 0, All.DebugSeverityLow, aMsg);

                clearMemoryGL(theCtx);
                return false;
            }
            else if (myIndices == null)
            {
                if (isAttribMutable && isAttribInterleaved)
                {
                    // for mutable interlaced array we can change dynamically number of vertexes (they will be just skipped at the end of buffer);
                    // this doesn't matter in case if we have indexed array
                    myVboAttribs.SetElemsNb(myAttribs.NbElements);
                }
                return true;
            }

            int aNbIndexes = !myIndices.IsMutable() ? myIndices.NbElements : myIndices.NbMaxElements();
            myVboIndices = new OpenGl_IndexBuffer();
            bool isOk = false;
            switch (myIndices.Stride)
            {
                case 2:
                    {
                        isOk = myVboIndices.Init(theCtx, 1, aNbIndexes, myIndices.DataFloat());
                        myVboIndices.SetElemsNb(myIndices.NbElements);
                        myIndices.Validate();
                        break;
                    }
                //case 4:
                //    {
                //        isOk = myVboIndices->Init(theCtx, 1, aNbIndexes, reinterpret_cast <const GLuint*> (myIndices->Data()));
                //        myVboIndices->SetElemsNb(myIndices->NbElements);
                //        myIndices->Validate();
                //        break;
                //    }
                default:
                    {
                        clearMemoryGL(theCtx);
                        return false;
                    }
            }
            return true;
        }

        internal int DrawMode()
        {
            return myDrawMode;
        }
    }
}
