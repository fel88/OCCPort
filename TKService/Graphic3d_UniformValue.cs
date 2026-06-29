namespace TKService
{
    //! Describes specific value of custom uniform variable.
    public class Graphic3d_UniformValue<T> : Graphic3d_ValueInterface
    {
        private T theValue;

        public Graphic3d_UniformValue(T theValue)
        {
            this.theValue = theValue;
        }

        public override int TypeID()
        {
            throw new NotImplementedException();
        }
    }
}
