using System;

namespace OCCPort.OpenGL
{
    [Serializable]
    internal class Aspect_GraphicDeviceDefinitionError : Exception
    {
        public Aspect_GraphicDeviceDefinitionError()
        {
        }

        public Aspect_GraphicDeviceDefinitionError(string message) : base(message)
        {
        }

        public Aspect_GraphicDeviceDefinitionError(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}