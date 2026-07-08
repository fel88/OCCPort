using System.Reflection.Metadata;
using TKernel;

namespace TKService
{
    //! Class defines a Clipping Volume as a logical OR (disjunction) operation between Graphic3d_ClipPlane in sequence.
    //! Each Graphic3d_ClipPlane represents either a single Plane clipping a halfspace (direction is specified by normal),
    //! or a sub-chain of planes defining a logical AND (conjunction) operation.
    //! Therefore, this collection allows defining a Clipping Volume through the limited set of Boolean operations between clipping Planes.
    //!
    //! The Clipping Volume can be assigned either to entire View or to a specific Object;
    //! in the latter case property ToOverrideGlobal() will specify if Object planes should override (suppress) globally defined ones
    //! or extend their definition through logical OR (disjunction) operation.
    //!
    //! Note that defining (many) planes will lead to performance degradation, and Graphics Driver may limit
    //! the overall number of simultaneously active clipping planes - but at least 6 planes should be supported on all configurations.
    public class Graphic3d_SequenceOfHClipPlane
    {
        //! Return TRUE if sequence is empty.
        public bool IsEmpty() { return myItems.IsEmpty(); }

        //! Return the number of items in sequence.
        public int Size() { return myItems.Size(); }

        NCollection_Sequence<Graphic3d_ClipPlane> myItems = new NCollection_Sequence<Graphic3d_ClipPlane>();
        bool myToOverrideGlobal;

        public class Iterator
        {
            public Iterator(Graphic3d_SequenceOfHClipPlane thePlanes)
            {
            }
            public void Init(Graphic3d_SequenceOfHClipPlane thePlanes)
            {
                throw new NotImplementedException();

                if (thePlanes != null)
                {
                    //NCollection_Sequence<Graphic3d_ClipPlane>.Iterator.Init(thePlanes.myItems);
                }
                else
                {
                    throw new NotImplementedException();
                    //*this = Iterator();
                }
            }
            public bool More()
            {
                throw new NotImplementedException();
            }

            public object Next()
            {
                throw new NotImplementedException();
            }

            public Graphic3d_ClipPlane Value()
            {
                throw new NotImplementedException();
            }
        }
    }
}
