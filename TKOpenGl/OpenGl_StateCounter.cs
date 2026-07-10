using System;
using System.Diagnostics.Metrics;

namespace OCCPort.OpenGL
{
    public class OpenGl_StateCounter
    {
        internal int Increment()
        {
            return ++myCounter;
        }
        int myCounter;

    }
}