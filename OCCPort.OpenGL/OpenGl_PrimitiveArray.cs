using OpenTK.Graphics.OpenGL;
using System;

namespace OCCPort.OpenGL
{
    public class OpenGl_PrimitiveArray : OpenGl_Element
    {

        OpenGl_IndexBuffer myVboIndices;
        protected OpenGl_VertexBuffer myVboAttribs;

        protected Graphic3d_IndexBuffer myIndices;
        protected Graphic3d_Buffer myAttribs;
        Graphic3d_BoundBuffer myBounds;
        short myDrawMode;
        bool myIsFillType;
        bool myIsVboInit;

        Standard_Size myUID; //!< Unique ID of primitive array. 

        public OpenGl_PrimitiveArray(OpenGl_GraphicDriver aDriver, Graphic3d_TypeOfPrimitiveArray theType, Graphic3d_IndexBuffer theIndices, Graphic3d_Buffer theAttribs, Graphic3d_BoundBuffer theBounds)
        {
        }

        public override void Render(OpenGl_Workspace theWorkspace)
        {
            OpenGl_Aspects anAspectFace = theWorkspace.Aspects();
            OpenGl_Context aCtx = theWorkspace.GetGlContext();


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

            //Graphic3d_TypeOfShadingModel aShadingModel = Graphic3d_TypeOfShadingModel_Unlit;
            //switch (myDrawMode)
            //{
            //    default:
            //        {
            //            aShadingModel = aCtx.ShaderManager().ChooseFaceShadingModel(anAspectFace->ShadingModel(), hasVertNorm);
            //            aCtx.ShaderManager().BindFaceProgram(aTextureSet,
            //                                                    aShadingModel,
            //                                                    aCtx->ShaderManager()->MaterialState().HasAlphaCutoff() ? Graphic3d_AlphaMode_Mask : Graphic3d_AlphaMode_Opaque,
            //                                                    toDrawInteriorEdges == 1 ? anAspectFace->Aspect()->InteriorStyle() : Aspect_IS_SOLID,
            //                                                    hasVertColor,
            //                                                    toEnableEnvMap,
            //                                                    toDrawInteriorEdges == 1,
            //                                                    anAspectFace->ShaderProgramRes(aCtx));
            //            if (toDrawInteriorEdges == 1)
            //            {
            //                aCtx->ShaderManager()->PushInteriorState(aCtx->ActiveProgram(), anAspectFace->Aspect());
            //            }
            //            else if (toSetLinePolygMode)
            //            {

            aCtx.SetPolygonMode((int)PolygonMode.Line);
            //            }
            //            break;
            //        }
            //}

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
                case 1: aVboAttribs = new OpenGl_VertexBufferT<OpenGl_VertexBufferCompat>(1, myAttribs); break;
                case 2: aVboAttribs = new OpenGl_VertexBufferT<OpenGl_VertexBufferCompat>(2, myAttribs); break;
                case 3: aVboAttribs = new OpenGl_VertexBufferT<OpenGl_VertexBufferCompat>(3, myAttribs); break;
                case 4: aVboAttribs = new OpenGl_VertexBufferT<OpenGl_VertexBufferCompat>(4, myAttribs); break;
                case 5: aVboAttribs = new OpenGl_VertexBufferT<OpenGl_VertexBufferCompat>(5, myAttribs); break;
                case 6: aVboAttribs = new OpenGl_VertexBufferT<OpenGl_VertexBufferCompat>(6, myAttribs); break;
                case 7: aVboAttribs = new OpenGl_VertexBufferT<OpenGl_VertexBufferCompat>(7, myAttribs); break;
                case 8: aVboAttribs = new OpenGl_VertexBufferT<OpenGl_VertexBufferCompat>(8, myAttribs); break;
                case 9: aVboAttribs = new OpenGl_VertexBufferT<OpenGl_VertexBufferCompat>(9, myAttribs); break;
                case 10: aVboAttribs = new OpenGl_VertexBufferT<OpenGl_VertexBufferCompat>(10, myAttribs); break;
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

        private bool initNormalVbo(OpenGl_Context theCtx)
        {
            throw new NotImplementedException();
        }
    }
}
