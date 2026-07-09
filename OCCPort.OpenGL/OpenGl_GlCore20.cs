using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using OCCPort.Common;
using TKService;

namespace OCCPort.OpenGL
{
    //! OpenGL 2.0 core based on 1.5 version.
    public class OpenGl_GlCore20 : OpenGl_GlCore15
    {
        public int glGetUniformLocation(int myProgramID, string theName)
        {
            return GL.GetUniformLocation(myProgramID, theName);
        }

        internal void glAttachShader(int myProgramID, int myShaderID)
        {
            GL.AttachShader(myProgramID, myShaderID);
        }

        internal void glBindAttribLocation(int myProgramID, int theIndex, string theName)
        {
            GL.BindAttribLocation(myProgramID, theIndex, theName);
        }

        internal void glClear(ClearBufferMask value)
        {
            GL.Clear(value);
        }

        internal void glClearColor(float v1, float v2, float v3, float v4)
        {
            GL.ClearColor(v1, v2, v3, v4);
        }

        internal void glClearDepth(double v)
        {
            GL.ClearDepth(v);
        }

        internal void glCompileShader(int myShaderID)
        {
            GL.CompileShader(myShaderID);
        }

        internal int glCreateProgram()
        {
            return GL.CreateProgram();
        }

        internal int glCreateShader(uint myType)
        {
            return GL.CreateShader((ShaderType)myType);
        }

        internal void glDeleteProgram(int myProgramID)
        {
            GL.DeleteProgram(myProgramID);
        }

        internal void glDepthFunc(DepthFunction always)
        {
            GL.DepthFunc(always);
        }

        internal void glDepthMask(bool v)
        {
            GL.DepthMask(v);
        }

        internal void glDisable(All depthTest)
        {
            GL.Disable((EnableCap)depthTest);
        }

        internal void glDisableVertexAttribArray(uint theAttribLoc)
        {
            GL.DisableVertexAttribArray(theAttribLoc);
        }
        internal void glDisableVertexAttribArray(int theAttribLoc)
        {
            GL.DisableVertexAttribArray(theAttribLoc);
        }

        internal void glDrawArrays(All triangleStrip, int v1, int v2)
        {
            GL.DrawArrays((PrimitiveType)triangleStrip, v1, v2);
        }

        internal void glEnable(All depthTest)
        {
            GL.Enable((EnableCap)depthTest);
        }

        internal void glEnableVertexAttribArray(uint theAttribLoc)
        {
            GL.EnableVertexAttribArray(theAttribLoc);
        }
        internal void glEnableVertexAttribArray(int theAttribLoc)
        {
            GL.EnableVertexAttribArray(theAttribLoc);
        }

        internal void glGetProgramInfoLog(int myProgramID, int aLength, out int len, out string aLog)
        {
            GL.GetProgramInfoLog(myProgramID, aLength, out len, out aLog);
        }

        internal void glGetProgramiv(int myProgramID, GetProgramParameterName status, ref int aStatus)
        {
            GL.GetProgram(myProgramID, status, out aStatus);
        }

        internal void glGetShaderInfoLog(int myShaderID, int bufSize, ref int len, ref string aLog)
        {
            GL.GetShaderInfoLog(myShaderID, bufSize, out len, out aLog);
        }

        internal void glGetShaderiv(int myShaderID, ShaderParameter param, ref int v)
        {
            GL.GetShader(myShaderID, param, out v);
        }

        internal void glLinkProgram(int myProgramID)
        {
            GL.LinkProgram(myProgramID);
        }

        internal void glShaderSource(int myShaderID, int v, string code, out int len)
        {
            len = 0;
            GL.ShaderSource(myShaderID,  code);
        }

        internal void glTexParameteri(All texture2D, All textureMinFilter, All aFilterGl)
        {
            GL.TextureParameter((int)texture2D,(TextureParameterName)textureMinFilter, (int)aFilterGl);
        }

        internal void glUniform1f(int theLocation, float theValue)
        {
            GL.Uniform1(theLocation, theValue);
        }

        internal void glUniform1i(int theLocation, Graphic3d_TextureUnit theTextureUnit)
        {
            GL.Uniform1(theLocation, (int)theTextureUnit);
        }
        internal void glUniform1i(int theLocation, int theTextureUnit)
        {
            GL.Uniform1(theLocation, theTextureUnit);
        }

        internal void glUniform1iv(int theLocation, int theCount, int[] data)
        {
            GL.Uniform1(theLocation, theCount, data);
        }

        internal void glUniform2fv(int theLocation, int theCount, float[] floats)
        {
            GL.Uniform2(theLocation, theCount, floats);
        }

        internal void glUniform4fv(int location, int v2, Vector4 theValue)
        {
            GL.Uniform4(location, v2, theValue.ToFloatArray());
        }
        internal void glUniform4fv(int location, int v2, float[] theValue)
        {
            GL.Uniform4(location, v2, theValue);
        }

        internal void glUniformMatrix4fv(int theLocation, int count, bool transpose, float[] data)
        {
            GL.UniformMatrix4(theLocation, count, transpose, data);
        }

        internal void glUseProgram(int v)
        {
            GL.UseProgram(v);
        }
        internal void glVertexAttribPointer(int index, int size, int type, bool normalized, int stride, int pointer)
        {
            GL.VertexAttribPointer(index, size, (VertexAttribPointerType)type, normalized, stride, pointer);
        }
        internal void glVertexAttribPointer(uint index, uint size, All type, bool normalized, int stride, int pointer)
        {
            GL.VertexAttribPointer((int)index, (int)size, (VertexAttribPointerType)type, normalized, stride, pointer);
        }
    }
}