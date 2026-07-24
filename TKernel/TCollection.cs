using OCCPort.Common;

namespace TKernel
{
    //! The package <TCollection> provides the services for the
    //! transient basic data structures.
    public class TCollection
    {
        // The array of prime numbers used as consecutive steps for
        // size of array of buckets in the map.
        // The prime numbers are used for array size with the hope that this will 
        // lead to less probablility of having the same hash codes for
        // different map items (note that all hash codes are modulo that size).
        // The value of each next step is chosen to be ~2 times greater than previous.
        // Though this could be thought as too much, actually the amount of 
        // memory overhead in that case is only ~15% as compared with total size of
        // all auxiliary data structures (each map node takes ~24 bytes), 
        // and this proves to pay off in performance (see OCC13189).
        const int THE_NB_PRIMES = 24;

         static  int[] THE_TCollection_Primes = new[]
{
         101,
        1009,
        2003,
        5003,
       10007,
       20011,
       37003,
       57037,
       65003,
      100019,
      209953, // The following are Pierpont primes [List of prime numbers]
      472393,
      995329,
     2359297,
     4478977,
     9437185,
    17915905,
    35831809,
    71663617,
   150994945,
   301989889,
   573308929,
  1019215873,
  2038431745
};
        public static int NextPrimeForMap(int N)
        {
            for (int aPrimeIter = 0; aPrimeIter < THE_NB_PRIMES; ++aPrimeIter)
            {
                if (THE_TCollection_Primes[aPrimeIter] > N)
                {
                    return THE_TCollection_Primes[aPrimeIter];
                }
            }
            throw new Standard_OutOfRange("TCollection::NextPrimeForMap() - requested too big size");
        }
    }
}