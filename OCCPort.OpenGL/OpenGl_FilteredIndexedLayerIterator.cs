using System;

namespace OCCPort
{
    internal class OpenGl_FilteredIndexedLayerIterator
    {
        public OpenGl_FilteredIndexedLayerIterator(object aLayerIterStart)
        {
        }

        public OpenGl_FilteredIndexedLayerIterator(object aLayerIterStart, bool theToDrawImmediate, OpenGl_LayerFilter theLayersToProcess) : this(aLayerIterStart)
        {
        }

        internal bool More()
        {
            throw new NotImplementedException();
        }

        internal object Next()
        {
            throw new NotImplementedException();
        }

        internal OpenGl_Layer Value()
        {
            throw new NotImplementedException();
        }
    }
}