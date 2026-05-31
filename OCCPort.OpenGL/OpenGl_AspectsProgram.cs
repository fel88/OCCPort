using System;
using System.Reflection.Metadata;

namespace OCCPort.OpenGL
{
    public class OpenGl_AspectsProgram
    {
        public void UpdateRediness(Graphic3d_Aspects myAspect)
        {

        }
        void build(OpenGl_Context theCtx,
                                   Graphic3d_ShaderProgram theShader)
        {
            if (theCtx.core20fwd == null)
            {
                return;
            }

            // release old shader program resources
            if (myShaderProgram != null)
            {
                theCtx.ShaderManager().Unregister(ref myShaderProgramId, myShaderProgram);
                myShaderProgramId.Clear();
                myShaderProgram = null;
            }
            if (theShader == null)
            {
                return;
            }

            theCtx.ShaderManager().Create(theShader, ref myShaderProgramId, myShaderProgram);
        }


        //! Return shading program.
        public OpenGl_ShaderProgram ShaderProgram(OpenGl_Context theCtx,
                                                      Graphic3d_ShaderProgram theShader)
        {
            if (!myIsShaderReady)
            {
                build(theCtx, theShader);
                myIsShaderReady = true;
            }
            return myShaderProgram;
        }

        OpenGl_ShaderProgram myShaderProgram;
        string myShaderProgramId;
        bool myIsShaderReady;
    }
}