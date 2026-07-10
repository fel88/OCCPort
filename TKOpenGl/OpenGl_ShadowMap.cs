using OCCPort.Common;
using System;
using TKernel;
using TKService;

namespace OCCPort.OpenGL
{
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


        //! Return light source mapping matrix.
        public  Graphic3d_Mat4 LightSourceMatrix()  { return myLightMatrix; }
        Graphic3d_Mat4 myLightMatrix = new NCollection_Mat4<float>();   //!< light source matrix


        public bool IsValid()
        {
            return myShadowMapFbo.IsValid();
        }

        float myShadowMapBias; //!< shadowmap bias
        Graphic3d_Camera myShadowCamera;  //!< rendering camera


        OpenGl_FrameBuffer myShadowMapFbo;  //!< frame buffer for rendering shadow map

        public OpenGl_Texture Texture()
        {
            return myShadowMapFbo.DepthStencilTexture();
        }


        //! Returns shadowmap bias.
        public float ShadowMapBias()  { return myShadowMapBias; }

}
}