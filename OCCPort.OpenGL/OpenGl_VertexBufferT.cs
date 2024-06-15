namespace OCCPort.OpenGL
{
    internal class OpenGl_VertexBufferT<T1> : OpenGl_VertexBufferCompat
    {
        private object value;

        public OpenGl_VertexBufferT(int c,object value)
        {
            this.value = value;
        }
    }
}