namespace TKernel
{
    public interface IElementBinaryExtractor<T>
    {
        T Get(byte[] data, int idx);
    }
}