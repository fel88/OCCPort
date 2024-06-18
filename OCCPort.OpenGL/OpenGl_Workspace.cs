using System;

namespace OCCPort.OpenGL
{
    public class OpenGl_Workspace
    {
        public OpenGl_Context GetGlContext()
        {
            throw new NotImplementedException();
        }

        internal bool Activate()
        {
            throw new NotImplementedException();
        }

        internal OpenGl_Aspects Aspects()
        {
            throw new NotImplementedException();
        }

        internal int RenderFilter()
        {
            throw new NotImplementedException();
        }

        internal void ResetAppliedAspect()
        {
            throw new NotImplementedException();
        }

		internal void SetAllowFaceCulling(object value)
        {
            throw new NotImplementedException();
        }

        internal OpenGl_Aspects SetAspects(OpenGl_Aspects myCubeMapParams)
        {
            throw new NotImplementedException();
        }

        internal void SetRenderFilter(int aPrevFilter)
        {
            throw new NotImplementedException();
        }

        internal bool SetUseZBuffer(bool v)
        {
            throw new NotImplementedException();
        }

		internal bool ShouldRender(OpenGl_Element theElement, OpenGl_Group openGl_Group)
        {
            throw new NotImplementedException();
        }
    }
}