namespace OCCPort
{
    internal class Graphic3d_UniformValue<T>:Graphic3d_ValueInterface
    {
        private T theValue;

        public Graphic3d_UniformValue(T theValue)
        {
            this.theValue = theValue;
        }
    }
}