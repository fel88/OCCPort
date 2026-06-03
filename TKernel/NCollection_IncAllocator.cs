namespace TKernel
{
    /**
     *  Class NCollection_IncAllocator - incremental memory  allocator. This class
     *  allocates  memory  on  request  returning  the  pointer  to  an  allocated
     *  block. This memory is never returned  to the system until the allocator is
     *  destroyed.
     *
     *  By comparison with  the standard new() and malloc()  calls, this method is
     *  faster and consumes very small additional memory to maintain the heap.
     *
     *  All pointers  returned by Allocate() are  aligned to the size  of the data
     *  type "aligned_t". To  modify the size of memory  blocks requested from the
     *  OS,  use the parameter  of the  constructor (measured  in bytes);  if this
     *  parameter is  smaller than  25 bytes on  32bit or  49 bytes on  64bit, the
     *  block size will be the default 12 kbytes.
     *
     *  It is not recommended  to use memory blocks  larger than 16KB  on  Windows
     *  platform  for the repeated operations  because  Low Fragmentation Heap  is
     *  not going to be  used  for  these  allocations  which  may lead  to memory
     *  fragmentation and the general performance slow down.
     *
     *  Note that this allocator is most suitable for single-threaded algorithms
     *  (consider creating dedicated allocators per working thread),
     *  and thread-safety of allocations is DISABLED by default (see SetThreadSafe()).
     */
    public class NCollection_IncAllocator : NCollection_BaseAllocator
    {
        public NCollection_IncAllocator() { }
        public NCollection_IncAllocator(int mEMORY_BLOCK_SIZE_HUGE)
        {
        }
    }
}