using System;
using System.Reflection.Metadata;

namespace OCCPort.OpenGL
{
    //! Wrapper for OpenGL shader object.
    public class OpenGl_ShaderObject : OpenGl_Resource
    {
        //! Non-valid shader name.
        const int NO_SHADER = 0;
        // =======================================================================
        // function : Create
        // purpose  : Creates new empty shader object of specified type
        // =======================================================================
        public bool Create(OpenGl_Context theCtx)
        {
            if (myShaderID == NO_SHADER
             && theCtx.core20fwd != null)
            {
                myShaderID = theCtx.core20fwd.glCreateShader(myType);
            }

            return myShaderID != NO_SHADER;
        }

        //! Creates uninitialized shader object.
        public OpenGl_ShaderObject(GLenum theType)
        {
            myType = (theType);
            myShaderID = (NO_SHADER);
        }
        //! Compiles the shader object.
        public bool Compile(OpenGl_Context theCtx)
        {
            if (myShaderID == NO_SHADER)
            {
                return false;
            }

            // Try to compile shader
            theCtx.core20fwd.glCompileShader(myShaderID);

            // Check compile status
            GLint aStatus = 0;
            theCtx.core20fwd.glGetShaderiv(myShaderID, OpenTK.Graphics.OpenGL.ShaderParameter.CompileStatus, ref aStatus);
            return aStatus != 0;
        }
        //! Loads shader source code.
        public bool LoadSource(OpenGl_Context theCtx,
                                               string theSource)
        {
            if (myShaderID == NO_SHADER)
            {
                return false;
            }

            string aLines = theSource;
            theCtx.core20fwd.glShaderSource(myShaderID, 1, [aLines], out int len);
            return true;
        }

        //! Wrapper for compiling shader object with verbose printing on error.
        //! @param theCtx bound OpenGL context
        //! @param theId  GLSL program id to define file name
        //! @param theSource source code to load
        //! @param theIsVerbose flag to print log on error
        //! @param theToPrintSource flag to print source code on error
        public bool LoadAndCompile(OpenGl_Context theCtx,
                                                    string theId,
                                                    string theSource,
                                                   bool theIsVerbose = true,
                                                   bool theToPrintSource = true)
        {
            if (!theIsVerbose)
            {
                return LoadSource(theCtx, theSource)
                    && Compile(theCtx);
            }

            if (!LoadSource(theCtx, theSource))
            {
                if (theToPrintSource)
                {
                    //theCtx->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH, theSource);
                }
                //   theCtx->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH,
                //                        ("Error! Failed to set ") + getShaderTypeString(myType) + " [" + theId + "] source");
                return false;
            }

            if (!Compile(theCtx))
            {
                if (theToPrintSource)
                {
                    //theCtx->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH, putLineNumbers(theSource));
                }
                string aLog;
                //    FetchInfoLog(theCtx, aLog);
                // if (aLog.IsEmpty())
                {
                    //     aLog = "Compilation log is empty.";
                }
                //    theCtx->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_ERROR, 0, GL_DEBUG_SEVERITY_HIGH,
                //                       ("Failed to compile ") + getShaderTypeString(myType) + " [" + theId + "]. Compilation log:\n" + aLog);
                return false;
            }
            //else if (theCtx.caps.glslWarnings)
            {
                string aLog;
                //   FetchInfoLog(theCtx, aLog);
                //   if (!aLog.IsEmpty()
                //    && !aLog.IsEqual("No errors.\n"))
                {
                    //       theCtx->PushMessage(GL_DEBUG_SOURCE_APPLICATION, GL_DEBUG_TYPE_PORTABILITY, 0, GL_DEBUG_SEVERITY_LOW,
                    //                          getShaderTypeString(myType) + " [" + theId + "] compilation log:\n" + aLog);
                }
            }
            return true;
        }

        DateTime myDumpDate; //!< The recent date of the shader dump
        GLenum myType;     //!< Type of OpenGL shader object
        public int myShaderID; //!< Handle of OpenGL shader object

    }
}