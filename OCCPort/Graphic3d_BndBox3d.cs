using System;

namespace OCCPort
{
    public class Graphic3d_BndBox3d : BVH_Box
    {


		public

  BVH_VecNt myMinPoint; //!< Minimum point of bounding box
		public BVH_VecNt myMaxPoint; //!< Maximum point of bounding box
        protected bool myIsInited; //!< Is bounding box initialized?

		public Graphic3d_BndBox3d()
		{
			myIsInited = false;
		}
		public Graphic3d_BndBox3d(BVH_VecNt min, BVH_VecNt max)
		{
			myIsInited = true;
			myMaxPoint = max;
			myMinPoint = min;
		}


        //! Returns minimum point of bounding box.
        public BVH_VecNt CornerMin() { return myMinPoint; }

        //! Returns maximum point of bounding box.
        public BVH_VecNt CornerMax() { return myMaxPoint; }


		//! Is bounding box valid?
		public bool IsValid() { return myIsInited; }

    }
}
