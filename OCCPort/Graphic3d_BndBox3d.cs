using System;

namespace OCCPort
{
    public class Graphic3d_BndBox3d : BVH_Box
    {


        protected

  BVH_VecNt myMinPoint; //!< Minimum point of bounding box
        protected BVH_VecNt myMaxPoint; //!< Maximum point of bounding box
        protected bool myIsInited; //!< Is bounding box initialized?



        //! Returns minimum point of bounding box.
        public BVH_VecNt CornerMin() { return myMinPoint; }

        //! Returns maximum point of bounding box.
        public BVH_VecNt CornerMax() { return myMaxPoint; }


        internal bool IsValid()
        {
            throw new NotImplementedException();
        }
    }
}
