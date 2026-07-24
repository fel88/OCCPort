namespace TKernel
{
    public interface IHasher<T> : IEqualityComparer<T>
    {

        //! Computes a hash code for the given value of the Standard_Integer type, in range [1, theUpperBound]
        //! @param theValue the value of the Standard_Integer type which hash code is to be computed
        //! @param theUpperBound the upper bound of the range a computing hash code must be within
        //! @return a computed hash code, in range [1, theUpperBound]
        int HashCode(T theValue, int theUpperBound);
        bool IsEqual(T theKeyType, T theKey1);
    }
}