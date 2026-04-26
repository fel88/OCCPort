namespace OCCPort
{
    public interface IParametersCollection
    {
        int Lower();
        int Upper();
        double Value(int index);
    }
}