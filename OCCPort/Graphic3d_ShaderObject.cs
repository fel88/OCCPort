using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading;

namespace OCCPort
{
    public class Graphic3d_ShaderObject
    {
        public Graphic3d_ShaderObject(Graphic3d_TypeOfShaderObject theType)
        {
            myType = (theType);
            myID = ("Graphic3d_ShaderObject_")
                 + (Interlocked.Increment(ref THE_SHADER_OBJECT_COUNTER));
        }
        static volatile int THE_SHADER_OBJECT_COUNTER = 0;

        //! The type of shader object.
        Graphic3d_TypeOfShaderObject myType;

        string myID;     //!< the ID of shader object
        string mySource; //!< the source code of shader object
                         //OSD_Path myPath;   //!< the path to shader source (may be empty)


        //! This is a preprocessor for Graphic3d_ShaderObject::CreateFromSource() function.
        //! Creates a new shader object from specified source according to list of uniforms and in/out variables.
        //! @param theSource      shader object source code to modify
        //! @param theType        shader object type to create
        //! @param theUniforms    list of uniform variables
        //! @param theStageInOuts list of stage in/out variables
        //! @param theInName      name of input  variables block;
        //!                       can be empty for accessing each variable without block prefix
        //!                       (mandatory for stages accessing both inputs and outputs)
        //! @param theOutName     name of output variables block;
        //!                       can be empty for accessing each variable without block prefix
        //!                       (mandatory for stages accessing both inputs and outputs)
        //! @param theNbGeomInputVerts number of geometry shader input vertexes
        public static Graphic3d_ShaderObject CreateFromSource(string theSource, Graphic3d_TypeOfShaderObject theType,
            ShaderVariableList theUniforms,
            ShaderVariableList theStageInOuts,
            string theInName = "",
            string theOutName = "",
            int theNbGeomInputVerts = 0)
        {
            if (string.IsNullOrEmpty(theSource))
            {
                return null;
            }
            string aSrcUniforms = "", aSrcInOuts = "", aSrcInStructs = "", aSrcOutStructs = "";
            foreach (var aVar in theUniforms)
            {
                if ((aVar.Stages & (int)theType) != 0)
                {
                    aSrcUniforms += ("\nuniform ") + aVar.Name + ";";
                }
            }
            foreach (var aVar in theStageInOuts)
            {


                int aStageLower = Standard_Integer.IntegerLast(), aStageUpper = Standard_Integer.IntegerFirst();
                int aNbStages = 0;
                for (int aStageIter = (int)Graphic3d_TypeOfShaderObject.Graphic3d_TOS_VERTEX; aStageIter <= (int)Graphic3d_TypeOfShaderObject.Graphic3d_TOS_COMPUTE; aStageIter = aStageIter << 1)
                {
                    if ((aVar.Stages & aStageIter) != 0)
                    {
                        ++aNbStages;
                        aStageLower = Math.Min(aStageLower, aStageIter);
                        aStageUpper = Math.Max(aStageUpper, aStageIter);
                    }
                }
                if ((int)theType < aStageLower
                 || (int)theType > aStageUpper)
                {
                    continue;
                }

                bool hasGeomStage = theNbGeomInputVerts > 0
                                                  && aStageLower < (int)Graphic3d_TypeOfShaderObject.Graphic3d_TOS_GEOMETRY
                                                  && aStageUpper >= (int)Graphic3d_TypeOfShaderObject.Graphic3d_TOS_GEOMETRY;
                bool isAllStagesVar = aStageLower == (int)Graphic3d_TypeOfShaderObject.Graphic3d_TOS_VERTEX
                                                    && aStageUpper == (int)Graphic3d_TypeOfShaderObject.Graphic3d_TOS_FRAGMENT;
                if (hasGeomStage
                || !theInName.IsEmpty()
                || !theOutName.IsEmpty())
                {
                    if (aSrcInStructs.IsEmpty()
                     && aSrcOutStructs.IsEmpty()
                     && isAllStagesVar)
                    {
                        if ((int)theType == aStageLower)
                        {
                            aSrcOutStructs = "\nout VertexData\n{";
                        }
                        else if ((int)theType == aStageUpper)
                        {
                            aSrcInStructs = "\nin VertexData\n{";
                        }
                        else // requires theInName/theOutName
                        {
                            aSrcInStructs = "\nin  VertexData\n{";
                            aSrcOutStructs = "\nout VertexData\n{";
                        }
                    }
                }

                if (isAllStagesVar
                 && (!aSrcInStructs.IsEmpty()
                  || !aSrcOutStructs.IsEmpty()))
                {
                    if (!aSrcInStructs.IsEmpty())
                    {
                        aSrcInStructs += ("\n  ") + aVar.Name + ";";
                    }
                    if (!aSrcOutStructs.IsEmpty())
                    {
                        aSrcOutStructs += ("\n  ") + aVar.Name + ";";
                    }
                }
                else
                {
                    if ((int)theType == aStageLower)
                    {
                        aSrcInOuts += ("\nTHE_SHADER_OUT ") + aVar.Name + ";";
                    }
                    else if ((int)theType == aStageUpper)
                    {
                        aSrcInOuts += ("\nTHE_SHADER_IN ") + aVar.Name + ";";
                    }
                }
            }

            if (theType == Graphic3d_TypeOfShaderObject.Graphic3d_TOS_GEOMETRY)
            {
                aSrcUniforms.Prepend(
                                     "\nlayout (triangles) in;" +


                                      "\nlayout (triangle_strip, max_vertices = " + theNbGeomInputVerts + ") out;");
            }
            if (!aSrcInStructs.IsEmpty()
             && theType == Graphic3d_TypeOfShaderObject.Graphic3d_TOS_GEOMETRY)
            {
                aSrcInStructs += ("\n} ") + theInName + "[" + theNbGeomInputVerts + "];";
            }
            else if (!aSrcInStructs.IsEmpty())
            {
                aSrcInStructs += "\n}";
                if (!theInName.IsEmpty())
                {
                    aSrcInStructs += " ";
                    aSrcInStructs += theInName;
                }
                aSrcInStructs += ";";
            }

            if (!aSrcOutStructs.IsEmpty())
            {
                aSrcOutStructs += "\n}";
                if (!theOutName.IsEmpty())
                {
                    aSrcOutStructs += " ";
                    aSrcOutStructs += theOutName;
                }
                aSrcOutStructs += ";";
            }


            theSource.Prepend(aSrcUniforms + aSrcInStructs + aSrcOutStructs + aSrcInOuts);
            return Graphic3d_ShaderObject.CreateFromSource(theType, theSource);

        }

        private static Graphic3d_ShaderObject CreateFromSource(Graphic3d_TypeOfShaderObject theType, string theSource)
        {
            Graphic3d_ShaderObject aShader = new Graphic3d_ShaderObject(theType);
            aShader.mySource = theSource;
            return aShader;
        }

        public class ShaderVariableList : List<ShaderVariable>
        {
        }

        public class ShaderVariable
        {
            
            
            //! The name of uniform shader variable.
          public   string Name;

            public int Stages { get; internal set; }

            
            public ShaderVariable()
            {

            }
            public ShaderVariable(string theVarName, Graphic3d_TypeOfShaderObject theShaderStageBits)
            {
                this.Name  = theVarName;
                this.Stages = (int)theShaderStageBits;
            }
        }
    }

}