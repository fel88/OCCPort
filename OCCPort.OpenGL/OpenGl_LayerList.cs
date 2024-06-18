using OCCPort;
using OCCPort.OpenGL;
using System;
using System.Collections.Generic;

namespace OCCPort
{
    //! Class defining the list of layers.
    internal class OpenGl_LayerList
    {
        List<Graphic3d_Layer> myLayers = new List<Graphic3d_Layer>();
        internal Graphic3d_Layer[] Layers()
        {
            return myLayers.ToArray();
        }
        //! Method returns the number of available priorities
        public int NbPriorities() { return Constants.Graphic3d_DisplayPriority_NB; }

        //! Number of displayed structures
        public int NbStructures() { return myNbStructures; }

        //! Return number of structures within immediate layers
        public int NbImmediateStructures() { return myImmediateNbStructures; }

        internal void UpdateCulling(OpenGl_Workspace myWorkspace, bool theToDrawImmediate)
        {
            throw new NotImplementedException();
        }
        void renderLayer(OpenGl_Workspace theWorkspace,
                                     OpenGl_GlobalLayerSettings theDefaultSettings,
                                     Graphic3d_Layer theLayer)
        {
            // render priority list
            int aViewId = theWorkspace.View().Identification();
            for (int aPriorityIter = (int)Graphic3d_DisplayPriority.Graphic3d_DisplayPriority_Bottom; aPriorityIter <= (int)Graphic3d_DisplayPriority.Graphic3d_DisplayPriority_Topmost; ++aPriorityIter)
            {
                Graphic3d_IndexedMapOfStructure aStructures = theLayer.Structures((Graphic3d_DisplayPriority)aPriorityIter);
                for (OpenGl_Structure.StructIterator aStructIter = new OpenGl_Structure.StructIterator(aStructures); aStructIter.More(); aStructIter.Next())
                {
                    OpenGl_Structure aStruct = aStructIter.Value();
                    if (aStruct.IsCulled()
                    || !aStruct.IsVisible(aViewId))
                    {
                        continue;
                    }

                    aStruct.Render(theWorkspace);
                }
            }
        }

        internal void Render(OpenGl_Workspace myWorkspace, bool theToDrawImmediate, OpenGl_LayerFilter openGl_LF_Upper, OpenGl_FrameBuffer theReadDrawFbo, OpenGl_FrameBuffer theOitAccumFbo)
        {
            throw new NotImplementedException();
        }

        Dictionary<Graphic3d_ZLayerId, Graphic3d_Layer> myLayerIds;
        Select3D_BVHBuilder3d myBVHBuilder;      //!< BVH tree builder for frustum culling

        int myNbStructures;
        int myImmediateNbStructures; //!< number of structures within immediate layers

        Standard_Size myModifStateOfRaytraceable;

        //! Collection of references to layers with transparency gathered during rendering pass.
        OpenGl_LayerStack myTransparentToProcess;



    }

}