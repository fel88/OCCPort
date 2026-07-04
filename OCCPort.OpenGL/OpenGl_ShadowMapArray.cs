using OCCPort.Common;
using System.Reflection.Metadata;
using TKernel;
using TKService;

namespace OCCPort.OpenGL
{
    public class OpenGl_ShadowMapArray : NCollection_Array1<OpenGl_ShadowMap>
    {
    }
    //! This class contains shadow mapping resources.
    public class OpenGl_ShadowMap : OpenGl_NamedResource
    {

        public OpenGl_ShadowMap() : base("shadow_map")
        {
            myShadowMapFbo = (new OpenGl_FrameBuffer(myResourceId + ":fbo"));
            myShadowCamera = (new Graphic3d_Camera());
            myShadowMapBias = (0.0f);

            //
        }

        float myShadowMapBias; //!< shadowmap bias
        Graphic3d_Camera myShadowCamera;  //!< rendering camera


        OpenGl_FrameBuffer myShadowMapFbo;  //!< frame buffer for rendering shadow map

        public OpenGl_Texture Texture()
        {
            return myShadowMapFbo.DepthStencilTexture();
        }

    }
}