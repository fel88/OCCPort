using System;
using System.Collections.Generic;
using System.Linq;

namespace OCCPort
{
    public class Graphic3d_ShaderObject
    {
        public static Graphic3d_ShaderObject CreateFromSource(string theSource, Graphic3d_TypeOfShaderObject theType,
            ShaderVariableList theUniforms,
            ShaderVariableList theStageInOuts,
            string theInName,
            string theOutName,
            int theNbGeomInputVerts)
        {
            if (string.IsNullOrEmpty(theSource))
            {
                return new Graphic3d_ShaderObject();
            }
            string aSrcUniforms = "", aSrcInOuts = "", aSrcInStructs = "", aSrcOutStructs = "";
            foreach (var aVar in theUniforms)
            {
                if ((aVar.Stages & (int)theType) != 0)
                {
                    aSrcUniforms += ("\nuniform ") + aVar.Name() + ";";
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
                        aSrcInStructs += ("\n  ") + aVar.Name() + ";";
                    }
                    if (!aSrcOutStructs.IsEmpty())
                    {
                        aSrcOutStructs += ("\n  ") + aVar.Name() + ";";
                    }
                }
                else
                {
                    if ((int)theType == aStageLower)
                    {
                        aSrcInOuts += ("\nTHE_SHADER_OUT ") + aVar.Name() + ";";
                    }
                    else if ((int)theType == aStageUpper)
                    {
                        aSrcInOuts += ("\nTHE_SHADER_IN ") + aVar.Name() + ";";
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
            throw new NotImplementedException();
        }

        public class ShaderVariableList : List<ShaderVariable>
        {
        }

        public class ShaderVariable
        {
            private string v;
            private object graphic3d_TOS_GEOMETRY;
            //! The name of uniform shader variable.
            string myName;

            public int Stages { get; internal set; }

            // =======================================================================
            // function : Name
            // purpose  : Returns name of shader variable
            // =======================================================================
            public string Name()
            {
                return myName;
            }
            public ShaderVariable(string v, object graphic3d_TOS_GEOMETRY)
            {
                this.v = v;
                this.graphic3d_TOS_GEOMETRY = graphic3d_TOS_GEOMETRY;
            }
        }
    }

}