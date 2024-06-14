using OpenTK.Graphics.OpenGL;

namespace OCCPort.OpenGL
{
    public class OpenGl_PrimitiveArray : OpenGl_Element
    {
        public void Render(OpenGl_Workspace theWorkspace)
        {
            //const OpenGl_Aspects* anAspectFace = theWorkspace->Aspects();
            OpenGl_Context aCtx = theWorkspace.GetGlContext();
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
    }
}
