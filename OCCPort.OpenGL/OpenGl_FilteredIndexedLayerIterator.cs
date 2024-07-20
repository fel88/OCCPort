using System;
using System.Collections;
using System.Collections.Generic;

namespace OCCPort
{
    //! Iterator through layers with filter.
    internal class OpenGl_FilteredIndexedLayerIterator:IEnumerable<Graphic3d_Layer>
    {
        public OpenGl_FilteredIndexedLayerIterator(List<Graphic3d_Layer> aLayerIterStart)
        {
            myIter = new OpenGl_IndexedLayerIterator(aLayerIterStart);

        }

        public OpenGl_FilteredIndexedLayerIterator(LayersCollection aLayerIterStart, bool theToDrawImmediate, OpenGl_LayerFilter theLayersToProcess) : this(aLayerIterStart)
        {
            myIter = new OpenGl_IndexedLayerIterator(aLayerIterStart);
        }

        //! Return true if iterator points to the valid value.
        public bool More()  { return myIter.More(); }

        public OpenGl_IndexedLayerIterator myIter;
        OpenGl_LayerFilter myLayersToProcess;
        bool myToDrawImmediate;

        

        internal void Next()
        {
            myIter.Next();
            next();
        } 
        
        //! Look for the nearest item passing filters.
        void next()
        {
            for (; myIter.More(); myIter.Next())
            {
                 Graphic3d_Layer aLayer = myIter.Value();
                if (aLayer.IsImmediate() != myToDrawImmediate)
                {
                    continue;
                }

                switch (myLayersToProcess)
                {
                    case OpenGl_LayerFilter. OpenGl_LF_All:
                        {
                            return;
                        }
                    case OpenGl_LayerFilter.OpenGl_LF_Upper:
                        {
                            /*if (aLayer->LayerId() != Graphic3d_ZLayerId_BotOSD
                             && (!aLayer->LayerSettings().IsRaytracable()
                               || aLayer->IsImmediate()))
                            {
                                return;
                            }*/
                            break;
                        }
                    case OpenGl_LayerFilter.OpenGl_LF_Bottom:
                        {
                           /* if (aLayer->LayerId() == Graphic3d_ZLayerId_BotOSD
                            && !aLayer->LayerSettings().IsRaytracable())
                            {
                                return;
                            }*/
                            break;
                        }
                    case OpenGl_LayerFilter.OpenGl_LF_RayTracable:
                        {
                           /* if (aLayer->LayerSettings().IsRaytracable()
                            && !aLayer->IsImmediate())
                            {
                                return;
                            }*/
                            break;
                        }
                }
            }
        }

        internal Graphic3d_Layer Value()
        {
            return myIter.Value();
        }

        public IEnumerator<Graphic3d_Layer> GetEnumerator()
        {
            return myIter;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return myIter;
        }
    }
}